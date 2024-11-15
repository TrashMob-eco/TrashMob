// socialmediaaccounttypes

import { ApiService } from '.';
import PartnerSocialMediaAccountData from '../components/Models/PartnerSocialMediaAccountData';
import SocialMediaAccountTypeData from '../components/Models/SocialMediaAccountTypeData';

export type GetSocialMediaAccountTypes_Response = SocialMediaAccountTypeData[];
export const GetSocialMediaAccountTypes = () => ({
    key: ['/socialMediaAccounttypes'],
    service: async () =>
        ApiService('public').fetchData<GetSocialMediaAccountTypes_Response>({
            url: '/socialMediaAccounttypes',
            method: 'get',
        }),
});

export type GetPartnerSocialMediaAccountsByPartnerId_Params = {
    partnerId: string;
};
export type GetPartnerSocialMediaAccountsByPartnerId_Response = PartnerSocialMediaAccountData[];
export const GetPartnerSocialMediaAccountsByPartnerId = (params: GetPartnerSocialMediaAccountsByPartnerId_Params) => ({
    key: ['/partnersocialmediaaccounts/getbypartner/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerSocialMediaAccountsByPartnerId_Response>({
            url: `/partnersocialmediaaccounts/getbypartner/${params.partnerId}`,
            method: 'get',
        }),
});

export type GetPartnerSocialMediaAccount_Params = { partnerAccountId: string };
export type GetPartnerSocialMediaAccount_Response = PartnerSocialMediaAccountData;
export const GetPartnerSocialMediaAccount = () => ({
    key: ['/partnersocialmediaaccounts/', 'get one'],
    service: async (params: GetPartnerSocialMediaAccount_Params) =>
        ApiService('protected').fetchData<GetPartnerSocialMediaAccount_Response>({
            url: `/partnersocialmediaaccounts/${params.partnerAccountId}`,
            method: 'get',
        }),
});

export type CreatePartnerSocialMediaAccount_Body = PartnerSocialMediaAccountData;
export type CreatePartnerSocialMediaAccount_Response = unknown;
export const CreatePartnerSocialMediaAccount = () => ({
    key: ['/partnersocialmediaaccounts', 'create'],
    service: async (body: CreatePartnerSocialMediaAccount_Body) =>
        ApiService('protected').fetchData<
            CreatePartnerSocialMediaAccount_Response,
            CreatePartnerSocialMediaAccount_Body
        >({ url: '/partnersocialmediaaccounts', method: 'post', data: body }),
});

export type UpdatePartnerSocialMediaAccount_Body = PartnerSocialMediaAccountData;
export type UpdatePartnerSocialMediaAccount_Response = unknown;
export const UpdatePartnerSocialMediaAccount = () => ({
    key: ['/partnersocialmediaaccounts', 'update'],
    service: async (body: UpdatePartnerSocialMediaAccount_Body) =>
        ApiService('protected').fetchData<
            UpdatePartnerSocialMediaAccount_Response,
            UpdatePartnerSocialMediaAccount_Body
        >({ url: '/partnersocialmediaaccounts', method: 'put', data: body }),
});

export type DeletePartnerSocialMediaAccountById_Params = { accountId: string };
export type DeletePartnerSocialMediaAccountById_Response = unknown;
export const DeletePartnerSocialMediaAccountById = () => ({
    key: ['/partnersocialmediaaccounts/', 'delete'],
    service: async (params: DeletePartnerSocialMediaAccountById_Params) =>
        ApiService('protected').fetchData<DeletePartnerSocialMediaAccountById_Response>({
            url: `/partnersocialmediaaccounts/${params.accountId}`,
            method: 'delete',
        }),
});
