import { useState } from 'react';
import { useParams, Link } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { DataTable } from '@/components/ui/data-table';
import { Send, Mail, ArrowLeft, Loader2 } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { getColumns } from './columns';
import { GetTeamInviteBatches, CreateTeamInviteBatch } from '@/services/email-invites';
import { GetTeamById, GetTeamMembers } from '@/services/teams';
import TeamData from '@/components/Models/TeamData';
import TeamMemberData from '@/components/Models/TeamMemberData';
import { useLogin } from '@/hooks/useLogin';

export const TeamInvitesPage = () => {
    const { teamId } = useParams<{ teamId: string }>() as { teamId: string };
    const queryClient = useQueryClient();
    const { currentUser } = useLogin();
    const [emailInput, setEmailInput] = useState('');
    const [error, setError] = useState('');

    const { data: team, isLoading: isLoadingTeam } = useQuery<AxiosResponse<TeamData>, unknown, TeamData>({
        queryKey: GetTeamById({ teamId }).key,
        queryFn: GetTeamById({ teamId }).service,
        select: (res) => res.data,
        enabled: !!teamId,
    });

    const { data: members } = useQuery<AxiosResponse<TeamMemberData[]>, unknown, TeamMemberData[]>({
        queryKey: GetTeamMembers({ teamId }).key,
        queryFn: GetTeamMembers({ teamId }).service,
        select: (res) => res.data,
        enabled: !!teamId,
    });

    const isLead = members?.some((m) => m.userId === currentUser.id && m.isTeamLead);

    const { data: batches } = useQuery({
        queryKey: GetTeamInviteBatches({ teamId }).key,
        queryFn: GetTeamInviteBatches({ teamId }).service,
        select: (res) => res.data,
        enabled: !!teamId && isLead,
    });

    const createBatch = useMutation({
        mutationKey: CreateTeamInviteBatch({ teamId }).key,
        mutationFn: CreateTeamInviteBatch({ teamId }).service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetTeamInviteBatches({ teamId }).key,
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

        // Parse email list (one per line or comma-separated)
        const emails = emailInput
            .split(/[\n,]/)
            .map((e) => e.trim())
            .filter((e) => e.length > 0);

        if (emails.length === 0) {
            setError('Please enter at least one email address');
            return;
        }

        // Basic email validation
        const invalidEmails = emails.filter((e) => !e.includes('@'));
        if (invalidEmails.length > 0) {
            setError(
                `Invalid email addresses: ${invalidEmails.slice(0, 3).join(', ')}${invalidEmails.length > 3 ? '...' : ''}`,
            );
            return;
        }

        createBatch.mutate({ emails });
    };

    if (isLoadingTeam) {
        return (
            <div>
                <HeroSection Title='Team Invites' Description='Loading...' />
                <div className='container py-8 text-center'>
                    <Loader2 className='h-8 w-8 animate-spin mx-auto' />
                </div>
            </div>
        );
    }

    if (!team) {
        return (
            <div>
                <HeroSection Title='Team Not Found' Description='' />
                <div className='container py-8 text-center'>
                    <p className='mb-4'>This team could not be found.</p>
                    <Button asChild>
                        <Link to='/teams'>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Teams
                        </Link>
                    </Button>
                </div>
            </div>
        );
    }

    if (!isLead) {
        return (
            <div>
                <HeroSection Title='Access Denied' Description='' />
                <div className='container py-8 text-center'>
                    <p className='mb-4'>You must be a team lead to manage invites.</p>
                    <Button asChild>
                        <Link to={`/teams/${teamId}`}>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Team
                        </Link>
                    </Button>
                </div>
            </div>
        );
    }

    const columns = getColumns(teamId);
    const len = (batches || []).length;

    return (
        <div>
            <HeroSection Title={`Invite Members to ${team.name}`} Description='Send bulk email invitations' />
            <div className='container py-8'>
                <div className='mb-4'>
                    <Button variant='outline' asChild>
                        <Link to={`/teams/${teamId}/edit`}>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Team Management
                        </Link>
                    </Button>
                </div>

                <div className='space-y-6'>
                    {/* Send Invites Card */}
                    <Card>
                        <CardHeader>
                            <CardTitle className='flex items-center gap-2'>
                                <Mail className='h-5 w-5' />
                                Invite Members to {team.name}
                            </CardTitle>
                            <CardDescription>
                                Invite potential members to join {team.name} on TrashMob.eco. Enter email addresses one
                                per line or comma-separated.
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
            </div>
        </div>
    );
};

export default TeamInvitesPage;
