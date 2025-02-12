import { Ellipsis, Loader2, SquareCheck, SquareX } from 'lucide-react';
import { useCallback, useState } from 'react';
import { useQueryClient, useMutation, useQuery } from '@tanstack/react-query';
import moment from 'moment';
import { useGetPartnerServiceTypes } from '@/hooks/useGetPartnerServiceTypes';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';

import DisplayPartnerLocationEventServiceData from '@/components/Models/DisplayPartnerLocationEventServiceData';
import EventPartnerLocationServiceStatusData from '@/components/Models/EventPartnerLocationServiceStatusData';
import ServiceTypeData from '@/components/Models/ServiceTypeData';
import {
    GetEventPartnerLocationServiceStatuses,
    GetPartnerLocationEventServicesByLocationId,
    UpdateEventPartnerLocationServices,
    GetPartnerLocationEventServicesByUserId,
} from '@/services/locations';
import { useLogin } from '@/hooks/useLogin';

const useGetEventPartnerLocationServiceStatuses = () => {
    return useQuery({
        queryKey: GetEventPartnerLocationServiceStatuses().key,
        queryFn: GetEventPartnerLocationServiceStatuses().service,
        select: (res) => res.data,
    });
};

interface PartnerEventRequestTableProps {
    isLoading: boolean;
    data?: DisplayPartnerLocationEventServiceData[];
}
export const PartnerEventRequestTable = ({ isLoading, data }: PartnerEventRequestTableProps) => {
    const queryClient = useQueryClient();
    const { currentUser } = useLogin();
    const [isUpdating, setIsUpdating] = useState<string | null>(null);

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

    const updateEventPartnerLocationServices = useMutation({
        mutationKey: UpdateEventPartnerLocationServices().key,
        mutationFn: UpdateEventPartnerLocationServices().service,
    });

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
                return Promise.all([
                    queryClient.invalidateQueries({
                        queryKey: GetPartnerLocationEventServicesByLocationId({ locationId: partnerLocationId }).key,
                        refetchType: 'all',
                    }),
                    queryClient.invalidateQueries({
                        queryKey: GetPartnerLocationEventServicesByUserId({ userId: currentUser.id }).key,
                        refetchType: 'all',
                    }),
                ]);
            })
            .finally(() => setIsUpdating(null));
    }

    return (
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
                {isLoading ? (
                    <TableRow>
                        <TableCell colSpan={7} className='text-center'>
                            <Loader2 className='animate-spin mx-auto my-4' />
                        </TableCell>
                    </TableRow>
                ) : data && data.length === 0 ? (
                    <TableRow>
                        <TableCell colSpan={7} className='text-center'>
                            <div className='mx-auto my-4'>There are no event requests for this partner.</div>
                        </TableCell>
                    </TableRow>
                ) : null}
                {(data || []).map((eventRequest, index) => {
                    const status = getStatus(eventRequest.eventPartnerLocationStatusId);
                    return (
                        <TableRow
                            key={`${eventRequest.eventId}-${index}`}
                            className={isUpdating === eventRequest.eventId ? 'opacity-20' : ''}
                        >
                            <TableCell>{eventRequest.partnerLocationName}</TableCell>
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
    );
};
