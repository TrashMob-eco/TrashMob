import { ApiService } from '.';
import {
    WaiverVersionData,
    CommunityWaiverData,
    WaiverVersionRequest,
    AssignWaiverRequest,
    WaiverComplianceSummary,
    UserWaiverFilter,
    UserWaiverListResult,
    UserWaiverData,
} from '../components/Models/WaiverVersionData';
import UserData from '../components/Models/UserData';

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

// ========================================
// Waiver Compliance Dashboard Endpoints
// ========================================

/**
 * Gets waiver compliance summary statistics for the admin dashboard.
 */
export type GetComplianceSummary_Response = WaiverComplianceSummary;
export const GetComplianceSummary = () => ({
    key: ['/admin/waivers/compliance/summary'],
    service: async () =>
        ApiService('protected').fetchData<GetComplianceSummary_Response>({
            url: '/admin/waivers/compliance/summary',
            method: 'get',
        }),
});

/**
 * Gets paginated list of all signed user waivers with filtering options.
 */
export type GetUserWaivers_Body = UserWaiverFilter;
export type GetUserWaivers_Response = UserWaiverListResult;
export const GetUserWaivers = () => ({
    key: ['/admin/waivers/compliance/waivers'],
    service: async (body: GetUserWaivers_Body) =>
        ApiService('protected').fetchData<GetUserWaivers_Response, GetUserWaivers_Body>({
            url: '/admin/waivers/compliance/waivers',
            method: 'post',
            data: body,
        }),
});

/**
 * Gets users with waivers expiring within the specified number of days.
 */
export type GetUsersWithExpiringWaivers_Params = { days?: number };
export type GetUsersWithExpiringWaivers_Response = UserData[];
export const GetUsersWithExpiringWaivers = (params?: GetUsersWithExpiringWaivers_Params) => ({
    key: ['/admin/waivers/compliance/expiring', params?.days ?? 30],
    service: async () =>
        ApiService('protected').fetchData<GetUsersWithExpiringWaivers_Response>({
            url: `/admin/waivers/compliance/expiring${params?.days ? `?days=${params.days}` : ''}`,
            method: 'get',
        }),
});

/**
 * Exports user waivers to CSV format for legal review.
 */
export type ExportWaivers_Body = UserWaiverFilter;
export const ExportWaivers = () => ({
    key: ['/admin/waivers/compliance/export'],
    service: async (body: ExportWaivers_Body) =>
        ApiService('protected').fetchData<Blob, ExportWaivers_Body>({
            url: '/admin/waivers/compliance/export',
            method: 'post',
            data: body,
            responseType: 'blob',
        }),
});

/**
 * Gets a specific user waiver record with full details.
 */
export type GetUserWaiverDetails_Params = { userWaiverId: string };
export type GetUserWaiverDetails_Response = UserWaiverData;
export const GetUserWaiverDetails = (params: GetUserWaiverDetails_Params) => ({
    key: ['/admin/waivers/compliance/waivers', params.userWaiverId],
    service: async () =>
        ApiService('protected').fetchData<GetUserWaiverDetails_Response>({
            url: `/admin/waivers/compliance/waivers/${params.userWaiverId}`,
            method: 'get',
        }),
});
