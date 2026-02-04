/**
 * Represents a photo in a partner/community photo gallery.
 */
export default interface PartnerPhotoData {
    id: string;
    partnerId: string;
    imageUrl: string;
    caption: string;
    uploadedByUserId: string;
    uploadedDate: string;
    moderationStatus: number;
    inReview: boolean;
    reviewRequestedByUserId?: string;
    reviewRequestedDate?: string;
    reviewedByUserId?: string;
    reviewedDate?: string;
    moderationNotes?: string;
    createdByUserId: string;
    createdDate: string;
    lastUpdatedByUserId: string;
    lastUpdatedDate: string;
}
