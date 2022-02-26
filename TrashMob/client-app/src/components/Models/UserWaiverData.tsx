import { Guid } from "guid-typescript";

class UserWaiverData {
    userId: string = Guid.createEmpty().toString();
    waiverId: string = Guid.createEmpty().toString();
    version: string = "";
    effectiveDate = new Date();
    expiryDate: Date = new Date();
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default UserWaiverData;