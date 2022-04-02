import { Guid } from "guid-typescript";

class EventData {
    id: string = Guid.createEmpty().toString();
    name: string = "";
    description: string = "";
    eventDate: Date = new Date();
    durationHours: number = 1;
    durationMinutes: number = 0;
    eventTypeId: number = 0;
    eventStatusId: number = 0;
    streetAddress: string = "";
    city: string = "";
    region: string = "";
    country: string = "";
    postalCode: string = "";
    createdByUserId: string = "";
    createdDate: Date = new Date();
    latitude: number = 0;
    longitude: number = 0;
    maxNumberOfParticipants: number = 0;
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
    isEventPublic: boolean = true;
    createdByUserName: string = "";
}

export default EventData;