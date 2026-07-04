import { ApiService } from '.';

/**
 * V2 API service factories for the municipal sales pipeline reports (Project 63).
 * SalesRep and SiteAdmin roles can consume these; anonymous callers are rejected.
 */

export interface WeeklySalesReportDto {
    periodStart: string;
    periodEnd: string;
    prospectsResearched: number;
    newContactsAdded: number;
    outreachTouches: number;
    followUpTouches: number;
    responses: number;
    meetingsRequested: number;
    meetingsScheduled: number;
    meetingsHeld: number;
    keyMunicipalFeedback: string[];
    pricingFeedback: string[];
    nextSteps: string | null;
}

export type GetWeeklySalesReport_Params = { weekEnding?: string };

export const GetWeeklySalesReport = (params: GetWeeklySalesReport_Params = {}) => ({
    key: ['/reports/sales/weekly', params.weekEnding ?? 'today'],
    service: async () =>
        ApiService('protected').fetchData<WeeklySalesReportDto>({
            url: `/v2/reports/sales/weekly${params.weekEnding ? `?weekEnding=${params.weekEnding}` : ''}`,
            method: 'get',
        }),
});
