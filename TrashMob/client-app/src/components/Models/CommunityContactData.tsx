import { Guid } from "guid-typescript";

class CommunityContactData {
    id: string = Guid.createEmpty().toString();
    communityId: string = Guid.createEmpty().toString();
    communityContactTypeId: number = 0;
    name: string = "";
    email: string = "";
    phone: string = "";
    notes: string = "";
    isActive: boolean = true;
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default CommunityContactData;