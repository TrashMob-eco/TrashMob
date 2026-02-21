// Professional Companies API service

import { ApiService } from '.';
import ProfessionalCompanyData from '../components/Models/ProfessionalCompanyData';

// ============================================================================
// Professional Company Operations
// ============================================================================

export type GetProfessionalCompanies_Params = { partnerId: string };
export type GetProfessionalCompanies_Response = ProfessionalCompanyData[];
export const GetProfessionalCompanies = (params: GetProfessionalCompanies_Params) => ({
    key: ['/communities/', params.partnerId, '/professional-companies'],
    service: async () =>
        ApiService('protected').fetchData<GetProfessionalCompanies_Response>({
            url: `/communities/${params.partnerId}/professional-companies`,
            method: 'get',
        }),
});

export type GetProfessionalCompany_Params = { partnerId: string; companyId: string };
export type GetProfessionalCompany_Response = ProfessionalCompanyData;
export const GetProfessionalCompany = (params: GetProfessionalCompany_Params) => ({
    key: ['/communities/', params.partnerId, '/professional-companies/', params.companyId],
    service: async () =>
        ApiService('protected').fetchData<GetProfessionalCompany_Response>({
            url: `/communities/${params.partnerId}/professional-companies/${params.companyId}`,
            method: 'get',
        }),
});

export type CreateProfessionalCompany_Params = { partnerId: string };
export type CreateProfessionalCompany_Body = ProfessionalCompanyData;
export type CreateProfessionalCompany_Response = ProfessionalCompanyData;
export const CreateProfessionalCompany = () => ({
    key: ['/communities/professional-companies', 'create'],
    service: async (params: CreateProfessionalCompany_Params, body: CreateProfessionalCompany_Body) =>
        ApiService('protected').fetchData<CreateProfessionalCompany_Response, CreateProfessionalCompany_Body>({
            url: `/communities/${params.partnerId}/professional-companies`,
            method: 'post',
            data: body,
        }),
});

export type UpdateProfessionalCompany_Params = { partnerId: string; companyId: string };
export type UpdateProfessionalCompany_Body = ProfessionalCompanyData;
export type UpdateProfessionalCompany_Response = ProfessionalCompanyData;
export const UpdateProfessionalCompany = () => ({
    key: ['/communities/professional-companies', 'update'],
    service: async (params: UpdateProfessionalCompany_Params, body: UpdateProfessionalCompany_Body) =>
        ApiService('protected').fetchData<UpdateProfessionalCompany_Response, UpdateProfessionalCompany_Body>({
            url: `/communities/${params.partnerId}/professional-companies/${params.companyId}`,
            method: 'put',
            data: body,
        }),
});

// ============================================================================
// Company User Operations
// ============================================================================

export interface CompanyUserData {
    id: string;
    userName: string;
    email: string;
}

export type GetCompanyUsers_Params = { partnerId: string; companyId: string };
export type GetCompanyUsers_Response = CompanyUserData[];
export const GetCompanyUsers = (params: GetCompanyUsers_Params) => ({
    key: ['/communities/', params.partnerId, '/professional-companies/', params.companyId, '/users'],
    service: async () =>
        ApiService('protected').fetchData<GetCompanyUsers_Response>({
            url: `/communities/${params.partnerId}/professional-companies/${params.companyId}/users`,
            method: 'get',
        }),
});

export interface AssignCompanyUser_Body {
    professionalCompanyId: string;
    userId: string;
}

export type AssignCompanyUser_Params = { partnerId: string; companyId: string };
export type AssignCompanyUser_Response = AssignCompanyUser_Body;
export const AssignCompanyUser = () => ({
    key: ['/communities/professional-companies/users', 'assign'],
    service: async (params: AssignCompanyUser_Params, body: AssignCompanyUser_Body) =>
        ApiService('protected').fetchData<AssignCompanyUser_Response, AssignCompanyUser_Body>({
            url: `/communities/${params.partnerId}/professional-companies/${params.companyId}/users`,
            method: 'post',
            data: body,
        }),
});

export type RemoveCompanyUser_Params = { partnerId: string; companyId: string; userId: string };
export type RemoveCompanyUser_Response = void;
export const RemoveCompanyUser = () => ({
    key: ['/communities/professional-companies/users', 'remove'],
    service: async (params: RemoveCompanyUser_Params) =>
        ApiService('protected').fetchData<RemoveCompanyUser_Response>({
            url: `/communities/${params.partnerId}/professional-companies/${params.companyId}/users/${params.userId}`,
            method: 'delete',
        }),
});
