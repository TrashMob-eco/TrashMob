import { useQuery, useQueries } from '@tanstack/react-query';

import { GetEventAttendees, GetEventById } from '@/services/events';
import {
    GetEventPartnerLocationServices,
    GetEventPartnerLocationServicesByLocationId,
    GetEventPartnerLocationServiceStatuses,
} from '@/services/locations';

import { useGetEventTypes } from '@/hooks/useGetEventTypes';
import { useGetPartnerServiceTypes } from '@/hooks/useGetPartnerServiceTypes';

export const useEditEventPageQueries = (eventId: string) => {
    const { data: eventTypes } = useGetEventTypes();
    const { data: serviceTypes } = useGetPartnerServiceTypes();
    const { data: serviceStatuses } = useQuery({
        queryKey: GetEventPartnerLocationServiceStatuses().key,
        queryFn: GetEventPartnerLocationServiceStatuses().service,
        select: (res) => res.data,
    });
    const { data: event } = useQuery({
        queryKey: GetEventById({ eventId }).key,
        queryFn: GetEventById({ eventId }).service,
        select: (res) => res.data,
    });

    const { data: eventAttendees } = useQuery({
        queryKey: GetEventAttendees({ eventId }).key,
        queryFn: GetEventAttendees({ eventId }).service,
        select: (res) => res.data,
    });

    const { data: eventPartnerLocations } = useQuery({
        queryKey: GetEventPartnerLocationServices({ eventId }).key,
        queryFn: GetEventPartnerLocationServices({ eventId }).service,
        select: (res) => res.data,
    });

    const servicesByLocation = useQueries({
        queries: (eventPartnerLocations || []).map((item) => ({
            queryKey: GetEventPartnerLocationServicesByLocationId({ locationId: item.partnerLocationId, eventId }).key,
            queryFn: GetEventPartnerLocationServicesByLocationId({ locationId: item.partnerLocationId, eventId })
                .service,
            select: (res) => res.data,
        })),
    });

    return {
        // Masterdata
        eventTypes,
        serviceTypes,
        serviceStatuses,

        // Event
        event,
        eventAttendees,
        eventPartnerLocations,
        servicesByLocation,
    };
};
