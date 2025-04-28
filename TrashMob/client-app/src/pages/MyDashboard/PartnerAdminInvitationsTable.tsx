import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { Ellipsis, SquareCheck, SquareX } from 'lucide-react';
import {
    AcceptPartnerAdminInvitation,
    DeclinePartnerAdminInvitation,
    GetPartnerAdminInvitationsByUser,
} from '@/services/invitations';
import { GetPartnerAdminsForUser } from '@/services/admin';

import DisplayPartnerAdminInvitationData from '@/components/Models/DisplayPartnerAdminInvitationData';

type PartnerAdminInvitationTableProps = {
    readonly items: DisplayPartnerAdminInvitationData[];
    readonly userId: string;
};

export const PartnerAdminInvitationsTable = ({ userId, items }: PartnerAdminInvitationTableProps) => {
    const queryClient = useQueryClient();

    const acceptPartnerAdminInvitation = useMutation({
        mutationKey: AcceptPartnerAdminInvitation().key,
        mutationFn: AcceptPartnerAdminInvitation().service,
        onSuccess: () => {
            queryClient.invalidateQueries([
                GetPartnerAdminInvitationsByUser({ userId }).key,
                GetPartnerAdminsForUser({ userId }).key,
            ]);
        },
    });

    const declinePartnerAdminInvitation = useMutation({
        mutationKey: DeclinePartnerAdminInvitation().key,
        mutationFn: DeclinePartnerAdminInvitation().service,
        onSuccess: () => {
            queryClient.invalidateQueries([
                GetPartnerAdminInvitationsByUser({ userId }).key,
                GetPartnerAdminsForUser({ userId }).key,
            ]);
        },
    });

    const headerTitles = ['Partner Name', 'Actions'];
    return (
        <Table>
            <TableHeader>
                <TableRow>
                    {headerTitles.map((header) => (
                        <TableHead key={header}>{header}</TableHead>
                    ))}
                </TableRow>
            </TableHeader>
            <TableBody>
                {(items || []).map((displayInvitation) => (
                    <TableRow key={displayInvitation.id.toString()}>
                        <TableCell>{displayInvitation.partnerName}</TableCell>
                        <TableCell className='py-0'>
                            <DropdownMenu>
                                <DropdownMenuTrigger asChild>
                                    <Button size='icon' variant='ghost'>
                                        <Ellipsis />
                                    </Button>
                                </DropdownMenuTrigger>
                                <DropdownMenuContent className='w-56'>
                                    <DropdownMenuItem
                                        onClick={() =>
                                            acceptPartnerAdminInvitation.mutate({ invitationId: displayInvitation.id })
                                        }
                                    >
                                        <SquareCheck />
                                        Accept Invitation
                                    </DropdownMenuItem>
                                    <DropdownMenuItem
                                        onClick={() =>
                                            declinePartnerAdminInvitation.mutate({ invitationId: displayInvitation.id })
                                        }
                                    >
                                        <SquareX />
                                        Decline Invitation
                                    </DropdownMenuItem>
                                </DropdownMenuContent>
                            </DropdownMenu>
                        </TableCell>
                    </TableRow>
                ))}
            </TableBody>
        </Table>
    );
};
