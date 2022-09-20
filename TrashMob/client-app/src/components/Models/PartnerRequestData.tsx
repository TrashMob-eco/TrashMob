import { Guid } from "guid-typescript";

class PartnerRequestData {
    id: string = Guid.createEmpty().toString();
    name: string = "";
    phone: string = "";
    email: string = "";
    website: string = "";
    city: string = "";
    region: string = "";
    country: string = "";
    postalCode: string = "";
    latitude: number = 0;
    longitude: number = 0;
    partnerRequestStatusId: number = 0;
    notes: string = "";
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default PartnerRequestData;