import { Guid } from 'guid-typescript';

class JobOpportunityData {
    id: string = Guid.createEmpty().toString();

    title: string = '';

    tagLine: string = '';

    fullDescription: string = '';

    isActive: boolean = true;

    createdByUserId: string = '';

    createdDate: Date | null = null;

    lastUpdatedByUserId: string = '';

    lastUpdatedDate: Date | null = null;
}

export default JobOpportunityData;
