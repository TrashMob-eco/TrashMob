import { Guid } from 'guid-typescript';

class TeamData {
    id: string = Guid.createEmpty().toString();

    name: string = '';

    description: string = '';

    logoUrl: string = '';

    isPublic: boolean = true;

    requiresApproval: boolean = false;

    latitude: number | null = null;

    longitude: number | null = null;

    city: string = '';

    region: string = '';

    country: string = '';

    postalCode: string = '';

    isActive: boolean = true;

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default TeamData;
