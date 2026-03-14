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
import { PagedResponse } from '../lib/api-errors';
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
            url: '/v2/lookups/partner-types',
            method: 'get',
        }),
});

export type GetPartnerStatuses_Response = PartnerStatusData[];
export const GetPartnerStatuses = () => ({
    key: ['/partnerStatuses'],
    service: async () =>
        ApiService('public').fetchData<GetPartnerTypes_Response>({
            url: '/v2/lookups/partner-statuses',
            method: 'get',
        }),
});

export type GetPartnerRequestStatuses_Response = PartnerRequestStatusData[];
export const GetPartnerRequestStatuses = () => ({
    key: ['/partnerrequeststatuses'],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerRequestStatuses_Response>({
            url: '/v2/lookups/partner-request-statuses',
            method: 'get',
        }),
});

export type GetPartnerRequests_Response = PartnerRequestData[];
export const GetPartnerRequests = () => ({
    key: ['/partnerrequests'],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerRequests_Response>({
            url: '/v2/partner-requests',
            method: 'get',
        }),
});

export type GetPartnerRequestById_Params = { id: string };
export type GetPartnerRequestById_Response = PartnerRequestData;
export const GetPartnerRequestById = (params: GetPartnerRequestById_Params) => ({
    key: ['/partnerrequests', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerRequestById_Response>({
            url: `/v2/partner-requests/${params.id}`,
            method: 'get',
        }),
});

export type GetPartnerRequestByUserId_Params = { userId: string };
export type GetPartnerRequestByUserId_Response = DisplayPartnershipData[];
export const GetPartnerRequestByUserId = (params: GetPartnerRequestByUserId_Params) => ({
    key: ['/partnerrequests/byuserid/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerRequestByUserId_Response>({
            url: `/v2/partner-requests/by-user/${params.userId}`,
            method: 'get',
        }),
});

export type CreatePartnerRequest_Body = PartnerRequestData;
export type CreatePartnerRequest_Response = PartnerRequestData;
export const CreatePartnerRequest = () => ({
    key: ['/PartnerRequests', 'create'],
    service: async (body: CreatePartnerRequest_Body) =>
        ApiService('protected').fetchData<CreatePartnerRequest_Response, CreatePartnerRequest_Body>({
            url: '/v2/partner-requests',
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
            url: `/v2/partner-requests/${params.id}/approve`,
            method: 'put',
        }),
});

export type DenyPartnerRequest_Params = { id: string };
export type DenyPartnerRequest_Response = PartnerRequestData[];
export const DenyPartnerRequest = () => ({
    key: ['/partnerrequests/deny/'],
    service: async (params: DenyPartnerRequest_Params) =>
        ApiService('protected').fetchData<DenyPartnerRequest_Response>({
            url: `/v2/partner-requests/${params.id}/deny`,
            method: 'put',
        }),
});

export type GetPartners_Response = PagedResponse<PartnerData>;
export const GetPartners = () => ({
    key: ['/partners'],
    service: async () =>
        ApiService('protected').fetchData<GetPartners_Response>({
            url: '/v2/partners?pageSize=100',
            method: 'get',
        }),
});

export type GetPartnerById_Params = { partnerId: string };
export type GetPartnerById_Response = PartnerData;
export const GetPartnerById = (params: GetPartnerById_Params) => ({
    key: ['/partners/', params.partnerId],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerById_Response>({
            url: `/v2/partners/${params.partnerId}`,
            method: 'get',
        }),
});

export type DeletePartnerById_Params = { id: string };
export type DeletePartnerById_Response = unknown;
export const DeletePartnerById = () => ({
    key: ['/partners', 'delete'],
    service: async (params: DeletePartnerById_Params) =>
        ApiService('protected').fetchData<DeletePartnerById_Response>({
            url: `/v2/partners/${params.id}`,
            method: 'delete',
        }),
});

export type UpdatePartner_Body = PartnerData;
export type UpdatePartner_Response = unknown;
export const UpdatePartner = () => ({
    key: ['/Partners', 'update'],
    service: async (body: UpdatePartner_Body) =>
        ApiService('protected').fetchData<UpdatePartner_Response, UpdatePartner_Body>({
            url: '/v2/partners',
            method: 'put',
            data: body,
        }),
});

// Get partners (communities) the current user is an admin of
export type GetMyPartners_Response = PartnerData[];
export const GetMyPartners = () => ({
    key: ['/partneradmins/my'],
    service: async () =>
        ApiService('protected').fetchData<GetMyPartners_Response>({
            url: '/v2/partner-admins/my',
            method: 'get',
        }),
});
