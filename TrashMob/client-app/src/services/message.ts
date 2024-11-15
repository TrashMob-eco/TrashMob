// messagerequest

import { ApiService } from '.';
import MessageRequestData from '../components/Models/MessageRequestData';

export type CreateMessageRequest_Body = MessageRequestData;
export type CreateMessageRequest_Response = unknown;
export const CreateMessageRequest = () => ({
    key: ['/messagerequest/'],
    service: async (body: CreateMessageRequest_Body) =>
        ApiService('protected').fetchData<CreateMessageRequest_Response, CreateMessageRequest_Body>({
            url: '/messagerequest/',
            method: 'post',
            data: body,
        }),
});
