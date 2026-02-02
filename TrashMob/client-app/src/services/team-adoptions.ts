// Team Adoptions API service

import { ApiService } from '.';
import TeamAdoptionData from '../components/Models/TeamAdoptionData';
import { AdoptionComplianceStats } from '../components/Models/AdoptionComplianceStats';

// ============================================================================
// Team-Centric Operations
// ============================================================================

export type GetTeamAdoptions_Params = { teamId: string };
export type GetTeamAdoptions_Response = TeamAdoptionData[];
export const GetTeamAdoptions = (params: GetTeamAdoptions_Params) => ({
    key: ['/teams/', params.teamId, '/adoptions'],
    service: async () =>
        ApiService('protected').fetchData<GetTeamAdoptions_Response>({
            url: `/teams/${params.teamId}/adoptions`,
            method: 'get',
        }),
});

export type SubmitAdoption_Params = { teamId: string };
export type SubmitAdoption_Body = {
    adoptableAreaId: string;
    applicationNotes?: string;
};
export type SubmitAdoption_Response = TeamAdoptionData;
export const SubmitAdoption = () => ({
    key: ['/teams/adoptions', 'submit'],
    service: async (params: SubmitAdoption_Params, body: SubmitAdoption_Body) =>
        ApiService('protected').fetchData<SubmitAdoption_Response, SubmitAdoption_Body>({
            url: `/teams/${params.teamId}/adoptions`,
            method: 'post',
            data: body,
        }),
});

// ============================================================================
// Community Admin Operations
// ============================================================================

export type GetPendingApplications_Params = { partnerId: string };
export type GetPendingApplications_Response = TeamAdoptionData[];
export const GetPendingApplications = (params: GetPendingApplications_Params) => ({
    key: ['/communities/', params.partnerId, '/adoptions/pending'],
    service: async () =>
        ApiService('protected').fetchData<GetPendingApplications_Response>({
            url: `/communities/${params.partnerId}/adoptions/pending`,
            method: 'get',
        }),
});

export type GetApprovedAdoptions_Params = { partnerId: string };
export type GetApprovedAdoptions_Response = TeamAdoptionData[];
export const GetApprovedAdoptions = (params: GetApprovedAdoptions_Params) => ({
    key: ['/communities/', params.partnerId, '/adoptions/approved'],
    service: async () =>
        ApiService('protected').fetchData<GetApprovedAdoptions_Response>({
            url: `/communities/${params.partnerId}/adoptions/approved`,
            method: 'get',
        }),
});

export type ApproveAdoption_Params = { partnerId: string; adoptionId: string };
export type ApproveAdoption_Response = TeamAdoptionData;
export const ApproveAdoption = () => ({
    key: ['/communities/adoptions', 'approve'],
    service: async (params: ApproveAdoption_Params) =>
        ApiService('protected').fetchData<ApproveAdoption_Response>({
            url: `/communities/${params.partnerId}/adoptions/${params.adoptionId}/approve`,
            method: 'post',
        }),
});

export type RejectAdoption_Params = { partnerId: string; adoptionId: string };
export type RejectAdoption_Body = { rejectionReason: string };
export type RejectAdoption_Response = TeamAdoptionData;
export const RejectAdoption = () => ({
    key: ['/communities/adoptions', 'reject'],
    service: async (params: RejectAdoption_Params, body: RejectAdoption_Body) =>
        ApiService('protected').fetchData<RejectAdoption_Response, RejectAdoption_Body>({
            url: `/communities/${params.partnerId}/adoptions/${params.adoptionId}/reject`,
            method: 'post',
            data: body,
        }),
});

// ============================================================================
// Community Admin - Compliance & Reporting (Phase 4)
// ============================================================================

export type GetDelinquentAdoptions_Params = { partnerId: string };
export type GetDelinquentAdoptions_Response = TeamAdoptionData[];
export const GetDelinquentAdoptions = (params: GetDelinquentAdoptions_Params) => ({
    key: ['/communities/', params.partnerId, '/adoptions/delinquent'],
    service: async () =>
        ApiService('protected').fetchData<GetDelinquentAdoptions_Response>({
            url: `/communities/${params.partnerId}/adoptions/delinquent`,
            method: 'get',
        }),
});

export type GetComplianceStats_Params = { partnerId: string };
export type GetComplianceStats_Response = AdoptionComplianceStats;
export const GetComplianceStats = (params: GetComplianceStats_Params) => ({
    key: ['/communities/', params.partnerId, '/adoptions/stats'],
    service: async () =>
        ApiService('protected').fetchData<GetComplianceStats_Response>({
            url: `/communities/${params.partnerId}/adoptions/stats`,
            method: 'get',
        }),
});

export type ExportAdoptions_Params = { partnerId: string };
export const ExportAdoptions = (params: ExportAdoptions_Params) => ({
    key: ['/communities/', params.partnerId, '/adoptions/export'],
    service: async () =>
        ApiService('protected').fetchData<Blob>({
            url: `/communities/${params.partnerId}/adoptions/export`,
            method: 'get',
            responseType: 'blob',
        }),
});
