import { Guid } from "guid-typescript";

class EventSummaryData {
    eventId: string = Guid.createEmpty().toString();
    actualNumberOfAttendees: number = 0;
    numberOfBags: number = 0;
    numberOfBuckets: number = 0;
    durationInMinutes: number = 0;
    notes: string = "";
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default EventSummaryData;