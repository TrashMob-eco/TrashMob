// Sponsored Adoptions API service

import { ApiService } from '.';
import SponsoredAdoptionData from '../components/Models/SponsoredAdoptionData';
import ProfessionalCleanupLogData from '../components/Models/ProfessionalCleanupLogData';
import { SponsoredAdoptionComplianceStats } from '../components/Models/SponsoredAdoptionComplianceStats';

// ============================================================================
// Sponsored Adoption Operations
// ============================================================================

export type GetSponsoredAdoptions_Params = { partnerId: string };
export type GetSponsoredAdoptions_Response = SponsoredAdoptionData[];
export const GetSponsoredAdoptions = (params: GetSponsoredAdoptions_Params) => ({
    key: ['/communities/', params.partnerId, '/sponsored-adoptions'],
    service: async () =>
        ApiService('protected').fetchData<GetSponsoredAdoptions_Response>({
            url: `/communities/${params.partnerId}/sponsored-adoptions`,
            method: 'get',
        }),
});

export type GetSponsoredAdoption_Params = { partnerId: string; id: string };
export type GetSponsoredAdoption_Response = SponsoredAdoptionData;
export const GetSponsoredAdoption = (params: GetSponsoredAdoption_Params) => ({
    key: ['/communities/', params.partnerId, '/sponsored-adoptions/', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetSponsoredAdoption_Response>({
            url: `/communities/${params.partnerId}/sponsored-adoptions/${params.id}`,
            method: 'get',
        }),
});

export type CreateSponsoredAdoption_Params = { partnerId: string };
export type CreateSponsoredAdoption_Body = SponsoredAdoptionData;
export type CreateSponsoredAdoption_Response = SponsoredAdoptionData;
export const CreateSponsoredAdoption = () => ({
    key: ['/communities/sponsored-adoptions', 'create'],
    service: async (params: CreateSponsoredAdoption_Params, body: CreateSponsoredAdoption_Body) =>
        ApiService('protected').fetchData<CreateSponsoredAdoption_Response, CreateSponsoredAdoption_Body>({
            url: `/communities/${params.partnerId}/sponsored-adoptions`,
            method: 'post',
            data: body,
        }),
});

export type UpdateSponsoredAdoption_Params = { partnerId: string; id: string };
export type UpdateSponsoredAdoption_Body = SponsoredAdoptionData;
export type UpdateSponsoredAdoption_Response = SponsoredAdoptionData;
export const UpdateSponsoredAdoption = () => ({
    key: ['/communities/sponsored-adoptions', 'update'],
    service: async (params: UpdateSponsoredAdoption_Params, body: UpdateSponsoredAdoption_Body) =>
        ApiService('protected').fetchData<UpdateSponsoredAdoption_Response, UpdateSponsoredAdoption_Body>({
            url: `/communities/${params.partnerId}/sponsored-adoptions/${params.id}`,
            method: 'put',
            data: body,
        }),
});

// ============================================================================
// Compliance Stats
// ============================================================================

export type GetSponsoredAdoptionCompliance_Params = { partnerId: string };
export type GetSponsoredAdoptionCompliance_Response = SponsoredAdoptionComplianceStats;
export const GetSponsoredAdoptionCompliance = (params: GetSponsoredAdoptionCompliance_Params) => ({
    key: ['/communities/', params.partnerId, '/sponsored-adoptions/compliance'],
    service: async () =>
        ApiService('protected').fetchData<GetSponsoredAdoptionCompliance_Response>({
            url: `/communities/${params.partnerId}/sponsored-adoptions/compliance`,
            method: 'get',
        }),
});

// ============================================================================
// Cleanup Log Reports
// ============================================================================

export type GetAdoptionCleanupLogs_Params = { sponsorId: string; adoptionId: string };
export type GetAdoptionCleanupLogs_Response = ProfessionalCleanupLogData[];
export const GetAdoptionCleanupLogs = (params: GetAdoptionCleanupLogs_Params) => ({
    key: ['/sponsors/', params.sponsorId, '/adoptions/', params.adoptionId, '/reports'],
    service: async () =>
        ApiService('protected').fetchData<GetAdoptionCleanupLogs_Response>({
            url: `/sponsors/${params.sponsorId}/adoptions/${params.adoptionId}/reports`,
            method: 'get',
        }),
});
