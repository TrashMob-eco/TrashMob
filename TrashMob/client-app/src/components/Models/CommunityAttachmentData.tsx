import { Guid } from "guid-typescript";

class CommunityAttachmentData {
    id: string = Guid.createEmpty().toString();
    communityId: string = Guid.createEmpty().toString();
    attachmentUrl: string = "";
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default CommunityAttachmentData;