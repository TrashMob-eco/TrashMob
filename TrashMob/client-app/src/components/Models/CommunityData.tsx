import { Guid } from 'guid-typescript';

class CommunityData {
    id: string = Guid.createEmpty().toString();

    name: string = '';

    website: string = '';

    publicNotes: string = '';

    slug: string = '';

    homePageEnabled: boolean = false;

    isFeatured: boolean = false;

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

    regionType: number | null = null;

    countyName: string = '';

    boundsNorth: number | null = null;

    boundsSouth: number | null = null;

    boundsEast: number | null = null;

    boundsWest: number | null = null;

    logoUrl: string = '';

    contactEmail: string = '';

    contactPhone: string = '';

    physicalAddress: string = '';

    defaultCleanupFrequencyDays: number | null = null;

    defaultMinEventsPerYear: number | null = null;

    defaultSafetyRequirements: string = '';

    defaultAllowCoAdoption: boolean | null = null;

    partnerStatusId: number = 0;

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default CommunityData;
