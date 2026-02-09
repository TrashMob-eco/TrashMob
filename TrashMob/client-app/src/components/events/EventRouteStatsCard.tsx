import { useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { GetEventRouteStats } from '@/services/event-routes';

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

function formatArea(sqMeters: number): string {
    const sqMetersPerAcre = 4046.86;
    if (sqMeters >= sqMetersPerAcre) {
        return `${(sqMeters / sqMetersPerAcre).toFixed(1)} acres`;
    }
    const sqFeet = sqMeters * 10.7639;
    return `${Math.round(sqFeet).toLocaleString()} sq ft`;
}

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
                                <p className='text-2xl font-bold'>{stats.totalWeightCollected.toFixed(1)} lbs</p>
                                <p className='text-sm text-muted-foreground'>Weight Collected</p>
                            </div>
                        )}
                    </div>
                )}
            </CardContent>
        </Card>
    );
};
