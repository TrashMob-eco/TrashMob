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

// ---------- Monthly report (Project 63 Phase 3) ----------

export interface MonthlySalesMetricDto {
    metric: number;
    metricName: string;
    label: string;
    target: number;
    actual: number;
    status: 'Behind' | 'OnTrack' | 'Exceeded' | 'NoTarget';
}

export interface MarketIntelligenceCountDto {
    label: string;
    count: number;
}

export interface MonthlySalesReportDto {
    periodStart: string;
    periodEnd: string;
    metrics: MonthlySalesMetricDto[];
    bestRespondingDepartments: MarketIntelligenceCountDto[];
    commonObjections: MarketIntelligenceCountDto[];
    pricingFeedback: MarketIntelligenceCountDto[];
    nextMonthPriority: string | null;
}

export type GetMonthlySalesReport_Params = { month?: string };

export const GetMonthlySalesReport = (params: GetMonthlySalesReport_Params = {}) => ({
    key: ['/reports/sales/monthly', params.month ?? 'this-month'],
    service: async () =>
        ApiService('protected').fetchData<MonthlySalesReportDto>({
            url: `/v2/reports/sales/monthly${params.month ? `?month=${params.month}` : ''}`,
            method: 'get',
        }),
});

export interface MonthlyTargetUpdate {
    metric: number;
    target: number;
}

export interface UpdateMonthlyTargetsBody {
    targets: MonthlyTargetUpdate[];
}

export type UpdateMonthlyTargets_Params = { month: string };

export const UpdateMonthlyTargets = (params: UpdateMonthlyTargets_Params) => ({
    key: ['/reports/sales/monthly', params.month, 'targets'],
    service: async (body: UpdateMonthlyTargetsBody) =>
        ApiService('protected').fetchData<unknown, UpdateMonthlyTargetsBody>({
            url: `/v2/reports/sales/monthly/${params.month}/targets`,
            method: 'put',
            data: body,
        }),
});
