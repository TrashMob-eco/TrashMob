import { ApiService } from '.';
import { DisplayAnonymizedRoute, DisplayEventRouteStats, DisplayUserRouteHistory } from '../components/Models/RouteData';

export type GetEventRoutes_Params = { eventId: string };
export type GetEventRoutes_Response = DisplayAnonymizedRoute[];
export const GetEventRoutes = (params: GetEventRoutes_Params) => ({
    key: ['/events/', params.eventId, '/routes'],
    service: async () =>
        ApiService('public').fetchData<GetEventRoutes_Response>({
            url: `/events/${params.eventId}/routes`,
            method: 'get',
        }),
});

export type GetEventRouteStats_Params = { eventId: string };
export type GetEventRouteStats_Response = DisplayEventRouteStats;
export const GetEventRouteStats = (params: GetEventRouteStats_Params) => ({
    key: ['/events/', params.eventId, '/routes/stats'],
    service: async () =>
        ApiService('public').fetchData<GetEventRouteStats_Response>({
            url: `/events/${params.eventId}/routes/stats`,
            method: 'get',
        }),
});

export type GetMyRoutes_Response = DisplayUserRouteHistory[];
export const GetMyRoutes = () => ({
    key: ['/users/me/routes'],
    service: async () =>
        ApiService('protected').fetchData<GetMyRoutes_Response>({
            url: '/users/me/routes',
            method: 'get',
        }),
});
