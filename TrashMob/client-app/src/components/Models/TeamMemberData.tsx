import { Guid } from 'guid-typescript';

class TeamMemberData {
    id: string = Guid.createEmpty().toString();

    teamId: string = Guid.createEmpty().toString();

    userId: string = Guid.createEmpty().toString();

    isTeamLead: boolean = false;

    joinedDate: Date = new Date();

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();

    // Optional user info for display purposes
    userName?: string;
}

export default TeamMemberData;
