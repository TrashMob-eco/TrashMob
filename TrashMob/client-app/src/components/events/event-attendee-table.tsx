import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { useToast } from '@/hooks/use-toast';
import { useLogin } from '@/hooks/useLogin';
import { GetEventLeads, PromoteToLead, DemoteFromLead, GetEventAttendees } from '@/services/events';
import { GetEventAttendeeWaiverStatus } from '@/services/user-waivers';
import { PaperWaiverUploadDialog } from '@/components/Waivers';
import UserData from '../Models/UserData';
import EventData from '../Models/EventData';
import { ChevronUp, ChevronDown, MoreHorizontal, FileText, Upload } from 'lucide-react';

interface EventAttendeeTableProps {
    users: UserData[];
    event: EventData;
}

export const EventAttendeeTable = (props: EventAttendeeTableProps) => {
    const { users, event } = props;
    const { currentUser } = useLogin();
    const { toast } = useToast();
    const queryClient = useQueryClient();

    // State for paper waiver upload dialog
    const [uploadDialogOpen, setUploadDialogOpen] = useState(false);
    const [selectedUserForUpload, setSelectedUserForUpload] = useState<UserData | null>(null);

    // Fetch event leads to determine who has lead status
    const { data: eventLeads } = useQuery({
        queryKey: GetEventLeads({ eventId: event.id }).key,
        queryFn: GetEventLeads({ eventId: event.id }).service,
        select: (res) => res.data,
        enabled: !!event.id,
    });

    // Check if current user is an event lead
    const isCurrentUserLead = (eventLeads || []).some((lead) => lead.id === currentUser.id);

    // Fetch waiver status for attendees (only if current user is lead)
    const { data: waiverStatuses } = useQuery({
        queryKey: GetEventAttendeeWaiverStatus({ eventId: event.id }).key,
        queryFn: GetEventAttendeeWaiverStatus({ eventId: event.id }).service,
        select: (res) => res.data,
        enabled: !!event.id && isCurrentUserLead,
    });

    // Create a map of userId -> waiver status for quick lookup
    const waiverStatusMap = new Map(
        (waiverStatuses || []).map((status) => [status.userId, status.hasValidWaiver])
    );

    // Check if a user is a lead
    const isUserLead = (userId: string) => {
        return (eventLeads || []).some((lead) => lead.id === userId);
    };

    // Promote mutation
    const promoteMutation = useMutation({
        mutationKey: PromoteToLead().key,
        mutationFn: PromoteToLead().service,
        onSuccess: async () => {
            toast({ variant: 'primary', title: 'User promoted to event lead' });
            await queryClient.invalidateQueries({ queryKey: GetEventLeads({ eventId: event.id }).key });
            await queryClient.invalidateQueries({ queryKey: GetEventAttendees({ eventId: event.id }).key });
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Failed to promote user', description: error.message });
        },
    });

    // Demote mutation
    const demoteMutation = useMutation({
        mutationKey: DemoteFromLead().key,
        mutationFn: DemoteFromLead().service,
        onSuccess: async () => {
            toast({ variant: 'primary', title: 'User demoted from event lead' });
            await queryClient.invalidateQueries({ queryKey: GetEventLeads({ eventId: event.id }).key });
            await queryClient.invalidateQueries({ queryKey: GetEventAttendees({ eventId: event.id }).key });
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Failed to demote user', description: error.message });
        },
    });

    const handlePromote = (userId: string) => {
        promoteMutation.mutate({ eventId: event.id, userId });
    };

    const handleDemote = (userId: string) => {
        demoteMutation.mutate({ eventId: event.id, userId });
    };

    const handleOpenUploadDialog = (user: UserData) => {
        setSelectedUserForUpload(user);
        setUploadDialogOpen(true);
    };

    const handleCloseUploadDialog = () => {
        setUploadDialogOpen(false);
        setSelectedUserForUpload(null);
    };

    const handleWaiverUploaded = async () => {
        toast({ variant: 'primary', title: 'Paper waiver uploaded successfully' });
        handleCloseUploadDialog();
        await queryClient.invalidateQueries({
            queryKey: GetEventAttendeeWaiverStatus({ eventId: event.id }).key,
        });
    };

    // Count leads to prevent demoting the last one
    const leadCount = (eventLeads || []).length;

    return (
        <>
            <div className='overflow-auto'>
                <Table className='table table-striped' aria-labelledby='tableLabel'>
                    <TableHeader>
                        <TableRow className='bg-ice'>
                            <TableHead>User Name</TableHead>
                            <TableHead>City</TableHead>
                            <TableHead>Country</TableHead>
                            <TableHead>Member Since</TableHead>
                            {isCurrentUserLead ? <TableHead>Waiver</TableHead> : null}
                            {isCurrentUserLead ? <TableHead className='w-[100px]'>Actions</TableHead> : null}
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {(users || []).map((user) => {
                            const userIsLead = isUserLead(user.id);
                            const canDemote = userIsLead && leadCount > 1;
                            const canPromote = !userIsLead && leadCount < 5;
                            const hasValidWaiver = waiverStatusMap.get(user.id) ?? false;

                            return (
                                <TableRow key={user.id.toString()}>
                                    <TableCell>
                                        <span className='flex items-center gap-2'>
                                            {user.userName}
                                            {userIsLead ? (
                                                <Badge variant='secondary' className='text-xs'>
                                                    Lead
                                                </Badge>
                                            ) : null}
                                        </span>
                                    </TableCell>
                                    <TableCell>{user.city}</TableCell>
                                    <TableCell>{user.country}</TableCell>
                                    <TableCell>{new Date(user.memberSince).toLocaleDateString()}</TableCell>
                                    {isCurrentUserLead ? (
                                        <TableCell>
                                            {hasValidWaiver ? (
                                                <Badge
                                                    variant='outline'
                                                    className='text-xs bg-green-50 text-green-700 border-green-200'
                                                >
                                                    <FileText className='mr-1 h-3 w-3' />
                                                    Signed
                                                </Badge>
                                            ) : (
                                                <Badge
                                                    variant='outline'
                                                    className='text-xs bg-amber-50 text-amber-700 border-amber-200'
                                                >
                                                    Pending
                                                </Badge>
                                            )}
                                        </TableCell>
                                    ) : null}
                                    {isCurrentUserLead ? (
                                        <TableCell>
                                            <DropdownMenu>
                                                <DropdownMenuTrigger asChild>
                                                    <Button variant='ghost' size='icon'>
                                                        <MoreHorizontal className='h-4 w-4' />
                                                    </Button>
                                                </DropdownMenuTrigger>
                                                <DropdownMenuContent align='end'>
                                                    {canPromote ? (
                                                        <DropdownMenuItem
                                                            onClick={() => handlePromote(user.id)}
                                                            disabled={promoteMutation.isPending}
                                                        >
                                                            <ChevronUp className='mr-2 h-4 w-4' />
                                                            Promote to Lead
                                                        </DropdownMenuItem>
                                                    ) : null}
                                                    {canDemote ? (
                                                        <DropdownMenuItem
                                                            onClick={() => handleDemote(user.id)}
                                                            disabled={demoteMutation.isPending}
                                                        >
                                                            <ChevronDown className='mr-2 h-4 w-4' />
                                                            Remove Lead Status
                                                        </DropdownMenuItem>
                                                    ) : null}
                                                    {(canPromote || canDemote) && !hasValidWaiver ? (
                                                        <DropdownMenuSeparator />
                                                    ) : null}
                                                    {!hasValidWaiver ? (
                                                        <DropdownMenuItem onClick={() => handleOpenUploadDialog(user)}>
                                                            <Upload className='mr-2 h-4 w-4' />
                                                            Upload Paper Waiver
                                                        </DropdownMenuItem>
                                                    ) : null}
                                                    {!canPromote && !canDemote && hasValidWaiver ? (
                                                        <DropdownMenuItem disabled>No actions available</DropdownMenuItem>
                                                    ) : null}
                                                </DropdownMenuContent>
                                            </DropdownMenu>
                                        </TableCell>
                                    ) : null}
                                </TableRow>
                            );
                        })}
                    </TableBody>
                </Table>
            </div>

            {/* Paper Waiver Upload Dialog */}
            {selectedUserForUpload ? (
                <PaperWaiverUploadDialog
                    open={uploadDialogOpen}
                    onClose={handleCloseUploadDialog}
                    onUploaded={handleWaiverUploaded}
                    userId={selectedUserForUpload.id}
                    userName={selectedUserForUpload.userName}
                    eventId={event.id}
                />
            ) : null}
        </>
    );
};
