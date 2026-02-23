import { useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { GetEventRouteStats } from '@/services/event-routes';
import { formatDistance, formatDuration, formatArea, formatDensity } from '@/lib/route-format';

interface EventRouteStatsCardProps {
    eventId: string;
}

export const EventRouteStatsCard = ({ eventId }: EventRouteStatsCardProps) => {
    const { data: stats, isLoading } = useQuery({
        queryKey: GetEventRouteStats({ eventId }).key,
        queryFn: GetEventRouteStats({ eventId }).service,
        select: (res) => res.data,
        enabled: !!eventId,
    });

    if (isLoading) {
        return null;
    }

    if (!stats || stats.totalRoutes === 0) {
        return null;
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle className='text-primary'>Route Tracking</CardTitle>
            </CardHeader>
            <CardContent>
                <div className='grid grid-cols-2 md:grid-cols-4 gap-4'>
                    <div className='text-center'>
                        <p className='text-2xl font-bold'>{stats.totalRoutes}</p>
                        <p className='text-sm text-muted-foreground'>Routes</p>
                    </div>
                    <div className='text-center'>
                        <p className='text-2xl font-bold'>{formatDistance(stats.totalDistanceMeters)}</p>
                        <p className='text-sm text-muted-foreground'>Distance</p>
                    </div>
                    <div className='text-center'>
                        <p className='text-2xl font-bold'>{formatDuration(stats.totalDurationMinutes)}</p>
                        <p className='text-sm text-muted-foreground'>Total Time</p>
                    </div>
                    <div className='text-center'>
                        <p className='text-2xl font-bold'>{stats.uniqueContributors}</p>
                        <p className='text-sm text-muted-foreground'>Contributors</p>
                    </div>
                    {stats.coverageAreaSquareMeters > 0 && (
                        <div className='text-center'>
                            <p className='text-2xl font-bold'>{formatArea(stats.coverageAreaSquareMeters)}</p>
                            <p className='text-sm text-muted-foreground'>Area Covered</p>
                        </div>
                    )}
                </div>
                {stats.averageDensityGramsPerMeter != null && (
                    <div className='grid grid-cols-2 gap-4 mt-4 pt-4 border-t'>
                        <div className='text-center'>
                            <p className='text-2xl font-bold'>
                                {formatDensity(stats.averageDensityGramsPerMeter)}
                            </p>
                            <p className='text-sm text-muted-foreground'>Avg Density</p>
                        </div>
                        {stats.maxDensityGramsPerMeter != null && (
                            <div className='text-center'>
                                <p className='text-2xl font-bold'>
                                    {formatDensity(stats.maxDensityGramsPerMeter)}
                                </p>
                                <p className='text-sm text-muted-foreground'>Max Density</p>
                            </div>
                        )}
                    </div>
                )}
                {(stats.totalBagsCollected > 0 || stats.totalWeightCollected > 0) && (
                    <div className='grid grid-cols-2 gap-4 mt-4 pt-4 border-t'>
                        {stats.totalBagsCollected > 0 && (
                            <div className='text-center'>
                                <p className='text-2xl font-bold'>{stats.totalBagsCollected}</p>
                                <p className='text-sm text-muted-foreground'>Bags Collected</p>
                            </div>
                        )}
                        {stats.totalWeightCollected > 0 && (
                            <div className='text-center'>
                                <p className='text-2xl font-bold'>
                                    {stats.totalWeightCollected.toFixed(1)}{' '}
                                    {stats.totalWeightUnitId === 2 ? 'kg' : 'lbs'}
                                </p>
                                <p className='text-sm text-muted-foreground'>Weight Collected</p>
                            </div>
                        )}
                    </div>
                )}
            </CardContent>
        </Card>
    );
};
