// Communities API service

import { ApiService } from '.';
import CommunityData from '../components/Models/CommunityData';
import EventData from '../components/Models/EventData';
import LitterReportData from '../components/Models/LitterReportData';
import StatsData from '../components/Models/StatsData';
import TeamData from '../components/Models/TeamData';

// ============================================================================
// Community Operations
// ============================================================================

export type GetCommunities_Params = {
    latitude?: number;
    longitude?: number;
    radiusMiles?: number;
};
export type GetCommunities_Response = CommunityData[];
export const GetCommunities = (params?: GetCommunities_Params) => ({
    key: ['/communities', params],
    service: async () => {
        const queryParams = new URLSearchParams();
        if (params?.latitude !== undefined) queryParams.append('latitude', params.latitude.toString());
        if (params?.longitude !== undefined) queryParams.append('longitude', params.longitude.toString());
        if (params?.radiusMiles !== undefined) queryParams.append('radiusMiles', params.radiusMiles.toString());
        const queryString = queryParams.toString();
        return ApiService('public').fetchData<GetCommunities_Response>({
            url: `/communities${queryString ? `?${queryString}` : ''}`,
            method: 'get',
        });
    },
});

export type GetCommunityBySlug_Params = { slug: string };
export type GetCommunityBySlug_Response = CommunityData;
export const GetCommunityBySlug = (params: GetCommunityBySlug_Params) => ({
    key: ['/communities/', params.slug],
    service: async () =>
        ApiService('public').fetchData<GetCommunityBySlug_Response>({
            url: `/communities/${params.slug}`,
            method: 'get',
        }),
});

export type CheckCommunitySlug_Params = { slug: string; excludePartnerId?: string };
export type CheckCommunitySlug_Response = boolean;
export const CheckCommunitySlug = (params: CheckCommunitySlug_Params) => ({
    key: ['/communities/check-slug', params],
    service: async () => {
        const queryParams = new URLSearchParams({ slug: params.slug });
        if (params.excludePartnerId) queryParams.append('excludePartnerId', params.excludePartnerId);
        return ApiService('public').fetchData<CheckCommunitySlug_Response>({
            url: `/communities/check-slug?${queryParams.toString()}`,
            method: 'get',
        });
    },
});

// ============================================================================
// Community Content Operations
// ============================================================================

export type GetCommunityEvents_Params = { slug: string; upcomingOnly?: boolean };
export type GetCommunityEvents_Response = EventData[];
export const GetCommunityEvents = (params: GetCommunityEvents_Params) => ({
    key: ['/communities/', params.slug, '/events', params.upcomingOnly],
    service: async () => {
        const queryParams = params.upcomingOnly !== undefined ? `?upcomingOnly=${params.upcomingOnly}` : '';
        return ApiService('public').fetchData<GetCommunityEvents_Response>({
            url: `/communities/${params.slug}/events${queryParams}`,
            method: 'get',
        });
    },
});

export type GetCommunityTeams_Params = { slug: string; radiusMiles?: number };
export type GetCommunityTeams_Response = TeamData[];
export const GetCommunityTeams = (params: GetCommunityTeams_Params) => ({
    key: ['/communities/', params.slug, '/teams', params.radiusMiles],
    service: async () => {
        const queryParams = params.radiusMiles !== undefined ? `?radiusMiles=${params.radiusMiles}` : '';
        return ApiService('public').fetchData<GetCommunityTeams_Response>({
            url: `/communities/${params.slug}/teams${queryParams}`,
            method: 'get',
        });
    },
});

export type GetCommunityLitterReports_Params = { slug: string };
export type GetCommunityLitterReports_Response = LitterReportData[];
export const GetCommunityLitterReports = (params: GetCommunityLitterReports_Params) => ({
    key: ['/communities/', params.slug, '/litterreports'],
    service: async () =>
        ApiService('public').fetchData<GetCommunityLitterReports_Response>({
            url: `/communities/${params.slug}/litterreports`,
            method: 'get',
        }),
});

export type GetCommunityStats_Params = { slug: string };
export type GetCommunityStats_Response = StatsData;
export const GetCommunityStats = (params: GetCommunityStats_Params) => ({
    key: ['/communities/', params.slug, '/stats'],
    service: async () =>
        ApiService('public').fetchData<GetCommunityStats_Response>({
            url: `/communities/${params.slug}/stats`,
            method: 'get',
        }),
});
