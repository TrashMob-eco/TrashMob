import { Guid } from "guid-typescript";

class CommunityNoteData {
    id: string = Guid.createEmpty().toString();
    communityId: string = Guid.createEmpty().toString();
    communityContactTypeId: number = 0;
    notes: string = "";
    isPublic: boolean = true;
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default CommunityNoteData;