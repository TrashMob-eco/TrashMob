// Leaderboards API service

import { ApiService } from '.';
import { LeaderboardOptions, LeaderboardResponse, UserRankResponse } from '../components/Models/LeaderboardData';

// ============================================================================
// Leaderboard Operations
// ============================================================================

export type GetLeaderboard_Params = {
    type?: string; // Events, Bags, Weight, Hours (default: Events)
    timeRange?: string; // Week, Month, Year, AllTime (default: Month)
    scope?: string; // Global, Region, City (default: Global)
    location?: string; // Required if scope is not Global
    limit?: number; // 1-100 (default: 50)
};
export type GetLeaderboard_Response = LeaderboardResponse;
export const GetLeaderboard = (params?: GetLeaderboard_Params) => ({
    key: ['/leaderboards', params],
    service: async () => {
        const queryParams = new URLSearchParams();
        if (params?.type) queryParams.append('type', params.type);
        if (params?.timeRange) queryParams.append('timeRange', params.timeRange);
        if (params?.scope) queryParams.append('scope', params.scope);
        if (params?.location) queryParams.append('location', params.location);
        if (params?.limit !== undefined) queryParams.append('limit', params.limit.toString());
        const queryString = queryParams.toString();
        return ApiService('public').fetchData<GetLeaderboard_Response>({
            url: `/leaderboards${queryString ? `?${queryString}` : ''}`,
            method: 'get',
        });
    },
});

export type GetMyRank_Params = {
    type?: string; // Events, Bags, Weight, Hours (default: Events)
    timeRange?: string; // Week, Month, Year, AllTime (default: AllTime)
};
export type GetMyRank_Response = UserRankResponse;
export const GetMyRank = (params?: GetMyRank_Params) => ({
    key: ['/leaderboards/my-rank', params],
    service: async () => {
        const queryParams = new URLSearchParams();
        if (params?.type) queryParams.append('type', params.type);
        if (params?.timeRange) queryParams.append('timeRange', params.timeRange);
        const queryString = queryParams.toString();
        return ApiService('protected').fetchData<GetMyRank_Response>({
            url: `/leaderboards/my-rank${queryString ? `?${queryString}` : ''}`,
            method: 'get',
        });
    },
});

export type GetLeaderboardOptions_Response = LeaderboardOptions;
export const GetLeaderboardOptions = () => ({
    key: ['/leaderboards/options'],
    service: async () =>
        ApiService('public').fetchData<GetLeaderboardOptions_Response>({
            url: '/leaderboards/options',
            method: 'get',
        }),
});
