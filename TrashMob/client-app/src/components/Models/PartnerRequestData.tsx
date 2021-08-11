import { Guid } from "guid-typescript";

class PartnerRequestData {
    id: string = Guid.createEmpty().toString();
    name: string = "";
    primaryPhone: string = "";
    secondaryPhone: string = "";
    primaryEmail: string = "";
    secondaryEmail: string = "";
    partnerRequestStatusId: number = 0;
    notes: string = "";
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default PartnerRequestData;