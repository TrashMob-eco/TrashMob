// Dependent Invitation API service

import { ApiService } from '.';

// ============================================================================
// Types
// ============================================================================

export interface DependentInvitationData {
    id: string;
    dependentId: string;
    parentUserId: string;
    email: string;
    invitationStatusId: number;
    dateInvited: string;
    expiresDate: string;
    dateAccepted?: string;
    acceptedByUserId?: string;
}

export interface DependentInvitationInfo {
    parentName: string;
    dependentFirstName: string;
    isValid: boolean;
    errorMessage: string;
}

// ============================================================================
// Public Endpoints (no auth)
// ============================================================================

export type VerifyDependentInvitation_Params = { token: string };
export type VerifyDependentInvitation_Response = DependentInvitationInfo;
export const VerifyDependentInvitation = (params: VerifyDependentInvitation_Params) => ({
    key: ['/dependentinvitations/verify', params.token],
    service: async () =>
        ApiService('public').fetchData<VerifyDependentInvitation_Response>({
            url: `/v2/dependentinvitations/verify?token=${encodeURIComponent(params.token)}`,
            method: 'get',
        }),
});

// ============================================================================
// Protected Endpoints
// ============================================================================

export type GetDependentInvitations_Params = { userId: string; dependentId: string };
export type GetDependentInvitations_Response = DependentInvitationData[];
export const GetDependentInvitations = (params: GetDependentInvitations_Params) => ({
    key: ['/dependentinvitations', params.userId, params.dependentId],
    service: async () =>
        ApiService('protected').fetchData<GetDependentInvitations_Response>({
            url: `/v2/dependentinvitations/users/${params.userId}/dependents/${params.dependentId}`,
            method: 'get',
        }),
});

export type CreateDependentInvitation_Params = { userId: string; dependentId: string };
export type CreateDependentInvitation_Body = { email: string };
export type CreateDependentInvitation_Response = DependentInvitationData;
export const CreateDependentInvitation = (params: CreateDependentInvitation_Params) => ({
    key: ['/dependentinvitations', 'create'],
    service: async (body: CreateDependentInvitation_Body) =>
        ApiService('protected').fetchData<CreateDependentInvitation_Response, CreateDependentInvitation_Body>({
            url: `/v2/dependentinvitations/users/${params.userId}/dependents/${params.dependentId}`,
            method: 'post',
            data: body,
        }),
});

export type CancelDependentInvitation_Params = { invitationId: string };
export type CancelDependentInvitation_Response = void;
export const CancelDependentInvitation = (params: CancelDependentInvitation_Params) => ({
    key: ['/dependentinvitations', 'cancel'],
    service: async () =>
        ApiService('protected').fetchData<CancelDependentInvitation_Response>({
            url: `/v2/dependentinvitations/${params.invitationId}/cancel`,
            method: 'post',
        }),
});

export type ResendDependentInvitation_Params = { invitationId: string };
export type ResendDependentInvitation_Response = DependentInvitationData;
export const ResendDependentInvitation = (params: ResendDependentInvitation_Params) => ({
    key: ['/dependentinvitations', 'resend'],
    service: async () =>
        ApiService('protected').fetchData<ResendDependentInvitation_Response>({
            url: `/v2/dependentinvitations/${params.invitationId}/resend`,
            method: 'post',
        }),
});
