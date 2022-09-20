import { Guid } from "guid-typescript";

class PartnerSocialMediaAccountData {
    id: string = Guid.createEmpty().toString();
    partnerId: string = Guid.createEmpty().toString();
    socialMediaAccountTypeId: number = 0;
    accountIdentifier: string = "";
    isActive: boolean = true;
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default PartnerSocialMediaAccountData;