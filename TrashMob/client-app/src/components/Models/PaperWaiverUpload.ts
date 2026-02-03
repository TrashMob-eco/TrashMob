/**
 * Request model for uploading a paper waiver.
 */
export interface PaperWaiverUploadRequest {
    formFile: File;
    userId: string;
    waiverVersionId: string;
    signerName: string;
    dateSigned: string;
    eventId?: string;
    isMinor?: boolean;
    guardianName?: string;
    guardianRelationship?: string;
}

/**
 * Represents the waiver status for an event attendee.
 */
export interface AttendeeWaiverStatus {
    userId: string;
    userName: string;
    hasValidWaiver: boolean;
}
