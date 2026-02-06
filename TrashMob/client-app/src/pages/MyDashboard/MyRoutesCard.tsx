import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { GetMyRoutes } from '@/services/event-routes';
import moment from 'moment';

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

export const MyRoutesCard = () => {
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
            <CardHeader>
                <CardTitle className='text-primary'>My Routes ({routeList.length})</CardTitle>
            </CardHeader>
            <CardContent>
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
                                            route.privacyLevel === 'Public'
                                                ? 'bg-green-100 text-green-700'
                                                : route.privacyLevel === 'Private'
                                                  ? 'bg-red-100 text-red-700'
                                                  : 'bg-blue-100 text-blue-700'
                                        }`}
                                    >
                                        {route.privacyLevel}
                                    </span>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </CardContent>
        </Card>
    );
};
