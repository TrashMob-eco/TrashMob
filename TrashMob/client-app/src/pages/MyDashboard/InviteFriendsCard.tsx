import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Mail, Send, Users, Info, CheckCircle, XCircle, Clock, Loader2 } from 'lucide-react';

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Badge } from '@/components/ui/badge';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import {
    GetUserInviteBatches,
    GetUserInviteQuota,
    CreateUserInviteBatch,
    EmailInviteBatch,
    UserInviteQuota,
} from '@/services/email-invites';
import { useToast } from '@/hooks/use-toast';

export const InviteFriendsCard = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [emailText, setEmailText] = useState('');

    const { data: quota, isLoading: isLoadingQuota } = useQuery<AxiosResponse<UserInviteQuota>, unknown, UserInviteQuota>(
        {
            queryKey: GetUserInviteQuota().key,
            queryFn: GetUserInviteQuota().service,
            select: (res) => res.data,
        },
    );

    const { data: batches, isLoading: isLoadingBatches } = useQuery<
        AxiosResponse<EmailInviteBatch[]>,
        unknown,
        EmailInviteBatch[]
    >({
        queryKey: GetUserInviteBatches().key,
        queryFn: GetUserInviteBatches().service,
        select: (res) => res.data,
    });

    const createBatchMutation = useMutation({
        mutationFn: (emails: string[]) => CreateUserInviteBatch().service({ emails }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetUserInviteBatches().key });
            queryClient.invalidateQueries({ queryKey: GetUserInviteQuota().key });
            setEmailText('');
            toast({
                title: 'Invites Sent',
                description: 'Your invitations have been sent successfully.',
            });
        },
        onError: (error: { response?: { data?: { message?: string } } }) => {
            const message = error.response?.data?.message || 'Failed to send invites. Please try again.';
            toast({
                title: 'Error',
                description: message,
                variant: 'destructive',
            });
        },
    });

    const handleSubmit = () => {
        const emails = emailText
            .split(/[\n,;]+/)
            .map((e) => e.trim())
            .filter((e) => e && e.includes('@'));

        if (emails.length === 0) {
            toast({
                title: 'No valid emails',
                description: 'Please enter at least one valid email address.',
                variant: 'destructive',
            });
            return;
        }

        if (quota && emails.length > quota.maxPerBatch) {
            toast({
                title: 'Too many emails',
                description: `Maximum ${quota.maxPerBatch} emails per batch allowed.`,
                variant: 'destructive',
            });
            return;
        }

        createBatchMutation.mutate(emails);
    };

    const getStatusBadge = (status: string) => {
        switch (status) {
            case 'Complete':
                return (
                    <Badge variant='success' className='gap-1'>
                        <CheckCircle className='h-3 w-3' />
                        Complete
                    </Badge>
                );
            case 'Failed':
                return (
                    <Badge variant='destructive' className='gap-1'>
                        <XCircle className='h-3 w-3' />
                        Failed
                    </Badge>
                );
            case 'Processing':
                return (
                    <Badge variant='outline' className='gap-1'>
                        <Loader2 className='h-3 w-3 animate-spin' />
                        Processing
                    </Badge>
                );
            case 'Pending':
            default:
                return (
                    <Badge variant='outline' className='gap-1'>
                        <Clock className='h-3 w-3' />
                        Pending
                    </Badge>
                );
        }
    };

    return (
        <Card>
            <CardHeader>
                <CardTitle className='flex items-center gap-2 text-primary'>
                    <Users className='h-5 w-5' />
                    Invite Friends to TrashMob
                </CardTitle>
                <CardDescription>Share TrashMob with your friends and help grow our community of volunteers.</CardDescription>
            </CardHeader>
            <CardContent className='space-y-6'>
                {/* Quota Info */}
                {!isLoadingQuota && quota ? <Alert>
                        <Info className='h-4 w-4' />
                        <AlertDescription>
                            You can invite up to <strong>{quota.maxPerBatch}</strong> friends at a time, and{' '}
                            <strong>{quota.remainingThisMonth}</strong> more this month (
                            {quota.usedThisMonth}/{quota.maxPerMonth} used).
                        </AlertDescription>
                    </Alert> : null}

                {/* Email Input */}
                <div className='space-y-2'>
                    <label className='text-sm font-medium'>Email Addresses</label>
                    <Textarea
                        placeholder='Enter email addresses (one per line, or separated by commas)'
                        value={emailText}
                        onChange={(e) => setEmailText(e.target.value)}
                        rows={4}
                        disabled={createBatchMutation.isPending || (quota && quota.remainingThisMonth <= 0)}
                    />
                    <p className='text-xs text-muted-foreground'>
                        Enter up to {quota?.maxPerBatch || 10} email addresses, one per line or separated by commas.
                    </p>
                </div>

                {/* Submit Button */}
                <Button
                    onClick={handleSubmit}
                    disabled={
                        createBatchMutation.isPending || !emailText.trim() || (quota && quota.remainingThisMonth <= 0)
                    }
                    className='w-full sm:w-auto'
                >
                    {createBatchMutation.isPending ? (
                        <>
                            <Loader2 className='mr-2 h-4 w-4 animate-spin' />
                            Sending...
                        </>
                    ) : (
                        <>
                            <Send className='mr-2 h-4 w-4' />
                            Send Invites
                        </>
                    )}
                </Button>

                {/* Recent Invites */}
                {!isLoadingBatches && batches && batches.length > 0 ? <div className='pt-4 border-t'>
                        <h4 className='font-medium mb-3 flex items-center gap-2'>
                            <Mail className='h-4 w-4' />
                            Recent Invites
                        </h4>
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Date</TableHead>
                                    <TableHead>Emails</TableHead>
                                    <TableHead>Sent</TableHead>
                                    <TableHead>Failed</TableHead>
                                    <TableHead>Status</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {batches.slice(0, 5).map((batch) => (
                                    <TableRow key={batch.id}>
                                        <TableCell className='text-sm'>
                                            {new Date(batch.createdDate).toLocaleDateString()}
                                        </TableCell>
                                        <TableCell>{batch.totalCount}</TableCell>
                                        <TableCell className='text-green-600'>{batch.sentCount}</TableCell>
                                        <TableCell className='text-red-600'>{batch.failedCount}</TableCell>
                                        <TableCell>{getStatusBadge(batch.status)}</TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </div> : null}
            </CardContent>
        </Card>
    );
};
