import { useQueries, UseQueryResult } from '@tanstack/react-query';
import { useParams } from 'react-router';
import { SidebarLayout } from '../../layouts/_layout.sidebar';
import { GetPartnerLocationEventServicesByLocationId } from '@/services/locations';
import { useGetPartnerLocations } from '@/hooks/useGetPartnerLocations';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import DisplayPartnerLocationEventServiceData from '@/components/Models/DisplayPartnerLocationEventServiceData';
import { AxiosResponse, AxiosError } from 'axios';
import { PartnerEventRequestTable } from '@/components/Partners/partner-event-request-table';

export const PartnerIndex = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };

    const { data: locations, isLoading, error: locationsError } = useGetPartnerLocations({ partnerId });
    const eventRequestsByLocation: UseQueryResult<DisplayPartnerLocationEventServiceData[], AxiosError>[] = useQueries({
        queries: (locations || []).map((location, locIndex) => {
            return {
                queryKey: GetPartnerLocationEventServicesByLocationId({ locationId: location.id }).key,
                queryFn: GetPartnerLocationEventServicesByLocationId({ locationId: location.id }).service,
                select: (res: AxiosResponse<DisplayPartnerLocationEventServiceData[]>) => res.data,
            };
        }),
    });

    const eventRequests = eventRequestsByLocation.map((loc) => loc.data || []).flat();
    const hasAuthError = eventRequestsByLocation.some(
        (query) => query.error && (query.error as AxiosError)?.response?.status === 403,
    );
    const hasError = locationsError || eventRequestsByLocation.some((query) => query.isError);

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
                        <PartnerEventRequestTable
                            isLoading={isLoading || eventRequestsByLocation.some((query) => query.isLoading)}
                            data={eventRequests}
                            hasError={hasError}
                            hasAuthError={hasAuthError}
                        />
                    </CardContent>
                </Card>
            </div>
        </SidebarLayout>
    );
};
