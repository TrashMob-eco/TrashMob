import { Guid } from "guid-typescript";

class CommunityDocumentData {
    id: string = Guid.createEmpty().toString();
    communityId: string = Guid.createEmpty().toString();
    name: string = "";
    url: string = "";
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default CommunityDocumentData;