// Newsletter Services

import { ApiService } from '.';

// ==================== Types ====================

export interface NewsletterCategory {
    id: number;
    name: string;
    description?: string;
    isDefault: boolean;
}

export interface UserNewsletterPreference {
    categoryId: number;
    categoryName: string;
    categoryDescription?: string;
    isSubscribed: boolean;
    subscribedDate?: string;
    unsubscribedDate?: string;
}

export interface Newsletter {
    id: string;
    categoryId: number;
    categoryName?: string;
    subject: string;
    previewText?: string;
    htmlContent?: string;
    textContent?: string;
    targetType: 'All' | 'Community' | 'Team';
    targetId?: string;
    status: 'Draft' | 'Scheduled' | 'Sending' | 'Sent';
    scheduledDate?: string;
    sentDate?: string;
    recipientCount: number;
    sentCount: number;
    deliveredCount: number;
    openCount: number;
    clickCount: number;
    bounceCount: number;
    unsubscribeCount: number;
    createdDate?: string;
    lastUpdatedDate?: string;
}

export interface NewsletterTemplate {
    id: number;
    name: string;
    description?: string;
    htmlContent?: string;
    textContent?: string;
    thumbnailUrl?: string;
}

// ==================== User Preference Endpoints ====================

// Get all newsletter categories (public)
export type GetNewsletterCategories_Response = NewsletterCategory[];
export const GetNewsletterCategories = () => ({
    key: ['/newsletter-preferences/categories'],
    service: async () =>
        ApiService('public').fetchData<GetNewsletterCategories_Response>({
            url: '/newsletter-preferences/categories',
            method: 'get',
        }),
});

// Get current user's newsletter preferences
export type GetMyNewsletterPreferences_Response = UserNewsletterPreference[];
export const GetMyNewsletterPreferences = () => ({
    key: ['/newsletter-preferences'],
    service: async () =>
        ApiService('protected').fetchData<GetMyNewsletterPreferences_Response>({
            url: '/newsletter-preferences',
            method: 'get',
        }),
});

// Update a newsletter preference
export type UpdateNewsletterPreference_Body = {
    categoryId: number;
    isSubscribed: boolean;
};
export type UpdateNewsletterPreference_Response = {
    categoryId: number;
    isSubscribed: boolean;
    subscribedDate?: string;
    unsubscribedDate?: string;
};
export const UpdateNewsletterPreference = () => ({
    key: ['/newsletter-preferences', 'update'],
    service: async (body: UpdateNewsletterPreference_Body) =>
        ApiService('protected').fetchData<UpdateNewsletterPreference_Response, UpdateNewsletterPreference_Body>({
            url: '/newsletter-preferences',
            method: 'put',
            data: body,
        }),
});

// Unsubscribe from all newsletters
export type UnsubscribeAllNewsletters_Response = { message: string };
export const UnsubscribeAllNewsletters = () => ({
    key: ['/newsletter-preferences', 'unsubscribe-all'],
    service: async () =>
        ApiService('protected').fetchData<UnsubscribeAllNewsletters_Response>({
            url: '/newsletter-preferences/unsubscribe-all',
            method: 'post',
        }),
});

// ==================== Admin Newsletter Endpoints ====================

// Get all newsletters (admin only)
export type GetNewsletters_Params = { status?: string };
export type GetNewsletters_Response = Newsletter[];
export const GetNewsletters = (params?: GetNewsletters_Params) => ({
    key: ['/admin/newsletters', params],
    service: async () =>
        ApiService('protected').fetchData<GetNewsletters_Response>({
            url: params?.status ? `/admin/newsletters?status=${params.status}` : '/admin/newsletters',
            method: 'get',
        }),
});

// Get a single newsletter (admin only)
export type GetNewsletter_Params = { id: string };
export type GetNewsletter_Response = Newsletter;
export const GetNewsletter = (params: GetNewsletter_Params) => ({
    key: ['/admin/newsletters', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetNewsletter_Response>({
            url: `/admin/newsletters/${params.id}`,
            method: 'get',
        }),
});

// Create a new newsletter (admin only)
export type CreateNewsletter_Body = {
    categoryId: number;
    subject: string;
    previewText?: string;
    htmlContent?: string;
    textContent?: string;
    targetType?: string;
    targetId?: string;
};
export type CreateNewsletter_Response = Newsletter;
export const CreateNewsletter = () => ({
    key: ['/admin/newsletters', 'create'],
    service: async (body: CreateNewsletter_Body) =>
        ApiService('protected').fetchData<CreateNewsletter_Response, CreateNewsletter_Body>({
            url: '/admin/newsletters',
            method: 'post',
            data: body,
        }),
});

// Update a newsletter (admin only)
export type UpdateNewsletter_Params = { id: string };
export type UpdateNewsletter_Body = {
    categoryId?: number;
    subject?: string;
    previewText?: string;
    htmlContent?: string;
    textContent?: string;
    targetType?: string;
    targetId?: string;
};
export type UpdateNewsletter_Response = Newsletter;
export const UpdateNewsletter = () => ({
    key: ['/admin/newsletters', 'update'],
    service: async (params: UpdateNewsletter_Params, body: UpdateNewsletter_Body) =>
        ApiService('protected').fetchData<UpdateNewsletter_Response, UpdateNewsletter_Body>({
            url: `/admin/newsletters/${params.id}`,
            method: 'put',
            data: body,
        }),
});

// Schedule a newsletter (admin only)
export type ScheduleNewsletter_Params = { id: string };
export type ScheduleNewsletter_Body = { scheduledDate: string };
export type ScheduleNewsletter_Response = Newsletter;
export const ScheduleNewsletter = () => ({
    key: ['/admin/newsletters', 'schedule'],
    service: async (params: ScheduleNewsletter_Params, body: ScheduleNewsletter_Body) =>
        ApiService('protected').fetchData<ScheduleNewsletter_Response, ScheduleNewsletter_Body>({
            url: `/admin/newsletters/${params.id}/schedule`,
            method: 'post',
            data: body,
        }),
});

// Send a newsletter immediately (admin only)
export type SendNewsletter_Params = { id: string };
export type SendNewsletter_Response = { message: string; newsletter: Newsletter };
export const SendNewsletter = () => ({
    key: ['/admin/newsletters', 'send'],
    service: async (params: SendNewsletter_Params) =>
        ApiService('protected').fetchData<SendNewsletter_Response>({
            url: `/admin/newsletters/${params.id}/send`,
            method: 'post',
        }),
});

// Delete a newsletter (admin only)
export type DeleteNewsletter_Params = { id: string };
export type DeleteNewsletter_Response = { message: string };
export const DeleteNewsletter = () => ({
    key: ['/admin/newsletters', 'delete'],
    service: async (params: DeleteNewsletter_Params) =>
        ApiService('protected').fetchData<DeleteNewsletter_Response>({
            url: `/admin/newsletters/${params.id}`,
            method: 'delete',
        }),
});

// Get newsletter templates (admin only)
export type GetNewsletterTemplates_Response = NewsletterTemplate[];
export const GetNewsletterTemplates = () => ({
    key: ['/admin/newsletters/templates'],
    service: async () =>
        ApiService('protected').fetchData<GetNewsletterTemplates_Response>({
            url: '/admin/newsletters/templates',
            method: 'get',
        }),
});

// Send a test email (admin only)
export type SendTestEmail_Params = { id: string };
export type SendTestEmail_Body = { emails: string[] };
export type SendTestEmail_Response = { message: string };
export const SendTestEmail = () => ({
    key: ['/admin/newsletters', 'test'],
    service: async (params: SendTestEmail_Params, body: SendTestEmail_Body) =>
        ApiService('protected').fetchData<SendTestEmail_Response, SendTestEmail_Body>({
            url: `/admin/newsletters/${params.id}/test`,
            method: 'post',
            data: body,
        }),
});
