// docusign

import { ApiService } from '.';
import EnvelopeResponse from '../components/Models/EnvelopeResponse';

export type GetUserDocusignByEnvelopeId_Params = {
    userId: string;
    envelopeId: string;
};
export type GetUserDocusignByEnvelopeId_Response = string;
export const GetUsertDocusignByEnvelopeId = () => ({
    key: ['/docusign', 'by envelope id'],
    service: async (params: GetUserDocusignByEnvelopeId_Params) =>
        ApiService('protected').fetchData<GetUserDocusignByEnvelopeId_Response>({
            url: `/docusign/${params.userId}/${params.envelopeId}`,
            method: 'get',
        }),
});

export type CreateDocusign_Body = {
    signerEmail: string | undefined;
    signerName: string;
    createdByUserId: string;
    returnUrl: string;
};
export type CreateDocusign_Response = EnvelopeResponse;
export const CreateDocusign = () => ({
    key: ['/docusign', 'create'],
    service: async (body: CreateDocusign_Body) =>
        ApiService('protected').fetchData<CreateDocusign_Response, CreateDocusign_Body>({
            url: '/docusign',
            method: 'post',
            data: body,
        }),
});
