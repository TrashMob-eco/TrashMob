import { ApiService } from '.';
import StatsData from '../components/Models/StatsData';

export type GetStats_Response = StatsData;
export const GetStats = () => ({
    key: ['/stats'],
    service: async () =>
        ApiService('public')
            .fetchData<GetStats_Response>({ url: '/stats', method: 'get' })
            .then((res) => res.data),
});

export type GetStatsForUser_Params = { userId: string };
export type GetStatsForUser_Response = StatsData;
export const GetStatsForUser = (params: GetStatsForUser_Params) => ({
    key: ['/stats/', params],
    service: async () =>
        ApiService('protected').fetchData<GetStats_Response>({
            url: `/stats/${params.userId}`,
            method: 'get',
        }),
});
