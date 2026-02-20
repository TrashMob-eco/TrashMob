import { useEffect, useRef } from 'react';
import { useMap } from '@vis.gl/react-google-maps';
import AdoptableAreaData, { AdoptableAreaStatus } from '@/components/Models/AdoptableAreaData';
import { parseGeoJson, polygonCoordsToPath, lineStringCoordsToPath, computeCentroid } from '@/lib/geojson';
import { getAreaColor } from '@/lib/area-colors';

/** Status-based stroke/fill styling (weight and opacity only — color comes from getAreaColor). */
const STATUS_STROKE: Record<AdoptableAreaStatus, { strokeWeight: number; strokeOpacity: number; fillOpacity: number }> =
    {
        Available: { strokeWeight: 2, strokeOpacity: 0.7, fillOpacity: 0.15 },
        Adopted: { strokeWeight: 4, strokeOpacity: 0.9, fillOpacity: 0.25 },
        Unavailable: { strokeWeight: 1, strokeOpacity: 0.4, fillOpacity: 0.08 },
    };

/** Status-based centroid marker colors. */
const STATUS_MARKER_COLOR: Record<AdoptableAreaStatus, string> = {
    Available: '#16A34A',
    Adopted: '#2563EB',
    Unavailable: '#6B7280',
};

interface ExistingAreasOverlayProps {
    mapId: string;
    areas: AdoptableAreaData[];
    excludeAreaId?: string;
    /** When true, auto-fits the map viewport to show all areas. */
    fitBounds?: boolean;
}

export const ExistingAreasOverlay = ({ mapId, areas, excludeAreaId, fitBounds }: ExistingAreasOverlayProps) => {
    const map = useMap(mapId);
    const overlaysRef = useRef<(google.maps.Polygon | google.maps.Polyline)[]>([]);
    const markersRef = useRef<google.maps.Marker[]>([]);
    const infoWindowRef = useRef<google.maps.InfoWindow | null>(null);

    useEffect(() => {
        if (!map) return;

        // Clean previous overlays and markers
        overlaysRef.current.forEach((o) => o.setMap(null));
        overlaysRef.current = [];
        markersRef.current.forEach((m) => m.setMap(null));
        markersRef.current = [];
        if (infoWindowRef.current) {
            infoWindowRef.current.close();
        }

        const filtered = areas.filter(
            (a) =>
                a.id !== excludeAreaId &&
                (a.geoJson || (a.startLatitude != null && a.startLongitude != null)),
        );
        const bounds = fitBounds ? new google.maps.LatLngBounds() : null;

        const showInfoWindow = (area: AdoptableAreaData, position: google.maps.LatLng | null) => {
            if (!position) return;
            if (!infoWindowRef.current) {
                infoWindowRef.current = new google.maps.InfoWindow();
            }
            infoWindowRef.current.setContent(
                `<div style="font-size:13px"><strong>${area.name}</strong><br/><span style="color:#666">${area.status} &middot; ${area.areaType}</span></div>`,
            );
            infoWindowRef.current.setPosition(position);
            infoWindowRef.current.open(map);
        };

        filtered.forEach((area) => {
            const parsed = parseGeoJson(area.geoJson);

            const color = getAreaColor(area.id);
            const stroke = STATUS_STROKE[area.status] ?? STATUS_STROKE.Available;
            let path: google.maps.LatLngLiteral[];
            let isFallback = false;

            if (!parsed) {
                // Fallback: render marker at known coordinates when GeoJSON is missing
                if (area.startLatitude != null && area.startLongitude != null) {
                    path = [{ lat: area.startLatitude, lng: area.startLongitude }];
                    isFallback = true;
                } else {
                    return;
                }
            } else if (parsed.type === 'Polygon') {
                path = polygonCoordsToPath(parsed.coordinates);
                if (path.length < 3) return;

                const poly = new google.maps.Polygon({
                    map,
                    paths: path,
                    strokeColor: color,
                    strokeOpacity: stroke.strokeOpacity,
                    strokeWeight: stroke.strokeWeight,
                    fillColor: color,
                    fillOpacity: stroke.fillOpacity,
                    clickable: true,
                    zIndex: 1,
                });

                poly.addListener('click', (e: google.maps.MapMouseEvent) => showInfoWindow(area, e.latLng));
                overlaysRef.current.push(poly);
            } else if (parsed.type === 'LineString') {
                path = lineStringCoordsToPath(parsed.coordinates);
                if (path.length < 2) return;

                const line = new google.maps.Polyline({
                    map,
                    path,
                    strokeColor: color,
                    strokeOpacity: stroke.strokeOpacity,
                    strokeWeight: stroke.strokeWeight + 1, // lines need a bit more weight to be visible
                    clickable: true,
                    zIndex: 1,
                });

                line.addListener('click', (e: google.maps.MapMouseEvent) => showInfoWindow(area, e.latLng));
                overlaysRef.current.push(line);
            } else if (parsed.type === 'Point') {
                // Point geometry: render as a pin marker (no polygon outline available)
                const [lng, lat] = parsed.coordinates;
                path = [{ lat, lng }];
            } else {
                return;
            }

            // Extend bounds for fitBounds
            if (bounds) {
                path.forEach((p) => bounds.extend(p));
            }

            // Add centroid status marker
            const centroid = path.length === 1 ? path[0] : computeCentroid(path);
            const markerColor = STATUS_MARKER_COLOR[area.status] ?? STATUS_MARKER_COLOR.Available;

            const marker = new google.maps.Marker({
                map,
                position: centroid,
                icon: {
                    path: google.maps.SymbolPath.CIRCLE,
                    scale: isFallback ? 10 : 8,
                    fillColor: markerColor,
                    fillOpacity: 1,
                    strokeColor: isFallback ? markerColor : '#ffffff',
                    strokeWeight: isFallback ? 3 : 2,
                },
                title: `${area.name} (${area.status})`,
                zIndex: 2,
            });

            marker.addListener('click', () => {
                showInfoWindow(area, new google.maps.LatLng(centroid.lat, centroid.lng));
            });

            markersRef.current.push(marker);
        });

        // Fit map to all areas
        if (bounds && !bounds.isEmpty()) {
            map.fitBounds(bounds, { top: 40, right: 40, bottom: 40, left: 40 });
        }

        return () => {
            overlaysRef.current.forEach((o) => o.setMap(null));
            overlaysRef.current = [];
            markersRef.current.forEach((m) => m.setMap(null));
            markersRef.current = [];
            if (infoWindowRef.current) {
                infoWindowRef.current.close();
                infoWindowRef.current = null;
            }
        };
    }, [map, areas, excludeAreaId, fitBounds]);

    return null;
};
