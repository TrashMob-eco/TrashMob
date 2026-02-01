import { Guid } from 'guid-typescript';

class TeamEventData {
    id: string = Guid.createEmpty().toString();

    teamId: string = Guid.createEmpty().toString();

    eventId: string = Guid.createEmpty().toString();

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default TeamEventData;
