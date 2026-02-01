// Communities API service

import { ApiService } from '.';
import CommunityData from '../components/Models/CommunityData';

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
