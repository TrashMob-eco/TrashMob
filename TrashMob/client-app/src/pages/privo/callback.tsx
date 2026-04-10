import { FC } from 'react';
import { useSearchParams, Link } from 'react-router';
import { BadgeCheck, LogIn } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { useLogin } from '@/hooks/useLogin';
import { getApiConfig, getMsalClientInstance } from '@/store/AuthStore';

export const PrivoCallback: FC = () => {
    const [searchParams] = useSearchParams();
    const status = searchParams.get('status');
    const { isUserLoaded } = useLogin();

    const handleSignIn = () => {
        const apiConfig = getApiConfig();
        getMsalClientInstance().loginRedirect({
            scopes: apiConfig.scopes,
        });
    };

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
                        {isUserLoaded ? (
                            <>
                                <p className='text-sm text-muted-foreground'>
                                    Your verification is being processed. Check your dashboard for the current status.
                                </p>
                                <div className='flex gap-2'>
                                    <Button asChild>
                                        <Link to='/mydashboard'>Go to Dashboard</Link>
                                    </Button>
                                </div>
                            </>
                        ) : (
                            <>
                                <p className='text-sm text-muted-foreground'>
                                    PRIVO opened this page in a new tab. Please sign in to continue to your dashboard.
                                </p>
                                <div className='flex gap-2'>
                                    <Button onClick={handleSignIn}>
                                        <LogIn className='mr-2 h-4 w-4' />
                                        Sign In
                                    </Button>
                                    <Button variant='outline' asChild>
                                        <Link to='/'>Home</Link>
                                    </Button>
                                </div>
                            </>
                        )}
                    </CardContent>
                </Card>
            </div>
        </div>
    );
};

export default PrivoCallback;
