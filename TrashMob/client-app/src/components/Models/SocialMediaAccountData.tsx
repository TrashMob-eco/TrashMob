import { Guid } from "guid-typescript";

class SocialMediaAccountData {
    id: string = Guid.createEmpty().toString();
    socialMediaAccountTypeId: number = 0;
    accountIdentifier: string = "";
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default SocialMediaAccountData;