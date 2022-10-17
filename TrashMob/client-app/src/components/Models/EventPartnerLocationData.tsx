import { Guid } from "guid-typescript";

class EventPartnerLocationData {
    eventId: string = Guid.createEmpty().toString();
    partnerLocationId: string = Guid.createEmpty().toString();
    eventPartnerLocationStatusId: number = 0;
}

export default EventPartnerLocationData;