import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import moment from 'moment';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { useMutation, useQueries, useQuery, useQueryClient, UseQueryResult } from '@tanstack/react-query';
import { Ellipsis, SquareX, SquareCheck, Loader2 } from 'lucide-react';
import { useParams } from 'react-router';
import { SidebarLayout } from './_layout.sidebar';
import { useCallback, useState } from 'react';
import {
    GetEventPartnerLocationServiceStatuses,
    GetPartnerLocationEventServicesByLocationId,
    UpdateEventPartnerLocationServices,
} from '@/services/locations';
import { Badge } from '@/components/ui/badge';
import { useGetPartnerLocations } from '@/hooks/useGetPartnerLocations';
import { getIndexedColor } from '@/lib/color';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useGetPartnerServiceTypes } from '@/hooks/useGetPartnerServiceTypes';
import ServiceTypeData from '@/components/Models/ServiceTypeData';
import DisplayPartnerLocationEventServiceData from '@/components/Models/DisplayPartnerLocationEventServiceData';
import { AxiosResponse } from 'axios';
import EventPartnerLocationServiceStatusData from '@/components/Models/EventPartnerLocationServiceStatusData';

type EventServiceWithBadge = DisplayPartnerLocationEventServiceData & {
    badgeColor: string;
};

const useGetEventPartnerLocationServiceStatuses = () => {
    return useQuery({
        queryKey: GetEventPartnerLocationServiceStatuses().key,
        queryFn: GetEventPartnerLocationServiceStatuses().service,
        select: (res) => res.data,
    });
};

export const PartnerIndex = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const queryClient = useQueryClient();

    const { data: serviceTypes } = useGetPartnerServiceTypes();
    const getServiceType = useCallback(
        (id: number): ServiceTypeData => {
            const st = (serviceTypes || [])?.find((st) => st.id === id);
            return st || ({ name: 'Unknown' } as ServiceTypeData);
        },
        [serviceTypes],
    );

    const { data: statuses } = useGetEventPartnerLocationServiceStatuses();
    const getStatus = useCallback(
        (id: number): EventPartnerLocationServiceStatusData => {
            const status = (statuses || [])?.find((st) => st.id === id);
            return status || ({ name: 'Unknown' } as EventPartnerLocationServiceStatusData);
        },
        [statuses],
    );

    const { data: locations, isLoading } = useGetPartnerLocations({ partnerId });
    const eventRequestsByLocation: UseQueryResult<EventServiceWithBadge[]>[] = useQueries({
        queries: (locations || []).map((location, locIndex) => {
            const badgeColor = getIndexedColor(locIndex);
            return {
                queryKey: GetPartnerLocationEventServicesByLocationId({ locationId: location.id }).key,
                queryFn: GetPartnerLocationEventServicesByLocationId({ locationId: location.id }).service,
                select: (res: AxiosResponse<DisplayPartnerLocationEventServiceData[]>) =>
                    (res.data || []).map((item) => ({ ...item, badgeColor })),
            };
        }),
    });

    const eventRequests = eventRequestsByLocation.map((loc) => loc.data || []).flat();

    const updateEventPartnerLocationServices = useMutation({
        mutationKey: UpdateEventPartnerLocationServices().key,
        mutationFn: UpdateEventPartnerLocationServices().service,
    });

    const [isUpdating, setIsUpdating] = useState<string | null>(null);

    function handleRequestPartnerAssistance(
        eventId: string,
        partnerLocationId: string,
        serviceTypeId: number,
        acceptDecline: 'accept' | 'decline',
    ) {
        setIsUpdating(eventId);
        updateEventPartnerLocationServices
            .mutateAsync({
                eventId,
                partnerLocationId,
                serviceTypeId,
                acceptDecline,
            })
            .then(() => {
                return queryClient.invalidateQueries({
                    queryKey: GetPartnerLocationEventServicesByLocationId({ locationId: partnerLocationId }).key,
                    refetchType: 'all',
                });
            })
            .finally(() => setIsUpdating(null));
    }

    return (
        <SidebarLayout
            title='Partner Location Service Requests'
            description='This page allows you to respond to requests from TrashMob.eco users to help them clean up the local community. When a new event is set up, and a user selects one of your services the location contacts will be notified to accept or decline the request here.'
            useDefaultCard={false}
        >
            <div className='space-y-4'>
                <Card>
                    <CardHeader>
                        <CardTitle>Service Requests</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div>
                            <Table>
                                <TableHeader>
                                    <TableRow>
                                        <TableHead>Location Name</TableHead>
                                        <TableHead>Event Name</TableHead>
                                        <TableHead>Event Date</TableHead>
                                        <TableHead>Event Address</TableHead>
                                        <TableHead>Service Type</TableHead>
                                        <TableHead>Status</TableHead>
                                        <TableHead>Action</TableHead>
                                    </TableRow>
                                </TableHeader>
                                <TableBody>
                                    {isLoading || eventRequestsByLocation.some((query) => query.isLoading) ? (
                                        <TableRow>
                                            <TableCell colSpan={7} className='text-center'>
                                                <Loader2 className='animate-spin mx-auto my-4' />
                                            </TableCell>
                                        </TableRow>
                                    ) : eventRequests.length === 0 ? (
                                        <TableRow>
                                            <TableCell colSpan={7} className='text-center'>
                                                <div className='mx-auto my-4'>
                                                    There are no event requests for this partner.
                                                </div>
                                            </TableCell>
                                        </TableRow>
                                    ) : null}
                                    {(eventRequests || []).map((eventRequest, index) => {
                                        const status = getStatus(eventRequest.eventPartnerLocationStatusId);
                                        return (
                                            <TableRow
                                                key={`${eventRequest.eventId}-${index}`}
                                                className={isUpdating === eventRequest.eventId ? 'opacity-20' : ''}
                                            >
                                                <TableCell>
                                                    <Badge className={eventRequest.badgeColor}>
                                                        {eventRequest.partnerLocationName}
                                                    </Badge>
                                                </TableCell>
                                                <TableCell>{eventRequest.eventName}</TableCell>
                                                <TableCell>{moment(eventRequest.eventDate).format('L')}</TableCell>
                                                <TableCell>
                                                    {eventRequest.eventStreetAddress},{eventRequest.eventCity}
                                                </TableCell>
                                                <TableCell>{getServiceType(eventRequest.serviceTypeId).name}</TableCell>
                                                <TableCell>
                                                    <Badge variant={status.name === 'Accepted' ? 'success' : 'default'}>
                                                        {status.name}
                                                    </Badge>
                                                </TableCell>
                                                <TableCell>
                                                    <DropdownMenu>
                                                        <DropdownMenuTrigger asChild>
                                                            <Button variant='ghost' size='icon'>
                                                                <Ellipsis />
                                                            </Button>
                                                        </DropdownMenuTrigger>
                                                        <DropdownMenuContent className='w-56'>
                                                            <DropdownMenuItem
                                                                onClick={() =>
                                                                    handleRequestPartnerAssistance(
                                                                        eventRequest.eventId,
                                                                        eventRequest.partnerLocationId,
                                                                        eventRequest.serviceTypeId,
                                                                        'accept',
                                                                    )
                                                                }
                                                            >
                                                                <SquareCheck />
                                                                Accept Partner Assistance Request
                                                            </DropdownMenuItem>
                                                            <DropdownMenuItem
                                                                onClick={() =>
                                                                    handleRequestPartnerAssistance(
                                                                        eventRequest.eventId,
                                                                        eventRequest.partnerLocationId,
                                                                        eventRequest.serviceTypeId,
                                                                        'decline',
                                                                    )
                                                                }
                                                            >
                                                                <SquareX />
                                                                Decline Partner Assistance Request
                                                            </DropdownMenuItem>
                                                        </DropdownMenuContent>
                                                    </DropdownMenu>
                                                </TableCell>
                                            </TableRow>
                                        );
                                    })}
                                </TableBody>
                            </Table>
                        </div>
                    </CardContent>
                </Card>
            </div>
        </SidebarLayout>
    );
};
