import { FC } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Loader2, Package, Scale, Clock, Users, Trophy } from 'lucide-react';

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Badge } from '@/components/ui/badge';
import { GetPublicMetrics } from '@/services/event-attendee-metrics';

interface EventContributorsCardProps {
    eventId: string;
    isEventCompleted: boolean;
}

export const EventContributorsCard: FC<EventContributorsCardProps> = ({ eventId, isEventCompleted }) => {
    const { data: publicMetrics, isLoading } = useQuery({
        queryKey: GetPublicMetrics({ eventId }).key,
        queryFn: GetPublicMetrics({ eventId }).service,
        select: (res) => res.data,
        enabled: isEventCompleted,
    });

    if (!isEventCompleted) {
        return null;
    }

    if (isLoading) {
        return (
            <Card>
                <CardContent className='py-8 text-center'>
                    <Loader2 className='h-8 w-8 animate-spin mx-auto' />
                </CardContent>
            </Card>
        );
    }

    if (!publicMetrics || publicMetrics.contributorCount === 0) {
        return null;
    }

    const formatWeight = (pounds: number): string => {
        if (pounds >= 1) {
            return `${pounds.toFixed(1)} lbs`;
        }
        return `${(pounds * 16).toFixed(0)} oz`;
    };

    const formatDuration = (minutes: number): string => {
        if (minutes >= 60) {
            const hours = Math.floor(minutes / 60);
            const mins = minutes % 60;
            return mins > 0 ? `${hours}h ${mins}m` : `${hours}h`;
        }
        return `${minutes}m`;
    };

    return (
        <Card>
            <CardHeader>
                <div className='flex items-center justify-between'>
                    <div>
                        <CardTitle className='flex items-center gap-2'>
                            <Trophy className='h-5 w-5 text-primary' />
                            Event Impact
                        </CardTitle>
                        <CardDescription>
                            {publicMetrics.contributorCount} attendee
                            {publicMetrics.contributorCount !== 1 ? 's' : ''} reported their contributions
                        </CardDescription>
                    </div>
                </div>
            </CardHeader>
            <CardContent className='space-y-6'>
                <div className='grid gap-4 md:grid-cols-4'>
                    <div className='flex items-center gap-3'>
                        <div className='p-2 bg-primary/10 rounded-lg'>
                            <Users className='h-5 w-5 text-primary' />
                        </div>
                        <div>
                            <p className='text-sm text-muted-foreground'>Contributors</p>
                            <p className='text-2xl font-bold'>{publicMetrics.contributorCount}</p>
                        </div>
                    </div>
                    <div className='flex items-center gap-3'>
                        <div className='p-2 bg-primary/10 rounded-lg'>
                            <Package className='h-5 w-5 text-primary' />
                        </div>
                        <div>
                            <p className='text-sm text-muted-foreground'>Bags Collected</p>
                            <p className='text-2xl font-bold'>{publicMetrics.totalBagsCollected}</p>
                        </div>
                    </div>
                    <div className='flex items-center gap-3'>
                        <div className='p-2 bg-primary/10 rounded-lg'>
                            <Scale className='h-5 w-5 text-primary' />
                        </div>
                        <div>
                            <p className='text-sm text-muted-foreground'>Weight Collected</p>
                            <p className='text-2xl font-bold'>{formatWeight(publicMetrics.totalWeightPounds)}</p>
                        </div>
                    </div>
                    <div className='flex items-center gap-3'>
                        <div className='p-2 bg-primary/10 rounded-lg'>
                            <Clock className='h-5 w-5 text-primary' />
                        </div>
                        <div>
                            <p className='text-sm text-muted-foreground'>Total Time</p>
                            <p className='text-2xl font-bold'>{formatDuration(publicMetrics.totalDurationMinutes)}</p>
                        </div>
                    </div>
                </div>

                {publicMetrics.contributors.length > 0 ? (
                    <div>
                        <h4 className='font-medium mb-3'>Top Contributors</h4>
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead className='w-12'>#</TableHead>
                                    <TableHead>Volunteer</TableHead>
                                    <TableHead className='text-right'>Bags</TableHead>
                                    <TableHead className='text-right'>Weight</TableHead>
                                    <TableHead className='text-right'>Time</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {publicMetrics.contributors.map((contributor, index) => (
                                    <TableRow key={contributor.userId}>
                                        <TableCell>
                                            {index < 3 ? (
                                                <Badge
                                                    variant={index === 0 ? 'default' : 'secondary'}
                                                    className={
                                                        index === 0
                                                            ? 'bg-yellow-500'
                                                            : index === 1
                                                              ? 'bg-gray-400'
                                                              : 'bg-orange-400'
                                                    }
                                                >
                                                    {index + 1}
                                                </Badge>
                                            ) : (
                                                <span className='text-muted-foreground'>{index + 1}</span>
                                            )}
                                        </TableCell>
                                        <TableCell className='font-medium'>{contributor.userName}</TableCell>
                                        <TableCell className='text-right'>{contributor.bagsCollected ?? '-'}</TableCell>
                                        <TableCell className='text-right'>
                                            {contributor.weightPounds ? formatWeight(contributor.weightPounds) : '-'}
                                        </TableCell>
                                        <TableCell className='text-right'>
                                            {contributor.durationMinutes
                                                ? formatDuration(contributor.durationMinutes)
                                                : '-'}
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </div>
                ) : null}
            </CardContent>
        </Card>
    );
};

export default EventContributorsCard;
