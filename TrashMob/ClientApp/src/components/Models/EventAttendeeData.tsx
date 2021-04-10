import { Guid } from "guid-typescript";

class EventAttendeeData {
    eventId: string = Guid.createEmpty().toString();
    attendeeId: string = Guid.createEmpty().toString();
}

export default EventAttendeeData;