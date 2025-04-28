// ContactRequest

import { ApiService } from '.';
import ContactRequestData from '../components/Models/ContactRequestData';
import PartnerContactData from '../components/Models/PartnerContactData';
import PartnerLocationContactData from '../components/Models/PartnerLocationContactData';

export type CreateContactRequest_Body = ContactRequestData;
export type CreateContactRequest_Response = unknown;
export const CreateContactRequest = () => ({
    key: ['/ContactRequest', 'create'],
    service: async (body: CreateContactRequest_Body, captchaToken: string) =>
        ApiService('public').fetchData<CreateContactRequest_Response, CreateContactRequest_Body>({
            url: `/ContactRequest?captchaToken=${captchaToken}`,
            method: 'post',
            data: body,
        }),
});

export type GetPartnerLocationContactsByLocationId_Params = {
    locationId: string;
};
export type GetPartnerLocationContactsByLocationId_Response = PartnerLocationContactData[];
export const GetPartnerLocationContactsByLocationId = (params: GetPartnerLocationContactsByLocationId_Params) => ({
    key: ['/partnerlocationcontacts/getbypartnerlocation/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerLocationContactsByLocationId_Response>({
            url: `/partnerlocationcontacts/getbypartnerlocation/${params.locationId}`,
            method: 'get',
        }),
});

export type GetPartnerLocationContactByContactId_Params = { contactId: string };
export type GetPartnerLocationContactByContactId_Response = PartnerLocationContactData;
export const GetPartnerLocationContactByContactId = (params: GetPartnerLocationContactByContactId_Params) => ({
    key: ['/partnerlocationcontacts/', params.contactId],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerLocationContactByContactId_Response>({
            url: `/partnerlocationcontacts/${params.contactId}`,
            method: 'get',
        }),
});

export type CreatePartnerLocationContact_Body = PartnerLocationContactData;
export type CreatePartnerLocationContact_Response = unknown;
export const CreatePartnerLocationContact = () => ({
    key: ['/partnerlocationcontacts', 'create'],
    service: async (body: CreatePartnerLocationContact_Body) =>
        ApiService('protected').fetchData<CreatePartnerLocationContact_Response, CreatePartnerLocationContact_Body>({
            url: '/partnerlocationcontacts',
            method: 'post',
            data: body,
        }),
});

export type UpdatePartnerLocationContact_Body = PartnerLocationContactData;
export type UpdatePartnerLocationContact_Response = unknown;
export const UpdatePartnerLocationContact = () => ({
    key: ['/partnerlocationcontacts', 'update'],
    service: async (body: UpdatePartnerLocationContact_Body) =>
        ApiService('protected').fetchData<UpdatePartnerLocationContact_Response, UpdatePartnerLocationContact_Body>({
            url: '/partnerlocationcontacts',
            method: 'put',
            data: body,
        }),
});

export type DeletePartnerLocationContactByContactId_Params = {
    contactId: string;
};
export type DeletePartnerLocationContactByContactId_Response = unknown;
export const DeletePartnerLocationContactByContactId = () => ({
    key: ['/partnerlocationcontacts', 'delete by contactId'],
    service: async (params: DeletePartnerLocationContactByContactId_Params) =>
        ApiService('protected').fetchData<DeletePartnerLocationContactByContactId_Response>({
            url: `/partnerlocationcontacts/${params.contactId}`,
            method: 'delete',
        }),
});

export type GetPartnerContactsByPartnerId_Params = { partnerId: string };
export type GetPartnerContactsByPartnerId_Response = PartnerContactData[];
export const GetPartnerContactsByPartnerId = (params: GetPartnerContactsByPartnerId_Params) => ({
    key: ['/partnercontacts/getbypartner/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerContactsByPartnerId_Response>({
            url: `/partnercontacts/getbypartner/${params.partnerId}`,
            method: 'get',
        }),
});

export type GetPartnerContactsByContactId_Params = { contactId: string };
export type GetPartnerContactsByContactId_Response = PartnerContactData;
export const GetPartnerContactsByContactId = (params: GetPartnerContactsByContactId_Params) => ({
    key: ['/partnercontacts/', params.contactId],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerContactsByContactId_Response>({
            url: `/partnercontacts/${params.contactId}`,
            method: 'get',
        }),
});

export type CreatePartnerContact_Body = PartnerContactData;
export type CreatePartnerContact_Response = unknown;
export const CreatePartnerContact = () => ({
    key: ['/partnercontacts', 'create'],
    service: async (body: CreatePartnerContact_Body) =>
        ApiService('protected').fetchData<CreatePartnerContact_Response, CreatePartnerContact_Body>({
            url: '/partnercontacts',
            method: 'post',
            data: body,
        }),
});

export type UpdatePartnerContact_Body = PartnerContactData;
export type UpdatePartnerContact_Response = unknown;
export const UpdatePartnerContact = () => ({
    key: ['/partnercontacts', 'update'],
    service: async (body: UpdatePartnerContact_Body) =>
        ApiService('protected').fetchData<UpdatePartnerContact_Response, UpdatePartnerContact_Body>({
            url: '/partnercontacts',
            method: 'put',
            data: body,
        }),
});

export type DeletePartnerContactByContactId_Params = { contactId: string };
export type DeletePartnerContactByContactId_Response = unknown;
export const DeletePartnerContactByContactId = () => ({
    key: ['/partnercontacts/', 'delete by contactId'],
    service: async (params: DeletePartnerContactByContactId_Params) =>
        ApiService('protected').fetchData<DeletePartnerContactByContactId_Response>({
            url: `/partnercontacts/${params.contactId}`,
            method: 'delete',
        }),
});
