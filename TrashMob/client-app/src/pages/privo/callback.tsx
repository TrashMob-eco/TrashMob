import { FC } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useSearchParams, Link } from 'react-router';
import { BadgeCheck, Clock, XCircle } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { GetVerificationStatus } from '@/services/privo-consent';

export const PrivoCallback: FC = () => {
    const [searchParams] = useSearchParams();
    const status = searchParams.get('status');

    const statusQuery = useQuery({
        queryKey: GetVerificationStatus().key,
        queryFn: GetVerificationStatus().service,
        select: (res) => res.data,
        refetchInterval: (query) => {
            // Poll every 3 seconds while pending
            const data = query.state.data;
            return data?.status === 1 ? 3000 : false;
        },
    });

    const consent = statusQuery.data;
    const isVerified = consent?.status === 2;
    const isDenied = consent?.status === 3;
    const isPending = consent?.status === 1;

    return (
        <div>
            <HeroSection Title='Identity Verification' Description='PRIVO verification status' />
            <div className='container mx-auto py-5 max-w-lg'>
                {isVerified && (
                    <Card>
                        <CardHeader>
                            <div className='flex items-center gap-2'>
                                <BadgeCheck className='h-5 w-5 text-green-600' />
                                <CardTitle>Verification Complete</CardTitle>
                            </div>
                            <CardDescription>
                                Your identity has been verified. You can now add children aged 13-17 as dependents.
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <div className='flex gap-2'>
                                <Button asChild>
                                    <Link to='/mydashboard'>Go to Dashboard</Link>
                                </Button>
                                <Button variant='outline' asChild>
                                    <Link to='/myprofile'>My Profile</Link>
                                </Button>
                            </div>
                        </CardContent>
                    </Card>
                )}

                {isPending && (
                    <Card>
                        <CardHeader>
                            <div className='flex items-center gap-2'>
                                <Clock className='h-5 w-5 text-amber-500 animate-pulse' />
                                <CardTitle>Verification In Progress</CardTitle>
                            </div>
                            <CardDescription>
                                Your identity verification is being processed. This page will update automatically.
                            </CardDescription>
                        </CardHeader>
                    </Card>
                )}

                {isDenied && (
                    <Card>
                        <CardHeader>
                            <div className='flex items-center gap-2'>
                                <XCircle className='h-5 w-5 text-destructive' />
                                <CardTitle>Verification Not Completed</CardTitle>
                            </div>
                            <CardDescription>
                                Identity verification was not completed. You can try again from your profile page.
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <Button variant='outline' asChild>
                                <Link to='/myprofile'>Back to Profile</Link>
                            </Button>
                        </CardContent>
                    </Card>
                )}

                {!consent && !statusQuery.isLoading && (
                    <Card>
                        <CardHeader>
                            <CardTitle>Returning from Verification</CardTitle>
                            <CardDescription>
                                {status === 'success'
                                    ? 'Verification is being finalized. Please check your profile shortly.'
                                    : 'Please check your profile for the current verification status.'}
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <Button asChild>
                                <Link to='/myprofile'>Go to Profile</Link>
                            </Button>
                        </CardContent>
                    </Card>
                )}
            </div>
        </div>
    );
};

export default PrivoCallback;
