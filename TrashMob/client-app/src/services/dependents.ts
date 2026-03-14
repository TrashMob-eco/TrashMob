// Dependents API service

import { ApiService } from '.';
import DependentData from '../components/Models/DependentData';

// ============================================================================
// Dependent Profile Operations
// ============================================================================

export type GetMyDependents_Params = { userId: string };
export type GetMyDependents_Response = DependentData[];
export const GetMyDependents = (params: GetMyDependents_Params) => ({
    key: ['/users', params.userId, 'dependents'],
    service: async () =>
        ApiService('protected').fetchData<GetMyDependents_Response>({
            url: `/v2/users/${params.userId}/dependents`,
            method: 'get',
        }),
});

export type AddDependent_Params = { userId: string };
export type AddDependent_Body = DependentData;
export type AddDependent_Response = DependentData;
export const AddDependent = (params: AddDependent_Params) => ({
    key: ['/users', params.userId, 'dependents', 'create'],
    service: async (body: AddDependent_Body) =>
        ApiService('protected').fetchData<AddDependent_Response, AddDependent_Body>({
            url: `/v2/users/${params.userId}/dependents`,
            method: 'post',
            data: body,
        }),
});

export type UpdateDependent_Params = { userId: string };
export type UpdateDependent_Body = DependentData;
export type UpdateDependent_Response = DependentData;
export const UpdateDependent = (params: UpdateDependent_Params) => ({
    key: ['/users', params.userId, 'dependents', 'update'],
    service: async (body: UpdateDependent_Body) =>
        ApiService('protected').fetchData<UpdateDependent_Response, UpdateDependent_Body>({
            url: `/v2/users/${params.userId}/dependents/${body.id}`,
            method: 'put',
            data: body,
        }),
});

export type DeleteDependent_Params = { userId: string; dependentId: string };
export type DeleteDependent_Response = void;
export const DeleteDependent = (params: DeleteDependent_Params) => ({
    key: ['/users', params.userId, 'dependents', 'delete'],
    service: async () =>
        ApiService('protected').fetchData<DeleteDependent_Response>({
            url: `/v2/users/${params.userId}/dependents/${params.dependentId}`,
            method: 'delete',
        }),
});

// ============================================================================
// Dependent Waiver Operations
// ============================================================================

export type GetDependentCurrentWaiver_Params = { dependentId: string };
export type GetDependentCurrentWaiver_Response = {
    id: string;
    dependentId: string;
    expiryDate: string;
    acceptedDate: string;
};
export const GetDependentCurrentWaiver = (params: GetDependentCurrentWaiver_Params) => ({
    key: ['/dependents', params.dependentId, 'waiver'],
    service: async () =>
        ApiService('protected').fetchData<GetDependentCurrentWaiver_Response>({
            url: `/v2/dependents/${params.dependentId}/waiver`,
            method: 'get',
        }),
});

// ============================================================================
// Event Dependent Operations
// ============================================================================

export type GetEventDependentCount_Params = { eventId: string };
export type GetEventDependentCount_Response = number;
export const GetEventDependentCount = (params: GetEventDependentCount_Params) => ({
    key: ['/events', params.eventId, 'dependents', 'count'],
    service: async () =>
        ApiService('protected').fetchData<GetEventDependentCount_Response>({
            url: `/v2/events/${params.eventId}/dependents/count`,
            method: 'get',
        }),
});

export type RegisterEventDependents_Params = { eventId: string };
export type RegisterEventDependents_Body = { dependentIds: string[] };
export type RegisterEventDependents_Response = unknown[];
export const RegisterEventDependents = (params: RegisterEventDependents_Params) => ({
    key: ['/events', params.eventId, 'dependents', 'register'],
    service: async (body: RegisterEventDependents_Body) =>
        ApiService('protected').fetchData<RegisterEventDependents_Response, RegisterEventDependents_Body>({
            url: `/v2/events/${params.eventId}/dependents`,
            method: 'post',
            data: body,
        }),
});

export type GetEventDependents_Params = { eventId: string };
export type GetEventDependents_Response = Array<{
    id: string;
    eventId: string;
    dependentId: string;
    parentUserId: string;
    dependent?: DependentData;
}>;
export const GetEventDependents = (params: GetEventDependents_Params) => ({
    key: ['/events', params.eventId, 'dependents'],
    service: async () =>
        ApiService('protected').fetchData<GetEventDependents_Response>({
            url: `/v2/events/${params.eventId}/dependents`,
            method: 'get',
        }),
});

export type UnregisterEventDependent_Params = { eventId: string; dependentId: string };
export type UnregisterEventDependent_Response = void;
export const UnregisterEventDependent = (params: UnregisterEventDependent_Params) => ({
    key: ['/events', params.eventId, 'dependents', 'unregister'],
    service: async () =>
        ApiService('protected').fetchData<UnregisterEventDependent_Response>({
            url: `/v2/events/${params.eventId}/dependents/${params.dependentId}`,
            method: 'delete',
        }),
});
