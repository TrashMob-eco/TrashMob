import { Guid } from "guid-typescript";

class EventMediaData {
    id: string = Guid.createEmpty().toString();
    mediaUrl: string = "";
    mediaTypeId: number = 0;
    mediaUsageTypeId: number = 0;
    eventStatusId: number = 0;
    eventId: string = Guid.createEmpty().toString();
    createdByUserId: string = Guid.createEmpty().toString();
    createdDate: Date = new Date();
    lastUpdatedDate: Date = new Date();
}

export default EventMediaData;