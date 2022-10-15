import { Guid } from "guid-typescript";

class EventPartnerLocationData {
    eventId: string = Guid.createEmpty().toString();
    partnerId: string = Guid.createEmpty().toString();
    partnerLocationId: string = Guid.createEmpty().toString();
    eventPartnerStatusId: number = 0;
}

export default EventPartnerLocationData;