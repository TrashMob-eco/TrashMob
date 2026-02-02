import { useParams, Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Calendar, Users, Trash2, Scale, AlertTriangle, MapPin, Edit } from 'lucide-react';

import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { GetCommunityDashboard, CommunityDashboardData } from '@/services/communities';
import { GetCommunityBySlug } from '@/services/communities';
import CommunityData from '@/components/Models/CommunityData';
import moment from 'moment';

const StatCard = ({
    title,
    value,
    icon: Icon,
    description,
}: {
    title: string;
    value: string | number;
    icon: React.ElementType;
    description?: string;
}) => (
    <Card>
        <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
            <CardTitle className='text-sm font-medium'>{title}</CardTitle>
            <Icon className='h-4 w-4 text-muted-foreground' />
        </CardHeader>
        <CardContent>
            <div className='text-2xl font-bold'>{value}</div>
            {description ? <p className='text-xs text-muted-foreground'>{description}</p> : null}
        </CardContent>
    </Card>
);

export const CommunityAdminDashboard = () => {
    const { slug } = useParams<{ slug: string }>() as { slug: string };

    const { data: community } = useQuery<AxiosResponse<CommunityData>, unknown, CommunityData>({
        queryKey: GetCommunityBySlug({ slug }).key,
        queryFn: GetCommunityBySlug({ slug }).service,
        select: (res) => res.data,
        enabled: !!slug,
    });

    const { data: dashboard, isLoading } = useQuery<
        AxiosResponse<CommunityDashboardData>,
        unknown,
        CommunityDashboardData
    >({
        queryKey: GetCommunityDashboard({ communityId: community?.id || '' }).key,
        queryFn: GetCommunityDashboard({ communityId: community?.id || '' }).service,
        select: (res) => res.data,
        enabled: !!community?.id,
    });

    if (isLoading || !dashboard) {
        return (
            <div className='container py-8'>
                <div className='grid gap-4 md:grid-cols-2 lg:grid-cols-4'>
                    {[1, 2, 3, 4].map((i) => (
                        <Card key={i}>
                            <CardContent className='p-6'>
                                <div className='animate-pulse'>
                                    <div className='h-4 bg-muted rounded w-1/2 mb-2' />
                                    <div className='h-8 bg-muted rounded w-1/3' />
                                </div>
                            </CardContent>
                        </Card>
                    ))}
                </div>
            </div>
        );
    }

    return (
        <div className='container py-8'>
            <div className='flex justify-between items-center mb-6'>
                <h2 className='text-xl font-semibold'>Community Overview</h2>
                <Button asChild>
                    <Link to={`/communities/${slug}/admin/content`}>
                        <Edit className='h-4 w-4 mr-2' />
                        Edit Content
                    </Link>
                </Button>
            </div>

            {/* Stats Grid */}
            <div className='grid gap-4 md:grid-cols-2 lg:grid-cols-4 mb-8'>
                <StatCard
                    title='Total Events'
                    value={dashboard.stats.totalEvents}
                    icon={Calendar}
                    description='All time'
                />
                <StatCard
                    title='Total Participants'
                    value={dashboard.stats.totalParticipants}
                    icon={Users}
                    description='Across all events'
                />
                <StatCard
                    title='Bags Collected'
                    value={dashboard.stats.totalBags}
                    icon={Trash2}
                    description='Total litter bags'
                />
                <StatCard
                    title='Weight Collected'
                    value={`${Math.round(dashboard.stats.totalWeightInPounds)} lbs`}
                    icon={Scale}
                    description={`${Math.round(dashboard.stats.totalWeightInKilograms)} kg`}
                />
            </div>

            {/* Secondary Stats */}
            <div className='grid gap-4 md:grid-cols-3 mb-8'>
                <StatCard title='Teams Nearby' value={dashboard.teamCount} icon={Users} description='Within 50 miles' />
                <StatCard
                    title='Open Litter Reports'
                    value={dashboard.openLitterReportsCount}
                    icon={AlertTriangle}
                    description='Awaiting cleanup'
                />
                <StatCard
                    title='Upcoming Events'
                    value={dashboard.upcomingEvents.length}
                    icon={Calendar}
                    description='In your community'
                />
            </div>

            <div className='grid gap-6 lg:grid-cols-2'>
                {/* Upcoming Events */}
                <Card>
                    <CardHeader>
                        <CardTitle className='text-lg'>Upcoming Events</CardTitle>
                    </CardHeader>
                    <CardContent>
                        {dashboard.upcomingEvents.length === 0 ? (
                            <p className='text-sm text-muted-foreground'>No upcoming events in your community.</p>
                        ) : (
                            <div className='space-y-4'>
                                {dashboard.upcomingEvents.map((event) => (
                                    <div key={event.id} className='flex items-start gap-3'>
                                        <div className='rounded-full bg-primary/10 p-2'>
                                            <Calendar className='h-4 w-4 text-primary' />
                                        </div>
                                        <div className='flex-1 min-w-0'>
                                            <Link
                                                to={`/eventdetails/${event.id}`}
                                                className='font-medium text-sm hover:underline truncate block'
                                            >
                                                {event.name}
                                            </Link>
                                            <p className='text-xs text-muted-foreground'>
                                                {moment(event.eventDate).format('MMM D, YYYY h:mm A')}
                                            </p>
                                            <div className='flex items-center gap-1 text-xs text-muted-foreground'>
                                                <MapPin className='h-3 w-3' />
                                                <span>
                                                    {event.city}, {event.region}
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </CardContent>
                </Card>

                {/* Recent Activity */}
                <Card>
                    <CardHeader>
                        <CardTitle className='text-lg'>Recent Activity</CardTitle>
                    </CardHeader>
                    <CardContent>
                        {dashboard.recentActivity.length === 0 ? (
                            <p className='text-sm text-muted-foreground'>No recent activity.</p>
                        ) : (
                            <div className='space-y-4'>
                                {dashboard.recentActivity.map((activity, idx) => (
                                    <div key={idx} className='flex items-start gap-3'>
                                        <div className='rounded-full bg-green-100 p-2'>
                                            <Calendar className='h-4 w-4 text-green-600' />
                                        </div>
                                        <div className='flex-1 min-w-0'>
                                            <p className='font-medium text-sm'>{activity.description}</p>
                                            <p className='text-xs text-muted-foreground'>
                                                {moment(activity.activityDate).format('MMM D, YYYY')}
                                            </p>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </CardContent>
                </Card>
            </div>

            {/* Recent Events */}
            {dashboard.recentEvents.length > 0 ? (
                <Card className='mt-6'>
                    <CardHeader>
                        <CardTitle className='text-lg'>Recent Events (Last 30 Days)</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className='space-y-4'>
                            {dashboard.recentEvents.map((event) => (
                                <div key={event.id} className='flex items-start gap-3'>
                                    <div className='rounded-full bg-muted p-2'>
                                        <Calendar className='h-4 w-4 text-muted-foreground' />
                                    </div>
                                    <div className='flex-1 min-w-0'>
                                        <Link
                                            to={`/eventdetails/${event.id}`}
                                            className='font-medium text-sm hover:underline truncate block'
                                        >
                                            {event.name}
                                        </Link>
                                        <p className='text-xs text-muted-foreground'>
                                            {moment(event.eventDate).format('MMM D, YYYY')}
                                        </p>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </CardContent>
                </Card>
            ) : null}
        </div>
    );
};

export default CommunityAdminDashboard;
