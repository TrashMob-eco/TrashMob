import { FC } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router';
import { Loader2, Package, Scale, Clock, CalendarCheck, ChevronRight, Trophy } from 'lucide-react';
import moment from 'moment';

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { GetUserImpact } from '@/services/event-attendee-metrics';

interface MyImpactCardProps {
    userId: string;
    prefersMetric?: boolean;
}

export const MyImpactCard: FC<MyImpactCardProps> = ({ userId, prefersMetric = false }) => {
    const { data: impactStats, isLoading } = useQuery({
        queryKey: GetUserImpact({ userId }).key,
        queryFn: GetUserImpact({ userId }).service,
        select: (res) => res.data,
        enabled: !!userId,
    });

    if (isLoading) {
        return (
            <Card>
                <CardContent className='py-8 text-center'>
                    <Loader2 className='h-8 w-8 animate-spin mx-auto' />
                </CardContent>
            </Card>
        );
    }

    if (!impactStats || impactStats.eventsWithMetrics === 0) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <Trophy className='h-5 w-5 text-primary' />
                        My Verified Impact
                    </CardTitle>
                    <CardDescription>Your approved contributions from event metrics submissions</CardDescription>
                </CardHeader>
                <CardContent>
                    <p className='text-muted-foreground text-center py-6'>
                        No verified metrics yet. After attending events, submit your metrics and they will appear here
                        once approved by event leads.
                    </p>
                </CardContent>
            </Card>
        );
    }

    const formatWeight = (value: number): string => {
        if (value >= 1) {
            return `${value.toFixed(1)}`;
        }
        return '< 1';
    };

    const formatDuration = (minutes: number): string => {
        if (minutes >= 60) {
            const hours = Math.floor(minutes / 60);
            const mins = minutes % 60;
            return mins > 0 ? `${hours}h ${mins}m` : `${hours}h`;
        }
        return `${minutes}m`;
    };

    const weightValue = prefersMetric ? impactStats.totalWeightKilograms : impactStats.totalWeightPounds;
    const weightUnit = prefersMetric ? 'kg' : 'lbs';

    return (
        <Card>
            <CardHeader>
                <CardTitle className='flex items-center gap-2'>
                    <Trophy className='h-5 w-5 text-primary' />
                    My Verified Impact
                </CardTitle>
                <CardDescription>
                    Your approved contributions from {impactStats.eventsWithMetrics} event
                    {impactStats.eventsWithMetrics !== 1 ? 's' : ''}
                </CardDescription>
            </CardHeader>
            <CardContent className='space-y-6'>
                <div className='grid gap-4 sm:grid-cols-2 lg:grid-cols-4'>
                    <div className='flex items-center gap-3 p-3 bg-muted/50 rounded-lg'>
                        <div className='p-2 bg-primary/10 rounded-lg'>
                            <CalendarCheck className='h-5 w-5 text-primary' />
                        </div>
                        <div>
                            <p className='text-sm text-muted-foreground'>Events</p>
                            <p className='text-xl font-bold'>{impactStats.eventsWithMetrics}</p>
                        </div>
                    </div>
                    <div className='flex items-center gap-3 p-3 bg-muted/50 rounded-lg'>
                        <div className='p-2 bg-primary/10 rounded-lg'>
                            <Package className='h-5 w-5 text-primary' />
                        </div>
                        <div>
                            <p className='text-sm text-muted-foreground'>Bags</p>
                            <p className='text-xl font-bold'>{impactStats.totalBagsCollected}</p>
                        </div>
                    </div>
                    <div className='flex items-center gap-3 p-3 bg-muted/50 rounded-lg'>
                        <div className='p-2 bg-primary/10 rounded-lg'>
                            <Scale className='h-5 w-5 text-primary' />
                        </div>
                        <div>
                            <p className='text-sm text-muted-foreground'>Weight ({weightUnit})</p>
                            <p className='text-xl font-bold'>{formatWeight(weightValue)}</p>
                        </div>
                    </div>
                    <div className='flex items-center gap-3 p-3 bg-muted/50 rounded-lg'>
                        <div className='p-2 bg-primary/10 rounded-lg'>
                            <Clock className='h-5 w-5 text-primary' />
                        </div>
                        <div>
                            <p className='text-sm text-muted-foreground'>Time</p>
                            <p className='text-xl font-bold'>{formatDuration(impactStats.totalDurationMinutes)}</p>
                        </div>
                    </div>
                </div>

                {impactStats.eventBreakdown.length > 0 ? (
                    <div>
                        <h4 className='font-medium mb-3'>Recent Event Breakdown</h4>
                        <div className='overflow-auto'>
                            <Table>
                                <TableHeader>
                                    <TableRow>
                                        <TableHead>Event</TableHead>
                                        <TableHead>Date</TableHead>
                                        <TableHead className='text-right'>Bags</TableHead>
                                        <TableHead className='text-right'>Weight</TableHead>
                                        <TableHead className='text-right'>Time</TableHead>
                                        <TableHead />
                                    </TableRow>
                                </TableHeader>
                                <TableBody>
                                    {impactStats.eventBreakdown.slice(0, 5).map((event) => (
                                        <TableRow key={event.eventId}>
                                            <TableCell className='font-medium max-w-48 truncate'>
                                                {event.eventName}
                                            </TableCell>
                                            <TableCell className='text-muted-foreground'>
                                                {moment(event.eventDate).format('MMM D, YYYY')}
                                            </TableCell>
                                            <TableCell className='text-right'>{event.bagsCollected}</TableCell>
                                            <TableCell className='text-right'>
                                                {formatWeight(
                                                    prefersMetric ? event.weightPounds / 2.20462 : event.weightPounds,
                                                )}{' '}
                                                {weightUnit}
                                            </TableCell>
                                            <TableCell className='text-right'>
                                                {formatDuration(event.durationMinutes)}
                                            </TableCell>
                                            <TableCell>
                                                <Badge
                                                    variant={event.status === 'Approved' ? 'default' : 'secondary'}
                                                    className='text-xs'
                                                >
                                                    {event.status === 'Adjusted' ? 'Approved' : event.status}
                                                </Badge>
                                            </TableCell>
                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                        </div>
                        {impactStats.eventBreakdown.length > 5 ? (
                            <div className='mt-4 text-center'>
                                <Button variant='ghost' size='sm' asChild>
                                    <Link to='/my-impact'>
                                        View all {impactStats.eventsWithMetrics} events
                                        <ChevronRight className='h-4 w-4 ml-1' />
                                    </Link>
                                </Button>
                            </div>
                        ) : null}
                    </div>
                ) : null}
            </CardContent>
        </Card>
    );
};

export default MyImpactCard;
