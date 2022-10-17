import { Guid } from "guid-typescript";

class PartnerLocationContactData {
    id: string = Guid.createEmpty().toString();
    partnerLocationId: string = Guid.createEmpty().toString();
    name: string = "";
    email: string = "";
    phone: string = "";
    notes: string = "";
    isActive: boolean = true;
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default PartnerLocationContactData;