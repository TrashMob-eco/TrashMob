import { Guid } from "guid-typescript";

class WaiverData {
    id: string = Guid.createEmpty().toString();
    name: string = "";
    version: string = "";
    effectiveDate = new Date();
    waiverDurationTypeId: number = 1;
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default WaiverData;