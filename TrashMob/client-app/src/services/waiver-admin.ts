import { ApiService } from '.';
import {
    WaiverVersionData,
    CommunityWaiverData,
    WaiverVersionRequest,
    AssignWaiverRequest,
} from '../components/Models/WaiverVersionData';

// ========================================
// Waiver Version Admin Endpoints
// ========================================

export type GetAllWaiverVersions_Response = WaiverVersionData[];
export const GetAllWaiverVersions = () => ({
    key: ['/admin/waivers'],
    service: async () =>
        ApiService('protected').fetchData<GetAllWaiverVersions_Response>({
            url: '/admin/waivers',
            method: 'get',
        }),
});

export type GetActiveWaiverVersions_Response = WaiverVersionData[];
export const GetActiveWaiverVersions = () => ({
    key: ['/admin/waivers/active'],
    service: async () =>
        ApiService('protected').fetchData<GetActiveWaiverVersions_Response>({
            url: '/admin/waivers/active',
            method: 'get',
        }),
});

export type GetWaiverVersionById_Params = { id: string };
export type GetWaiverVersionById_Response = WaiverVersionData;
export const GetWaiverVersionById = (params: GetWaiverVersionById_Params) => ({
    key: ['/admin/waivers', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetWaiverVersionById_Response>({
            url: `/admin/waivers/${params.id}`,
            method: 'get',
        }),
});

export type CreateWaiverVersion_Body = WaiverVersionRequest;
export type CreateWaiverVersion_Response = WaiverVersionData;
export const CreateWaiverVersion = () => ({
    key: ['/admin/waivers', 'create'],
    service: async (body: CreateWaiverVersion_Body) =>
        ApiService('protected').fetchData<CreateWaiverVersion_Response, CreateWaiverVersion_Body>({
            url: '/admin/waivers',
            method: 'post',
            data: body,
        }),
});

export type UpdateWaiverVersion_Params = { id: string };
export type UpdateWaiverVersion_Body = WaiverVersionData;
export type UpdateWaiverVersion_Response = WaiverVersionData;
export const UpdateWaiverVersion = () => ({
    key: ['/admin/waivers', 'update'],
    service: async (params: UpdateWaiverVersion_Params & { body: UpdateWaiverVersion_Body }) =>
        ApiService('protected').fetchData<UpdateWaiverVersion_Response, UpdateWaiverVersion_Body>({
            url: `/admin/waivers/${params.id}`,
            method: 'put',
            data: params.body,
        }),
});

export type DeactivateWaiverVersion_Params = { id: string };
export type DeactivateWaiverVersion_Response = void;
export const DeactivateWaiverVersion = () => ({
    key: ['/admin/waivers', 'deactivate'],
    service: async (params: DeactivateWaiverVersion_Params) =>
        ApiService('protected').fetchData<DeactivateWaiverVersion_Response>({
            url: `/admin/waivers/${params.id}`,
            method: 'delete',
        }),
});

// ========================================
// Community Waiver Assignment Endpoints
// ========================================

export type GetCommunityWaivers_Params = { communityId: string };
export type GetCommunityWaivers_Response = CommunityWaiverData[];
export const GetCommunityWaivers = (params: GetCommunityWaivers_Params) => ({
    key: ['/admin/communities', params.communityId, 'waivers'],
    service: async () =>
        ApiService('protected').fetchData<GetCommunityWaivers_Response>({
            url: `/admin/communities/${params.communityId}/waivers`,
            method: 'get',
        }),
});

export type AssignWaiverToCommunity_Params = { communityId: string };
export type AssignWaiverToCommunity_Body = AssignWaiverRequest;
export type AssignWaiverToCommunity_Response = CommunityWaiverData;
export const AssignWaiverToCommunity = () => ({
    key: ['/admin/communities/waivers', 'assign'],
    service: async (params: AssignWaiverToCommunity_Params & { body: AssignWaiverToCommunity_Body }) =>
        ApiService('protected').fetchData<AssignWaiverToCommunity_Response, AssignWaiverToCommunity_Body>({
            url: `/admin/communities/${params.communityId}/waivers`,
            method: 'post',
            data: params.body,
        }),
});

export type RemoveWaiverFromCommunity_Params = { communityId: string; waiverId: string };
export type RemoveWaiverFromCommunity_Response = void;
export const RemoveWaiverFromCommunity = () => ({
    key: ['/admin/communities/waivers', 'remove'],
    service: async (params: RemoveWaiverFromCommunity_Params) =>
        ApiService('protected').fetchData<RemoveWaiverFromCommunity_Response>({
            url: `/admin/communities/${params.communityId}/waivers/${params.waiverId}`,
            method: 'delete',
        }),
});
