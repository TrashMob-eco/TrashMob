import { Guid } from 'guid-typescript';

class JobOpportunityData {
    id: string = Guid.createEmpty().toString();

    title: string = '';

    tagLine: string = '';

    fullDescription: string = '';

    isActive: boolean = false;

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default JobOpportunityData;
