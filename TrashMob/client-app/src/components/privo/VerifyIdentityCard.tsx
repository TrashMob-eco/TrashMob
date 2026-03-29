import { FC } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { BadgeCheck, ExternalLink, Loader2, ShieldCheck } from 'lucide-react';

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { useToast } from '@/hooks/use-toast';
import { GetPrivoEnabled, GetVerificationStatus, InitiateAdultVerification } from '@/services/privo-consent';

interface VerifyIdentityCardProps {
    isVerified: boolean;
}

export const VerifyIdentityCard: FC<VerifyIdentityCardProps> = ({ isVerified }) => {
    const { toast } = useToast();
    const queryClient = useQueryClient();

    const enabledQuery = useQuery({
        queryKey: GetPrivoEnabled().key,
        queryFn: GetPrivoEnabled().service,
        select: (res) => res.data?.enabled ?? false,
        staleTime: 5 * 60 * 1000,
    });

    const statusQuery = useQuery({
        queryKey: GetVerificationStatus().key,
        queryFn: GetVerificationStatus().service,
        select: (res) => res.data,
        enabled: !isVerified && enabledQuery.data === true,
    });

    const verifyMutation = useMutation({
        mutationKey: InitiateAdultVerification().key,
        mutationFn: InitiateAdultVerification().service,
        onSuccess: (res) => {
            const consent = res.data;
            if (consent.consentUrl) {
                window.location.href = consent.consentUrl;
            } else {
                toast({ variant: 'primary', title: 'Verification started. Check your email for next steps.' });
                queryClient.invalidateQueries({ queryKey: GetVerificationStatus().key });
            }
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Verification failed', description: error.message });
        },
    });

    const isPending = statusQuery.data?.status === 1;

    // Don't render until we know the feature is enabled
    if (enabledQuery.isLoading) return null;
    if (!enabledQuery.data && !isVerified) return null;

    if (isVerified) {
        return (
            <Card>
                <CardHeader>
                    <div className='flex items-center gap-2'>
                        <BadgeCheck className='h-5 w-5 text-green-600' />
                        <CardTitle className='text-lg'>Identity Verified</CardTitle>
                    </div>
                    <CardDescription>
                        Your identity has been verified via PRIVO. You can add children aged 13-17 as dependents.
                    </CardDescription>
                </CardHeader>
            </Card>
        );
    }

    return (
        <Card>
            <CardHeader>
                <div className='flex items-center gap-2'>
                    <ShieldCheck className='h-5 w-5 text-muted-foreground' />
                    <CardTitle className='text-lg'>Identity Verification</CardTitle>
                </div>
                <CardDescription>
                    Verify your identity to add children aged 13-17 as dependents. Verification is handled securely by
                    PRIVO.
                </CardDescription>
            </CardHeader>
            <CardContent>
                {isPending ? (
                    <div className='flex items-center gap-2'>
                        <Badge variant='outline'>Pending</Badge>
                        <span className='text-sm text-muted-foreground'>Verification in progress</span>
                    </div>
                ) : (
                    <Button onClick={() => verifyMutation.mutate()} disabled={verifyMutation.isPending}>
                        {verifyMutation.isPending ? (
                            <Loader2 className='mr-2 h-4 w-4 animate-spin' />
                        ) : (
                            <ExternalLink className='mr-2 h-4 w-4' />
                        )}
                        Verify My Identity
                    </Button>
                )}
            </CardContent>
        </Card>
    );
};
