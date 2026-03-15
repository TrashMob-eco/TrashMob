// Participation Report API service

import { ApiService } from '.';

export type RequestReport_Params = { eventId: string };
export const RequestParticipationReport = () => ({
    key: ['/participation-report', 'request'],
    service: async (params: RequestReport_Params) =>
        ApiService('protected').fetchData<void>({
            url: `/v2/events/${params.eventId}/participation-report`,
            method: 'post',
        }),
});

export type SendAllReports_Params = { eventId: string };
export type SendAllReports_Response = { sentCount: number };
export const SendAllParticipationReports = () => ({
    key: ['/participation-report', 'send-all'],
    service: async (params: SendAllReports_Params) =>
        ApiService('protected').fetchData<SendAllReports_Response>({
            url: `/v2/events/${params.eventId}/participation-report/send-all`,
            method: 'post',
        }),
});
