import { Guid } from "guid-typescript";

class EventData {
    id: string = Guid.createEmpty().toString();
    name: string = "";
    description: string = "";
    eventDate: Date = new Date();
    eventTypeId: number = 0;
    eventStatusId: number = 0;
    streetAddress: string = "";
    city: string = "";
    stateProvince: string = "";
    country: string = "";
    zipCode: string = "";
    createdByUserId: string = "";
    createdDate: Date = new Date();
    latitude: string = "";
    longitude: string = "";
    gpscoords: string = "";
    maxNumberOfParticipants: number = 0;
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default EventData;