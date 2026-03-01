import { ApiService } from '.';
import ContactData from '../components/Models/ContactData';
import ContactNoteData from '../components/Models/ContactNoteData';
import ContactTagData from '../components/Models/ContactTagData';
import DonationData from '../components/Models/DonationData';
import PledgeData from '../components/Models/PledgeData';

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

// --- Donations ---

export type GetDonations_Response = DonationData[];
export const GetDonations = () => ({
    key: ['/donations'],
    service: async () =>
        ApiService('protected').fetchData<GetDonations_Response>({
            url: '/donations',
            method: 'get',
        }),
});

export type GetDonationById_Params = { id: string };
export type GetDonationById_Response = DonationData;
export const GetDonationById = (params: GetDonationById_Params) => ({
    key: ['/donations', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetDonationById_Response>({
            url: `/donations/${params.id}`,
            method: 'get',
        }),
});

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

export type CreateDonation_Body = DonationData;
export type CreateDonation_Response = DonationData;
export const CreateDonation = () => ({
    key: ['/donations', 'create'],
    service: async (body: CreateDonation_Body) =>
        ApiService('protected').fetchData<CreateDonation_Response, CreateDonation_Body>({
            url: '/donations',
            method: 'post',
            data: body,
        }),
});

export type UpdateDonation_Body = DonationData;
export type UpdateDonation_Response = DonationData;
export const UpdateDonation = () => ({
    key: ['/donations', 'update'],
    service: async (body: UpdateDonation_Body) =>
        ApiService('protected').fetchData<UpdateDonation_Response, UpdateDonation_Body>({
            url: '/donations',
            method: 'put',
            data: body,
        }),
});

export type DeleteDonation_Params = { id: string };
export const DeleteDonation = () => ({
    key: ['/donations', 'delete'],
    service: async (params: DeleteDonation_Params) =>
        ApiService('protected').fetchData<unknown>({
            url: `/donations/${params.id}`,
            method: 'delete',
        }),
});

// --- Donation Emails ---

export type SendDonationThankYou_Params = { donationId: string };
export const SendDonationThankYou = () => ({
    key: ['/donations', 'send-thankyou'],
    service: async (params: SendDonationThankYou_Params) =>
        ApiService('protected').fetchData<unknown>({
            url: `/donations/${params.donationId}/send-thankyou`,
            method: 'post',
        }),
});

export type SendDonationReceipt_Params = { donationId: string };
export const SendDonationReceipt = () => ({
    key: ['/donations', 'send-receipt'],
    service: async (params: SendDonationReceipt_Params) =>
        ApiService('protected').fetchData<unknown>({
            url: `/donations/${params.donationId}/send-receipt`,
            method: 'post',
        }),
});

// --- Fundraising Appeals ---

export type SendAppeal_Body = { contactId: string; subject: string; body: string };
export const SendAppeal = () => ({
    key: ['/fundraising-appeals', 'send'],
    service: async (body: SendAppeal_Body) =>
        ApiService('protected').fetchData<unknown, SendAppeal_Body>({
            url: '/fundraising-appeals/send',
            method: 'post',
            data: body,
        }),
});

export type BulkAppealResult = { sentCount: number; failedCount: number; skippedCount: number };
export type SendBulkAppeal_Body = { contactIds: string[]; subject: string; body: string };
export const SendBulkAppeal = () => ({
    key: ['/fundraising-appeals', 'send-bulk'],
    service: async (body: SendBulkAppeal_Body) =>
        ApiService('protected').fetchData<BulkAppealResult, SendBulkAppeal_Body>({
            url: '/fundraising-appeals/send-bulk',
            method: 'post',
            data: body,
        }),
});

// --- Pledges ---

export type GetPledges_Response = PledgeData[];
export const GetPledges = () => ({
    key: ['/pledges'],
    service: async () =>
        ApiService('protected').fetchData<GetPledges_Response>({
            url: '/pledges',
            method: 'get',
        }),
});

export type GetPledgeById_Params = { id: string };
export type GetPledgeById_Response = PledgeData;
export const GetPledgeById = (params: GetPledgeById_Params) => ({
    key: ['/pledges', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetPledgeById_Response>({
            url: `/pledges/${params.id}`,
            method: 'get',
        }),
});

export type GetPledgesByContact_Params = { contactId: string };
export type GetPledgesByContact_Response = PledgeData[];
export const GetPledgesByContact = (params: GetPledgesByContact_Params) => ({
    key: ['/pledges', 'bycontact', params.contactId],
    service: async () =>
        ApiService('protected').fetchData<GetPledgesByContact_Response>({
            url: `/pledges/bycontact/${params.contactId}`,
            method: 'get',
        }),
});

export type CreatePledge_Body = PledgeData;
export type CreatePledge_Response = PledgeData;
export const CreatePledge = () => ({
    key: ['/pledges', 'create'],
    service: async (body: CreatePledge_Body) =>
        ApiService('protected').fetchData<CreatePledge_Response, CreatePledge_Body>({
            url: '/pledges',
            method: 'post',
            data: body,
        }),
});

export type UpdatePledge_Body = PledgeData;
export type UpdatePledge_Response = PledgeData;
export const UpdatePledge = () => ({
    key: ['/pledges', 'update'],
    service: async (body: UpdatePledge_Body) =>
        ApiService('protected').fetchData<UpdatePledge_Response, UpdatePledge_Body>({
            url: '/pledges',
            method: 'put',
            data: body,
        }),
});

export type DeletePledge_Params = { id: string };
export const DeletePledge = () => ({
    key: ['/pledges', 'delete'],
    service: async (params: DeletePledge_Params) =>
        ApiService('protected').fetchData<unknown>({
            url: `/pledges/${params.id}`,
            method: 'delete',
        }),
});

// --- Fundraising Analytics ---

export interface ContactEngagementScoreData {
    contactId: string;
    contactName: string;
    email: string;
    contactType: number;
    engagementScore: number;
    donorLifecycleStage: string;
    totalDonations: number;
    donationCount: number;
    lastDonationDate: string | null;
    eventsAttended: number;
    totalVolunteerMinutes: number;
    noteCount: number;
    lastInteractionDate: string | null;
    isLybunt: boolean;
    hasUserId: boolean;
    donationScoreComponent: number;
    volunteerScoreComponent: number;
    interactionScoreComponent: number;
}

export interface DonorLifecycleStatData {
    stage: string;
    count: number;
}

export interface MonthlyGivingStatData {
    month: string;
    amount: number;
    donationCount: number;
}

export interface CampaignStatData {
    campaign: string;
    totalRaised: number;
    donorCount: number;
    donationCount: number;
}

export interface GrantPipelineStatData {
    status: number;
    label: string;
    count: number;
    totalAmount: number;
}

export interface FundraisingDashboardData {
    totalRaisedYtd: number;
    totalRaisedLastYear: number;
    donorCountYtd: number;
    averageGiftSizeYtd: number;
    donationCountYtd: number;
    retentionRate: number;
    newDonorsYtd: number;
    repeatDonorsYtd: number;
    lapsedDonors: number;
    lifecycleBreakdown: DonorLifecycleStatData[];
    monthlyGiving: MonthlyGivingStatData[];
    campaignBreakdown: CampaignStatData[];
    totalGrantsAwarded: number;
    totalGrantsPending: number;
    activeGrantCount: number;
    upcomingDeadlineCount: number;
    grantPipeline: GrantPipelineStatData[];
    lybuntCount: number;
}

export type GetEngagementScores_Response = ContactEngagementScoreData[];
export const GetEngagementScores = () => ({
    key: ['/fundraising-analytics', 'engagement-scores'],
    service: async () =>
        ApiService('protected').fetchData<GetEngagementScores_Response>({
            url: '/fundraising-analytics/engagement-scores',
            method: 'get',
        }),
});

export type GetContactEngagementScore_Params = { contactId: string };
export type GetContactEngagementScore_Response = ContactEngagementScoreData;
export const GetContactEngagementScore = (params: GetContactEngagementScore_Params) => ({
    key: ['/fundraising-analytics', 'engagement-scores', params.contactId],
    service: async () =>
        ApiService('protected').fetchData<GetContactEngagementScore_Response>({
            url: `/fundraising-analytics/engagement-scores/${params.contactId}`,
            method: 'get',
        }),
});

export type GetFundraisingDashboard_Response = FundraisingDashboardData;
export const GetFundraisingDashboard = () => ({
    key: ['/fundraising-analytics', 'dashboard'],
    service: async () =>
        ApiService('protected').fetchData<GetFundraisingDashboard_Response>({
            url: '/fundraising-analytics/dashboard',
            method: 'get',
        }),
});

export type GetVolunteerPipeline_Response = ContactEngagementScoreData[];
export const GetVolunteerPipeline = () => ({
    key: ['/fundraising-analytics', 'volunteer-pipeline'],
    service: async () =>
        ApiService('protected').fetchData<GetVolunteerPipeline_Response>({
            url: '/fundraising-analytics/volunteer-pipeline',
            method: 'get',
        }),
});

export type GetLybuntContacts_Response = ContactEngagementScoreData[];
export const GetLybuntContacts = () => ({
    key: ['/fundraising-analytics', 'lybunt'],
    service: async () =>
        ApiService('protected').fetchData<GetLybuntContacts_Response>({
            url: '/fundraising-analytics/lybunt',
            method: 'get',
        }),
});

export const ExportDonorReport = () => ({
    key: ['/fundraising-analytics', 'export', 'donors'],
    service: async () =>
        ApiService('protected').fetchData<Blob>({
            url: '/fundraising-analytics/export/donors',
            method: 'get',
            responseType: 'blob',
        }),
});

export const ExportFundraisingSummary = () => ({
    key: ['/fundraising-analytics', 'export', 'summary'],
    service: async () =>
        ApiService('protected').fetchData<Blob>({
            url: '/fundraising-analytics/export/summary',
            method: 'get',
            responseType: 'blob',
        }),
});
