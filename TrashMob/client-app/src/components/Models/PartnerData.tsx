import { Guid } from "guid-typescript";

class PartnerData {
    id: string = Guid.createEmpty().toString();
    name: string = "";
    website: string = "";
    streetAddress: string = "";
    city: string = "";
    region: string = "";
    country: string = "";
    postalCode: string = "";
    latitude: number = 0;
    longitude: number = 0;
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