import { useEffect, useRef, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router';
import { useMap, InfoWindow } from '@vis.gl/react-google-maps';
import { List, Map as MapIcon, Scissors } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import { GoogleMapWithKey } from '@/components/Map/GoogleMap';
import { GetMyRoutes } from '@/services/event-routes';
import { DisplayUserRouteHistory } from '@/components/Models/RouteData';
import { formatDistance, formatDuration, formatDensity } from '@/lib/route-format';
import { RouteTimeTrimDialog } from '@/components/events/RouteTimeTrimDialog';
import moment from 'moment';

const MY_ROUTES_MAP_ID = 'myRoutesMap';

const privacyColors: Record<string, string> = {
    Public: 'bg-green-100 text-green-700',
    Private: 'bg-red-100 text-red-700',
};

// Distinct colors for different routes
const ROUTE_COLORS = ['#7C3AED', '#E11D48', '#F59E0B', '#10B981', '#3B82F6', '#EC4899', '#8B5CF6', '#14B8A6'];

const RoutePolylines = ({
    routes,
    onRouteClick,
}: {
    routes: DisplayUserRouteHistory[];
    onRouteClick: (route: DisplayUserRouteHistory, position: google.maps.LatLngLiteral) => void;
}) => {
    const map = useMap(MY_ROUTES_MAP_ID);
    const polylinesRef = useRef<google.maps.Polyline[]>([]);

    useEffect(() => {
        if (!map) return;

        // Clear previous polylines
        polylinesRef.current.forEach((p) => p.setMap(null));
        polylinesRef.current = [];

        const routesWithPath = routes.filter((r) => r.locations && r.locations.length >= 2);

        // Fit map to all route bounds
        if (routesWithPath.length > 0) {
            const bounds = new google.maps.LatLngBounds();
            routesWithPath.forEach((route) => {
                route.locations.forEach((loc) => bounds.extend({ lat: loc.latitude, lng: loc.longitude }));
            });
            map.fitBounds(bounds, 40);
        }

        routesWithPath.forEach((route, index) => {
            const path = route.locations
                .sort((a, b) => a.sortOrder - b.sortOrder)
                .map((loc) => ({ lat: loc.latitude, lng: loc.longitude }));

            const color = route.densityColor || ROUTE_COLORS[index % ROUTE_COLORS.length];

            const polyline = new google.maps.Polyline({
                map,
                path,
                strokeColor: color,
                strokeOpacity: 0.9,
                strokeWeight: 4,
                clickable: true,
                zIndex: 1,
            });

            polyline.addListener('click', (e: google.maps.MapMouseEvent) => {
                const position = e.latLng
                    ? { lat: e.latLng.lat(), lng: e.latLng.lng() }
                    : { lat: route.eventLatitude, lng: route.eventLongitude };
                onRouteClick(route, position);
            });

            polylinesRef.current.push(polyline);
        });

        return () => {
            polylinesRef.current.forEach((p) => p.setMap(null));
            polylinesRef.current = [];
        };
    }, [map, routes, onRouteClick]);

    return null;
};

const RoutesMapView = ({ routes }: { routes: DisplayUserRouteHistory[] }) => {
    const [selectedRoute, setSelectedRoute] = useState<{
        route: DisplayUserRouteHistory;
        position: google.maps.LatLngLiteral;
    } | null>(null);

    const routesWithPath = routes.filter((r) => r.locations && r.locations.length >= 2);

    // Fall back to event location for routes without GPS path data
    const routesWithLocationOnly = routes.filter(
        (r) => (!r.locations || r.locations.length < 2) && r.eventLatitude !== 0 && r.eventLongitude !== 0,
    );

    if (routesWithPath.length === 0 && routesWithLocationOnly.length === 0) {
        return (
            <p className='text-sm text-muted-foreground py-4 text-center'>No routes with location data available.</p>
        );
    }

    const defaultCenter =
        routesWithPath.length > 0
            ? { lat: routesWithPath[0].locations[0].latitude, lng: routesWithPath[0].locations[0].longitude }
            : { lat: routesWithLocationOnly[0].eventLatitude, lng: routesWithLocationOnly[0].eventLongitude };

    return (
        <div className='rounded-md overflow-hidden border'>
            <GoogleMapWithKey
                id={MY_ROUTES_MAP_ID}
                style={{ width: '100%', height: '400px' }}
                defaultCenter={defaultCenter}
                defaultZoom={12}
            >
                <RoutePolylines
                    routes={routesWithPath}
                    onRouteClick={(route, position) => setSelectedRoute({ route, position })}
                />

                {selectedRoute ? (
                    <InfoWindow position={selectedRoute.position} onCloseClick={() => setSelectedRoute(null)}>
                        <div className='text-sm space-y-1 max-w-[200px]'>
                            <Link
                                to={`/eventdetails/${selectedRoute.route.eventId}`}
                                className='font-semibold text-primary hover:underline block'
                            >
                                {selectedRoute.route.eventName || 'Unknown Event'}
                            </Link>
                            <p className='text-muted-foreground'>
                                {moment(selectedRoute.route.eventDate).format('MM/DD/YYYY')}
                            </p>
                            <div className='flex gap-3'>
                                <span>{formatDistance(selectedRoute.route.totalDistanceMeters)}</span>
                                <span>{formatDuration(selectedRoute.route.durationMinutes)}</span>
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
    const [trimRoute, setTrimRoute] = useState<DisplayUserRouteHistory | null>(null);

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
                                <TableHead>Density</TableHead>
                                <TableHead>Privacy</TableHead>
                                <TableHead />
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
                                        {route.densityGramsPerMeter != null ? (
                                            <span className='flex items-center gap-1.5'>
                                                <span
                                                    className='inline-block w-2.5 h-2.5 rounded-full'
                                                    style={{ backgroundColor: route.densityColor }}
                                                />
                                                {formatDensity(route.densityGramsPerMeter)}
                                            </span>
                                        ) : (
                                            <span className='text-muted-foreground'>--</span>
                                        )}
                                    </TableCell>
                                    <TableCell>
                                        <span
                                            className={`inline-flex items-center rounded-full px-2 py-1 text-xs font-medium ${
                                                privacyColors[route.privacyLevel] || 'bg-blue-100 text-blue-700'
                                            }`}
                                        >
                                            {route.privacyLevel}
                                        </span>
                                    </TableCell>
                                    <TableCell>
                                        <Button
                                            variant='ghost'
                                            size='sm'
                                            onClick={() => setTrimRoute(route)}
                                            title='Trim route end time'
                                        >
                                            <Scissors className='h-3.5 w-3.5' />
                                            {route.isTimeTrimmed ? (
                                                <span className='ml-1 text-xs text-muted-foreground'>Trimmed</span>
                                            ) : null}
                                        </Button>
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                )}
            </CardContent>
            <RouteTimeTrimDialog
                route={trimRoute}
                open={trimRoute !== null}
                onOpenChange={(open) => {
                    if (!open) setTrimRoute(null);
                }}
            />
        </Card>
    );
};
