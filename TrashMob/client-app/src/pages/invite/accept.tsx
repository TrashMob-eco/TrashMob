import { FC } from 'react';
import { useSearchParams, Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { UserPlus, CheckCircle, XCircle, Loader2 } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardHeader, CardTitle, CardDescription, CardFooter } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { VerifyDependentInvitation } from '@/services/dependent-invitations';
import { getApiConfig } from '@/store/AuthStore';
import { getMsalClientInstance } from '@/store/AuthStore';

export const AcceptInvitePage: FC = () => {
    const [searchParams] = useSearchParams();
    const token = searchParams.get('token');

    const verifyQuery = useQuery({
        queryKey: VerifyDependentInvitation({ token: token ?? '' }).key,
        queryFn: VerifyDependentInvitation({ token: token ?? '' }).service,
        enabled: !!token,
        retry: false,
    });

    function handleCreateAccount() {
        const apiConfig = getApiConfig();
        getMsalClientInstance().loginRedirect({
            scopes: apiConfig.scopes,
        });
    }

    const renderContent = () => {
        if (!token) {
            return (
                <Card className='max-w-md mx-auto'>
                    <CardHeader>
                        <div className='flex items-center gap-2'>
                            <XCircle className='h-6 w-6 text-destructive' />
                            <CardTitle>Invalid Link</CardTitle>
                        </div>
                        <CardDescription>This invitation link is invalid or missing a token.</CardDescription>
                    </CardHeader>
                    <CardContent>
                        <p className='text-muted-foreground'>
                            Please check the link in the email you received and try again.
                        </p>
                    </CardContent>
                    <CardFooter>
                        <Button asChild>
                            <Link to='/'>Go to Home</Link>
                        </Button>
                    </CardFooter>
                </Card>
            );
        }

        if (verifyQuery.isLoading) {
            return (
                <Card className='max-w-md mx-auto'>
                    <CardHeader>
                        <div className='flex items-center gap-2'>
                            <Loader2 className='h-6 w-6 animate-spin text-primary' />
                            <CardTitle>Verifying Invitation...</CardTitle>
                        </div>
                        <CardDescription>Please wait while we verify your invitation.</CardDescription>
                    </CardHeader>
                </Card>
            );
        }

        const info = verifyQuery.data?.data;

        if (verifyQuery.isError || !info?.isValid) {
            const errorMessage = info?.errorMessage || 'This invitation link is invalid or has expired.';
            return (
                <Card className='max-w-md mx-auto'>
                    <CardHeader>
                        <div className='flex items-center gap-2'>
                            <XCircle className='h-6 w-6 text-destructive' />
                            <CardTitle>Invitation Invalid</CardTitle>
                        </div>
                        <CardDescription>{errorMessage}</CardDescription>
                    </CardHeader>
                    <CardContent>
                        <p className='text-muted-foreground'>
                            The invitation may have expired or already been used. Please ask your parent or guardian to
                            send a new invitation.
                        </p>
                    </CardContent>
                    <CardFooter>
                        <Button asChild>
                            <Link to='/'>Go to Home</Link>
                        </Button>
                    </CardFooter>
                </Card>
            );
        }

        return (
            <Card className='max-w-md mx-auto'>
                <CardHeader>
                    <div className='flex items-center gap-2'>
                        <CheckCircle className='h-6 w-6 text-green-600' />
                        <CardTitle>You&apos;re Invited!</CardTitle>
                    </div>
                    <CardDescription>
                        {info.parentName} has invited you to create your own TrashMob account.
                    </CardDescription>
                </CardHeader>
                <CardContent className='space-y-4'>
                    <p className='text-muted-foreground'>
                        Hi {info.dependentFirstName}! Your parent/guardian has invited you to join TrashMob so you can
                        sign up for cleanup events on your own.
                    </p>
                    <p className='text-muted-foreground'>
                        Click the button below to create your account. Make sure to use the same email address this
                        invitation was sent to — your account will be automatically linked to {info.parentName}&apos;s
                        account.
                    </p>
                </CardContent>
                <CardFooter>
                    <Button onClick={handleCreateAccount} className='w-full'>
                        <UserPlus className='mr-2 h-4 w-4' />
                        Create My Account
                    </Button>
                </CardFooter>
            </Card>
        );
    };

    return (
        <div>
            <HeroSection Title='Join TrashMob' Description='Accept your invitation to make a difference' />
            <div className='container py-12'>
                <div className='flex justify-center mb-8'>
                    <UserPlus className='h-16 w-16 text-primary opacity-50' />
                </div>
                {renderContent()}
            </div>
        </div>
    );
};
