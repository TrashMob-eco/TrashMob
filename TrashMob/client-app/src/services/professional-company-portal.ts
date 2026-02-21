// Professional Company Portal API service (user-facing)

import { ApiService } from '.';
import ProfessionalCompanyData from '../components/Models/ProfessionalCompanyData';
import ProfessionalCleanupLogData from '../components/Models/ProfessionalCleanupLogData';
import SponsoredAdoptionData from '../components/Models/SponsoredAdoptionData';

// ============================================================================
// My Companies (authenticated user)
// ============================================================================

export type GetMyCompanies_Response = ProfessionalCompanyData[];
export const GetMyCompanies = () => ({
    key: ['/professional-companies/mine'],
    service: async () =>
        ApiService('protected').fetchData<GetMyCompanies_Response>({
            url: `/professional-companies/mine`,
            method: 'get',
        }),
});

// ============================================================================
// Company Assignments
// ============================================================================

export type GetCompanyAssignments_Params = { companyId: string };
export type GetCompanyAssignments_Response = SponsoredAdoptionData[];
export const GetCompanyAssignments = (params: GetCompanyAssignments_Params) => ({
    key: ['/professional-companies/', params.companyId, '/assignments'],
    service: async () =>
        ApiService('protected').fetchData<GetCompanyAssignments_Response>({
            url: `/professional-companies/${params.companyId}/cleanup-logs/assignments`,
            method: 'get',
        }),
});

// ============================================================================
// Company Cleanup Logs
// ============================================================================

export type GetCompanyCleanupLogs_Params = { companyId: string };
export type GetCompanyCleanupLogs_Response = ProfessionalCleanupLogData[];
export const GetCompanyCleanupLogs = (params: GetCompanyCleanupLogs_Params) => ({
    key: ['/professional-companies/', params.companyId, '/cleanup-logs'],
    service: async () =>
        ApiService('protected').fetchData<GetCompanyCleanupLogs_Response>({
            url: `/professional-companies/${params.companyId}/cleanup-logs`,
            method: 'get',
        }),
});

export type LogCleanup_Params = { companyId: string };
export type LogCleanup_Body = ProfessionalCleanupLogData;
export type LogCleanup_Response = ProfessionalCleanupLogData;
export const LogCleanup = () => ({
    key: ['/professional-companies/cleanup-logs', 'create'],
    service: async (params: LogCleanup_Params, body: LogCleanup_Body) =>
        ApiService('protected').fetchData<LogCleanup_Response, LogCleanup_Body>({
            url: `/professional-companies/${params.companyId}/cleanup-logs`,
            method: 'post',
            data: body,
        }),
});
