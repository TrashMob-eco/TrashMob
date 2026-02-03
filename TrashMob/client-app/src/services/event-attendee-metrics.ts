// Event Attendee Metrics API service

import { ApiService } from '.';
import EventAttendeeMetricsData from '../components/Models/EventAttendeeMetricsData';
import AttendeeMetricsTotals from '../components/Models/AttendeeMetricsTotals';

// ============================================================================
// Attendee Operations
// ============================================================================

export type GetMyMetrics_Params = { eventId: string };
export type GetMyMetrics_Response = EventAttendeeMetricsData;
export const GetMyMetrics = (params: GetMyMetrics_Params) => ({
    key: ['/events/', params.eventId, '/attendee-metrics/my-metrics'],
    service: async () =>
        ApiService('protected').fetchData<GetMyMetrics_Response>({
            url: `/events/${params.eventId}/attendee-metrics/my-metrics`,
            method: 'get',
        }),
});

export type SubmitMyMetrics_Params = { eventId: string };
export type SubmitMyMetrics_Body = Partial<EventAttendeeMetricsData>;
export type SubmitMyMetrics_Response = EventAttendeeMetricsData;
export const SubmitMyMetrics = () => ({
    key: ['/events/attendee-metrics', 'submit'],
    service: async (params: SubmitMyMetrics_Params, body: SubmitMyMetrics_Body) =>
        ApiService('protected').fetchData<SubmitMyMetrics_Response, SubmitMyMetrics_Body>({
            url: `/events/${params.eventId}/attendee-metrics/my-metrics`,
            method: 'post',
            data: body,
        }),
});

// ============================================================================
// Event Lead Operations
// ============================================================================

export type GetAllMetrics_Params = { eventId: string };
export type GetAllMetrics_Response = EventAttendeeMetricsData[];
export const GetAllMetrics = (params: GetAllMetrics_Params) => ({
    key: ['/events/', params.eventId, '/attendee-metrics'],
    service: async () =>
        ApiService('protected').fetchData<GetAllMetrics_Response>({
            url: `/events/${params.eventId}/attendee-metrics`,
            method: 'get',
        }),
});

export type GetPendingMetrics_Params = { eventId: string };
export type GetPendingMetrics_Response = EventAttendeeMetricsData[];
export const GetPendingMetrics = (params: GetPendingMetrics_Params) => ({
    key: ['/events/', params.eventId, '/attendee-metrics/pending'],
    service: async () =>
        ApiService('protected').fetchData<GetPendingMetrics_Response>({
            url: `/events/${params.eventId}/attendee-metrics/pending`,
            method: 'get',
        }),
});

export type ApproveMetrics_Params = { eventId: string; metricsId: string };
export type ApproveMetrics_Response = EventAttendeeMetricsData;
export const ApproveMetrics = () => ({
    key: ['/events/attendee-metrics', 'approve'],
    service: async (params: ApproveMetrics_Params) =>
        ApiService('protected').fetchData<ApproveMetrics_Response>({
            url: `/events/${params.eventId}/attendee-metrics/${params.metricsId}/approve`,
            method: 'post',
        }),
});

export type RejectMetrics_Params = { eventId: string; metricsId: string };
export type RejectMetrics_Body = { rejectionReason: string };
export type RejectMetrics_Response = EventAttendeeMetricsData;
export const RejectMetrics = () => ({
    key: ['/events/attendee-metrics', 'reject'],
    service: async (params: RejectMetrics_Params, body: RejectMetrics_Body) =>
        ApiService('protected').fetchData<RejectMetrics_Response, RejectMetrics_Body>({
            url: `/events/${params.eventId}/attendee-metrics/${params.metricsId}/reject`,
            method: 'post',
            data: body,
        }),
});

export type AdjustMetrics_Params = { eventId: string; metricsId: string };
export type AdjustMetrics_Body = {
    adjustedBagsCollected?: number;
    adjustedPickedWeight?: number;
    adjustedPickedWeightUnitId?: number;
    adjustedDurationMinutes?: number;
    adjustmentReason: string;
};
export type AdjustMetrics_Response = EventAttendeeMetricsData;
export const AdjustMetrics = () => ({
    key: ['/events/attendee-metrics', 'adjust'],
    service: async (params: AdjustMetrics_Params, body: AdjustMetrics_Body) =>
        ApiService('protected').fetchData<AdjustMetrics_Response, AdjustMetrics_Body>({
            url: `/events/${params.eventId}/attendee-metrics/${params.metricsId}/adjust`,
            method: 'put',
            data: body,
        }),
});

export type ApproveAllPending_Params = { eventId: string };
export type ApproveAllPending_Response = number;
export const ApproveAllPending = () => ({
    key: ['/events/attendee-metrics', 'approve-all'],
    service: async (params: ApproveAllPending_Params) =>
        ApiService('protected').fetchData<ApproveAllPending_Response>({
            url: `/events/${params.eventId}/attendee-metrics/approve-all`,
            method: 'post',
        }),
});

export type GetMetricsTotals_Params = { eventId: string };
export type GetMetricsTotals_Response = AttendeeMetricsTotals;
export const GetMetricsTotals = (params: GetMetricsTotals_Params) => ({
    key: ['/events/', params.eventId, '/attendee-metrics/totals'],
    service: async () =>
        ApiService('protected').fetchData<GetMetricsTotals_Response>({
            url: `/events/${params.eventId}/attendee-metrics/totals`,
            method: 'get',
        }),
});
