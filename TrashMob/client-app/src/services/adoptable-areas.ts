// Adoptable Areas API service

import { ApiService } from '.';
import AdoptableAreaData from '../components/Models/AdoptableAreaData';

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
