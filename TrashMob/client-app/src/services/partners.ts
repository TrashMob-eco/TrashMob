// partnerrequeststatuses
// partnerrequests
// PartnerRequests
// partnerRequestsStatuses
// partners
// eventpartnerlocationservices
// eventpartnerlocationservicestatuses
// partnerlocationeventservices
// partnerAdminInvitations
// partnerAdmins
// partnercontacts
// partnerdocuments
// partnerstatuses
// partnertypes
// partnerlocationcontacts
// partnerlocations
// partnerlocationservices
// partnersocialmediaaccounts

import { ApiService } from '.';
import DisplayPartnershipData from '../components/Models/DisplayPartnershipData';
import PartnerData from '../components/Models/PartnerData';
import PartnerRequestData from '../components/Models/PartnerRequestData';
import PartnerRequestStatusData from '../components/Models/PartnerRequestStatusData';
import PartnerStatusData from '../components/Models/PartnerStatusData';
import PartnerTypeData from '../components/Models/PartnerTypeData';

export type GetPartnerTypes_Response = PartnerTypeData[];
export const GetPartnerTypes = () => ({
    key: ['/partnerTypes'],
    service: async () =>
        ApiService('public').fetchData<GetPartnerTypes_Response>({
            url: '/partnerTypes',
            method: 'get',
        }),
});

export type GetPartnerStatuses_Response = PartnerStatusData[];
export const GetPartnerStatuses = () => ({
    key: ['/partnerStatuses'],
    service: async () =>
        ApiService('public').fetchData<GetPartnerTypes_Response>({
            url: '/partnerStatuses',
            method: 'get',
        }),
});

export type GetPartnerRequestStatuses_Response = PartnerRequestStatusData[];
export const GetPartnerRequestStatuses = () => ({
    key: ['/partnerrequeststatuses'],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerRequestStatuses_Response>({
            url: '/partnerrequeststatuses',
            method: 'get',
        }),
});

export type GetPartnerRequests_Response = PartnerRequestData[];
export const GetPartnerRequests = () => ({
    key: ['/partnerrequests'],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerRequests_Response>({
            url: '/partnerrequests',
            method: 'get',
        }),
});

export type GetPartnerRequestById_Params = { id: string };
export type GetPartnerRequestById_Response = PartnerRequestData;
export const GetPartnerRequestById = () => ({
    key: ['/partnerrequests', 'by id'],
    service: async (params: GetPartnerRequestById_Params) =>
        ApiService('protected').fetchData<GetPartnerRequestById_Response>({
            url: `/partnerrequests/${params.id}`,
            method: 'get',
        }),
});

export type GetPartnerRequestByUserId_Params = { userId: string };
export type GetPartnerRequestByUserId_Response = DisplayPartnershipData[];
export const GetPartnerRequestByUserId = (params: GetPartnerRequestByUserId_Params) => ({
    key: ['/partnerrequests/byuserid/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerRequestByUserId_Response>({
            url: `/partnerrequests/byuserid/${params.userId}`,
            method: 'get',
        }),
});

export type CreatePartnerRequest_Body = PartnerRequestData;
export type CreatePartnerRequest_Response = PartnerRequestData;
export const CreatePartnerRequest = () => ({
    key: ['/PartnerRequests', 'create'],
    service: async (body: CreatePartnerRequest_Body) =>
        ApiService('protected').fetchData<CreatePartnerRequest_Response, CreatePartnerRequest_Body>({
            url: '/PartnerRequests',
            method: 'post',
            data: body,
        }),
});

export type ApprovePartnerRequest_Params = { id: string };
export type ApprovePartnerRequest_Response = PartnerRequestData[];
export const ApprovePartnerRequest = () => ({
    key: ['/partnerrequests/approve/'],
    service: async (params: ApprovePartnerRequest_Params) =>
        ApiService('protected').fetchData<ApprovePartnerRequest_Response>({
            url: `/partnerrequests/approve/${params.id}`,
            method: 'put',
        }),
});

export type DenyPartnerRequest_Params = { id: string };
export type DenyPartnerRequest_Response = PartnerRequestData[];
export const DenyPartnerRequest = () => ({
    key: ['/partnerrequests/deny/'],
    service: async (params: DenyPartnerRequest_Params) =>
        ApiService('protected').fetchData<DenyPartnerRequest_Response>({
            url: `/partnerrequests/deny/${params.id}`,
            method: 'put',
        }),
});

export type GetPartners_Response = PartnerData[];
export const GetPartners = () => ({
    key: ['/partners'],
    service: async () =>
        ApiService('protected').fetchData<GetPartners_Response>({
            url: '/partners',
            method: 'get',
        }),
});

export type GetPartnerById_Params = { partnerId: string };
export type GetPartnerById_Response = PartnerData;
export const GetPartnerById = (params: GetPartnerById_Params) => ({
    key: ['/partners/', params.partnerId],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerById_Response>({
            url: `/partners/${params.partnerId}`,
            method: 'get',
        }),
});

export type DeletePartnerById_Params = { id: string };
export type DeletePartnerById_Response = unknown;
export const DeletePartnerById = () => ({
    key: ['/partners', 'delete'],
    service: async (params: DeletePartnerById_Params) =>
        ApiService('protected').fetchData<DeletePartnerById_Response>({
            url: `/partners/${params.id}`,
            method: 'delete',
        }),
});

export type UpdatePartner_Body = PartnerData;
export type UpdatePartner_Response = unknown;
export const UpdatePartner = () => ({
    key: ['/Partners', 'update'],
    service: async (body: UpdatePartner_Body) =>
        ApiService('protected').fetchData<UpdatePartner_Response, UpdatePartner_Body>({
            url: '/Partners',
            method: 'put',
            data: body,
        }),
});
