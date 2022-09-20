import { Guid } from "guid-typescript";

class PartnerNoteData {
    id: string = Guid.createEmpty().toString();
    partnerId: string = Guid.createEmpty().toString();
    notes: string = "";
    isPublic: boolean = true;
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default PartnerNoteData;