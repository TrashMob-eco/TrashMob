import { ApiService } from '.';
import DisplayPartnershipData from '../components/Models/DisplayPartnershipData';
import EmailTemplateData from '../components/Models/EmailTemplateData';
import UserData from '../components/Models/UserData';

export type GetAdminEmailTemplates_Response = EmailTemplateData[];
export const GetAdminEmailTemplates = () => ({
    key: ['/admin/emailtemplates'],
    service: async () =>
        ApiService('protected').fetchData<GetAdminEmailTemplates_Response>({
            url: '/admin/emailtemplates',
            method: 'get',
        }),
});

export type GetPartnerAdminsByPartnerId_Params = { partnerId: string };
export type GetPartnerAdminsByPartnerId_Response = UserData[];
export const GetPartnerAdminsByPartnerId = (params: GetPartnerAdminsByPartnerId_Params) => ({
    key: ['/partneradmins/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerAdminsByPartnerId_Response>({
            url: `/partneradmins/${params.partnerId}`,
            method: 'get',
        }),
});

export type DeletePartnerAdminsByPartnerAndUserId_Params = {
    partnerId: string;
    userId: string;
};
export type DeletePartnerAdminsByPartnerAndUserId_Response = unknown;
export const DeletePartnerAdminsByPartnerIAndUserId = () => ({
    key: ['/partneradmins/', 'delete by partnerId and userId'],
    service: async (params: DeletePartnerAdminsByPartnerAndUserId_Params) =>
        ApiService('protected').fetchData<DeletePartnerAdminsByPartnerAndUserId_Response>({
            url: `/partneradmins/${params.partnerId}/${params.userId}`,
            method: 'delete',
        }),
});

export type GetPartnerAdminsForUser_Params = { userId: string };
export type GetPartnerAdminsForUser_Response = DisplayPartnershipData[];
export const GetPartnerAdminsForUser = (params: GetPartnerAdminsForUser_Params) => ({
    key: ['/partneradmins/getpartnersforuser/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerAdminsForUser_Response>({
            url: `/partneradmins/getpartnersforuser/${params.userId}`,
            method: 'get',
        }),
});
