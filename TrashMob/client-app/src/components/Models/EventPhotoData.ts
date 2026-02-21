import { Guid } from 'guid-typescript';

export type EventPhotoType = 'Before' | 'During' | 'After';

class EventPhotoData {
    id: string = Guid.createEmpty().toString();

    eventId: string = Guid.createEmpty().toString();

    uploadedByUserId: string = Guid.EMPTY;

    imageUrl: string = '';

    thumbnailUrl: string = '';

    photoType: number = 1; // 0=Before, 1=During, 2=After

    caption: string = '';

    takenAt?: Date;

    uploadedDate: Date = new Date();

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();

    // Moderation fields
    moderationStatus: number = 0; // 0=Pending, 1=Approved, 2=Rejected

    inReview: boolean = false;

    reviewRequestedByUserId?: string;

    reviewRequestedDate?: Date;

    moderatedByUserId?: string;

    moderatedDate?: Date;

    moderationReason?: string;
}

export const EventPhotoTypeLabels: Record<number, string> = {
    0: 'Before',
    1: 'During',
    2: 'After',
};

export const getEventPhotoTypeLabel = (type: number): string => {
    return EventPhotoTypeLabels[type] || 'Unknown';
};

export default EventPhotoData;
