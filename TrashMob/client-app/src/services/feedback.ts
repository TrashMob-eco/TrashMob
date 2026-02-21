// UserFeedback Service

import { ApiService } from '.';

export interface UserFeedbackData {
    id?: string;
    userId?: string;
    category: 'Bug' | 'FeatureRequest' | 'General' | 'Praise';
    description: string;
    email?: string;
    screenshotUrl?: string;
    pageUrl?: string;
    userAgent?: string;
    status?: string;
    internalNotes?: string;
    reviewedByUserId?: string;
    reviewedDate?: string;
    gitHubIssueUrl?: string;
    createdDate?: string;
}

export interface UpdateFeedbackRequest {
    status: string;
    internalNotes?: string;
}

// Submit feedback (public endpoint)
export type SubmitFeedback_Body = Omit<
    UserFeedbackData,
    'id' | 'status' | 'internalNotes' | 'reviewedByUserId' | 'reviewedDate' | 'gitHubIssueUrl' | 'createdDate'
>;
export type SubmitFeedback_Response = UserFeedbackData;
export const SubmitFeedback = () => ({
    key: ['/feedback', 'submit'],
    service: async (body: SubmitFeedback_Body) =>
        ApiService('public').fetchData<SubmitFeedback_Response, SubmitFeedback_Body>({
            url: '/feedback',
            method: 'post',
            data: body,
        }),
});

// Get all feedback (admin only)
export type GetAllFeedback_Params = { status?: string };
export type GetAllFeedback_Response = UserFeedbackData[];
export const GetAllFeedback = (params?: GetAllFeedback_Params) => ({
    key: ['/feedback', params],
    service: async () =>
        ApiService('protected').fetchData<GetAllFeedback_Response>({
            url: params?.status ? `/feedback?status=${params.status}` : '/feedback',
            method: 'get',
        }),
});

// Get single feedback (admin only)
export type GetFeedback_Params = { id: string };
export type GetFeedback_Response = UserFeedbackData;
export const GetFeedback = (params: GetFeedback_Params) => ({
    key: ['/feedback', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetFeedback_Response>({
            url: `/feedback/${params.id}`,
            method: 'get',
        }),
});

// Update feedback status (admin only)
export type UpdateFeedback_Params = { id: string };
export type UpdateFeedback_Body = UpdateFeedbackRequest;
export type UpdateFeedback_Response = UserFeedbackData;
export const UpdateFeedback = () => ({
    key: ['/feedback', 'update'],
    service: async (params: UpdateFeedback_Params, body: UpdateFeedback_Body) =>
        ApiService('protected').fetchData<UpdateFeedback_Response, UpdateFeedback_Body>({
            url: `/feedback/${params.id}`,
            method: 'put',
            data: body,
        }),
});

// Delete feedback (admin only)
export type DeleteFeedback_Params = { id: string };
export type DeleteFeedback_Response = void;
export const DeleteFeedback = () => ({
    key: ['/feedback', 'delete'],
    service: async (params: DeleteFeedback_Params) =>
        ApiService('protected').fetchData<DeleteFeedback_Response>({
            url: `/feedback/${params.id}`,
            method: 'delete',
        }),
});
