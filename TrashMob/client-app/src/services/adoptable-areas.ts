// Adoptable Areas API service

import { ApiService } from '.';
import AdoptableAreaData from '../components/Models/AdoptableAreaData';
import AreaGenerationBatchData from '../components/Models/AreaGenerationBatchData';
import StagedAdoptableAreaData from '../components/Models/StagedAdoptableAreaData';

// ============================================================================
// Adoptable Area Operations
// ============================================================================

export type GetAdoptableAreas_Params = { partnerId: string };
export type GetAdoptableAreas_Response = AdoptableAreaData[];
export const GetAdoptableAreas = (params: GetAdoptableAreas_Params) => ({
    key: ['/communities/', params.partnerId, '/areas'],
    service: async () =>
        ApiService('public').fetchData<GetAdoptableAreas_Response>({
            url: `/communities/${params.partnerId}/areas`,
            method: 'get',
        }),
});

export type GetAvailableAreas_Params = { partnerId: string };
export type GetAvailableAreas_Response = AdoptableAreaData[];
export const GetAvailableAreas = (params: GetAvailableAreas_Params) => ({
    key: ['/communities/', params.partnerId, '/areas/available'],
    service: async () =>
        ApiService('public').fetchData<GetAvailableAreas_Response>({
            url: `/communities/${params.partnerId}/areas/available`,
            method: 'get',
        }),
});

export type GetAdoptableArea_Params = { partnerId: string; areaId: string };
export type GetAdoptableArea_Response = AdoptableAreaData;
export const GetAdoptableArea = (params: GetAdoptableArea_Params) => ({
    key: ['/communities/', params.partnerId, '/areas/', params.areaId],
    service: async () =>
        ApiService('public').fetchData<GetAdoptableArea_Response>({
            url: `/communities/${params.partnerId}/areas/${params.areaId}`,
            method: 'get',
        }),
});

export type CheckAreaName_Params = { partnerId: string; name: string; excludeAreaId?: string };
export type CheckAreaName_Response = boolean;
export const CheckAreaName = (params: CheckAreaName_Params) => ({
    key: ['/communities/', params.partnerId, '/areas/check-name', params],
    service: async () => {
        const queryParams = new URLSearchParams({ name: params.name });
        if (params.excludeAreaId) queryParams.append('excludeAreaId', params.excludeAreaId);
        return ApiService('public').fetchData<CheckAreaName_Response>({
            url: `/communities/${params.partnerId}/areas/check-name?${queryParams.toString()}`,
            method: 'get',
        });
    },
});

export type CreateAdoptableArea_Params = { partnerId: string };
export type CreateAdoptableArea_Body = AdoptableAreaData;
export type CreateAdoptableArea_Response = AdoptableAreaData;
export const CreateAdoptableArea = () => ({
    key: ['/communities/areas', 'create'],
    service: async (params: CreateAdoptableArea_Params, body: CreateAdoptableArea_Body) =>
        ApiService('protected').fetchData<CreateAdoptableArea_Response, CreateAdoptableArea_Body>({
            url: `/communities/${params.partnerId}/areas`,
            method: 'post',
            data: body,
        }),
});

export type UpdateAdoptableArea_Params = { partnerId: string; areaId: string };
export type UpdateAdoptableArea_Body = AdoptableAreaData;
export type UpdateAdoptableArea_Response = AdoptableAreaData;
export const UpdateAdoptableArea = () => ({
    key: ['/communities/areas', 'update'],
    service: async (params: UpdateAdoptableArea_Params, body: UpdateAdoptableArea_Body) =>
        ApiService('protected').fetchData<UpdateAdoptableArea_Response, UpdateAdoptableArea_Body>({
            url: `/communities/${params.partnerId}/areas/${params.areaId}`,
            method: 'put',
            data: body,
        }),
});

export type DeleteAdoptableArea_Params = { partnerId: string; areaId: string };
export type DeleteAdoptableArea_Response = void;
export const DeleteAdoptableArea = () => ({
    key: ['/communities/areas', 'delete'],
    service: async (params: DeleteAdoptableArea_Params) =>
        ApiService('protected').fetchData<DeleteAdoptableArea_Response>({
            url: `/communities/${params.partnerId}/areas/${params.areaId}`,
            method: 'delete',
        }),
});

export type ClearAllAreas_Params = { partnerId: string };
export type ClearAllAreas_Response = { areasDeactivated: number; stagedAreasDeleted: number; batchesDeleted: number };
export const ClearAllAreas = () => ({
    key: ['/communities/areas', 'clear-all'],
    service: async (params: ClearAllAreas_Params) =>
        ApiService('protected').fetchData<ClearAllAreas_Response>({
            url: `/communities/${params.partnerId}/areas/clear-all`,
            method: 'delete',
        }),
});

// ============================================================================
// Export
// ============================================================================

export type ExportAreas_Params = { partnerId: string; format: 'geojson' | 'kml' };
export const ExportAreas = (params: ExportAreas_Params) => ({
    key: ['/communities/', params.partnerId, '/areas/export', params.format],
    service: async () =>
        ApiService('protected').fetchData<Blob>({
            url: `/communities/${params.partnerId}/areas/export?format=${params.format}`,
            method: 'get',
            responseType: 'blob',
        }),
});

// ============================================================================
// AI Area Suggestion
// ============================================================================

export type SuggestArea_Params = { partnerId: string };
export type SuggestArea_Body = {
    description: string;
    centerLatitude?: number;
    centerLongitude?: number;
    communityName?: string;
    boundsNorth?: number;
    boundsSouth?: number;
    boundsEast?: number;
    boundsWest?: number;
};
export type SuggestArea_Response = {
    geoJson: string | null;
    suggestedName: string | null;
    suggestedAreaType: string | null;
    confidence: number;
    message: string | null;
};
export const SuggestArea = () => ({
    key: ['/communities/areas', 'suggest'],
    service: async (params: SuggestArea_Params, body: SuggestArea_Body) =>
        ApiService('protected').fetchData<SuggestArea_Response>({
            url: `/communities/${params.partnerId}/areas/suggest`,
            method: 'post',
            data: body,
        }),
});

// ============================================================================
// Bulk Import
// ============================================================================

export type ParseImportFile_Params = { partnerId: string };
export type ParseImportFile_Response = {
    features: Array<{
        geoJson: string;
        geometryType: string;
        properties: Record<string, string>;
        isValid: boolean;
        validationErrors: string[];
    }>;
    propertyKeys: string[];
    totalFeatures: number;
    validFeatures: number;
    warnings: string[];
    error: string | null;
};
export const ParseImportFile = () => ({
    key: ['/communities/areas', 'import-parse'],
    service: async (params: ParseImportFile_Params, formData: FormData) =>
        ApiService('protected').fetchData<ParseImportFile_Response>({
            url: `/communities/${params.partnerId}/areas/import/parse`,
            method: 'post',
            data: formData,
            headers: { 'Content-Type': 'multipart/form-data' },
        }),
});

export type BulkImportAreas_Params = { partnerId: string };
export type BulkImportAreas_Body = AdoptableAreaData[];
export type BulkImportAreas_Response = {
    createdCount: number;
    skippedDuplicateCount: number;
    errorCount: number;
    errors: Array<{ featureIndex: number; featureName: string; message: string }>;
    totalProcessed: number;
};
export const BulkImportAreas = () => ({
    key: ['/communities/areas', 'bulk-import'],
    service: async (params: BulkImportAreas_Params, body: BulkImportAreas_Body) =>
        ApiService('protected').fetchData<BulkImportAreas_Response>({
            url: `/communities/${params.partnerId}/areas/import`,
            method: 'post',
            data: body,
        }),
});

// ============================================================================
// AI Area Generation
// ============================================================================

export type StartAreaGeneration_Params = { partnerId: string };
export type StartAreaGeneration_Body = {
    category: string;
    boundsNorth?: number;
    boundsSouth?: number;
    boundsEast?: number;
    boundsWest?: number;
};
export type StartAreaGeneration_Response = AreaGenerationBatchData;
export const StartAreaGeneration = () => ({
    key: ['/communities/areas', 'generate'],
    service: async (params: StartAreaGeneration_Params, body: StartAreaGeneration_Body) =>
        ApiService('protected').fetchData<StartAreaGeneration_Response, StartAreaGeneration_Body>({
            url: `/communities/${params.partnerId}/areas/generate`,
            method: 'post',
            data: body,
        }),
});

export type GetGenerationStatus_Params = { partnerId: string };
export type GetGenerationStatus_Response = AreaGenerationBatchData;
export const GetGenerationStatus = (params: GetGenerationStatus_Params) => ({
    key: ['/communities/', params.partnerId, '/areas/generate/status'],
    service: async () =>
        ApiService('public').fetchData<GetGenerationStatus_Response>({
            url: `/communities/${params.partnerId}/areas/generate/status`,
            method: 'get',
        }),
});

export type GetGenerationBatches_Params = { partnerId: string };
export type GetGenerationBatches_Response = AreaGenerationBatchData[];
export const GetGenerationBatches = (params: GetGenerationBatches_Params) => ({
    key: ['/communities/', params.partnerId, '/areas/generate/batches'],
    service: async () =>
        ApiService('public').fetchData<GetGenerationBatches_Response>({
            url: `/communities/${params.partnerId}/areas/generate/batches`,
            method: 'get',
        }),
});

export type GetGenerationBatch_Params = { partnerId: string; batchId: string };
export type GetGenerationBatch_Response = AreaGenerationBatchData;
export const GetGenerationBatch = (params: GetGenerationBatch_Params) => ({
    key: ['/communities/', params.partnerId, '/areas/generate/batches/', params.batchId],
    service: async () =>
        ApiService('public').fetchData<GetGenerationBatch_Response>({
            url: `/communities/${params.partnerId}/areas/generate/batches/${params.batchId}`,
            method: 'get',
        }),
});

export type CancelGeneration_Params = { partnerId: string; batchId: string };
export type CancelGeneration_Response = void;
export const CancelGeneration = () => ({
    key: ['/communities/areas', 'cancel-generation'],
    service: async (params: CancelGeneration_Params) =>
        ApiService('protected').fetchData<CancelGeneration_Response>({
            url: `/communities/${params.partnerId}/areas/generate/batches/${params.batchId}`,
            method: 'delete',
        }),
});

// ============================================================================
// Staged Areas (Review Workflow)
// ============================================================================

export type GetStagedAreas_Params = { partnerId: string; batchId: string };
export type GetStagedAreas_Response = StagedAdoptableAreaData[];
export const GetStagedAreas = (params: GetStagedAreas_Params) => ({
    key: ['/communities/', params.partnerId, '/areas/staged', params.batchId],
    service: async () =>
        ApiService('public').fetchData<GetStagedAreas_Response>({
            url: `/communities/${params.partnerId}/areas/staged?batchId=${params.batchId}`,
            method: 'get',
        }),
});

export type ApproveStagedArea_Params = { partnerId: string; id: string };
export type ApproveStagedArea_Response = void;
export const ApproveStagedArea = () => ({
    key: ['/communities/areas/staged', 'approve'],
    service: async (params: ApproveStagedArea_Params) =>
        ApiService('protected').fetchData<ApproveStagedArea_Response>({
            url: `/communities/${params.partnerId}/areas/staged/${params.id}/approve`,
            method: 'put',
        }),
});

export type RejectStagedArea_Params = { partnerId: string; id: string };
export type RejectStagedArea_Response = void;
export const RejectStagedArea = () => ({
    key: ['/communities/areas/staged', 'reject'],
    service: async (params: RejectStagedArea_Params) =>
        ApiService('protected').fetchData<RejectStagedArea_Response>({
            url: `/communities/${params.partnerId}/areas/staged/${params.id}/reject`,
            method: 'put',
        }),
});

export type BulkApproveStagedAreas_Params = { partnerId: string };
export type BulkApproveStagedAreas_Body = { batchId: string; ids?: string[] };
export type BulkApproveStagedAreas_Response = number;
export const BulkApproveStagedAreas = () => ({
    key: ['/communities/areas/staged', 'approve-batch'],
    service: async (params: BulkApproveStagedAreas_Params, body: BulkApproveStagedAreas_Body) =>
        ApiService('protected').fetchData<BulkApproveStagedAreas_Response, BulkApproveStagedAreas_Body>({
            url: `/communities/${params.partnerId}/areas/staged/approve-batch`,
            method: 'post',
            data: body,
        }),
});

export type BulkRejectStagedAreas_Params = { partnerId: string };
export type BulkRejectStagedAreas_Body = { batchId: string; ids?: string[] };
export type BulkRejectStagedAreas_Response = number;
export const BulkRejectStagedAreas = () => ({
    key: ['/communities/areas/staged', 'reject-batch'],
    service: async (params: BulkRejectStagedAreas_Params, body: BulkRejectStagedAreas_Body) =>
        ApiService('protected').fetchData<BulkRejectStagedAreas_Response, BulkRejectStagedAreas_Body>({
            url: `/communities/${params.partnerId}/areas/staged/reject-batch`,
            method: 'post',
            data: body,
        }),
});

export type UpdateStagedAreaName_Params = { partnerId: string; id: string };
export type UpdateStagedAreaName_Body = { name: string };
export type UpdateStagedAreaName_Response = void;
export const UpdateStagedAreaName = () => ({
    key: ['/communities/areas/staged', 'update-name'],
    service: async (params: UpdateStagedAreaName_Params, body: UpdateStagedAreaName_Body) =>
        ApiService('protected').fetchData<UpdateStagedAreaName_Response, UpdateStagedAreaName_Body>({
            url: `/communities/${params.partnerId}/areas/staged/${params.id}/name`,
            method: 'put',
            data: body,
        }),
});

export type CreateApprovedAreas_Params = { partnerId: string };
export type CreateApprovedAreas_Body = { batchId: string };
export type CreateApprovedAreas_Response = BulkImportAreas_Response;
export const CreateApprovedAreas = () => ({
    key: ['/communities/areas/staged', 'create-approved'],
    service: async (params: CreateApprovedAreas_Params, body: CreateApprovedAreas_Body) =>
        ApiService('protected').fetchData<CreateApprovedAreas_Response, CreateApprovedAreas_Body>({
            url: `/communities/${params.partnerId}/areas/staged/create-approved`,
            method: 'post',
            data: body,
        }),
});
