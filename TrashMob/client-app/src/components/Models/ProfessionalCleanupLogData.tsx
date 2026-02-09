import { Guid } from 'guid-typescript';

class ProfessionalCleanupLogData {
    id: string = Guid.createEmpty().toString();

    sponsoredAdoptionId: string = Guid.EMPTY;

    professionalCompanyId: string = Guid.EMPTY;

    cleanupDate: string = '';

    durationMinutes: number = 0;

    bagsCollected: number = 0;

    weightInPounds: number | null = null;

    weightInKilograms: number | null = null;

    notes: string = '';

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default ProfessionalCleanupLogData;
