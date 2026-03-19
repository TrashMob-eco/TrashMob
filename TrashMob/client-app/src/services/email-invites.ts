import { ApiService } from '.';

// ========================================
// Types
// ========================================

export interface EmailInviteBatch {
    id: string;
    senderUserId: string;
    batchType: string;
    communityId?: string;
    teamId?: string;
    totalCount: number;
    sentCount: number;
    deliveredCount: number;
    bouncedCount: number;
    failedCount: number;
    status: string;
    completedDate?: string;
    createdDate: string;
    lastUpdatedDate: string;
    senderUser?: {
        userName: string;
        email: string;
    };
    invites?: EmailInvite[];
}

export interface EmailInvite {
    id: string;
    batchId: string;
    email: string;
    status: string;
    sentDate?: string;
    deliveredDate?: string;
    errorMessage?: string;
    signedUpUserId?: string;
    signedUpDate?: string;
}

export interface CreateEmailInviteBatchRequest {
    emails: string[];
}

// ========================================
// Community Email Invite Endpoints
// ========================================

export type GetCommunityInviteBatches_Params = { communityId: string };
export type GetCommunityInviteBatches_Response = EmailInviteBatch[];
export const GetCommunityInviteBatches = (params: GetCommunityInviteBatches_Params) => ({
    key: ['/communities', params.communityId, 'invites/batches'],
    service: async () =>
        ApiService('protected').fetchData<GetCommunityInviteBatches_Response>({
            url: `/v2/communities/${params.communityId}/invites/batches`,
            method: 'get',
        }),
});

export type GetCommunityInviteBatchDetails_Params = { communityId: string; id: string };
export type GetCommunityInviteBatchDetails_Response = EmailInviteBatch;
export const GetCommunityInviteBatchDetails = (params: GetCommunityInviteBatchDetails_Params) => ({
    key: ['/communities', params.communityId, 'invites/batches', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetCommunityInviteBatchDetails_Response>({
            url: `/v2/communities/${params.communityId}/invites/batches/${params.id}`,
            method: 'get',
        }),
});

export type CreateCommunityInviteBatch_Params = { communityId: string };
export type CreateCommunityInviteBatch_Body = CreateEmailInviteBatchRequest;
export type CreateCommunityInviteBatch_Response = EmailInviteBatch;
export const CreateCommunityInviteBatch = (params: CreateCommunityInviteBatch_Params) => ({
    key: ['/communities', params.communityId, 'invites/batch', 'create'],
    service: async (body: CreateCommunityInviteBatch_Body) =>
        ApiService('protected').fetchData<CreateCommunityInviteBatch_Response, CreateCommunityInviteBatch_Body>({
            url: `/v2/communities/${params.communityId}/invites/batch`,
            method: 'post',
            data: body,
        }),
});

// ========================================
// Team Email Invite Endpoints
// ========================================

export type GetTeamInviteBatches_Params = { teamId: string };
export type GetTeamInviteBatches_Response = EmailInviteBatch[];
export const GetTeamInviteBatches = (params: GetTeamInviteBatches_Params) => ({
    key: ['/teams', params.teamId, 'invites/batches'],
    service: async () =>
        ApiService('protected').fetchData<GetTeamInviteBatches_Response>({
            url: `/v2/teams/${params.teamId}/invites/batches`,
            method: 'get',
        }),
});

export type GetTeamInviteBatchDetails_Params = { teamId: string; id: string };
export type GetTeamInviteBatchDetails_Response = EmailInviteBatch;
export const GetTeamInviteBatchDetails = (params: GetTeamInviteBatchDetails_Params) => ({
    key: ['/teams', params.teamId, 'invites/batches', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetTeamInviteBatchDetails_Response>({
            url: `/v2/teams/${params.teamId}/invites/batches/${params.id}`,
            method: 'get',
        }),
});

export type CreateTeamInviteBatch_Params = { teamId: string };
export type CreateTeamInviteBatch_Body = CreateEmailInviteBatchRequest;
export type CreateTeamInviteBatch_Response = EmailInviteBatch;
export const CreateTeamInviteBatch = (params: CreateTeamInviteBatch_Params) => ({
    key: ['/teams', params.teamId, 'invites/batch', 'create'],
    service: async (body: CreateTeamInviteBatch_Body) =>
        ApiService('protected').fetchData<CreateTeamInviteBatch_Response, CreateTeamInviteBatch_Body>({
            url: `/v2/teams/${params.teamId}/invites/batch`,
            method: 'post',
            data: body,
        }),
});

// ========================================
// User Email Invite Endpoints
// ========================================

export interface UserInviteQuota {
    maxPerBatch: number;
    maxPerMonth: number;
    usedThisMonth: number;
    remainingThisMonth: number;
}

export type GetUserInviteBatches_Response = EmailInviteBatch[];
export const GetUserInviteBatches = () => ({
    key: ['/invites/batches'],
    service: async () =>
        ApiService('protected').fetchData<GetUserInviteBatches_Response>({
            url: '/v2/invites/batches',
            method: 'get',
        }),
});

export type GetUserInviteBatchDetails_Params = { id: string };
export type GetUserInviteBatchDetails_Response = EmailInviteBatch;
export const GetUserInviteBatchDetails = (params: GetUserInviteBatchDetails_Params) => ({
    key: ['/invites/batches', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetUserInviteBatchDetails_Response>({
            url: `/v2/invites/batches/${params.id}`,
            method: 'get',
        }),
});

export type GetUserInviteQuota_Response = UserInviteQuota;
export const GetUserInviteQuota = () => ({
    key: ['/invites/quota'],
    service: async () =>
        ApiService('protected').fetchData<GetUserInviteQuota_Response>({
            url: '/v2/invites/quota',
            method: 'get',
        }),
});

export type CreateUserInviteBatch_Body = CreateEmailInviteBatchRequest;
export type CreateUserInviteBatch_Response = EmailInviteBatch;
export const CreateUserInviteBatch = () => ({
    key: ['/invites/batch', 'create'],
    service: async (body: CreateUserInviteBatch_Body) =>
        ApiService('protected').fetchData<CreateUserInviteBatch_Response, CreateUserInviteBatch_Body>({
            url: '/v2/invites/batch',
            method: 'post',
            data: body,
        }),
});
