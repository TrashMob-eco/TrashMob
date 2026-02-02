import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { useToast } from '@/hooks/use-toast';
import { useLogin } from '@/hooks/useLogin';
import { GetEventLeads, PromoteToLead, DemoteFromLead, GetEventAttendees } from '@/services/events';
import UserData from '../Models/UserData';
import EventData from '../Models/EventData';
import { ChevronUp, ChevronDown, MoreHorizontal } from 'lucide-react';

interface EventAttendeeTableProps {
    users: UserData[];
    event: EventData;
}

export const EventAttendeeTable = (props: EventAttendeeTableProps) => {
    const { users, event } = props;
    const { currentUser } = useLogin();
    const { toast } = useToast();
    const queryClient = useQueryClient();

    // Fetch event leads to determine who has lead status
    const { data: eventLeads } = useQuery({
        queryKey: GetEventLeads({ eventId: event.id }).key,
        queryFn: GetEventLeads({ eventId: event.id }).service,
        select: (res) => res.data,
        enabled: !!event.id,
    });

    // Check if current user is an event lead
    const isCurrentUserLead = (eventLeads || []).some((lead) => lead.id === currentUser.id);

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

    // Count leads to prevent demoting the last one
    const leadCount = (eventLeads || []).length;

    return (
        <div className='overflow-auto'>
            <Table className='table table-striped' aria-labelledby='tableLabel'>
                <TableHeader>
                    <TableRow className='bg-ice'>
                        <TableHead>User Name</TableHead>
                        <TableHead>City</TableHead>
                        <TableHead>Country</TableHead>
                        <TableHead>Member Since</TableHead>
                        {isCurrentUserLead ? <TableHead className='w-[100px]'>Actions</TableHead> : null}
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {(users || []).map((user) => {
                        const userIsLead = isUserLead(user.id);
                        const canDemote = userIsLead && leadCount > 1;
                        const canPromote = !userIsLead && leadCount < 5;

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
                                                {!canPromote && !canDemote ? (
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
    );
};
