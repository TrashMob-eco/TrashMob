import { Guid } from 'guid-typescript';

class PartnerLocationServiceData {
    partnerLocationId: string = Guid.createEmpty().toString();

    serviceTypeId: number = 0;

    notes: string = '';

    isAutoApproved: boolean = false;

    isAdvanceNoticeRequired: boolean = true;

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default PartnerLocationServiceData;
