import { useEffect, useRef } from 'react';
import { useMap } from '@vis.gl/react-google-maps';
import { DisplayAnonymizedRoute } from '@/components/Models/RouteData';
import { formatDistance, formatDuration, formatWeight, formatDensity } from '@/lib/route-format';

export type RouteColorMode = 'index' | 'density';

const ROUTE_COLORS = [
    '#3B82F6',
    '#EF4444',
    '#10B981',
    '#F59E0B',
    '#8B5CF6',
    '#EC4899',
    '#06B6D4',
    '#F97316',
    '#6366F1',
    '#14B8A6',
];

function getRouteColor(index: number): string {
    return ROUTE_COLORS[index % ROUTE_COLORS.length];
}

function buildInfoContent(route: DisplayAnonymizedRoute, colorMode: RouteColorMode): string {
    const lines: string[] = [];
    lines.push(`<strong>${formatDistance(route.totalDistanceMeters)}</strong>`);
    lines.push(formatDuration(route.durationMinutes));
    if (route.bagsCollected && route.bagsCollected > 0) {
        lines.push(`${route.bagsCollected} bag${route.bagsCollected !== 1 ? 's' : ''}`);
    }
    if (route.weightCollected && route.weightCollected > 0) {
        lines.push(formatWeight(route.weightCollected, route.weightUnitId));
    }
    if (colorMode === 'density' && route.densityGramsPerMeter != null) {
        lines.push(formatDensity(route.densityGramsPerMeter));
    }
    return `<div style="font-size:12px;line-height:1.4">${lines.join(' &middot; ')}</div>`;
}

interface RoutePolylinesProps {
    mapId?: string;
    routes: DisplayAnonymizedRoute[];
    fitBounds?: boolean;
    colorMode?: RouteColorMode;
}

export const RoutePolylines = ({ mapId, routes, fitBounds = false, colorMode = 'index' }: RoutePolylinesProps) => {
    const map = useMap(mapId);
    const polylinesRef = useRef<google.maps.Polyline[]>([]);
    const infoWindowRef = useRef<google.maps.InfoWindow | null>(null);

    useEffect(() => {
        if (!map || routes.length === 0) return;

        // Clean previous
        polylinesRef.current.forEach((p) => p.setMap(null));
        polylinesRef.current = [];
        if (infoWindowRef.current) {
            infoWindowRef.current.close();
        }

        const bounds = fitBounds ? new google.maps.LatLngBounds() : null;

        routes.forEach((route, index) => {
            if (route.locations.length < 2) return;

            const path = route.locations
                .sort((a, b) => a.sortOrder - b.sortOrder)
                .map((loc) => {
                    const latLng = { lat: loc.latitude, lng: loc.longitude };
                    if (bounds) bounds.extend(latLng);
                    return latLng;
                });

            const strokeColor =
                colorMode === 'density' ? (route.densityColor || '#9E9E9E') : getRouteColor(index);

            const polyline = new google.maps.Polyline({
                path,
                strokeColor,
                strokeOpacity: 0.8,
                strokeWeight: 3,
                map,
            });

            polyline.addListener('mouseover', (e: google.maps.MapMouseEvent) => {
                if (!infoWindowRef.current) {
                    infoWindowRef.current = new google.maps.InfoWindow();
                }
                infoWindowRef.current.setContent(buildInfoContent(route, colorMode));
                if (e.latLng) {
                    infoWindowRef.current.setPosition(e.latLng);
                }
                infoWindowRef.current.open(map);
            });

            polyline.addListener('mouseout', () => {
                if (infoWindowRef.current) {
                    infoWindowRef.current.close();
                }
            });

            polylinesRef.current.push(polyline);
        });

        if (bounds && !bounds.isEmpty()) {
            map.fitBounds(bounds, { top: 50, right: 50, bottom: 50, left: 50 });
        }

        return () => {
            polylinesRef.current.forEach((p) => p.setMap(null));
            polylinesRef.current = [];
            if (infoWindowRef.current) {
                infoWindowRef.current.close();
                infoWindowRef.current = null;
            }
        };
    }, [map, routes, fitBounds, colorMode]);

    return null;
};
