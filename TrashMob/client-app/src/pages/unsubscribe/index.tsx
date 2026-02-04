import { FC, useEffect, useState } from 'react';
import { useSearchParams, Link } from 'react-router';
import { useMutation } from '@tanstack/react-query';
import { Mail, CheckCircle, XCircle, Loader2 } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardHeader, CardTitle, CardDescription, CardFooter } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { ProcessUnsubscribe } from '@/services/newsletters';

export const UnsubscribePage: FC = () => {
    const [searchParams] = useSearchParams();
    const token = searchParams.get('token');
    const [processed, setProcessed] = useState(false);

    const unsubscribeMutation = useMutation({
        mutationKey: ProcessUnsubscribe().key,
        mutationFn: ProcessUnsubscribe().service,
        onSettled: () => {
            setProcessed(true);
        },
    });

    useEffect(() => {
        if (token && !processed && !unsubscribeMutation.isPending) {
            unsubscribeMutation.mutate({ token });
        }
    }, [token, processed, unsubscribeMutation]);

    const renderContent = () => {
        // No token provided
        if (!token) {
            return (
                <Card className='max-w-md mx-auto'>
                    <CardHeader>
                        <div className='flex items-center gap-2'>
                            <XCircle className='h-6 w-6 text-destructive' />
                            <CardTitle>Invalid Link</CardTitle>
                        </div>
                        <CardDescription>This unsubscribe link is invalid or has expired.</CardDescription>
                    </CardHeader>
                    <CardContent>
                        <p className='text-muted-foreground'>
                            If you want to manage your newsletter preferences, please log in to your account.
                        </p>
                    </CardContent>
                    <CardFooter>
                        <Button asChild>
                            <Link to='/mydashboard'>Go to Dashboard</Link>
                        </Button>
                    </CardFooter>
                </Card>
            );
        }

        // Processing
        if (unsubscribeMutation.isPending || !processed) {
            return (
                <Card className='max-w-md mx-auto'>
                    <CardHeader>
                        <div className='flex items-center gap-2'>
                            <Loader2 className='h-6 w-6 animate-spin text-primary' />
                            <CardTitle>Processing...</CardTitle>
                        </div>
                        <CardDescription>Please wait while we process your request.</CardDescription>
                    </CardHeader>
                </Card>
            );
        }

        // Success
        if (unsubscribeMutation.isSuccess && unsubscribeMutation.data?.data?.success) {
            const result = unsubscribeMutation.data.data;
            return (
                <Card className='max-w-md mx-auto'>
                    <CardHeader>
                        <div className='flex items-center gap-2'>
                            <CheckCircle className='h-6 w-6 text-green-600' />
                            <CardTitle>Unsubscribed Successfully</CardTitle>
                        </div>
                        <CardDescription>
                            {result.allCategories
                                ? 'You have been unsubscribed from all newsletters.'
                                : `You have been unsubscribed from ${result.categoryName} newsletters.`}
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        <p className='text-muted-foreground'>
                            Email: <span className='font-medium'>{result.email}</span>
                        </p>
                        <p className='text-muted-foreground mt-2'>
                            Changed your mind? You can manage your newsletter preferences from your dashboard at any
                            time.
                        </p>
                    </CardContent>
                    <CardFooter className='flex gap-2'>
                        <Button asChild variant='outline'>
                            <Link to='/'>Go to Home</Link>
                        </Button>
                        <Button asChild>
                            <Link to='/mydashboard'>Manage Preferences</Link>
                        </Button>
                    </CardFooter>
                </Card>
            );
        }

        // Error
        const errorMessage =
            unsubscribeMutation.data?.data?.errorMessage ||
            (unsubscribeMutation.error as Error)?.message ||
            'An unexpected error occurred.';

        return (
            <Card className='max-w-md mx-auto'>
                <CardHeader>
                    <div className='flex items-center gap-2'>
                        <XCircle className='h-6 w-6 text-destructive' />
                        <CardTitle>Unable to Unsubscribe</CardTitle>
                    </div>
                    <CardDescription>{errorMessage}</CardDescription>
                </CardHeader>
                <CardContent>
                    <p className='text-muted-foreground'>
                        This link may have expired or already been used. Please log in to manage your newsletter
                        preferences directly.
                    </p>
                </CardContent>
                <CardFooter className='flex gap-2'>
                    <Button asChild variant='outline'>
                        <Link to='/'>Go to Home</Link>
                    </Button>
                    <Button asChild>
                        <Link to='/mydashboard'>Go to Dashboard</Link>
                    </Button>
                </CardFooter>
            </Card>
        );
    };

    return (
        <div>
            <HeroSection Title='Newsletter Unsubscribe' Description='Manage your email preferences' />
            <div className='container py-12'>
                <div className='flex justify-center mb-8'>
                    <Mail className='h-16 w-16 text-primary opacity-50' />
                </div>
                {renderContent()}
            </div>
        </div>
    );
};
