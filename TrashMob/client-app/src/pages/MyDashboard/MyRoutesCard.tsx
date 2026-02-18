import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router';
import { AdvancedMarker, InfoWindow } from '@vis.gl/react-google-maps';
import { List, Map as MapIcon } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import { GoogleMapWithKey } from '@/components/Map/GoogleMap';
import { GetMyRoutes } from '@/services/event-routes';
import { DisplayUserRouteHistory } from '@/components/Models/RouteData';
import moment from 'moment';

const MY_ROUTES_MAP_ID = 'myRoutesMap';

function formatDistance(meters: number): string {
    if (meters >= 1609) {
        return `${(meters / 1609.34).toFixed(1)} mi`;
    }
    return `${meters.toLocaleString()} m`;
}

function formatDuration(minutes: number): string {
    if (minutes >= 60) {
        const hours = Math.floor(minutes / 60);
        const mins = minutes % 60;
        return mins > 0 ? `${hours}h ${mins}m` : `${hours}h`;
    }
    return `${minutes}m`;
}

const privacyColors: Record<string, string> = {
    Public: 'bg-green-100 text-green-700',
    Private: 'bg-red-100 text-red-700',
};

const RouteMarkerPin = () => (
    <svg width='28' height='36' viewBox='0 0 28 36' fill='none' xmlns='http://www.w3.org/2000/svg'>
        <path d='M14 0C6.268 0 0 6.268 0 14c0 10.5 14 22 14 22s14-11.5 14-22C28 6.268 21.732 0 14 0z' fill='#7C3AED' />
        <circle cx='14' cy='14' r='6' fill='white' />
    </svg>
);

const RoutesMapView = ({ routes }: { routes: DisplayUserRouteHistory[] }) => {
    const [selectedRoute, setSelectedRoute] = useState<DisplayUserRouteHistory | null>(null);

    const routesWithLocation = routes.filter((r) => r.eventLatitude !== 0 && r.eventLongitude !== 0);

    if (routesWithLocation.length === 0) {
        return <p className='text-sm text-muted-foreground py-4 text-center'>No routes with location data available.</p>;
    }

    const avgLat = routesWithLocation.reduce((sum, r) => sum + r.eventLatitude, 0) / routesWithLocation.length;
    const avgLng = routesWithLocation.reduce((sum, r) => sum + r.eventLongitude, 0) / routesWithLocation.length;

    return (
        <div className='rounded-md overflow-hidden border'>
            <GoogleMapWithKey
                id={MY_ROUTES_MAP_ID}
                style={{ width: '100%', height: '400px' }}
                defaultCenter={{ lat: avgLat, lng: avgLng }}
                defaultZoom={10}
            >
                {routesWithLocation.map((route) => (
                    <AdvancedMarker
                        key={route.routeId}
                        position={{ lat: route.eventLatitude, lng: route.eventLongitude }}
                        onClick={() => setSelectedRoute(route)}
                    >
                        <RouteMarkerPin />
                    </AdvancedMarker>
                ))}

                {selectedRoute ? (
                    <InfoWindow
                        position={{ lat: selectedRoute.eventLatitude, lng: selectedRoute.eventLongitude }}
                        onCloseClick={() => setSelectedRoute(null)}
                    >
                        <div className='text-sm space-y-1 max-w-[200px]'>
                            <Link
                                to={`/eventdetails/${selectedRoute.eventId}`}
                                className='font-semibold text-primary hover:underline block'
                            >
                                {selectedRoute.eventName || 'Unknown Event'}
                            </Link>
                            <p className='text-muted-foreground'>
                                {moment(selectedRoute.eventDate).format('MM/DD/YYYY')}
                            </p>
                            <div className='flex gap-3'>
                                <span>{formatDistance(selectedRoute.totalDistanceMeters)}</span>
                                <span>{formatDuration(selectedRoute.durationMinutes)}</span>
                            </div>
                        </div>
                    </InfoWindow>
                ) : null}
            </GoogleMapWithKey>
        </div>
    );
};

export const MyRoutesCard = () => {
    const [viewMode, setViewMode] = useState<'table' | 'map'>('table');

    const { data: routes, isLoading } = useQuery({
        queryKey: GetMyRoutes().key,
        queryFn: GetMyRoutes().service,
        select: (res) => res.data,
    });

    const routeList = routes || [];

    if (isLoading) {
        return null;
    }

    if (routeList.length === 0) {
        return (
            <Card className='mb-4'>
                <CardHeader>
                    <CardTitle className='text-primary'>My Routes</CardTitle>
                </CardHeader>
                <CardContent>
                    <p className='text-muted-foreground'>
                        No routes recorded yet. Track your route during a cleanup event using the mobile app!
                    </p>
                </CardContent>
            </Card>
        );
    }

    return (
        <Card className='mb-4'>
            <CardHeader className='flex flex-row items-center justify-between'>
                <CardTitle className='text-primary'>My Routes ({routeList.length})</CardTitle>
                <div className='flex border rounded-md'>
                    <Button
                        variant={viewMode === 'table' ? 'default' : 'ghost'}
                        size='sm'
                        className='rounded-r-none'
                        onClick={() => setViewMode('table')}
                    >
                        <List className='h-4 w-4' />
                    </Button>
                    <Button
                        variant={viewMode === 'map' ? 'default' : 'ghost'}
                        size='sm'
                        className='rounded-l-none'
                        onClick={() => setViewMode('map')}
                    >
                        <MapIcon className='h-4 w-4' />
                    </Button>
                </div>
            </CardHeader>
            <CardContent>
                {viewMode === 'map' ? (
                    <RoutesMapView routes={routeList} />
                ) : (
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Event</TableHead>
                                <TableHead>Date</TableHead>
                                <TableHead>Distance</TableHead>
                                <TableHead>Duration</TableHead>
                                <TableHead>Privacy</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {routeList.map((route) => (
                                <TableRow key={route.routeId}>
                                    <TableCell>
                                        <Link
                                            to={`/eventdetails/${route.eventId}`}
                                            className='text-primary hover:underline'
                                        >
                                            {route.eventName || 'Unknown Event'}
                                        </Link>
                                    </TableCell>
                                    <TableCell>{moment(route.eventDate).format('MM/DD/YYYY')}</TableCell>
                                    <TableCell>{formatDistance(route.totalDistanceMeters)}</TableCell>
                                    <TableCell>{formatDuration(route.durationMinutes)}</TableCell>
                                    <TableCell>
                                        <span
                                            className={`inline-flex items-center rounded-full px-2 py-1 text-xs font-medium ${
                                                privacyColors[route.privacyLevel] || 'bg-blue-100 text-blue-700'
                                            }`}
                                        >
                                            {route.privacyLevel}
                                        </span>
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                )}
            </CardContent>
        </Card>
    );
};
