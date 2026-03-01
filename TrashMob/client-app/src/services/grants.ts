import { ApiService } from '.';
import GrantData from '../components/Models/GrantData';
import GrantTaskData from '../components/Models/GrantTaskData';

// --- Grants ---

export type GetGrants_Params = { status?: number };
export type GetGrants_Response = GrantData[];
export const GetGrants = (params?: GetGrants_Params) => {
    const query = params?.status !== undefined ? `?status=${params.status}` : '';
    return {
        key: ['/grants', params?.status],
        service: async () =>
            ApiService('protected').fetchData<GetGrants_Response>({
                url: `/grants${query}`,
                method: 'get',
            }),
    };
};

export type GetGrantById_Params = { id: string };
export type GetGrantById_Response = GrantData;
export const GetGrantById = (params: GetGrantById_Params) => ({
    key: ['/grants', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetGrantById_Response>({
            url: `/grants/${params.id}`,
            method: 'get',
        }),
});

export type CreateGrant_Body = GrantData;
export type CreateGrant_Response = GrantData;
export const CreateGrant = () => ({
    key: ['/grants', 'create'],
    service: async (body: CreateGrant_Body) =>
        ApiService('protected').fetchData<CreateGrant_Response, CreateGrant_Body>({
            url: '/grants',
            method: 'post',
            data: body,
        }),
});

export type UpdateGrant_Body = GrantData;
export type UpdateGrant_Response = GrantData;
export const UpdateGrant = () => ({
    key: ['/grants', 'update'],
    service: async (body: UpdateGrant_Body) =>
        ApiService('protected').fetchData<UpdateGrant_Response, UpdateGrant_Body>({
            url: '/grants',
            method: 'put',
            data: body,
        }),
});

export type DeleteGrant_Params = { id: string };
export const DeleteGrant = () => ({
    key: ['/grants', 'delete'],
    service: async (params: DeleteGrant_Params) =>
        ApiService('protected').fetchData<unknown>({
            url: `/grants/${params.id}`,
            method: 'delete',
        }),
});

// --- Grant Discovery ---

export interface DiscoveredGrantData {
    funderName: string | null;
    programName: string | null;
    description: string | null;
    amountMin: number | null;
    amountMax: number | null;
    deadline: string | null;
    url: string | null;
    eligibilityNotes: string | null;
    rationale: string | null;
}

export interface GrantDiscoveryResultData {
    grants: DiscoveredGrantData[];
    tokensUsed: number;
    message: string | null;
}

export type DiscoverGrants_Body = {
    prompt?: string;
    focusAreas?: string;
    maxResults: number;
};
export type DiscoverGrants_Response = GrantDiscoveryResultData;
export const DiscoverGrants = () => ({
    key: ['/grants', 'discover'],
    service: async (body: DiscoverGrants_Body) =>
        ApiService('protected').fetchData<DiscoverGrants_Response, DiscoverGrants_Body>({
            url: '/grants/discover',
            method: 'post',
            data: body,
        }),
});

// --- Grant Tasks ---

export type GetGrantTasks_Params = { grantId: string };
export type GetGrantTasks_Response = GrantTaskData[];
export const GetGrantTasks = (params: GetGrantTasks_Params) => ({
    key: ['/granttasks', params.grantId],
    service: async () =>
        ApiService('protected').fetchData<GetGrantTasks_Response>({
            url: `/granttasks/bygrant/${params.grantId}`,
            method: 'get',
        }),
});

export type CreateGrantTask_Body = GrantTaskData;
export type CreateGrantTask_Response = GrantTaskData;
export const CreateGrantTask = () => ({
    key: ['/granttasks', 'create'],
    service: async (body: CreateGrantTask_Body) =>
        ApiService('protected').fetchData<CreateGrantTask_Response, CreateGrantTask_Body>({
            url: '/granttasks',
            method: 'post',
            data: body,
        }),
});

export type UpdateGrantTask_Body = GrantTaskData;
export type UpdateGrantTask_Response = GrantTaskData;
export const UpdateGrantTask = () => ({
    key: ['/granttasks', 'update'],
    service: async (body: UpdateGrantTask_Body) =>
        ApiService('protected').fetchData<UpdateGrantTask_Response, UpdateGrantTask_Body>({
            url: '/granttasks',
            method: 'put',
            data: body,
        }),
});

export type DeleteGrantTask_Params = { id: string };
export const DeleteGrantTask = () => ({
    key: ['/granttasks', 'delete'],
    service: async (params: DeleteGrantTask_Params) =>
        ApiService('protected').fetchData<unknown>({
            url: `/granttasks/${params.id}`,
            method: 'delete',
        }),
});
