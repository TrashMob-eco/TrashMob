import { Guid } from 'guid-typescript';

class SponsorData {
    id: string = Guid.createEmpty().toString();

    name: string = '';

    contactEmail: string = '';

    contactPhone: string = '';

    logoUrl: string = '';

    partnerId: string = Guid.EMPTY;

    isActive: boolean = true;

    showOnPublicMap: boolean = true;

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default SponsorData;
