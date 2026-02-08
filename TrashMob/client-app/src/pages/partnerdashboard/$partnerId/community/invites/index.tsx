import { useState } from 'react';
import { useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { DataTable } from '@/components/ui/data-table';
import { Send, Mail } from 'lucide-react';

import { getColumns } from './columns';
import { GetCommunityInviteBatches, CreateCommunityInviteBatch } from '@/services/email-invites';
import { GetPartnerById } from '@/services/partners';

export const PartnerCommunityInvites = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const queryClient = useQueryClient();
    const [emailInput, setEmailInput] = useState('');
    const [error, setError] = useState('');

    const { data: partner } = useQuery({
        queryKey: GetPartnerById({ partnerId }).key,
        queryFn: GetPartnerById({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const { data: batches } = useQuery({
        queryKey: GetCommunityInviteBatches({ communityId: partnerId }).key,
        queryFn: GetCommunityInviteBatches({ communityId: partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const createBatch = useMutation({
        mutationKey: CreateCommunityInviteBatch({ communityId: partnerId }).key,
        mutationFn: CreateCommunityInviteBatch({ communityId: partnerId }).service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetCommunityInviteBatches({ communityId: partnerId }).key,
                refetchType: 'all',
            });
            setEmailInput('');
            setError('');
        },
        onError: (err: Error) => {
            setError(err.message || 'Failed to send invites');
        },
    });

    const handleSend = () => {
        setError('');

        const emails = emailInput
            .split(/[\n,]/)
            .map((e) => e.trim())
            .filter((e) => e.length > 0);

        if (emails.length === 0) {
            setError('Please enter at least one email address');
            return;
        }

        const invalidEmails = emails.filter((e) => !e.includes('@'));
        if (invalidEmails.length > 0) {
            setError(
                `Invalid email addresses: ${invalidEmails.slice(0, 3).join(', ')}${invalidEmails.length > 3 ? '...' : ''}`,
            );
            return;
        }

        createBatch.mutate({ emails });
    };

    const columns = getColumns(partnerId);
    const len = (batches || []).length;

    return (
        <div className='py-6 space-y-6'>
            {/* Send Invites Card */}
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <Mail className='h-5 w-5' />
                        Invite Volunteers to {partner?.name}
                    </CardTitle>
                    <CardDescription>
                        Invite potential volunteers to join {partner?.name || 'your community'} on TrashMob.eco. Enter
                        email addresses one per line or comma-separated.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <div className='space-y-4'>
                        <div className='space-y-2'>
                            <Label htmlFor='emails'>Email Addresses</Label>
                            <Textarea
                                id='emails'
                                placeholder='user1@example.com&#10;user2@example.com&#10;user3@example.com'
                                value={emailInput}
                                onChange={(e) => setEmailInput(e.target.value)}
                                rows={6}
                                className='font-mono text-sm'
                            />
                            <p className='text-xs text-muted-foreground'>
                                Enter one email per line or separate with commas
                            </p>
                        </div>
                        {error ? <p className='text-sm text-red-600'>{error}</p> : null}
                        <Button onClick={handleSend} disabled={createBatch.isPending || !emailInput.trim()}>
                            <Send className='mr-2 h-4 w-4' />
                            {createBatch.isPending ? 'Sending...' : 'Send Invites'}
                        </Button>
                    </div>
                </CardContent>
            </Card>

            {/* Batch History Card */}
            <Card>
                <CardHeader>
                    <CardTitle>Invite History ({len})</CardTitle>
                    <CardDescription>Recent bulk invite batches and their status</CardDescription>
                </CardHeader>
                <CardContent>
                    <DataTable columns={columns} data={batches || []} />
                </CardContent>
            </Card>
        </div>
    );
};

export default PartnerCommunityInvites;
