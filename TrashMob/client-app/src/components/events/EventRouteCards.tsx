import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Pencil, Scissors, Trash2 } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { useToast } from '@/hooks/use-toast';
import { DisplayEventAttendeeRoute } from '@/components/Models/RouteData';
import {
    GetEventAttendeeRoutesByEventId,
    GetEventRoutes,
    GetEventRouteStats,
    DeleteEventAttendeeRoute,
} from '@/services/event-routes';
import { formatDistance, formatDuration, formatWeight, formatDensity } from '@/lib/route-format';
import { RouteMetadataDialog } from './RouteMetadataDialog';
import { EventRouteTimeTrimDialog } from './EventRouteTimeTrimDialog';
import moment from 'moment';

const privacyColors: Record<string, string> = {
    Public: 'bg-green-100 text-green-700',
    Private: 'bg-red-100 text-red-700',
};

interface EventRouteCardsProps {
    eventId: string;
    currentUserId: string;
}

export const EventRouteCards = ({ eventId, currentUserId }: EventRouteCardsProps) => {
    const { toast } = useToast();
    const queryClient = useQueryClient();

    const [editingRoute, setEditingRoute] = useState<DisplayEventAttendeeRoute | null>(null);
    const [trimmingRoute, setTrimmingRoute] = useState<DisplayEventAttendeeRoute | null>(null);
    const [deletingRouteId, setDeletingRouteId] = useState<string | null>(null);

    const { data: routes, isLoading } = useQuery({
        queryKey: GetEventAttendeeRoutesByEventId({ eventId }).key,
        queryFn: GetEventAttendeeRoutesByEventId({ eventId }).service,
        select: (res) => res.data,
        enabled: !!eventId,
    });

    const deleteMutation = useMutation({
        mutationFn: async (routeId: string) => {
            await DeleteEventAttendeeRoute({ routeId }).service();
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetEventAttendeeRoutesByEventId({ eventId }).key });
            queryClient.invalidateQueries({ queryKey: GetEventRoutes({ eventId }).key });
            queryClient.invalidateQueries({ queryKey: GetEventRouteStats({ eventId }).key });
            toast({ variant: 'primary', title: 'Route deleted' });
            setDeletingRouteId(null);
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to delete route. Please try again.' });
        },
    });

    const routeList = routes || [];

    if (isLoading || routeList.length === 0) {
        return null;
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle className='text-primary'>Routes ({routeList.length})</CardTitle>
            </CardHeader>
            <CardContent className='space-y-3'>
                {routeList.map((route, index) => {
                    const isOwner = route.userId === currentUserId;
                    return (
                        <div key={route.id} className='border rounded-lg p-4 space-y-2'>
                            <div className='flex items-center justify-between'>
                                <div className='flex items-center gap-2'>
                                    <span className='text-sm font-semibold text-muted-foreground'>
                                        Route {index + 1}
                                    </span>
                                    <span
                                        className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${
                                            privacyColors[route.privacyLevel] || 'bg-blue-100 text-blue-700'
                                        }`}
                                    >
                                        {route.privacyLevel === 'EventOnly' ? 'Event Only' : route.privacyLevel}
                                    </span>
                                    {route.isTimeTrimmed ? (
                                        <span className='inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium bg-amber-100 text-amber-700'>
                                            Trimmed
                                        </span>
                                    ) : null}
                                </div>
                                {isOwner ? (
                                    <div className='flex gap-1'>
                                        <Button
                                            variant='ghost'
                                            size='sm'
                                            onClick={() => setEditingRoute(route)}
                                            title='Edit route metadata'
                                        >
                                            <Pencil className='h-3.5 w-3.5' />
                                        </Button>
                                        <Button
                                            variant='ghost'
                                            size='sm'
                                            onClick={() => setTrimmingRoute(route)}
                                            title={route.isTimeTrimmed ? 'Manage time trim' : 'Trim end time'}
                                        >
                                            <Scissors className='h-3.5 w-3.5' />
                                        </Button>
                                        <Button
                                            variant='ghost'
                                            size='sm'
                                            onClick={() => setDeletingRouteId(route.id)}
                                            title='Delete route'
                                            className='text-destructive hover:text-destructive'
                                        >
                                            <Trash2 className='h-3.5 w-3.5' />
                                        </Button>
                                    </div>
                                ) : null}
                            </div>

                            <div className='flex flex-wrap gap-x-4 gap-y-1 text-sm'>
                                <span>{formatDistance(route.totalDistanceMeters)}</span>
                                <span>{formatDuration(route.durationMinutes)}</span>
                                <span className='text-muted-foreground'>
                                    {moment(route.startTime).format('h:mm A')} –{' '}
                                    {moment(route.endTime).format('h:mm A')}
                                </span>
                                {route.densityGramsPerMeter != null ? (
                                    <span className='flex items-center gap-1'>
                                        <span
                                            className='inline-block w-2.5 h-2.5 rounded-full'
                                            style={{ backgroundColor: route.densityColor }}
                                        />
                                        {formatDensity(route.densityGramsPerMeter)}
                                    </span>
                                ) : null}
                            </div>

                            {route.bagsCollected != null || route.weightCollected != null ? (
                                <div className='flex flex-wrap gap-x-4 gap-y-1 text-sm text-muted-foreground'>
                                    {route.bagsCollected != null ? (
                                        <span>
                                            {route.bagsCollected} bag{route.bagsCollected !== 1 ? 's' : ''}
                                        </span>
                                    ) : null}
                                    {route.weightCollected != null ? (
                                        <span>{formatWeight(route.weightCollected, route.weightUnitId)}</span>
                                    ) : null}
                                </div>
                            ) : null}

                            {route.notes ? <p className='text-sm text-muted-foreground italic'>{route.notes}</p> : null}
                        </div>
                    );
                })}
            </CardContent>

            <RouteMetadataDialog
                route={editingRoute}
                open={editingRoute !== null}
                onOpenChange={(open) => {
                    if (!open) setEditingRoute(null);
                }}
                eventId={eventId}
            />

            <EventRouteTimeTrimDialog
                route={trimmingRoute}
                open={trimmingRoute !== null}
                onOpenChange={(open) => {
                    if (!open) setTrimmingRoute(null);
                }}
                eventId={eventId}
            />

            <AlertDialog
                open={deletingRouteId !== null}
                onOpenChange={(open) => {
                    if (!open) setDeletingRouteId(null);
                }}
            >
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>Delete Route</AlertDialogTitle>
                        <AlertDialogDescription>
                            Are you sure you want to delete this route? This action cannot be undone.
                        </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                        <AlertDialogCancel>Cancel</AlertDialogCancel>
                        <AlertDialogAction
                            className='bg-destructive text-destructive-foreground hover:bg-destructive/90'
                            onClick={(e) => {
                                e.preventDefault();
                                if (deletingRouteId) deleteMutation.mutate(deletingRouteId);
                            }}
                            disabled={deleteMutation.isPending}
                        >
                            {deleteMutation.isPending ? 'Deleting...' : 'Delete'}
                        </AlertDialogAction>
                    </AlertDialogFooter>
                </AlertDialogContent>
            </AlertDialog>
        </Card>
    );
};
