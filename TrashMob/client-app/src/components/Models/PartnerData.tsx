import { Guid } from 'guid-typescript';

class PartnerData {
    id: string = Guid.createEmpty().toString();

    name: string = '';

    website: string = '';

    publicNotes: string = '';

    privateNotes: string = '';

    partnerStatusId: number = 0;

    partnerTypeId: number = 0;

    // Community Home Page Properties
    slug: string = '';

    homePageEnabled: boolean = false;

    homePageStartDate: Date | null = null;

    homePageEndDate: Date | null = null;

    brandingPrimaryColor: string = '#3B82F6';

    brandingSecondaryColor: string = '#1E40AF';

    bannerImageUrl: string = '';

    tagline: string = '';

    latitude: number | null = null;

    longitude: number | null = null;

    city: string = '';

    region: string = '';

    country: string = '';

    logoUrl: string = '';

    contactEmail: string = '';

    contactPhone: string = '';

    physicalAddress: string = '';

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default PartnerData;
