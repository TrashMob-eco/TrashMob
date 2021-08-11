import { Guid } from "guid-typescript";

class PartnerData {
    id: string = Guid.createEmpty().toString();
    name: string = "";
    primaryPhone: string = "";
    secondaryPhone: string = "";
    primaryEmail: string = "";
    secondaryEmail: string = "";
    partnerStatusId: number = 0;
    notes: string = "";
    isActive: boolean = true;
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default PartnerData;