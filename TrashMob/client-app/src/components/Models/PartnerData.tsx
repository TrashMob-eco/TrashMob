import { Guid } from 'guid-typescript';

class PartnerData {
    id: string = Guid.createEmpty().toString();

    name: string = '';

    website: string = '';

    publicNotes: string = '';

    privateNotes: string = '';

    partnerStatusId: number = 0;

    partnerTypeId: number = 0;

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default PartnerData;
