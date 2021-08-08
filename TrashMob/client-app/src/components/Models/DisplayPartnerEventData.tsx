import { Guid } from "guid-typescript";

class DisplayEventPartnerData {
    eventId: string = Guid.createEmpty().toString();
    partnerId: string = Guid.createEmpty().toString();
    partnerLocationId: string = Guid.createEmpty().toString();
    eventPartnerStatusId: number = 0;
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

export default DisplayEventPartnerData;