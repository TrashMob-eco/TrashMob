import { Guid } from 'guid-typescript';

class LitterImageData {
    id: string = Guid.createEmpty().toString();

    litterReportId: string = '';

    /**
     * V2 API returns this as `imageUrl` (camelCase from C# `ImageUrl`).
     */
    imageUrl: string = '';

    /**
     * @deprecated Use `imageUrl` instead. V1 API used `imageURL` casing.
     */
    imageURL: string = '';

    streetAddress: string = '';

    city: string = '';

    region: string = '';

    country: string = '';

    postalCode: string = '';

    latitude: number | null = null;

    longitude: number | null = null;

    createdByUserId: string = '';

    createdDate: Date | null = null;

    lastUpdatedByUserId: string = '';

    lastUpdatedDate: Date | null = null;

    // Moderation fields
    moderationStatus: number = 0; // 0=Pending, 1=Approved, 2=Rejected

    inReview: boolean = false;
}

export default LitterImageData;
