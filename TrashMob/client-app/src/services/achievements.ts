// Achievements API service

import { ApiService } from '.';
import {
    AchievementType,
    NewAchievementNotification,
    UserAchievementsResponse,
} from '../components/Models/AchievementData';

// ============================================================================
// Achievement Type Operations
// ============================================================================

export type GetAchievementTypes_Response = AchievementType[];
export const GetAchievementTypes = () => ({
    key: ['/achievements/types'],
    service: async () =>
        ApiService('public').fetchData<GetAchievementTypes_Response>({
            url: '/achievements/types',
            method: 'get',
        }),
});

// ============================================================================
// User Achievement Operations
// ============================================================================

export type GetMyAchievements_Response = UserAchievementsResponse;
export const GetMyAchievements = () => ({
    key: ['/achievements/my'],
    service: async () =>
        ApiService('protected').fetchData<GetMyAchievements_Response>({
            url: '/achievements/my',
            method: 'get',
        }),
});

export type GetUserAchievements_Params = {
    userId: string;
};
export type GetUserAchievements_Response = UserAchievementsResponse;
export const GetUserAchievements = (params: GetUserAchievements_Params) => ({
    key: ['/achievements/user', params.userId],
    service: async () =>
        ApiService('public').fetchData<GetUserAchievements_Response>({
            url: `/achievements/user/${params.userId}`,
            method: 'get',
        }),
});

// ============================================================================
// Achievement Notification Operations
// ============================================================================

export type GetUnreadAchievements_Response = NewAchievementNotification[];
export const GetUnreadAchievements = () => ({
    key: ['/achievements/my/unread'],
    service: async () =>
        ApiService('protected').fetchData<GetUnreadAchievements_Response>({
            url: '/achievements/my/unread',
            method: 'get',
        }),
});

export type MarkAchievementsRead_Params = {
    achievementTypeIds: number[];
};
export type MarkAchievementsRead_Response = void;
export const MarkAchievementsRead = (params: MarkAchievementsRead_Params) => ({
    key: ['/achievements/my/mark-read'],
    service: async () =>
        ApiService('protected').fetchData<MarkAchievementsRead_Response>({
            url: '/achievements/my/mark-read',
            method: 'post',
            data: params.achievementTypeIds,
        }),
});
