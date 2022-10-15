import { Guid } from "guid-typescript";

class PartnerData {
    id: string = Guid.createEmpty().toString();
    name: string = "";
    website: string = "";
    publicNotes: string = "";
    privateNotes: string = "";
    partnerStatusId: number = 0;
    partnerTypeId: number = 0;
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default PartnerData;