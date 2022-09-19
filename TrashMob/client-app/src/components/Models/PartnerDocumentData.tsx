import { Guid } from "guid-typescript";

class PartnerDocumentData {
    id: string = Guid.createEmpty().toString();
    partnerId: string = Guid.createEmpty().toString();
    name: string = "";
    url: string = "";
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default PartnerDocumentData;