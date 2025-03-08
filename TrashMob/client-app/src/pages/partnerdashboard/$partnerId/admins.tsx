import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Ellipsis, Mail, Plus, SquareX, ToggleRight } from 'lucide-react';
import { Link, Outlet, useMatch, useNavigate, useParams } from 'react-router';
import { SidebarLayout } from './_layout.sidebar';
import { useCallback } from 'react';
import { useToast } from '@/hooks/use-toast';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DeletePartnerAdminsByPartnerIAndUserId, GetPartnerAdminsByPartnerId } from '@/services/admin';
import {
    DeletePartnerAdminInvitation,
    GetInvitationStatuses,
    GetPartnerAdminInvitationsByPartnerId,
    ResendPartnerAdminInvitation,
} from '@/services/invitations';
import InvitationStatusData from '@/components/Models/InvitationStatusData';
import { Badge } from '@/components/ui/badge';

export const PartnerAdmins = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const navigate = useNavigate();
    const { toast } = useToast();

    const queryClient = useQueryClient();
    const isInviting = useMatch(`/partnerdashboard/:partnerId/admins/invite`);

    const { data: invitationStatuses } = useQuery({
        queryKey: GetInvitationStatuses().key,
        queryFn: GetInvitationStatuses().service,
        select: (res) => res.data,
    });

    const getInvitationStatus = useCallback(
        (id: number): InvitationStatusData => {
            return invitationStatuses?.find((st) => st.id === id) || ({ name: 'Unknown' } as InvitationStatusData);
        },
        [invitationStatuses],
    );

    const { data: admins } = useQuery({
        queryKey: GetPartnerAdminsByPartnerId({ partnerId }).key,
        queryFn: GetPartnerAdminsByPartnerId({ partnerId }).service,
        select: (res) => res.data,
    });

    const { data: invitations } = useQuery({
        queryKey: GetPartnerAdminInvitationsByPartnerId({ partnerId }).key,
        queryFn: GetPartnerAdminInvitationsByPartnerId({ partnerId }).service,
        select: (res) => res.data,
    });

    const deletePartnerAdminsByPartnerIAndUserId = useMutation({
        mutationKey: DeletePartnerAdminsByPartnerIAndUserId().key,
        mutationFn: DeletePartnerAdminsByPartnerIAndUserId().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Admin deleted' });
            queryClient.invalidateQueries({
                queryKey: GetPartnerAdminsByPartnerId({ partnerId }).key,
                refetchType: 'all',
            });
        },
        onError: () => toast({ variant: 'destructive', title: 'Server error' }),
    });

    const resendPartnerAdminInvitation = useMutation({
        mutationKey: ResendPartnerAdminInvitation().key,
        mutationFn: ResendPartnerAdminInvitation().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Invitation resent' });
            queryClient.invalidateQueries({
                queryKey: GetPartnerAdminInvitationsByPartnerId({ partnerId }).key,
                refetchType: 'all',
            });
        },
        onError: () => toast({ variant: 'destructive', title: 'Server error' }),
    });

    const deletePartnerAdminInvitation = useMutation({
        mutationKey: DeletePartnerAdminInvitation().key,
        mutationFn: DeletePartnerAdminInvitation().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Invitation deleted' });
            queryClient.invalidateQueries({
                queryKey: GetPartnerAdminInvitationsByPartnerId({ partnerId }).key,
                refetchType: 'all',
            });
        },
        onError: () => toast({ variant: 'destructive', title: 'Server error' }),
    });

    const removeAdmin = (userId: string, email: string) => {
        if (
            !window.confirm(
                `Please confirm that you want to remove user with email: '${email}' as a user from this Partner?`,
            )
        )
            return;
        deletePartnerAdminsByPartnerIAndUserId.mutate({ partnerId, userId });
    };

    const resendInvitation = (invitationId: string, email: string) => {
        if (!window.confirm(`Please confirm you want to resend invite to user with Email: '${email}'`)) return;
        resendPartnerAdminInvitation.mutate({ invitationId });
    };
    const cancelInvitation = (invitationId: string, email: string) => {
        if (!window.confirm(`Please confirm you want to cancel invite for user with Email: '${email}'`)) return;
        deletePartnerAdminInvitation.mutate({ invitationId });
    };

    return (
        <SidebarLayout
            title='Edit Partner Admins'
            description='This page allows you to add more administrators to this partner so you can share the load of maintaining the configuration of the partner. You can invite new administrators by clicking the Invite Administrator button, and entering their email address into the text box and clicking "Send Invitation." The email address you set will be sent an invite to join TrashMob.eco if they are not already a user. Once they have joined TrashMob.eco and are logged in, they will see an invitation in their Dashboard. They can Accept or Decline the invitation from there.'
            useDefaultCard={false}
        >
            <div className='space-y-4'>
                <Card>
                    <CardHeader>
                        <div className='flex justify-between'>
                            <CardTitle>Current Admins</CardTitle>
                            <Button asChild>
                                <Link to='invite'>
                                    <Plus /> Invite Admin
                                </Link>
                            </Button>
                        </div>
                    </CardHeader>
                    <CardContent>
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Username</TableHead>
                                    <TableHead>Email</TableHead>
                                    <TableHead className='text-right'>Actions</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {(admins || []).map((admin) => {
                                    return (
                                        <TableRow key={admin.id}>
                                            <TableCell>{admin.userName}</TableCell>
                                            <TableCell>{admin.email}</TableCell>
                                            <TableCell className='text-right'>
                                                <DropdownMenu>
                                                    <DropdownMenuTrigger asChild>
                                                        <Button variant='ghost' size='icon'>
                                                            <Ellipsis />
                                                        </Button>
                                                    </DropdownMenuTrigger>
                                                    <DropdownMenuContent className='w-56'>
                                                        <DropdownMenuItem
                                                            onClick={() => removeAdmin(admin.id, admin.userName)}
                                                        >
                                                            <ToggleRight />
                                                            Remove Admin
                                                        </DropdownMenuItem>
                                                    </DropdownMenuContent>
                                                </DropdownMenu>
                                            </TableCell>
                                        </TableRow>
                                    );
                                })}
                            </TableBody>
                        </Table>
                    </CardContent>
                </Card>
                <Dialog open={!!isInviting} onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/admins`)}>
                    <DialogContent className='sm:max-w-[680px]' onOpenAutoFocus={(e) => e.preventDefault()}>
                        <DialogHeader>
                            <DialogTitle>Invite Admin</DialogTitle>
                        </DialogHeader>
                        <div>
                            <Outlet />
                        </div>
                    </DialogContent>
                </Dialog>
                <Card>
                    <CardHeader>
                        <CardTitle>Pending Invitations</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Email</TableHead>
                                    <TableHead>Status</TableHead>
                                    <TableHead className='text-right'>Actions</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {(invitations || []).map((invitation) => {
                                    return (
                                        <TableRow key={invitation.id}>
                                            <TableCell>{invitation.email}</TableCell>
                                            <TableCell>
                                                <Badge>{getInvitationStatus(invitation.invitationStatusId).name}</Badge>
                                            </TableCell>
                                            <TableCell className='text-right'>
                                                <DropdownMenu>
                                                    <DropdownMenuTrigger asChild>
                                                        <Button variant='ghost' size='icon'>
                                                            <Ellipsis />
                                                        </Button>
                                                    </DropdownMenuTrigger>
                                                    <DropdownMenuContent className='w-56'>
                                                        <DropdownMenuItem
                                                            onClick={() =>
                                                                resendInvitation(invitation.id, invitation.email)
                                                            }
                                                        >
                                                            <Mail />
                                                            Resend Invite
                                                        </DropdownMenuItem>
                                                        <DropdownMenuItem
                                                            onClick={() =>
                                                                cancelInvitation(invitation.id, invitation.email)
                                                            }
                                                        >
                                                            <SquareX />
                                                            Cancel Invite
                                                        </DropdownMenuItem>
                                                    </DropdownMenuContent>
                                                </DropdownMenu>
                                            </TableCell>
                                        </TableRow>
                                    );
                                })}
                            </TableBody>
                        </Table>
                    </CardContent>
                </Card>
            </div>
        </SidebarLayout>
    );
};
