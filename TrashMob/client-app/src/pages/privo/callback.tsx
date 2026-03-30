import { FC } from 'react';
import { useSearchParams, Link } from 'react-router';
import { BadgeCheck } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';

export const PrivoCallback: FC = () => {
    const [searchParams] = useSearchParams();
    const status = searchParams.get('status');

    return (
        <div>
            <HeroSection Title='Identity Verification' Description='PRIVO verification status' />
            <div className='container mx-auto py-5 max-w-lg'>
                <Card>
                    <CardHeader>
                        <div className='flex items-center gap-2'>
                            <BadgeCheck className='h-5 w-5 text-green-600' />
                            <CardTitle>Verification Submitted</CardTitle>
                        </div>
                        <CardDescription>
                            {status === 'success'
                                ? 'Your identity verification has been submitted successfully.'
                                : 'Your identity verification has been processed.'}
                        </CardDescription>
                    </CardHeader>
                    <CardContent className='space-y-3'>
                        <p className='text-sm text-muted-foreground'>
                            Please sign in to check your verification status. Once verified, you will be able to add
                            children aged 13-17 as dependents from your dashboard.
                        </p>
                        <div className='flex gap-2'>
                            <Button asChild>
                                <Link to='/mydashboard'>Go to Dashboard</Link>
                            </Button>
                            <Button variant='outline' asChild>
                                <Link to='/'>Home</Link>
                            </Button>
                        </div>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
};

export default PrivoCallback;
