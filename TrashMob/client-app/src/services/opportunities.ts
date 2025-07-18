import { ApiService } from '.';
import JobOpportunityData from '../components/Models/JobOpportunityData';

export type GetAllJobOpportunities_Response = JobOpportunityData[];
export const GetAllJobOpportunities = () => ({
    key: ['/jobopportunities'],
    service: async () =>
        ApiService('protected').fetchData<GetAllJobOpportunities_Response>({
            url: '/jobopportunities',
            method: 'get',
        }),
});

export type GetJobOpportunityById_Response = JobOpportunityData;
export const GetJobOpportunityById = (id: string) => ({
    key: ['/jobopportunities', id],
    service: async () =>
        ApiService('protected').fetchData<GetJobOpportunityById_Response>({
            url: `/jobopportunities/${id}`,
            method: 'get',
        }),
});

export type CreateJobOpportunity_Body = JobOpportunityData;
export type CreateJobOpportunity_Response = unknown;
export const CreateJobOpportunity = () => ({
    key: ['/jobopportunities', 'create'],
    service: async (body: CreateJobOpportunity_Body) =>
        ApiService('protected').fetchData<CreateJobOpportunity_Response, CreateJobOpportunity_Body>({
            url: '/jobopportunities',
            method: 'post',
            data: body,
        }),
});

export type UpdateJobOpportunity_Body = JobOpportunityData;
export type UpdateJobOpportunity_Response = unknown;
export const UpdateJobOpportunity = () => ({
    key: ['/jobopportunities', 'update'],
    service: async (body: UpdateJobOpportunity_Body) =>
        ApiService('protected').fetchData<CreateJobOpportunity_Response, UpdateJobOpportunity_Body>({
            url: '/jobopportunities',
            method: 'put',
            data: body,
        }),
});

export type DeleteJobOpportunityById_Params = { id: string };
export type DeleteJobOpportunityById_Response = JobOpportunityData | null;
export const DeleteJobOpportunityById = () => ({
    key: ['/jobopportunities/', 'delete'],
    service: async (params: DeleteJobOpportunityById_Params) =>
        ApiService('protected').fetchData<DeleteJobOpportunityById_Response>({
            url: `/jobopportunities/${params.id}`,
            method: 'delete',
        }),
});
