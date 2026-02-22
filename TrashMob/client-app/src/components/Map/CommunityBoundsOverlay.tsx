import { useEffect, useRef } from 'react';
import { useMap } from '@vis.gl/react-google-maps';

interface CommunityBoundsOverlayProps {
    mapId: string;
    /** Serialized GeoJSON geometry (Polygon or LineString) from Nominatim. */
    geoJson: string;
    /** When true, fit the map to the polygon bounds. */
    fitBounds?: boolean;
}

/**
 * Parses a GeoJSON geometry string and converts coordinates to Google Maps LatLng paths.
 * GeoJSON uses [longitude, latitude] order; Google Maps uses {lat, lng}.
 */
function parseGeoJsonPaths(geoJson: string): google.maps.LatLngLiteral[][] | null {
    try {
        const geo = JSON.parse(geoJson) as { type: string; coordinates: number[][][][] | number[][][] | number[][] };

        if (geo.type === 'MultiPolygon') {
            // MultiPolygon coordinates: number[][][][] (array of polygons, each with rings)
            const coords = geo.coordinates as number[][][][];
            return coords.flatMap((polygon) => polygon.map((ring) => ring.map(([lon, lat]) => ({ lat, lng: lon }))));
        }

        if (geo.type === 'Polygon') {
            // Polygon coordinates: number[][][] (array of rings, each ring is array of [lon, lat])
            const coords = geo.coordinates as number[][][];
            return coords.map((ring) => ring.map(([lon, lat]) => ({ lat, lng: lon })));
        }

        if (geo.type === 'LineString') {
            // LineString coordinates: number[][] (array of [lon, lat])
            const coords = geo.coordinates as number[][];
            return [coords.map(([lon, lat]) => ({ lat, lng: lon }))];
        }

        return null;
    } catch {
        return null;
    }
}

/**
 * Draws a community geographic boundary from GeoJSON on a Google Map.
 * Uses actual city/region outline polygon from Nominatim.
 */
export const CommunityBoundsOverlay = ({ mapId, geoJson, fitBounds }: CommunityBoundsOverlayProps) => {
    const map = useMap(mapId);
    const polygonRef = useRef<google.maps.Polygon | null>(null);

    useEffect(() => {
        if (!map || !geoJson) return;

        const paths = parseGeoJsonPaths(geoJson);
        if (!paths || paths.length === 0) return;

        if (!polygonRef.current) {
            polygonRef.current = new google.maps.Polygon({
                map,
                paths,
                strokeColor: '#E11D48',
                strokeOpacity: 0.9,
                strokeWeight: 2.5,
                fillColor: '#E11D48',
                fillOpacity: 0.12,
                clickable: false,
                zIndex: 0,
            });
        } else {
            polygonRef.current.setPaths(paths);
        }

        if (fitBounds) {
            const bounds = new google.maps.LatLngBounds();
            paths.forEach((ring) => ring.forEach((pt) => bounds.extend(pt)));
            map.fitBounds(bounds, 40);
        }

        return () => {
            if (polygonRef.current) {
                polygonRef.current.setMap(null);
                polygonRef.current = null;
            }
        };
    }, [map, geoJson, fitBounds]);

    return null;
};
