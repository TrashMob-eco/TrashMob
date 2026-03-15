import { Guid } from 'guid-typescript';

class EventAttendeeData {
    eventId: string = Guid.createEmpty().toString();

    userId: string = Guid.createEmpty().toString();

    userName: string = '';

    givenName: string = '';

    profilePhotoUrl: string = '';

    signUpDate: Date = new Date();

    isEventLead: boolean = false;

    // Fields from UserData for table compatibility (v2 attendees don't include these)
    city: string = '';

    country: string = '';

    memberSince: Date | null = null;

    isMinor: boolean = false;

    // Alias for compatibility — components reference user.id
    get id(): string {
        return this.userId;
    }
}

export default EventAttendeeData;
