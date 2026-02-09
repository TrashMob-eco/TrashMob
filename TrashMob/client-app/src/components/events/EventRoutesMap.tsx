import { useEffect, useMemo, useState } from 'react';
import { useMap, useMapsLibrary } from '@vis.gl/react-google-maps';
import { useQuery } from '@tanstack/react-query';
import { GoogleMapWithKey } from '@/components/Map/GoogleMap/GoogleMap';
import { GetEventRoutes } from '@/services/event-routes';
import { DisplayAnonymizedRoute } from '@/components/Models/RouteData';

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

interface RoutePolylinesProps {
    routes: DisplayAnonymizedRoute[];
}

const RoutePolylines = ({ routes }: RoutePolylinesProps) => {
    const map = useMap();

    useEffect(() => {
        if (!map || routes.length === 0) return;

        const polylines: google.maps.Polyline[] = [];
        const bounds = new google.maps.LatLngBounds();

        routes.forEach((route, index) => {
            if (route.locations.length < 2) return;

            const path = route.locations
                .sort((a, b) => a.sortOrder - b.sortOrder)
                .map((loc) => {
                    const latLng = { lat: loc.latitude, lng: loc.longitude };
                    bounds.extend(latLng);
                    return latLng;
                });

            const polyline = new google.maps.Polyline({
                path,
                strokeColor: getRouteColor(index),
                strokeOpacity: 0.8,
                strokeWeight: 3,
                map,
            });

            polylines.push(polyline);
        });

        if (!bounds.isEmpty()) {
            map.fitBounds(bounds, { top: 50, right: 50, bottom: 50, left: 50 });
        }

        return () => {
            polylines.forEach((p) => p.setMap(null));
        };
    }, [map, routes]);

    return null;
};

const HeatmapOverlay = ({ routes }: RoutePolylinesProps) => {
    const map = useMap();
    const visualization = useMapsLibrary('visualization');

    useEffect(() => {
        if (!map || !visualization || routes.length === 0) return;

        const points: google.maps.LatLng[] = [];
        const bounds = new google.maps.LatLngBounds();

        routes.forEach((route) => {
            route.locations.forEach((loc) => {
                const latLng = new google.maps.LatLng(loc.latitude, loc.longitude);
                points.push(latLng);
                bounds.extend(latLng);
            });
        });

        const heatmap = new google.maps.visualization.HeatmapLayer({
            data: points,
            map,
            radius: 20,
            opacity: 0.7,
        });

        if (!bounds.isEmpty()) {
            map.fitBounds(bounds, { top: 50, right: 50, bottom: 50, left: 50 });
        }

        return () => {
            heatmap.setMap(null);
        };
    }, [map, visualization, routes]);

    return null;
};

type ViewMode = 'routes' | 'heatmap';

interface EventRoutesMapProps {
    eventId: string;
    defaultCenter?: { lat: number; lng: number };
}

export const EventRoutesMap = ({ eventId, defaultCenter }: EventRoutesMapProps) => {
    const [viewMode, setViewMode] = useState<ViewMode>('routes');

    const { data: routes, isLoading } = useQuery({
        queryKey: GetEventRoutes({ eventId }).key,
        queryFn: GetEventRoutes({ eventId }).service,
        select: (res) => res.data,
        enabled: !!eventId,
    });

    const routeList = useMemo(() => routes || [], [routes]);

    if (isLoading) {
        return (
            <div className='flex items-center justify-center h-64 bg-muted rounded-lg'>
                <p className='text-muted-foreground'>Loading routes...</p>
            </div>
        );
    }

    if (routeList.length === 0) {
        return (
            <div className='flex items-center justify-center h-32 bg-muted rounded-lg'>
                <p className='text-muted-foreground'>No routes recorded for this event yet.</p>
            </div>
        );
    }

    return (
        <div className='rounded-lg overflow-hidden'>
            <div className='flex gap-1 p-2 bg-muted'>
                <button
                    type='button'
                    onClick={() => setViewMode('routes')}
                    className={`px-3 py-1.5 text-sm font-medium rounded-md transition-colors ${
                        viewMode === 'routes'
                            ? 'bg-primary text-primary-foreground'
                            : 'text-muted-foreground hover:text-foreground'
                    }`}
                >
                    Routes
                </button>
                <button
                    type='button'
                    onClick={() => setViewMode('heatmap')}
                    className={`px-3 py-1.5 text-sm font-medium rounded-md transition-colors ${
                        viewMode === 'heatmap'
                            ? 'bg-primary text-primary-foreground'
                            : 'text-muted-foreground hover:text-foreground'
                    }`}
                >
                    Heatmap
                </button>
            </div>
            <GoogleMapWithKey defaultCenter={defaultCenter} style={{ width: '100%', height: '400px' }}>
                {viewMode === 'routes' ? <RoutePolylines routes={routeList} /> : <HeatmapOverlay routes={routeList} />}
            </GoogleMapWithKey>
        </div>
    );
};
