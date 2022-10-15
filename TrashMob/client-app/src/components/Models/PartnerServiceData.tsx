import { Guid } from "guid-typescript";

class PartnerServiceData {
    partnerId: string = Guid.createEmpty().toString();
    serviceTypeId: number = 0;
    notes: string = "";
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default PartnerServiceData;