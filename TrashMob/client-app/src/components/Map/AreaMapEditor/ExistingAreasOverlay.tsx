import { useEffect, useRef } from 'react';
import { useMap } from '@vis.gl/react-google-maps';
import AdoptableAreaData, { AdoptableAreaStatus } from '@/components/Models/AdoptableAreaData';
import { parseGeoJson, polygonCoordsToPath, lineStringCoordsToPath } from '@/lib/geojson';

const STATUS_STYLES: Record<AdoptableAreaStatus, { strokeColor: string; fillColor: string }> = {
    Available: { strokeColor: '#16A34A', fillColor: '#16A34A' },
    Adopted: { strokeColor: '#2563EB', fillColor: '#2563EB' },
    Unavailable: { strokeColor: '#6B7280', fillColor: '#6B7280' },
};

interface ExistingAreasOverlayProps {
    mapId: string;
    areas: AdoptableAreaData[];
    excludeAreaId?: string;
}

export const ExistingAreasOverlay = ({ mapId, areas, excludeAreaId }: ExistingAreasOverlayProps) => {
    const map = useMap(mapId);
    const overlaysRef = useRef<(google.maps.Polygon | google.maps.Polyline)[]>([]);
    const infoWindowRef = useRef<google.maps.InfoWindow | null>(null);

    useEffect(() => {
        if (!map) return;

        // Clean previous overlays
        overlaysRef.current.forEach((o) => o.setMap(null));
        overlaysRef.current = [];
        if (infoWindowRef.current) {
            infoWindowRef.current.close();
        }

        const filtered = areas.filter((a) => a.id !== excludeAreaId && a.geoJson);

        const showInfoWindow = (area: AdoptableAreaData, e: google.maps.MapMouseEvent) => {
            if (!infoWindowRef.current) {
                infoWindowRef.current = new google.maps.InfoWindow();
            }
            infoWindowRef.current.setContent(
                `<div style="font-size:13px"><strong>${area.name}</strong><br/><span style="color:#666">${area.status} &middot; ${area.areaType}</span></div>`,
            );
            infoWindowRef.current.setPosition(e.latLng);
            infoWindowRef.current.open(map);
        };

        filtered.forEach((area) => {
            const parsed = parseGeoJson(area.geoJson);
            if (!parsed) return;

            const style = STATUS_STYLES[area.status] ?? STATUS_STYLES.Available;

            if (parsed.type === 'Polygon') {
                const path = polygonCoordsToPath(parsed.coordinates);
                if (path.length < 3) return;

                const poly = new google.maps.Polygon({
                    map,
                    paths: path,
                    strokeColor: style.strokeColor,
                    strokeOpacity: 0.7,
                    strokeWeight: 2,
                    fillColor: style.fillColor,
                    fillOpacity: 0.15,
                    clickable: true,
                    zIndex: 1,
                });

                poly.addListener('click', (e: google.maps.MapMouseEvent) => showInfoWindow(area, e));
                overlaysRef.current.push(poly);
            } else if (parsed.type === 'LineString') {
                const path = lineStringCoordsToPath(parsed.coordinates);
                if (path.length < 2) return;

                const line = new google.maps.Polyline({
                    map,
                    path,
                    strokeColor: style.strokeColor,
                    strokeOpacity: 0.7,
                    strokeWeight: 3,
                    clickable: true,
                    zIndex: 1,
                });

                line.addListener('click', (e: google.maps.MapMouseEvent) => showInfoWindow(area, e));
                overlaysRef.current.push(line);
            }
        });

        return () => {
            overlaysRef.current.forEach((o) => o.setMap(null));
            overlaysRef.current = [];
            if (infoWindowRef.current) {
                infoWindowRef.current.close();
                infoWindowRef.current = null;
            }
        };
    }, [map, areas, excludeAreaId]);

    return null;
};
