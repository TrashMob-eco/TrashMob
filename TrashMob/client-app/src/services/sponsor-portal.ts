// Sponsor Portal API service (user-facing, read-only)

import { ApiService } from '.';
import SponsorData from '../components/Models/SponsorData';
import SponsoredAdoptionData from '../components/Models/SponsoredAdoptionData';
import ProfessionalCleanupLogData from '../components/Models/ProfessionalCleanupLogData';

// ============================================================================
// My Sponsors (authenticated user)
// ============================================================================

export type GetMySponsors_Response = SponsorData[];
export const GetMySponsors = () => ({
    key: ['/sponsors/mine'],
    service: async () =>
        ApiService('protected').fetchData<GetMySponsors_Response>({
            url: `/sponsors/mine`,
            method: 'get',
        }),
});

// ============================================================================
// Sponsor Adoptions
// ============================================================================

export type GetSponsorAdoptions_Params = { sponsorId: string };
export type GetSponsorAdoptions_Response = SponsoredAdoptionData[];
export const GetSponsorAdoptions = (params: GetSponsorAdoptions_Params) => ({
    key: ['/sponsors/', params.sponsorId, '/adoptions'],
    service: async () =>
        ApiService('protected').fetchData<GetSponsorAdoptions_Response>({
            url: `/sponsors/${params.sponsorId}/adoptions`,
            method: 'get',
        }),
});

// ============================================================================
// Sponsor Cleanup Logs
// ============================================================================

export type GetSponsorCleanupLogs_Params = { sponsorId: string };
export type GetSponsorCleanupLogs_Response = ProfessionalCleanupLogData[];
export const GetSponsorCleanupLogs = (params: GetSponsorCleanupLogs_Params) => ({
    key: ['/sponsors/', params.sponsorId, '/cleanup-logs'],
    service: async () =>
        ApiService('protected').fetchData<GetSponsorCleanupLogs_Response>({
            url: `/sponsors/${params.sponsorId}/cleanup-logs`,
            method: 'get',
        }),
});

// ============================================================================
// Export Cleanup Logs (CSV)
// ============================================================================

export type ExportSponsorCleanupLogs_Params = { sponsorId: string };
export const ExportSponsorCleanupLogs = (params: ExportSponsorCleanupLogs_Params) => ({
    key: ['/sponsors/', params.sponsorId, '/cleanup-logs/export'],
    service: async () =>
        ApiService('protected').fetchData<Blob>({
            url: `/sponsors/${params.sponsorId}/cleanup-logs/export`,
            method: 'get',
            responseType: 'blob',
        }),
});
