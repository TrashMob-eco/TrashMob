import { CountUp } from 'use-count-up';
import { Calendar, Users, Trash2, Scale, Clock, FileText } from 'lucide-react';
import StatsData from '@/components/Models/StatsData';
import { useIsInViewport } from '@/hooks/useIsInViewport';
import { useLogin } from '@/hooks/useLogin';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';

interface CommunityStatsWidgetProps {
    stats: StatsData;
}

export const CommunityStatsWidget = ({ stats }: CommunityStatsWidgetProps) => {
    const { currentUser } = useLogin();
    const { ref: viewportRef, isInViewPort } = useIsInViewport<HTMLDivElement>();

    // Use user's weight preference (default to imperial/lbs for anonymous users)
    const prefersMetric = currentUser?.prefersMetric ?? false;
    const weightValue = Math.round(prefersMetric ? stats.totalWeightInKilograms : stats.totalWeightInPounds);
    const weightLabel = prefersMetric ? 'Weight (kg)' : 'Weight (lbs)';

    const statItems = [
        {
            id: 'events',
            title: 'Events',
            value: stats.totalEvents,
            icon: Calendar,
        },
        {
            id: 'participants',
            title: 'Participants',
            value: stats.totalParticipants,
            icon: Users,
        },
        {
            id: 'bags',
            title: 'Bags',
            value: stats.totalBags,
            icon: Trash2,
        },
        {
            id: 'weight',
            title: weightLabel,
            value: weightValue,
            icon: Scale,
        },
        {
            id: 'hours',
            title: 'Hours',
            value: stats.totalHours,
            icon: Clock,
        },
        {
            id: 'litterReports',
            title: 'Litter Reports',
            value: stats.totalLitterReportsSubmitted,
            icon: FileText,
        },
    ];

    return (
        <Card>
            <CardHeader className='pb-3'>
                <CardTitle className='text-lg'>Community Impact</CardTitle>
            </CardHeader>
            <CardContent ref={viewportRef}>
                <div className='grid grid-cols-2 gap-3'>
                    {statItems.map((item) => (
                        <div key={item.id} className='flex flex-col items-center p-3 bg-muted rounded-lg'>
                            <item.icon className='h-5 w-5 text-primary mb-1' />
                            <span className='text-xl font-bold text-primary'>
                                <CountUp isCounting={isInViewPort} end={item.value} duration={2} />
                            </span>
                            <span className='text-xs text-muted-foreground text-center'>{item.title}</span>
                        </div>
                    ))}
                </div>
            </CardContent>
        </Card>
    );
};
