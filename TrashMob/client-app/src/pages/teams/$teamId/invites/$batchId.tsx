import { useQuery } from '@tanstack/react-query';
import { useParams, Link } from 'react-router';
import { AxiosResponse } from 'axios';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Badge } from '@/components/ui/badge';
import { ArrowLeft, CheckCircle, XCircle, Clock, Mail, Loader2 } from 'lucide-react';
import { HeroSection } from '@/components/Customization/HeroSection';
import { GetTeamInviteBatchDetails } from '@/services/email-invites';
import { GetTeamById, GetTeamMembers } from '@/services/teams';
import TeamData from '@/components/Models/TeamData';
import TeamMemberData from '@/components/Models/TeamMemberData';
import { useLogin } from '@/hooks/useLogin';

export const TeamInviteDetailsPage = () => {
    const { teamId, batchId } = useParams<{ teamId: string; batchId: string }>();
    const { currentUser } = useLogin();

    const { data: team, isLoading: isLoadingTeam } = useQuery<AxiosResponse<TeamData>, unknown, TeamData>({
        queryKey: GetTeamById({ teamId: teamId! }).key,
        queryFn: GetTeamById({ teamId: teamId! }).service,
        select: (res) => res.data,
        enabled: !!teamId,
    });

    const { data: members } = useQuery<AxiosResponse<TeamMemberData[]>, unknown, TeamMemberData[]>({
        queryKey: GetTeamMembers({ teamId: teamId! }).key,
        queryFn: GetTeamMembers({ teamId: teamId! }).service,
        select: (res) => res.data,
        enabled: !!teamId,
    });

    const isLead = members?.some((m) => m.userId === currentUser.id && m.isTeamLead);

    const { data: batch, isLoading: isLoadingBatch } = useQuery({
        queryKey: GetTeamInviteBatchDetails({ teamId: teamId!, id: batchId! }).key,
        queryFn: GetTeamInviteBatchDetails({ teamId: teamId!, id: batchId! }).service,
        select: (res) => res.data,
        enabled: !!teamId && !!batchId && isLead,
    });

    if (isLoadingTeam || isLoadingBatch) {
        return (
            <div>
                <HeroSection Title='Batch Details' Description='Loading...' />
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
                    <p className='mb-4'>You must be a team lead to view invite details.</p>
                    <Button asChild>
                        <Link to={`/teams/${teamId}`}>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Team
                        </Link>
                    </Button>
                </div>
            </div>
        );
    }

    if (!batch) {
        return (
            <div>
                <HeroSection Title='Batch Not Found' Description='' />
                <div className='container py-8 text-center'>
                    <p className='mb-4'>This invite batch could not be found.</p>
                    <Button asChild>
                        <Link to={`/teams/${teamId}/invites`}>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Invites
                        </Link>
                    </Button>
                </div>
            </div>
        );
    }

    const getStatusBadge = (status: string) => {
        switch (status) {
            case 'Sent':
                return (
                    <Badge variant='success' className='gap-1'>
                        <CheckCircle className='h-3 w-3' />
                        Sent
                    </Badge>
                );
            case 'Failed':
                return (
                    <Badge variant='destructive' className='gap-1'>
                        <XCircle className='h-3 w-3' />
                        Failed
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
        <div>
            <HeroSection Title={`${team.name} - Batch Details`} Description='View individual invite status' />
            <div className='container py-8'>
                <div className='flex items-center gap-4 mb-6'>
                    <Button variant='ghost' size='icon' asChild>
                        <Link to={`/teams/${teamId}/invites`}>
                            <ArrowLeft className='h-4 w-4' />
                        </Link>
                    </Button>
                    <h1 className='text-2xl font-bold'>Batch Details</h1>
                </div>

                <div className='space-y-6'>
                    {/* Summary Card */}
                    <Card>
                        <CardHeader>
                            <CardTitle className='flex items-center gap-2'>
                                <Mail className='h-5 w-5' />
                                Batch Summary
                            </CardTitle>
                            <CardDescription>Created on {new Date(batch.createdDate).toLocaleString()}</CardDescription>
                        </CardHeader>
                        <CardContent>
                            <div className='grid gap-4 md:grid-cols-5'>
                                <div>
                                    <p className='text-sm text-muted-foreground'>Total</p>
                                    <p className='text-2xl font-bold'>{batch.totalCount}</p>
                                </div>
                                <div>
                                    <p className='text-sm text-muted-foreground'>Sent</p>
                                    <p className='text-2xl font-bold text-green-600'>{batch.sentCount}</p>
                                </div>
                                <div>
                                    <p className='text-sm text-muted-foreground'>Failed</p>
                                    <p className='text-2xl font-bold text-red-600'>{batch.failedCount}</p>
                                </div>
                                <div>
                                    <p className='text-sm text-muted-foreground'>Status</p>
                                    <p className='mt-1'>{getStatusBadge(batch.status)}</p>
                                </div>
                                <div>
                                    <p className='text-sm text-muted-foreground'>Sent By</p>
                                    <p className='text-sm font-medium'>{batch.senderUser?.userName || 'Unknown'}</p>
                                </div>
                            </div>
                        </CardContent>
                    </Card>

                    {/* Individual Invites */}
                    <Card>
                        <CardHeader>
                            <CardTitle>Individual Invites</CardTitle>
                            <CardDescription>
                                {batch.invites?.length || 0} email addresses in this batch
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <Table>
                                <TableHeader>
                                    <TableRow>
                                        <TableHead>Email</TableHead>
                                        <TableHead>Status</TableHead>
                                        <TableHead>Sent Date</TableHead>
                                        <TableHead>Error</TableHead>
                                    </TableRow>
                                </TableHeader>
                                <TableBody>
                                    {(batch.invites || []).map((invite) => (
                                        <TableRow key={invite.id}>
                                            <TableCell className='font-mono text-sm'>{invite.email}</TableCell>
                                            <TableCell>{getStatusBadge(invite.status)}</TableCell>
                                            <TableCell>
                                                {invite.sentDate ? new Date(invite.sentDate).toLocaleString() : '-'}
                                            </TableCell>
                                            <TableCell className='max-w-xs truncate text-sm text-red-600'>
                                                {invite.errorMessage || '-'}
                                            </TableCell>
                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                        </CardContent>
                    </Card>
                </div>
            </div>
        </div>
    );
};

export default TeamInviteDetailsPage;
