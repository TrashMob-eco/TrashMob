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
