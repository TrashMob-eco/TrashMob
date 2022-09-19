import { Guid } from "guid-typescript";

class PartnerSocialMediaAccountData {
    id: string = Guid.createEmpty().toString();
    partnerId: string = Guid.createEmpty().toString();
    socialMediaAccountId: string = Guid.createEmpty().toString();
    isActive: boolean = true;
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default PartnerSocialMediaAccountData;