import { Guid } from 'guid-typescript';

class EventAttendeeData {
    eventId: string = Guid.createEmpty().toString();

    userId: string = Guid.createEmpty().toString();
}

export default EventAttendeeData;
