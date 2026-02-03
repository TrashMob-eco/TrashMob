import { Guid } from 'guid-typescript';

export type AdoptionStatus = 'Pending' | 'Approved' | 'Rejected' | 'Revoked';

class TeamAdoptionData {
    id: string = Guid.createEmpty().toString();

    teamId: string = Guid.EMPTY;

    adoptableAreaId: string = Guid.EMPTY;

    applicationDate: Date = new Date();

    applicationNotes: string = '';

    status: AdoptionStatus = 'Pending';

    reviewedByUserId: string | null = null;

    reviewedDate: Date | null = null;

    rejectionReason: string = '';

    // Compliance tracking fields (Phase 3)
    adoptionStartDate: string | null = null;

    adoptionEndDate: string | null = null;

    lastEventDate: Date | null = null;

    eventCount: number = 0;

    isCompliant: boolean = false;

    // Navigation properties (populated by API when included)
    team?: {
        id: string;
        name: string;
        logoUrl?: string;
    };

    adoptableArea?: {
        id: string;
        name: string;
        areaType: string;
        partnerId: string;
        cleanupFrequencyDays: number;
        minEventsPerYear: number;
        safetyRequirements?: string;
        partner?: {
            id: string;
            name: string;
        };
    };

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default TeamAdoptionData;
