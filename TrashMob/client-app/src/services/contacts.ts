import { ApiService } from '.';
import ContactData from '../components/Models/ContactData';
import ContactNoteData from '../components/Models/ContactNoteData';
import ContactTagData from '../components/Models/ContactTagData';
import DonationData from '../components/Models/DonationData';

// --- Contacts ---

export type GetContacts_Params = { search?: string; contactType?: number; tagId?: string };
export type GetContacts_Response = ContactData[];
export const GetContacts = (params?: GetContacts_Params) => {
    const queryParts: string[] = [];
    if (params?.search) queryParts.push(`search=${encodeURIComponent(params.search)}`);
    if (params?.contactType !== undefined) queryParts.push(`contactType=${params.contactType}`);
    if (params?.tagId) queryParts.push(`tagId=${params.tagId}`);
    const query = queryParts.length > 0 ? `?${queryParts.join('&')}` : '';

    return {
        key: ['/contacts', params?.search, params?.contactType, params?.tagId],
        service: async () =>
            ApiService('protected').fetchData<GetContacts_Response>({
                url: `/contacts${query}`,
                method: 'get',
            }),
    };
};

export type GetContactById_Params = { id: string };
export type GetContactById_Response = ContactData;
export const GetContactById = (params: GetContactById_Params) => ({
    key: ['/contacts', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetContactById_Response>({
            url: `/contacts/${params.id}`,
            method: 'get',
        }),
});

export type CreateContact_Body = ContactData;
export type CreateContact_Response = ContactData;
export const CreateContact = () => ({
    key: ['/contacts', 'create'],
    service: async (body: CreateContact_Body) =>
        ApiService('protected').fetchData<CreateContact_Response, CreateContact_Body>({
            url: '/contacts',
            method: 'post',
            data: body,
        }),
});

export type UpdateContact_Body = ContactData;
export type UpdateContact_Response = ContactData;
export const UpdateContact = () => ({
    key: ['/contacts', 'update'],
    service: async (body: UpdateContact_Body) =>
        ApiService('protected').fetchData<UpdateContact_Response, UpdateContact_Body>({
            url: '/contacts',
            method: 'put',
            data: body,
        }),
});

export type DeleteContact_Params = { id: string };
export const DeleteContact = () => ({
    key: ['/contacts', 'delete'],
    service: async (params: DeleteContact_Params) =>
        ApiService('protected').fetchData<unknown>({
            url: `/contacts/${params.id}`,
            method: 'delete',
        }),
});

// --- Contact Tags on a Contact ---

export type GetContactTagIds_Params = { contactId: string };
export const GetContactTagIds = (params: GetContactTagIds_Params) => ({
    key: ['/contacts', params.contactId, 'tags'],
    service: async () =>
        ApiService('protected').fetchData<string[]>({
            url: `/contacts/${params.contactId}/tags`,
            method: 'get',
        }),
});

export type UpdateContactTags_Params = { contactId: string };
export type UpdateContactTags_Body = string[];
export const UpdateContactTags = (params: UpdateContactTags_Params) => ({
    key: ['/contacts', params.contactId, 'tags', 'update'],
    service: async (body: UpdateContactTags_Body) =>
        ApiService('protected').fetchData<unknown, UpdateContactTags_Body>({
            url: `/contacts/${params.contactId}/tags`,
            method: 'put',
            data: body,
        }),
});

// --- Contact Notes ---

export type GetContactNotes_Params = { contactId: string };
export type GetContactNotes_Response = ContactNoteData[];
export const GetContactNotes = (params: GetContactNotes_Params) => ({
    key: ['/contactnotes', params.contactId],
    service: async () =>
        ApiService('protected').fetchData<GetContactNotes_Response>({
            url: `/contactnotes/bycontact/${params.contactId}`,
            method: 'get',
        }),
});

export type CreateContactNote_Body = ContactNoteData;
export type CreateContactNote_Response = ContactNoteData;
export const CreateContactNote = () => ({
    key: ['/contactnotes', 'create'],
    service: async (body: CreateContactNote_Body) =>
        ApiService('protected').fetchData<CreateContactNote_Response, CreateContactNote_Body>({
            url: '/contactnotes',
            method: 'post',
            data: body,
        }),
});

export type UpdateContactNote_Body = ContactNoteData;
export type UpdateContactNote_Response = ContactNoteData;
export const UpdateContactNote = () => ({
    key: ['/contactnotes', 'update'],
    service: async (body: UpdateContactNote_Body) =>
        ApiService('protected').fetchData<UpdateContactNote_Response, UpdateContactNote_Body>({
            url: '/contactnotes',
            method: 'put',
            data: body,
        }),
});

export type DeleteContactNote_Params = { id: string };
export const DeleteContactNote = () => ({
    key: ['/contactnotes', 'delete'],
    service: async (params: DeleteContactNote_Params) =>
        ApiService('protected').fetchData<unknown>({
            url: `/contactnotes/${params.id}`,
            method: 'delete',
        }),
});

// --- Contact Tags (CRUD) ---

export type GetContactTags_Response = ContactTagData[];
export const GetContactTags = () => ({
    key: ['/contacttags'],
    service: async () =>
        ApiService('protected').fetchData<GetContactTags_Response>({
            url: '/contacttags',
            method: 'get',
        }),
});

export type CreateContactTag_Body = ContactTagData;
export type CreateContactTag_Response = ContactTagData;
export const CreateContactTag = () => ({
    key: ['/contacttags', 'create'],
    service: async (body: CreateContactTag_Body) =>
        ApiService('protected').fetchData<CreateContactTag_Response, CreateContactTag_Body>({
            url: '/contacttags',
            method: 'post',
            data: body,
        }),
});

export type UpdateContactTag_Body = ContactTagData;
export type UpdateContactTag_Response = ContactTagData;
export const UpdateContactTag = () => ({
    key: ['/contacttags', 'update'],
    service: async (body: UpdateContactTag_Body) =>
        ApiService('protected').fetchData<UpdateContactTag_Response, UpdateContactTag_Body>({
            url: '/contacttags',
            method: 'put',
            data: body,
        }),
});

export type DeleteContactTag_Params = { id: string };
export const DeleteContactTag = () => ({
    key: ['/contacttags', 'delete'],
    service: async (params: DeleteContactTag_Params) =>
        ApiService('protected').fetchData<unknown>({
            url: `/contacttags/${params.id}`,
            method: 'delete',
        }),
});

// --- Donations (read-only for Phase 2) ---

export type GetDonationsByContact_Params = { contactId: string };
export type GetDonationsByContact_Response = DonationData[];
export const GetDonationsByContact = (params: GetDonationsByContact_Params) => ({
    key: ['/donations', 'bycontact', params.contactId],
    service: async () =>
        ApiService('protected').fetchData<GetDonationsByContact_Response>({
            url: `/donations/bycontact/${params.contactId}`,
            method: 'get',
        }),
});
