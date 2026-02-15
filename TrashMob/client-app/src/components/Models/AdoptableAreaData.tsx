import { Guid } from 'guid-typescript';

export type AdoptableAreaType = 'Highway' | 'Park' | 'School' | 'Trail' | 'Waterway' | 'Street' | 'Spot';
export type AdoptableAreaStatus = 'Available' | 'Adopted' | 'Unavailable';

class AdoptableAreaData {
    id: string = Guid.createEmpty().toString();

    partnerId: string = Guid.EMPTY;

    name: string = '';

    description: string = '';

    areaType: AdoptableAreaType = 'Park';

    status: AdoptableAreaStatus = 'Available';

    geoJson: string = '';

    startLatitude: number | null = null;

    startLongitude: number | null = null;

    endLatitude: number | null = null;

    endLongitude: number | null = null;

    cleanupFrequencyDays: number = 90;

    minEventsPerYear: number = 4;

    safetyRequirements: string = '';

    allowCoAdoption: boolean = false;

    isActive: boolean = true;

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default AdoptableAreaData;
