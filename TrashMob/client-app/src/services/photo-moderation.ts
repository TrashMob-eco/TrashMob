// Photo Moderation Service

import { ApiService } from '.';

export type PhotoType = 'LitterImage' | 'TeamPhoto';
export type ModerationStatus = 'Pending' | 'Approved' | 'Rejected';

export interface PhotoModerationItem {
    photoId: string;
    photoType: PhotoType;
    imageUrl: string;
    moderationStatus: number;
    inReview: boolean;
    flaggedDate?: string;
    flagReason?: string;
    uploadedDate: string;
    uploadedByUserId: string;
    uploaderName?: string;
    uploaderEmail?: string;
    litterReportId?: string;
    litterReportName?: string;
    teamId?: string;
    teamName?: string;
    caption?: string;
    moderatedDate?: string;
    moderatedByName?: string;
    moderationReason?: string;
}

export interface PaginatedResult<T> {
    items: T[];
    pageIndex: number;
    totalPages: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
}

export interface RejectPhotoRequest {
    reason: string;
}

export interface FlagPhotoRequest {
    reason: string;
}

export interface PhotoFlag {
    id: string;
    photoId: string;
    photoType: string;
    flaggedByUserId: string;
    flagReason: string;
    flaggedDate: string;
    resolvedDate?: string;
    resolvedByUserId?: string;
    resolution?: string;
}

// Get pending photos (admin only)
export type GetPendingPhotos_Params = { page?: number; pageSize?: number };
export type GetPendingPhotos_Response = PhotoModerationItem[];
export const GetPendingPhotos = (params?: GetPendingPhotos_Params) => ({
    key: ['/admin/photos/pending', params],
    service: async () =>
        ApiService('protected').fetchData<GetPendingPhotos_Response>({
            url: `/admin/photos/pending?page=${params?.page ?? 1}&pageSize=${params?.pageSize ?? 50}`,
            method: 'get',
        }),
});

// Get flagged photos (admin only)
export type GetFlaggedPhotos_Params = { page?: number; pageSize?: number };
export type GetFlaggedPhotos_Response = PhotoModerationItem[];
export const GetFlaggedPhotos = (params?: GetFlaggedPhotos_Params) => ({
    key: ['/admin/photos/flagged', params],
    service: async () =>
        ApiService('protected').fetchData<GetFlaggedPhotos_Response>({
            url: `/admin/photos/flagged?page=${params?.page ?? 1}&pageSize=${params?.pageSize ?? 50}`,
            method: 'get',
        }),
});

// Get recently moderated photos (admin only)
export type GetModeratedPhotos_Params = { page?: number; pageSize?: number };
export type GetModeratedPhotos_Response = PhotoModerationItem[];
export const GetModeratedPhotos = (params?: GetModeratedPhotos_Params) => ({
    key: ['/admin/photos/moderated', params],
    service: async () =>
        ApiService('protected').fetchData<GetModeratedPhotos_Response>({
            url: `/admin/photos/moderated?page=${params?.page ?? 1}&pageSize=${params?.pageSize ?? 50}`,
            method: 'get',
        }),
});

// Approve photo (admin only)
export type ApprovePhoto_Params = { photoType: PhotoType; id: string };
export type ApprovePhoto_Response = PhotoModerationItem;
export const ApprovePhoto = () => ({
    key: ['/admin/photos', 'approve'],
    service: async (params: ApprovePhoto_Params) =>
        ApiService('protected').fetchData<ApprovePhoto_Response>({
            url: `/admin/photos/${params.photoType}/${params.id}/approve`,
            method: 'post',
        }),
});

// Reject photo (admin only)
export type RejectPhoto_Params = { photoType: PhotoType; id: string };
export type RejectPhoto_Body = RejectPhotoRequest;
export type RejectPhoto_Response = PhotoModerationItem;
export const RejectPhoto = () => ({
    key: ['/admin/photos', 'reject'],
    service: async (params: RejectPhoto_Params, body: RejectPhoto_Body) =>
        ApiService('protected').fetchData<RejectPhoto_Response, RejectPhoto_Body>({
            url: `/admin/photos/${params.photoType}/${params.id}/reject`,
            method: 'post',
            data: body,
        }),
});

// Dismiss flag (admin only)
export type DismissFlag_Params = { photoType: PhotoType; id: string };
export type DismissFlag_Response = PhotoModerationItem;
export const DismissFlag = () => ({
    key: ['/admin/photos', 'dismiss'],
    service: async (params: DismissFlag_Params) =>
        ApiService('protected').fetchData<DismissFlag_Response>({
            url: `/admin/photos/${params.photoType}/${params.id}/dismiss`,
            method: 'post',
        }),
});

// Flag photo (any authenticated user)
export type FlagPhoto_Params = { photoType: PhotoType; id: string };
export type FlagPhoto_Body = FlagPhotoRequest;
export type FlagPhoto_Response = PhotoFlag;
export const FlagPhoto = () => ({
    key: ['/photos', 'flag'],
    service: async (params: FlagPhoto_Params, body: FlagPhoto_Body) =>
        ApiService('protected').fetchData<FlagPhoto_Response, FlagPhoto_Body>({
            url: `/photos/${params.photoType}/${params.id}/flag`,
            method: 'post',
            data: body,
        }),
});
