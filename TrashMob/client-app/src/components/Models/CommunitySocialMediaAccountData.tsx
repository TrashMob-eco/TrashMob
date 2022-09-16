import { Guid } from "guid-typescript";

class CommunitySocialMediaAccountData {
    id: string = Guid.createEmpty().toString();
    communityId: string = Guid.createEmpty().toString();
    socialMediaAccountId: string = Guid.createEmpty().toString();
    isActive: boolean = true;
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default CommunitySocialMediaAccountData;