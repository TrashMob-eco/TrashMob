import { Guid } from 'guid-typescript';

export type SponsoredAdoptionStatus = 'Active' | 'Expired' | 'Terminated';

class SponsoredAdoptionData {
    id: string = Guid.createEmpty().toString();

    adoptableAreaId: string = Guid.EMPTY;

    sponsorId: string = Guid.EMPTY;

    professionalCompanyId: string = Guid.EMPTY;

    startDate: string = '';

    endDate: string | null = null;

    cleanupFrequencyDays: number = 14;

    status: SponsoredAdoptionStatus = 'Active';

    adoptableArea?: {
        id: string;
        name: string;
        areaType: string;
    };

    sponsor?: {
        id: string;
        name: string;
        logoUrl?: string;
    };

    professionalCompany?: {
        id: string;
        name: string;
    };

    cleanupLogs?: {
        cleanupDate: string;
    }[];

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default SponsoredAdoptionData;
