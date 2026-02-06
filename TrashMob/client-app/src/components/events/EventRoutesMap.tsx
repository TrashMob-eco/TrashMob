import { useEffect, useMemo } from 'react';
import { useMap } from '@vis.gl/react-google-maps';
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

interface EventRoutesMapProps {
    eventId: string;
    defaultCenter?: { lat: number; lng: number };
}

export const EventRoutesMap = ({ eventId, defaultCenter }: EventRoutesMapProps) => {
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
            <GoogleMapWithKey defaultCenter={defaultCenter} style={{ width: '100%', height: '400px' }}>
                <RoutePolylines routes={routeList} />
            </GoogleMapWithKey>
        </div>
    );
};
