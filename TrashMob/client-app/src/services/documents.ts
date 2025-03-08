import { ApiService } from '.';
import PartnerDocumentData from '../components/Models/PartnerDocumentData';

export type GetPartnerDocumentsByPartnerId_Params = { partnerId: string };
export type GetPartnerDocumentsByPartnerId_Response = PartnerDocumentData[];
export const GetPartnerDocumentsByPartnerId = (params: GetPartnerDocumentsByPartnerId_Params) => ({
    key: ['/partnerdocuments/getbypartner/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerDocumentsByPartnerId_Response>({
            url: `/partnerdocuments/getbypartner/${params.partnerId}`,
            method: 'get',
        }),
});

export type GetPartnerDocumentsByDocumentId_Params = { documentId: string };
export type GetPartnerDocumentsByDocumentId_Response = PartnerDocumentData;
export const GetPartnerDocumentsByDocumentId = (params: GetPartnerDocumentsByDocumentId_Params) => ({
    key: ['/partnerdocuments/', params.documentId],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerDocumentsByDocumentId_Response>({
            url: `/partnerdocuments/${params.documentId}`,
            method: 'get',
        }),
});

export type CreatePartnerDocument_Body = PartnerDocumentData;
export type CreatePartnerDocument_Response = unknown;
export const CreatePartnerDocument = () => ({
    key: ['/partnerdocuments', 'create'],
    service: async (body: CreatePartnerDocument_Body) =>
        ApiService('protected').fetchData<CreatePartnerDocument_Response, CreatePartnerDocument_Body>({
            url: '/partnerdocuments',
            method: 'post',
            data: body,
        }),
});

export type UpdatePartnerDocument_Body = PartnerDocumentData;
export type UpdatePartnerDocument_Response = unknown;
export const UpdatePartnerDocument = () => ({
    key: ['/partnerdocuments', 'update'],
    service: async (body: UpdatePartnerDocument_Body) =>
        ApiService('protected').fetchData<UpdatePartnerDocument_Response, UpdatePartnerDocument_Body>({
            url: '/partnerdocuments',
            method: 'put',
            data: body,
        }),
});

export type DeletePartnerDocumentByDocuemntId_Params = { documentId: string };
export type DeletePartnerDocumentByDocuemntId_Response = unknown;
export const DeletePartnerDocumentByDocuemntId = () => ({
    key: ['/partnerdocuments/', 'delete by documentId'],
    service: async (params: DeletePartnerDocumentByDocuemntId_Params) =>
        ApiService('protected').fetchData<DeletePartnerDocumentByDocuemntId_Response>({
            url: `/partnerdocuments/${params.documentId}`,
            method: 'delete',
        }),
});
