import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { TrimRouteTime, RestoreRouteTime, GetMyRoutes } from '@/services/event-routes';
import { DisplayUserRouteHistory } from '@/components/Models/RouteData';
import { formatDistance, formatDuration } from '@/lib/route-format';
import moment from 'moment';

interface RouteTimeTrimDialogProps {
    route: DisplayUserRouteHistory | null;
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

export const RouteTimeTrimDialog = ({ route, open, onOpenChange }: RouteTimeTrimDialogProps) => {
    const queryClient = useQueryClient();
    const [trimMinutes, setTrimMinutes] = useState(0);
    const [error, setError] = useState<string | null>(null);

    const startTime = route ? moment(route.startTime) : moment();
    const endTime = route ? moment(route.endTime) : moment();
    const totalMinutes = Math.max(endTime.diff(startTime, 'minutes'), 1);

    const newEndTime = moment(startTime).add(totalMinutes - trimMinutes, 'minutes');
    const removedDistance = route
        ? Math.round((route.totalDistanceMeters / totalMinutes) * trimMinutes)
        : 0;

    const trimMutation = useMutation({
        mutationFn: async () => {
            if (!route) return;
            const result = await TrimRouteTime({
                routeId: route.routeId,
                newEndTime: newEndTime.toISOString(),
            }).service();
            return result.data;
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetMyRoutes().key });
            onOpenChange(false);
        },
        onError: (err: Error) => {
            setError(err.message || 'Failed to trim route.');
        },
    });

    const restoreMutation = useMutation({
        mutationFn: async () => {
            if (!route) return;
            const result = await RestoreRouteTime({ routeId: route.routeId }).service();
            return result.data;
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetMyRoutes().key });
            onOpenChange(false);
        },
        onError: (err: Error) => {
            setError(err.message || 'Failed to restore route.');
        },
    });

    const handleOpenChange = (newOpen: boolean) => {
        if (!newOpen) {
            setTrimMinutes(0);
            setError(null);
        }
        onOpenChange(newOpen);
    };

    if (!route) return null;

    const isBusy = trimMutation.isPending || restoreMutation.isPending;

    return (
        <Dialog open={open} onOpenChange={handleOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>Trim Route End Time</DialogTitle>
                    <DialogDescription>
                        Remove time from the end of your route (e.g., if you forgot to stop tracking).
                    </DialogDescription>
                </DialogHeader>

                <div className='space-y-4'>
                    <div className='grid grid-cols-2 gap-3 text-sm'>
                        <div>
                            <p className='text-muted-foreground'>Current distance</p>
                            <p className='font-medium'>{formatDistance(route.totalDistanceMeters)}</p>
                        </div>
                        <div>
                            <p className='text-muted-foreground'>Current duration</p>
                            <p className='font-medium'>{formatDuration(route.durationMinutes)}</p>
                        </div>
                    </div>

                    <div>
                        <label className='block text-sm font-medium mb-1'>
                            Remove from end: {trimMinutes} min
                        </label>
                        <input
                            type='range'
                            min={0}
                            max={Math.max(totalMinutes - 1, 1)}
                            value={trimMinutes}
                            onChange={(e) => setTrimMinutes(Number(e.target.value))}
                            className='w-full accent-primary'
                        />
                        <div className='flex justify-between text-xs text-muted-foreground mt-1'>
                            <span>0 min</span>
                            <span>{totalMinutes - 1} min</span>
                        </div>
                    </div>

                    {trimMinutes > 0 && (
                        <div className='bg-muted rounded-md p-3 text-sm space-y-1'>
                            <p>
                                New end time:{' '}
                                <span className='font-medium'>{newEndTime.format('h:mm A')}</span>
                            </p>
                            <p>
                                New duration:{' '}
                                <span className='font-medium'>
                                    {formatDuration(totalMinutes - trimMinutes)}
                                </span>
                            </p>
                            <p>
                                Est. distance removed:{' '}
                                <span className='font-medium'>~{formatDistance(removedDistance)}</span>
                            </p>
                        </div>
                    )}

                    {error ? <p className='text-sm text-destructive'>{error}</p> : null}
                </div>

                <DialogFooter>
                    {route.isTimeTrimmed ? (
                        <Button
                            variant='outline'
                            onClick={() => restoreMutation.mutate()}
                            disabled={isBusy}
                        >
                            Restore Original
                        </Button>
                    ) : null}
                    <Button
                        onClick={() => trimMutation.mutate()}
                        disabled={trimMinutes === 0 || isBusy}
                    >
                        {trimMutation.isPending ? 'Saving...' : 'Save Trim'}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};
