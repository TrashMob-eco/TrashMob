import { ApiService } from '.';

/**
 * V2 API service factories for the sales-report distribution list
 * (Project 63 Phase 4b). SiteAdmin only.
 */

export interface SalesReportSubscriberDto {
    id: string;
    userId: string;
    userName: string | null;
    email: string | null;
    includeWeekly: boolean;
    includeMonthly: boolean;
}

export const GetSalesReportSubscribers = () => ({
    key: ['/reports/sales/subscribers'],
    service: async () =>
        ApiService('protected').fetchData<SalesReportSubscriberDto[]>({
            url: '/v2/reports/sales/subscribers',
            method: 'get',
        }),
});

export interface AddSalesReportSubscriberBody {
    userId: string;
    includeWeekly: boolean;
    includeMonthly: boolean;
}

export const AddSalesReportSubscriber = () => ({
    key: ['/reports/sales/subscribers', 'add'],
    service: async (body: AddSalesReportSubscriberBody) =>
        ApiService('protected').fetchData<SalesReportSubscriberDto, AddSalesReportSubscriberBody>({
            url: '/v2/reports/sales/subscribers',
            method: 'post',
            data: body,
        }),
});

export interface UpdateSalesReportSubscriberBody {
    includeWeekly: boolean;
    includeMonthly: boolean;
}

export type UpdateSalesReportSubscriber_Params = { subscriptionId: string };

export const UpdateSalesReportSubscriber = (params: UpdateSalesReportSubscriber_Params) => ({
    key: ['/reports/sales/subscribers', params.subscriptionId, 'update'],
    service: async (body: UpdateSalesReportSubscriberBody) =>
        ApiService('protected').fetchData<SalesReportSubscriberDto, UpdateSalesReportSubscriberBody>({
            url: `/v2/reports/sales/subscribers/${params.subscriptionId}`,
            method: 'put',
            data: body,
        }),
});

export const DeleteSalesReportSubscriber = () => ({
    key: ['/reports/sales/subscribers', 'delete'],
    service: async (params: { subscriptionId: string }) =>
        ApiService('protected').fetchData<unknown>({
            url: `/v2/reports/sales/subscribers/${params.subscriptionId}`,
            method: 'delete',
        }),
});
