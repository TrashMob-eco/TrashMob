import { Guid } from "guid-typescript";

class DisplayEventPartnerLocationServiceData {
    eventId: string = Guid.createEmpty().toString();
    partnerLocationId: string = Guid.createEmpty().toString();
    serviceTypeId: number = 0;
    isAdvanceNoticeRequired: boolean = true;
    eventPartnerLocationServiceStatusId: number = 0;
    partnerName: string = "";
    partnerLocationName: string = "";
    partnerLocationServicePublicNotes: string = "";
}

export default DisplayEventPartnerLocationServiceData;