import { Link, useParams } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import moment from 'moment';
import { ArrowLeft, Mail, MapPin, Calendar, Shield } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { GetUserById } from '@/services/users';

export const SiteAdminUserDetail = () => {
    const { userId } = useParams<{ userId: string }>() as { userId: string };

    const { data: user } = useQuery({
        queryKey: GetUserById({ userId }).key,
        queryFn: GetUserById({ userId }).service,
        select: (res) => res.data,
        enabled: !!userId,
    });

    if (!user) {
        return <p>Loading...</p>;
    }

    return (
        <div className='space-y-6'>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <div className='flex items-center gap-3'>
                        <Button variant='ghost' size='icon' asChild>
                            <Link to='/siteadmin/users'>
                                <ArrowLeft className='h-4 w-4' />
                            </Link>
                        </Button>
                        <CardTitle>{user.userName}</CardTitle>
                        {user.isSiteAdmin ? <Badge variant='destructive'><Shield className='mr-1 h-3 w-3' />Site Admin</Badge> : null}
                    </div>
                </CardHeader>
                <CardContent>
                    <div className='grid grid-cols-12 gap-4'>
                        <div className='col-span-12 md:col-span-6 space-y-3'>
                            <div>
                                <h4 className='text-sm font-medium text-muted-foreground'>Name</h4>
                                <p>{[user.givenName, user.surname].filter(Boolean).join(' ') || '—'}</p>
                            </div>
                            <div className='flex items-center gap-2'>
                                <Mail className='h-4 w-4 text-muted-foreground' />
                                <span>{user.email || '—'}</span>
                            </div>
                            <div className='flex items-center gap-2'>
                                <Calendar className='h-4 w-4 text-muted-foreground' />
                                <span>Member since {moment(user.memberSince).format('MMM D, YYYY')}</span>
                            </div>
                            {user.dateOfBirth ? (
                                <div>
                                    <h4 className='text-sm font-medium text-muted-foreground'>Date of Birth</h4>
                                    <p>{user.dateOfBirth}</p>
                                </div>
                            ) : null}
                        </div>
                        <div className='col-span-12 md:col-span-6 space-y-3'>
                            <div className='flex items-center gap-2'>
                                <MapPin className='h-4 w-4 text-muted-foreground' />
                                <span>
                                    {[user.city, user.region, user.country].filter(Boolean).join(', ') || '—'}
                                </span>
                            </div>
                            {user.postalCode ? (
                                <div>
                                    <h4 className='text-sm font-medium text-muted-foreground'>Postal Code</h4>
                                    <p>{user.postalCode}</p>
                                </div>
                            ) : null}
                            <div>
                                <h4 className='text-sm font-medium text-muted-foreground'>Preferences</h4>
                                <p className='text-sm'>
                                    {user.prefersMetric ? 'Metric' : 'Imperial'} · Travel limit: {user.travelLimitForLocalEvents} mi
                                </p>
                            </div>
                        </div>
                    </div>
                </CardContent>
            </Card>
        </div>
    );
};
