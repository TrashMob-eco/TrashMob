import { Guid } from "guid-typescript";

class CommunityUserData {
    id: string = Guid.createEmpty().toString();
    communityId: string = Guid.createEmpty().toString();
    userId: string = Guid.createEmpty().toString();
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default CommunityUserData;