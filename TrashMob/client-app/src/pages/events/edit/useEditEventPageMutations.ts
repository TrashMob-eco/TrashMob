import moment from 'moment';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router';
import { useLogin } from '@/hooks/useLogin';

import {
    GetUserEvents,
    UpdateEvent,
    PromoteToLead,
    DemoteFromLead,
    GetEventAttendees,
    GetEventLeads,
} from '@/services/events';
import {
    CreateEventPartnerLocationService,
    DeleteEventPartnerLocationService,
    GetEventPartnerLocationServicesByLocationId,
} from '@/services/locations';
import { useToast } from '@/hooks/use-toast';

export const useEditEventPageMutations = () => {
    const { toast } = useToast();
    const { currentUser } = useLogin();
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const updateEvent = useMutation({
        mutationKey: UpdateEvent().key,
        mutationFn: UpdateEvent().service,
        onSuccess: (data, variable) => {
            toast({
                duration: 10000,
                variant: 'primary',
                title: `Event ${variable.name} updated!`,
                description: `${moment(variable.eventDate).format('dddd, MMMM Do YYYY [at] h:mm a')}`,
            });
            queryClient.invalidateQueries({
                queryKey: GetUserEvents({ userId: currentUser.id }).key,
                refetchType: 'all',
            });
            navigate('/mydashboard', {
                state: {
                    newEventCreated: true,
                },
            });
        },
        onError: (error: Error) => {
            toast({
                variant: 'destructive',
                title: 'Update Event Error',
                description: error.message,
            });
        },
    });

    const createEventPartnerLocationService = useMutation({
        mutationKey: CreateEventPartnerLocationService().key,
        mutationFn: CreateEventPartnerLocationService().service,
        onSuccess: (_data, variables) => {
            const { eventId, partnerLocationId: locationId } = variables;
            queryClient.invalidateQueries({
                queryKey: GetEventPartnerLocationServicesByLocationId({ locationId, eventId }).key,
                refetchType: 'all',
            });
            toast({
                duration: 10000,
                variant: 'primary',
                title: 'Service requested!',
            });
        },
    });

    const deleteEventPartnerLocationService = useMutation({
        mutationKey: DeleteEventPartnerLocationService().key,
        mutationFn: DeleteEventPartnerLocationService().service,
        onSuccess: (_data, variables) => {
            const { eventId, partnerLocationId: locationId } = variables;
            queryClient.invalidateQueries({
                queryKey: GetEventPartnerLocationServicesByLocationId({ locationId, eventId }).key,
                refetchType: 'all',
            });
            toast({
                duration: 10000,
                variant: 'primary',
                title: 'Service removed!',
            });
        },
    });

    const promoteToLead = useMutation({
        mutationKey: PromoteToLead().key,
        mutationFn: PromoteToLead().service,
        onSuccess: (_data, variables) => {
            queryClient.invalidateQueries({
                queryKey: GetEventAttendees({ eventId: variables.eventId }).key,
                refetchType: 'all',
            });
            queryClient.invalidateQueries({
                queryKey: GetEventLeads({ eventId: variables.eventId }).key,
                refetchType: 'all',
            });
            toast({
                duration: 5000,
                variant: 'primary',
                title: 'Co-lead added!',
                description: 'The attendee has been promoted to event co-lead.',
            });
        },
        onError: (error: Error) => {
            toast({
                variant: 'destructive',
                title: 'Promote Error',
                description: error.message,
            });
        },
    });

    const demoteFromLead = useMutation({
        mutationKey: DemoteFromLead().key,
        mutationFn: DemoteFromLead().service,
        onSuccess: (_data, variables) => {
            queryClient.invalidateQueries({
                queryKey: GetEventAttendees({ eventId: variables.eventId }).key,
                refetchType: 'all',
            });
            queryClient.invalidateQueries({
                queryKey: GetEventLeads({ eventId: variables.eventId }).key,
                refetchType: 'all',
            });
            toast({
                duration: 5000,
                variant: 'primary',
                title: 'Co-lead removed',
                description: 'The co-lead has been demoted to regular attendee.',
            });
        },
        onError: (error: Error) => {
            toast({
                variant: 'destructive',
                title: 'Demote Error',
                description: error.message,
            });
        },
    });

    return {
        updateEvent,
        createEventPartnerLocationService,
        deleteEventPartnerLocationService,
        promoteToLead,
        demoteFromLead,
    };
};
