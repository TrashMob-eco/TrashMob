import { Guid } from 'guid-typescript';

class EventData {
    id: string = Guid.createEmpty().toString();

    name: string = '';

    description: string = '';

    eventDate: Date = new Date();

    durationHours: number = 1;

    durationMinutes: number = 0;

    eventTypeId: number = 0;

    eventStatusId: number = 0;

    streetAddress: string = '';

    city: string = '';

    region: string = '';

    country: string = '';

    postalCode: string = '';

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    latitude: number | null = null;

    longitude: number | null = null;

    maxNumberOfParticipants: number = 0;

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();

    isEventPublic: boolean = true;

    eventVisibilityId: number = 1;

    teamId: string | null = null;

    createdByUserName: string = '';

    isAttending?: boolean;
}

export default EventData;
