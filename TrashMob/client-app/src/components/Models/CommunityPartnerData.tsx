import { Guid } from "guid-typescript";

class CommunityPartnerData {
    id: string = Guid.createEmpty().toString();
    communityId: string = Guid.createEmpty().toString();
    partnerId: string = Guid.createEmpty().toString();
    partnerLocationId: string = Guid.createEmpty().toString();
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default CommunityPartnerData;