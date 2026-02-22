import { useEffect, useMemo, useState } from 'react';
import { useMap, useMapsLibrary } from '@vis.gl/react-google-maps';
import { useQuery } from '@tanstack/react-query';
import { GoogleMapWithKey } from '@/components/Map/GoogleMap/GoogleMap';
import { GetEventRoutes } from '@/services/event-routes';
import { DisplayAnonymizedRoute } from '@/components/Models/RouteData';
import { RoutePolylines } from '@/components/Map/RoutePolylines';

const HeatmapOverlay = ({ routes }: { routes: DisplayAnonymizedRoute[] }) => {
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
                {viewMode === 'routes' ? (
                    <RoutePolylines routes={routeList} fitBounds />
                ) : (
                    <HeatmapOverlay routes={routeList} />
                )}
            </GoogleMapWithKey>
        </div>
    );
};
