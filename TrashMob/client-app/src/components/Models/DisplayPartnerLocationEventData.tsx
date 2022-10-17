import { Guid } from "guid-typescript";

class DisplayPartnerLocationEventData {
    eventId: string = Guid.createEmpty().toString();
    partnerLocationId: string = Guid.createEmpty().toString();
    eventPartnerLocationStatusId: number = 0;
    partnerName: string = "";
    partnerLocationName: string = "";
    eventName: string = "";
    eventDescription: string = "";
    eventDate: Date = new Date();
    eventStreetAddress: string = "";
    eventCity: string = "";
    eventRegion: string = "";
    eventCountry: string = "";
    eventPostalCode: string = "";
}

export default DisplayPartnerLocationEventData;