// invitationstatuses

import { ApiService } from '.';
import DisplayPartnerAdminInvitationData from '../components/Models/DisplayPartnerAdminInvitationData';
import InvitationStatusData from '../components/Models/InvitationStatusData';
import PartnerAdminInvitationData from '../components/Models/PartnerAdminInvitationData';

export type GetInvitationStatuses_Response = InvitationStatusData[];
export const GetInvitationStatuses = () => ({
    key: ['/invitationStatuses'],
    service: async () =>
        ApiService('public').fetchData<GetInvitationStatuses_Response>({
            url: '/invitationStatuses',
            method: 'get',
        }),
});

export type GetPartnerAdminInvitationsByPartnerId_Params = {
    partnerId: string;
};
export type GetPartnerAdminInvitationsByPartnerId_Response = PartnerAdminInvitationData[];
export const GetPartnerAdminInvitationsByPartnerId = (params: GetPartnerAdminInvitationsByPartnerId_Params) => ({
    key: ['/partneradmininvitations/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerAdminInvitationsByPartnerId_Response>({
            url: `/partneradmininvitations/${params.partnerId}`,
            method: 'get',
        }),
});

export type ResendPartnerAdminInvitation_Params = { invitationId: string };
export type ResendPartnerAdminInvitation_Response = unknown;
export const ResendPartnerAdminInvitation = () => ({
    key: ['/partneradmininvitations/resend/', 'resend Invitation'],
    service: async (params: ResendPartnerAdminInvitation_Params) =>
        ApiService('protected').fetchData<ResendPartnerAdminInvitation_Response>({
            url: `/partneradmininvitations/resend/${params.invitationId}`,
            method: 'post',
        }),
});

export type GetPartnerAdminInvitationsByGetByPartnerId_Params = {
    partnerId: string;
};
export type GetPartnerAdminInvitationsByGetByPartnerId_Response = PartnerAdminInvitationData[];
export const GetPartnerAdminInvitationsByGetByPartnerId = (
    params: GetPartnerAdminInvitationsByGetByPartnerId_Params,
) => ({
    key: ['/partneradmininvitations/getbypartner/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerAdminInvitationsByGetByPartnerId_Response>({
            url: `/partneradmininvitations/getbypartner/${params.partnerId}`,
            method: 'get',
        }),
});

export type CreatePartnerAdminInvitation_Body = PartnerAdminInvitationData;
export type CreatePartnerAdminInvitation_Response = unknown;
export const CreatePartnerAdminInvitation = () => ({
    key: ['/partneradmininvitations/', 'create'],
    service: async (body: CreatePartnerAdminInvitation_Body) =>
        ApiService('protected').fetchData<CreatePartnerAdminInvitation_Response>({
            url: '/partneradmininvitations',
            method: 'post',
            data: body,
        }),
});

export type DeletePartnerAdminInvitation_Params = { invitationId: string };
export type DeletePartnerAdminInvitation_Response = unknown;
export const DeletePartnerAdminInvitation = () => ({
    key: ['/partneradmininvitations/', 'delete by invitationId'],
    service: async (params: DeletePartnerAdminInvitation_Params) =>
        ApiService('protected').fetchData<DeletePartnerAdminInvitation_Response>({
            url: `/partneradmininvitations/${params.invitationId}`,
            method: 'delete',
        }),
});

export type GetPartnerAdminInvitationsByUser_Params = { userId: string };
export type GetPartnerAdminInvitationsByUser_Response = DisplayPartnerAdminInvitationData[];
export const GetPartnerAdminInvitationsByUser = (params: GetPartnerAdminInvitationsByUser_Params) => ({
    key: ['/partnerAdminInvitations/getbyuser/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerAdminInvitationsByUser_Response>({
            url: `/partnerAdminInvitations/getbyuser/${params.userId}`,
            method: 'get',
        }),
});

export type AcceptPartnerAdminInvitation_Params = { invitationId: string };
export type AcceptPartnerAdminInvitation_Response = unknown;
export const AcceptPartnerAdminInvitation = () => ({
    key: ['/partneradmininvitations/accept/', 'by invitationId'],
    service: async (params: AcceptPartnerAdminInvitation_Params) =>
        ApiService('protected').fetchData<AcceptPartnerAdminInvitation_Response>({
            url: `/partneradmininvitations/accept/${params.invitationId}`,
            method: 'post',
        }),
});

export type DeclinePartnerAdminInvitation_Params = { invitationId: string };
export type DeclinePartnerAdminInvitation_Response = unknown;
export const DeclinePartnerAdminInvitation = () => ({
    key: ['/partneradmininvitations/decline/', 'by invitationId'],
    service: async (params: DeclinePartnerAdminInvitation_Params) =>
        ApiService('protected').fetchData<DeclinePartnerAdminInvitation_Response>({
            url: `/partneradmininvitations/decline/${params.invitationId}`,
            method: 'post',
        }),
});
