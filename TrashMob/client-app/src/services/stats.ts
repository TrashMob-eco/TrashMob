import { ApiService } from '.';
import StatsData from '../components/Models/StatsData';
import { trashmobApiClient } from '../api/trashmob-api'
import type { Stats } from "../api/trashmob-api.v2.generated";

export const GetStats = () => ({
  key: ['/stats/'],
  service: async (): Promise<Stats> => {
    return await trashmobApiClient.stats();
  }
});

export type GetStatsForUser_Params = { userId: string };
export type GetStatsForUser_Response = StatsData;
export const GetStatsForUser = (params: GetStatsForUser_Params) => ({
    key: ['/stats/', params],
    service: async () =>
        ApiService('protected').fetchData<GetStatsForUser_Response>({
            url: `/stats/${params.userId}`,
            method: 'get',
        }),
});
