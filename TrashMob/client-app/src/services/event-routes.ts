import { ApiService } from '.';
import {
    DisplayAnonymizedRoute,
    DisplayEventAttendeeRoute,
    DisplayEventRouteStats,
    DisplayUserRouteHistory,
    EventSummaryPrefill,
    UpdateRouteMetadataRequest,
} from '../components/Models/RouteData';

export type GetEventRoutes_Params = { eventId: string };
export type GetEventRoutes_Response = DisplayAnonymizedRoute[];
export const GetEventRoutes = (params: GetEventRoutes_Params) => ({
    key: ['/events/', params.eventId, '/routes'],
    service: async () =>
        ApiService('public').fetchData<GetEventRoutes_Response>({
            url: `/v2/events/${params.eventId}/routes`,
            method: 'get',
        }),
});

export type GetEventRouteStats_Params = { eventId: string };
export type GetEventRouteStats_Response = DisplayEventRouteStats;
export const GetEventRouteStats = (params: GetEventRouteStats_Params) => ({
    key: ['/events/', params.eventId, '/routes/stats'],
    service: async () =>
        ApiService('public').fetchData<GetEventRouteStats_Response>({
            url: `/v2/events/${params.eventId}/routes/stats`,
            method: 'get',
        }),
});

export type GetMyRoutes_Response = DisplayUserRouteHistory[];
export const GetMyRoutes = () => ({
    key: ['/users/me/routes'],
    service: async () =>
        ApiService('protected').fetchData<GetMyRoutes_Response>({
            url: '/v2/users/me/routes',
            method: 'get',
        }),
});

export type TrimRouteTime_Params = { routeId: string; newEndTime: string };
export type TrimRouteTime_Response = DisplayEventAttendeeRoute;
export const TrimRouteTime = (params: TrimRouteTime_Params) => ({
    service: async () =>
        ApiService('protected').fetchData<TrimRouteTime_Response>({
            url: `/v2/eventattendeeroutes/${params.routeId}/trim-time`,
            method: 'put',
            data: { newEndTime: params.newEndTime },
        }),
});

export type RestoreRouteTime_Params = { routeId: string };
export type RestoreRouteTime_Response = DisplayEventAttendeeRoute;
export const RestoreRouteTime = (params: RestoreRouteTime_Params) => ({
    service: async () =>
        ApiService('protected').fetchData<RestoreRouteTime_Response>({
            url: `/v2/eventattendeeroutes/${params.routeId}/restore-time`,
            method: 'put',
        }),
});

export type GetEventSummaryPrefill_Params = { eventId: string; weightUnitId?: number };
export type GetEventSummaryPrefill_Response = EventSummaryPrefill;
export const GetEventSummaryPrefill = (params: GetEventSummaryPrefill_Params) => ({
    key: ['/events/', params.eventId, '/routes/summary-prefill'],
    service: async () =>
        ApiService('protected').fetchData<GetEventSummaryPrefill_Response>({
            url: `/v2/events/${params.eventId}/routes/summary-prefill?weightUnitId=${params.weightUnitId ?? 1}`,
            method: 'get',
        }),
});

export type GetEventAttendeeRoutesByEventId_Params = { eventId: string };
export type GetEventAttendeeRoutesByEventId_Response = DisplayEventAttendeeRoute[];
export const GetEventAttendeeRoutesByEventId = (params: GetEventAttendeeRoutesByEventId_Params) => ({
    key: ['/eventattendeeroutes/byeventid/', params.eventId],
    service: async () =>
        ApiService('protected').fetchData<GetEventAttendeeRoutesByEventId_Response>({
            url: `/v2/eventattendeeroutes/by-event/${params.eventId}`,
            method: 'get',
        }),
});

export type UpdateRouteMetadata_Params = { routeId: string };
export const UpdateRouteMetadata = (params: UpdateRouteMetadata_Params) => ({
    service: async (data: UpdateRouteMetadataRequest) =>
        ApiService('protected').fetchData<DisplayEventAttendeeRoute>({
            url: `/v2/eventattendeeroutes/${params.routeId}`,
            method: 'put',
            data,
        }),
});

export type DeleteEventAttendeeRoute_Params = { routeId: string };
export const DeleteEventAttendeeRoute = (params: DeleteEventAttendeeRoute_Params) => ({
    service: async () =>
        ApiService('protected').fetchData<void>({
            url: `/v2/eventattendeeroutes/${params.routeId}`,
            method: 'delete',
        }),
});
