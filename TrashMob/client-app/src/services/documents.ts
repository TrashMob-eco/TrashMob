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

export type UploadPartnerDocument_Params = {
    partnerId: string;
    name: string;
    documentTypeId: number;
    expirationDate?: string;
    file: File;
};
export type UploadPartnerDocument_Response = PartnerDocumentData;
export const UploadPartnerDocument = () => ({
    key: ['/partnerdocuments', 'upload'],
    service: async (params: UploadPartnerDocument_Params) => {
        const formData = new FormData();
        formData.append('partnerId', params.partnerId);
        formData.append('name', params.name);
        formData.append('documentTypeId', params.documentTypeId.toString());
        if (params.expirationDate) formData.append('expirationDate', params.expirationDate);
        formData.append('formFile', params.file);
        return ApiService('protected').fetchData<UploadPartnerDocument_Response>({
            url: '/partnerdocuments/upload',
            method: 'post',
            data: formData,
            headers: { 'Content-Type': 'multipart/form-data' },
        });
    },
});

export type DownloadPartnerDocument_Params = { documentId: string };
export type DownloadPartnerDocument_Response = { downloadUrl: string };
export const DownloadPartnerDocument = (params: DownloadPartnerDocument_Params) => ({
    key: ['/partnerdocuments', params.documentId, 'download'],
    service: async () =>
        ApiService('protected').fetchData<DownloadPartnerDocument_Response>({
            url: `/partnerdocuments/${params.documentId}/download`,
            method: 'get',
        }),
});

export type GetPartnerStorageUsage_Params = { partnerId: string };
export type GetPartnerStorageUsage_Response = { usageBytes: number; limitBytes: number };
export const GetPartnerStorageUsage = (params: GetPartnerStorageUsage_Params) => ({
    key: ['/partnerdocuments/storageusage/', params.partnerId],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerStorageUsage_Response>({
            url: `/partnerdocuments/storageusage/${params.partnerId}`,
            method: 'get',
        }),
});

export type AdminPartnerDocumentData = PartnerDocumentData & { partner?: { name: string } };
export type GetAllPartnerDocuments_Response = AdminPartnerDocumentData[];
export const GetAllPartnerDocuments = () => ({
    key: ['/admin/partnerdocuments'],
    service: async () =>
        ApiService('protected').fetchData<GetAllPartnerDocuments_Response>({
            url: '/admin/partnerdocuments',
            method: 'get',
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
