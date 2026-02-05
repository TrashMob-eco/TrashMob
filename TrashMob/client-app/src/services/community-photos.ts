// Community Photos API service

import { ApiService } from '.';
import PartnerPhotoData from '../components/Models/PartnerPhotoData';

// ============================================================================
// Get Community Photos
// ============================================================================

export type GetCommunityPhotos_Params = {
    slug: string;
};
export type GetCommunityPhotos_Response = PartnerPhotoData[];
export const GetCommunityPhotos = (params: GetCommunityPhotos_Params) => ({
    key: ['/communities', params.slug, 'photos'],
    service: async () =>
        ApiService('public').fetchData<GetCommunityPhotos_Response>({
            url: `/communities/${params.slug}/photos`,
            method: 'get',
        }),
});

// ============================================================================
// Upload Photo
// ============================================================================

export type UploadCommunityPhoto_Params = { slug: string };
export type UploadCommunityPhoto_Response = PartnerPhotoData;
export const UploadCommunityPhoto = () => ({
    key: ['/communities/photos', 'upload'],
    service: async (params: UploadCommunityPhoto_Params, file: File) => {
        const formData = new FormData();
        formData.append('formFile', file);
        return ApiService('protected').fetchData<UploadCommunityPhoto_Response>({
            url: `/communities/${params.slug}/photos`,
            method: 'post',
            data: formData,
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
    },
});

// ============================================================================
// Update Photo Caption
// ============================================================================

export type UpdateCommunityPhoto_Params = { slug: string; photoId: string };
export type UpdateCommunityPhoto_Response = PartnerPhotoData;
export const UpdateCommunityPhotoCaption = () => ({
    key: ['/communities/photos', 'update'],
    service: async (params: UpdateCommunityPhoto_Params, caption: string) =>
        ApiService('protected').fetchData<UpdateCommunityPhoto_Response, string>({
            url: `/communities/${params.slug}/photos/${params.photoId}`,
            method: 'put',
            data: caption,
        }),
});

// ============================================================================
// Delete Photo
// ============================================================================

export type DeleteCommunityPhoto_Params = { slug: string; photoId: string };
export type DeleteCommunityPhoto_Response = void;
export const DeleteCommunityPhoto = () => ({
    key: ['/communities/photos', 'delete'],
    service: async (params: DeleteCommunityPhoto_Params) =>
        ApiService('protected').fetchData<DeleteCommunityPhoto_Response>({
            url: `/communities/${params.slug}/photos/${params.photoId}`,
            method: 'delete',
        }),
});
