import { ApiService } from '.';
import CommunityProspectData from '../components/Models/CommunityProspectData';
import CsvImportResultData from '../components/Models/CsvImportResultData';
import DiscoveryResultData from '../components/Models/DiscoveryResultData';
import FitScoreBreakdownData from '../components/Models/FitScoreBreakdownData';
import GeographicGapData from '../components/Models/GeographicGapData';
import ProspectActivityData from '../components/Models/ProspectActivityData';

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
                url: `/communityprospects${query}`,
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
            url: `/communityprospects/${params.id}`,
            method: 'get',
        }),
});

export type CreateCommunityProspect_Body = CommunityProspectData;
export type CreateCommunityProspect_Response = CommunityProspectData;
export const CreateCommunityProspect = () => ({
    key: ['/communityprospects', 'create'],
    service: async (body: CreateCommunityProspect_Body) =>
        ApiService('protected').fetchData<CreateCommunityProspect_Response, CreateCommunityProspect_Body>({
            url: '/communityprospects',
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
            url: '/communityprospects',
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
            url: `/communityprospects/${params.id}`,
            method: 'delete',
        }),
});

export type UpdateProspectPipelineStage_Params = { id: string; stage: number };
export type UpdateProspectPipelineStage_Response = CommunityProspectData;
export const UpdateProspectPipelineStage = () => ({
    key: ['/communityprospects', 'updateStage'],
    service: async (params: UpdateProspectPipelineStage_Params) =>
        ApiService('protected').fetchData<UpdateProspectPipelineStage_Response>({
            url: `/communityprospects/${params.id}/stage`,
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
            url: `/communityprospects/${params.id}/activities`,
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
            url: `/communityprospects/${params.id}/activities`,
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
            url: '/communityprospects/discover',
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
            url: `/communityprospects/${params.id}/score`,
            method: 'get',
        }),
});

export type RescoreAllProspects_Response = number;
export const RescoreAllProspects = () => ({
    key: ['/communityprospects', 'rescore'],
    service: async () =>
        ApiService('protected').fetchData<RescoreAllProspects_Response>({
            url: '/communityprospects/rescore',
            method: 'post',
        }),
});

export type GetGeographicGaps_Response = GeographicGapData[];
export const GetGeographicGaps = () => ({
    key: ['/communityprospects', 'gaps'],
    service: async () =>
        ApiService('protected').fetchData<GetGeographicGaps_Response>({
            url: '/communityprospects/gaps',
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
            url: '/communityprospects/import',
            method: 'post',
            data: formData,
        });
    },
});
