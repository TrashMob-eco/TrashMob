import { ApiService } from '.';

/**
 * V2 API service factories for role administration (Project 64 Phase 3).
 * All endpoints require the caller to hold the SiteAdmin role.
 */

export interface RoleDto {
    id: number;
    name: string;
    description?: string | null;
    displayOrder?: number | null;
    isActive?: boolean | null;
    memberCount: number;
}

export interface UserRoleGrantDto {
    id: string;
    userId: string;
    roleId: number;
    roleName: string;
    roleDescription?: string | null;
    grantedByUserId: string;
    grantedDate: string;
    expiryDate?: string | null;
}

export interface GrantRoleRequest {
    roleName: string;
    expiryDate?: string | null;
}

export const GetAllRoles = () => ({
    key: ['/roles'],
    service: async () =>
        ApiService('protected').fetchData<RoleDto[]>({
            url: '/v2/roles',
            method: 'get',
        }),
});

export type GetRoleMembers_Params = { roleName: string };
export const GetRoleMembers = (params: GetRoleMembers_Params) => ({
    key: ['/roles', params.roleName, 'members'],
    service: async () =>
        ApiService('protected').fetchData<unknown[]>({
            url: `/v2/roles/${encodeURIComponent(params.roleName)}/members`,
            method: 'get',
        }),
});

export type GetUserRoles_Params = { userId: string };
export const GetUserRoles = (params: GetUserRoles_Params) => ({
    key: ['/users', params.userId, 'roles'],
    service: async () =>
        ApiService('protected').fetchData<UserRoleGrantDto[]>({
            url: `/v2/users/${params.userId}/roles`,
            method: 'get',
        }),
});

export type GrantUserRole_Params = { userId: string };
export const GrantUserRole = (params: GrantUserRole_Params) => ({
    key: ['/users', params.userId, 'roles', 'grant'],
    service: async (body: GrantRoleRequest) =>
        ApiService('protected').fetchData<UserRoleGrantDto, GrantRoleRequest>({
            url: `/v2/users/${params.userId}/roles`,
            method: 'post',
            data: body,
        }),
});

export type RevokeUserRole_Params = { userId: string; roleName: string; reason?: string };
export const RevokeUserRole = () => ({
    key: ['/users', 'roles', 'revoke'],
    service: async (params: RevokeUserRole_Params) => {
        const reasonQuery = params.reason ? `?reason=${encodeURIComponent(params.reason)}` : '';
        return ApiService('protected').fetchData<unknown>({
            url: `/v2/users/${params.userId}/roles/${encodeURIComponent(params.roleName)}${reasonQuery}`,
            method: 'delete',
        });
    },
});
