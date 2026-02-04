// Event Photos API service

import { ApiService } from '.';
import EventPhotoData from '../components/Models/EventPhotoData';

// ============================================================================
// Get Event Photos
// ============================================================================

export type GetEventPhotos_Params = {
    eventId: string;
    photoType?: number;
};
export type GetEventPhotos_Response = EventPhotoData[];
export const GetEventPhotos = (params: GetEventPhotos_Params) => ({
    key: ['/events', params.eventId, 'photos', params.photoType],
    service: async () => {
        const queryParams = new URLSearchParams();
        if (params.photoType !== undefined) {
            queryParams.append('photoType', params.photoType.toString());
        }
        const queryString = queryParams.toString();
        return ApiService('public').fetchData<GetEventPhotos_Response>({
            url: `/events/${params.eventId}/photos${queryString ? `?${queryString}` : ''}`,
            method: 'get',
        });
    },
});

// ============================================================================
// Get Single Photo
// ============================================================================

export type GetEventPhoto_Params = { eventId: string; photoId: string };
export type GetEventPhoto_Response = EventPhotoData;
export const GetEventPhoto = (params: GetEventPhoto_Params) => ({
    key: ['/events', params.eventId, 'photos', params.photoId],
    service: async () =>
        ApiService('public').fetchData<GetEventPhoto_Response>({
            url: `/events/${params.eventId}/photos/${params.photoId}`,
            method: 'get',
        }),
});

// ============================================================================
// Upload Photo
// ============================================================================

export type UploadEventPhoto_Params = { eventId: string };
export type UploadEventPhoto_Response = EventPhotoData;
export const UploadEventPhoto = () => ({
    key: ['/events/photos', 'upload'],
    service: async (params: UploadEventPhoto_Params, file: File) => {
        const formData = new FormData();
        formData.append('formFile', file);
        return ApiService('protected').fetchData<UploadEventPhoto_Response>({
            url: `/events/${params.eventId}/photos`,
            method: 'post',
            data: formData,
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
    },
});

// ============================================================================
// Update Photo
// ============================================================================

export type UpdateEventPhoto_Params = { eventId: string; photoId: string };
export type UpdateEventPhoto_Body = {
    caption: string;
    photoType: number;
};
export type UpdateEventPhoto_Response = EventPhotoData;
export const UpdateEventPhoto = () => ({
    key: ['/events/photos', 'update'],
    service: async (params: UpdateEventPhoto_Params, body: UpdateEventPhoto_Body) =>
        ApiService('protected').fetchData<UpdateEventPhoto_Response, UpdateEventPhoto_Body>({
            url: `/events/${params.eventId}/photos/${params.photoId}`,
            method: 'put',
            data: body,
        }),
});

// ============================================================================
// Delete Photo
// ============================================================================

export type DeleteEventPhoto_Params = { eventId: string; photoId: string };
export type DeleteEventPhoto_Response = void;
export const DeleteEventPhoto = () => ({
    key: ['/events/photos', 'delete'],
    service: async (params: DeleteEventPhoto_Params) =>
        ApiService('protected').fetchData<DeleteEventPhoto_Response>({
            url: `/events/${params.eventId}/photos/${params.photoId}`,
            method: 'delete',
        }),
});

// ============================================================================
// Flag Photo
// ============================================================================

export type FlagEventPhoto_Params = { eventId: string; photoId: string };
export type FlagEventPhoto_Body = { reason: string };
export type FlagEventPhoto_Response = { message: string };
export const FlagEventPhoto = () => ({
    key: ['/events/photos', 'flag'],
    service: async (params: FlagEventPhoto_Params, body: FlagEventPhoto_Body) =>
        ApiService('protected').fetchData<FlagEventPhoto_Response, FlagEventPhoto_Body>({
            url: `/events/${params.eventId}/photos/${params.photoId}/flag`,
            method: 'post',
            data: body,
        }),
});
