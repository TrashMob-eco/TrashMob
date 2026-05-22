import { ApiService } from '.';
import BatchOutreachResultData from '../components/Models/BatchOutreachResultData';
import { OutreachSendResultData } from '../components/Models/BatchOutreachResultData';
import CommunityProspectData from '../components/Models/CommunityProspectData';
import CsvImportResultData from '../components/Models/CsvImportResultData';
import DiscoveryResultData from '../components/Models/DiscoveryResultData';
import FitScoreBreakdownData from '../components/Models/FitScoreBreakdownData';
import GeographicGapData from '../components/Models/GeographicGapData';
import OutreachPreviewData from '../components/Models/OutreachPreviewData';
import OutreachSettingsData from '../components/Models/OutreachSettingsData';
import PipelineAnalyticsData from '../components/Models/PipelineAnalyticsData';
import ProspectConversionResultData from '../components/Models/ProspectConversionResultData';
import ProspectActivityData from '../components/Models/ProspectActivityData';
import ProspectContactData from '../components/Models/ProspectContactData';
import ProspectOutreachEmailData from '../components/Models/ProspectOutreachEmailData';

export type GetCommunityProspects_Params = { stage?: number; search?: string };
export type GetCommunityProspects_Response = CommunityProspectData[];
export const GetCommunityProspects = (params?: GetCommunityProspects_Params) => {
    const queryParts: string[] = [];
    if (params?.stage !== undefined) queryParts.push(`stage=${params.stage}`);
    if (params?.search) queryParts.push(`search=${encodeURIComponent(params.search)}`);
    const query = queryParts.length > 0 ? `?${queryParts.join('&')}` : '';

    return {
        key: ['/communityprospects', params?.stage, params?.search],
        service: async () =>
            ApiService('protected').fetchData<GetCommunityProspects_Response>({
                url: `/v2/community-prospects${query}`,
                method: 'get',
            }),
    };
};

export type GetCommunityProspectById_Params = { id: string };
export type GetCommunityProspectById_Response = CommunityProspectData;
export const GetCommunityProspectById = (params: GetCommunityProspectById_Params) => ({
    key: ['/communityprospects', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetCommunityProspectById_Response>({
            url: `/v2/community-prospects/${params.id}`,
            method: 'get',
        }),
});

export type CreateCommunityProspect_Body = CommunityProspectData;
export type CreateCommunityProspect_Response = CommunityProspectData;
export const CreateCommunityProspect = () => ({
    key: ['/communityprospects', 'create'],
    service: async (body: CreateCommunityProspect_Body) =>
        ApiService('protected').fetchData<CreateCommunityProspect_Response, CreateCommunityProspect_Body>({
            url: '/v2/community-prospects',
            method: 'post',
            data: body,
        }),
});

export type UpdateCommunityProspect_Body = CommunityProspectData;
export type UpdateCommunityProspect_Response = CommunityProspectData;
export const UpdateCommunityProspect = () => ({
    key: ['/communityprospects', 'update'],
    service: async (body: UpdateCommunityProspect_Body) =>
        ApiService('protected').fetchData<UpdateCommunityProspect_Response, UpdateCommunityProspect_Body>({
            url: '/v2/community-prospects',
            method: 'put',
            data: body,
        }),
});

export type DeleteCommunityProspect_Params = { id: string };
export type DeleteCommunityProspect_Response = unknown;
export const DeleteCommunityProspect = () => ({
    key: ['/communityprospects', 'delete'],
    service: async (params: DeleteCommunityProspect_Params) =>
        ApiService('protected').fetchData<DeleteCommunityProspect_Response>({
            url: `/v2/community-prospects/${params.id}`,
            method: 'delete',
        }),
});

export type UpdateProspectPipelineStage_Params = { id: string; stage: number };
export type UpdateProspectPipelineStage_Response = CommunityProspectData;
export const UpdateProspectPipelineStage = () => ({
    key: ['/communityprospects', 'updateStage'],
    service: async (params: UpdateProspectPipelineStage_Params) =>
        ApiService('protected').fetchData<UpdateProspectPipelineStage_Response>({
            url: `/v2/community-prospects/${params.id}/stage`,
            method: 'put',
            data: { stage: params.stage },
        }),
});

export type GetProspectActivities_Params = { id: string };
export type GetProspectActivities_Response = ProspectActivityData[];
export const GetProspectActivities = (params: GetProspectActivities_Params) => ({
    key: ['/communityprospects', params.id, 'activities'],
    service: async () =>
        ApiService('protected').fetchData<GetProspectActivities_Response>({
            url: `/v2/community-prospects/${params.id}/activities`,
            method: 'get',
        }),
});

export type CreateProspectActivity_Params = { id: string };
export type CreateProspectActivity_Body = ProspectActivityData;
export type CreateProspectActivity_Response = ProspectActivityData;
export const CreateProspectActivity = (params: CreateProspectActivity_Params) => ({
    key: ['/communityprospects', params.id, 'activities', 'create'],
    service: async (body: CreateProspectActivity_Body) =>
        ApiService('protected').fetchData<CreateProspectActivity_Response, CreateProspectActivity_Body>({
            url: `/v2/community-prospects/${params.id}/activities`,
            method: 'post',
            data: body,
        }),
});

// --- Phase 2: AI Discovery, Scoring, and Import ---

export type DiscoverProspects_Body = {
    prompt?: string;
    city?: string;
    region?: string;
    country?: string;
    maxResults: number;
};
export type DiscoverProspects_Response = DiscoveryResultData;
export const DiscoverProspects = () => ({
    key: ['/communityprospects', 'discover'],
    service: async (body: DiscoverProspects_Body) =>
        ApiService('protected').fetchData<DiscoverProspects_Response, DiscoverProspects_Body>({
            url: '/v2/community-prospects/discover',
            method: 'post',
            data: body,
        }),
});

export type GetProspectScoreBreakdown_Params = { id: string };
export type GetProspectScoreBreakdown_Response = FitScoreBreakdownData;
export const GetProspectScoreBreakdown = (params: GetProspectScoreBreakdown_Params) => ({
    key: ['/communityprospects', params.id, 'score'],
    service: async () =>
        ApiService('protected').fetchData<GetProspectScoreBreakdown_Response>({
            url: `/v2/community-prospects/${params.id}/score`,
            method: 'get',
        }),
});

export type RescoreAllProspects_Response = number;
export const RescoreAllProspects = () => ({
    key: ['/communityprospects', 'rescore'],
    service: async () =>
        ApiService('protected').fetchData<RescoreAllProspects_Response>({
            url: '/v2/community-prospects/rescore',
            method: 'post',
        }),
});

export type GetGeographicGaps_Response = GeographicGapData[];
export const GetGeographicGaps = () => ({
    key: ['/communityprospects', 'gaps'],
    service: async () =>
        ApiService('protected').fetchData<GetGeographicGaps_Response>({
            url: '/v2/community-prospects/gaps',
            method: 'get',
        }),
});

export type ImportProspectsCsv_Response = CsvImportResultData;
export const ImportProspectsCsv = () => ({
    key: ['/communityprospects', 'import'],
    service: async (file: File) => {
        const formData = new FormData();
        formData.append('file', file);
        return ApiService('protected').fetchData<ImportProspectsCsv_Response>({
            url: '/v2/community-prospects/import',
            method: 'post',
            data: formData,
        });
    },
});

// --- Phase 3: Outreach & Communication ---

export type PreviewOutreach_Params = { id: string };
export type PreviewOutreach_Response = OutreachPreviewData;
export const PreviewOutreach = (params: PreviewOutreach_Params) => ({
    key: ['/communityprospects', params.id, 'outreach', 'preview'],
    service: async () =>
        ApiService('protected').fetchData<PreviewOutreach_Response>({
            url: `/v2/community-prospects/${params.id}/outreach/preview`,
            method: 'post',
        }),
});

export type SendOutreach_Params = {
    id: string;
    subject?: string;
    htmlBody?: string;
    prospectContactId?: string;
};
export type SendOutreach_Response = OutreachSendResultData;
export const SendOutreach = () => ({
    key: ['/communityprospects', 'outreach', 'send'],
    service: async (params: SendOutreach_Params) => {
        const hasBody = params.subject || params.htmlBody || params.prospectContactId;
        return ApiService('protected').fetchData<SendOutreach_Response>({
            url: `/v2/community-prospects/${params.id}/outreach`,
            method: 'post',
            data: hasBody
                ? {
                      subject: params.subject,
                      htmlBody: params.htmlBody,
                      prospectContactId: params.prospectContactId,
                  }
                : undefined,
        });
    },
});

export type GetOutreachHistory_Params = { id: string };
export type GetOutreachHistory_Response = ProspectOutreachEmailData[];
export const GetOutreachHistory = (params: GetOutreachHistory_Params) => ({
    key: ['/communityprospects', params.id, 'outreach', 'history'],
    service: async () =>
        ApiService('protected').fetchData<GetOutreachHistory_Response>({
            url: `/v2/community-prospects/${params.id}/outreach/history`,
            method: 'get',
        }),
});

export type SendBatchOutreach_Body = { prospectIds: string[] };
export type SendBatchOutreach_Response = BatchOutreachResultData;
export const SendBatchOutreach = () => ({
    key: ['/communityprospects', 'outreach', 'batch'],
    service: async (body: SendBatchOutreach_Body) =>
        ApiService('protected').fetchData<SendBatchOutreach_Response, SendBatchOutreach_Body>({
            url: '/v2/community-prospects/outreach/batch',
            method: 'post',
            data: body,
        }),
});

export type GetOutreachSettings_Response = OutreachSettingsData;
export const GetOutreachSettings = () => ({
    key: ['/communityprospects', 'outreach', 'settings'],
    service: async () =>
        ApiService('protected').fetchData<GetOutreachSettings_Response>({
            url: '/v2/community-prospects/outreach/settings',
            method: 'get',
        }),
});

// --- Phase 4: Analytics & Conversion ---

export type GetPipelineAnalytics_Response = PipelineAnalyticsData;
export const GetPipelineAnalytics = () => ({
    key: ['/communityprospects', 'analytics'],
    service: async () =>
        ApiService('protected').fetchData<GetPipelineAnalytics_Response>({
            url: '/v2/community-prospects/analytics',
            method: 'get',
        }),
});

export type ConvertProspectToPartner_Params = {
    id: string;
    partnerTypeId?: number;
    sendWelcomeEmail?: boolean;
};
export type ConvertProspectToPartner_Response = ProspectConversionResultData;
export const ConvertProspectToPartner = () => ({
    key: ['/communityprospects', 'convert'],
    service: async (params: ConvertProspectToPartner_Params) =>
        ApiService('protected').fetchData<ConvertProspectToPartner_Response>({
            url: `/v2/community-prospects/${params.id}/convert`,
            method: 'post',
            data: {
                prospectId: params.id,
                partnerTypeId: params.partnerTypeId ?? 1,
                sendWelcomeEmail: params.sendWelcomeEmail ?? true,
            },
        }),
});

// --- Project 60: Prospect Contacts ---

export type GetProspectContacts_Params = { prospectId: string };
export type GetProspectContacts_Response = ProspectContactData[];
export const GetProspectContacts = (params: GetProspectContacts_Params) => ({
    key: ['/communityprospects', params.prospectId, 'contacts'],
    service: async () =>
        ApiService('protected').fetchData<GetProspectContacts_Response>({
            url: `/v2/community-prospects/${params.prospectId}/contacts`,
            method: 'get',
        }),
});

export type CreateProspectContact_Params = { prospectId: string };
export type CreateProspectContact_Body = ProspectContactData;
export type CreateProspectContact_Response = ProspectContactData;
export const CreateProspectContact = (params: CreateProspectContact_Params) => ({
    key: ['/communityprospects', params.prospectId, 'contacts', 'create'],
    service: async (body: CreateProspectContact_Body) =>
        ApiService('protected').fetchData<CreateProspectContact_Response, CreateProspectContact_Body>({
            url: `/v2/community-prospects/${params.prospectId}/contacts`,
            method: 'post',
            data: body,
        }),
});

export type UpdateProspectContact_Params = { prospectId: string; contactId: string };
export type UpdateProspectContact_Body = ProspectContactData;
export type UpdateProspectContact_Response = ProspectContactData;
export const UpdateProspectContact = (params: UpdateProspectContact_Params) => ({
    key: ['/communityprospects', params.prospectId, 'contacts', params.contactId, 'update'],
    service: async (body: UpdateProspectContact_Body) =>
        ApiService('protected').fetchData<UpdateProspectContact_Response, UpdateProspectContact_Body>({
            url: `/v2/community-prospects/${params.prospectId}/contacts/${params.contactId}`,
            method: 'put',
            data: body,
        }),
});

export type DeleteProspectContact_Params = { prospectId: string; contactId: string };
export const DeleteProspectContact = () => ({
    key: ['/communityprospects', 'contacts', 'delete'],
    service: async (params: DeleteProspectContact_Params) =>
        ApiService('protected').fetchData<unknown>({
            url: `/v2/community-prospects/${params.prospectId}/contacts/${params.contactId}`,
            method: 'delete',
        }),
});

export type SetPrimaryProspectContact_Params = { prospectId: string; contactId: string };
export type SetPrimaryProspectContact_Response = ProspectContactData;
export const SetPrimaryProspectContact = () => ({
    key: ['/communityprospects', 'contacts', 'set-primary'],
    service: async (params: SetPrimaryProspectContact_Params) =>
        ApiService('protected').fetchData<SetPrimaryProspectContact_Response>({
            url: `/v2/community-prospects/${params.prospectId}/contacts/${params.contactId}/set-primary`,
            method: 'post',
        }),
});

export type UpdateProspectContactStatus_Params = {
    prospectId: string;
    contactId: string;
    status: number;
};
export type UpdateProspectContactStatus_Response = ProspectContactData;
export const UpdateProspectContactStatus = () => ({
    key: ['/communityprospects', 'contacts', 'status'],
    service: async (params: UpdateProspectContactStatus_Params) =>
        ApiService('protected').fetchData<UpdateProspectContactStatus_Response>({
            url: `/v2/community-prospects/${params.prospectId}/contacts/${params.contactId}/status`,
            method: 'put',
            data: { status: params.status },
        }),
});
