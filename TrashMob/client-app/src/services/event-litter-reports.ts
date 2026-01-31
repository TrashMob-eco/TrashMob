import { EventLitterReportData, FullEventLitterReportData } from '@/components/Models/EventLitterReportData';
import { ApiService } from '.';

export type GetEventLitterReports_Params = { eventId: string };
export type GetEventLitterReports_Response = FullEventLitterReportData[];
export const GetEventLitterReports = (params: GetEventLitterReports_Params) => ({
    key: ['/eventlitterreports', params.eventId],
    service: async () =>
        ApiService('protected').fetchData<GetEventLitterReports_Response>({
            url: `/eventlitterreports/${params.eventId}`,
            method: 'get',
        }),
});

export type AddEventLitterReport_Params = {
    eventId: string;
    litterReportId: string;
    notes?: string;
};
export const AddEventLitterReport = () => ({
    key: ['/eventlitterreports', 'add'],
    service: async (params: AddEventLitterReport_Params) =>
        ApiService('protected').fetchData<void>({
            url: '/eventlitterreports',
            method: 'post',
            data: {
                eventId: params.eventId,
                litterReportId: params.litterReportId,
                notes: params.notes || '',
            } as Partial<EventLitterReportData>,
        }),
});

export type DeleteEventLitterReport_Params = { eventId: string; litterReportId: string };
export const DeleteEventLitterReport = (params: DeleteEventLitterReport_Params) => ({
    key: ['/eventlitterreports', 'delete', params.eventId, params.litterReportId],
    service: async () =>
        ApiService('protected').fetchData<void>({
            url: `/eventlitterreports/${params.eventId}/${params.litterReportId}`,
            method: 'delete',
        }),
});
