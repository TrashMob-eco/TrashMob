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
// Admin Email Invite Endpoints
// ========================================

export type GetAdminInviteBatches_Response = EmailInviteBatch[];
export const GetAdminInviteBatches = () => ({
    key: ['/admin/invites/batches'],
    service: async () =>
        ApiService('protected').fetchData<GetAdminInviteBatches_Response>({
            url: '/admin/invites/batches',
            method: 'get',
        }),
});

export type GetAdminInviteBatchDetails_Params = { id: string };
export type GetAdminInviteBatchDetails_Response = EmailInviteBatch;
export const GetAdminInviteBatchDetails = (params: GetAdminInviteBatchDetails_Params) => ({
    key: ['/admin/invites/batches', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetAdminInviteBatchDetails_Response>({
            url: `/admin/invites/batches/${params.id}`,
            method: 'get',
        }),
});

export type CreateAdminInviteBatch_Body = CreateEmailInviteBatchRequest;
export type CreateAdminInviteBatch_Response = EmailInviteBatch;
export const CreateAdminInviteBatch = () => ({
    key: ['/admin/invites/batch', 'create'],
    service: async (body: CreateAdminInviteBatch_Body) =>
        ApiService('protected').fetchData<CreateAdminInviteBatch_Response, CreateAdminInviteBatch_Body>({
            url: '/admin/invites/batch',
            method: 'post',
            data: body,
        }),
});

// ========================================
// Community Email Invite Endpoints
// ========================================

export type GetCommunityInviteBatches_Params = { communityId: string };
export type GetCommunityInviteBatches_Response = EmailInviteBatch[];
export const GetCommunityInviteBatches = (params: GetCommunityInviteBatches_Params) => ({
    key: ['/communities', params.communityId, 'invites/batches'],
    service: async () =>
        ApiService('protected').fetchData<GetCommunityInviteBatches_Response>({
            url: `/communities/${params.communityId}/invites/batches`,
            method: 'get',
        }),
});

export type GetCommunityInviteBatchDetails_Params = { communityId: string; id: string };
export type GetCommunityInviteBatchDetails_Response = EmailInviteBatch;
export const GetCommunityInviteBatchDetails = (params: GetCommunityInviteBatchDetails_Params) => ({
    key: ['/communities', params.communityId, 'invites/batches', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetCommunityInviteBatchDetails_Response>({
            url: `/communities/${params.communityId}/invites/batches/${params.id}`,
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
            url: `/communities/${params.communityId}/invites/batch`,
            method: 'post',
            data: body,
        }),
});
