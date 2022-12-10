import { Guid } from "guid-typescript";

class PartnerRequestData {
    id: string = Guid.createEmpty().toString();
    name: string = "";
    phone: string = "";
    email: string = "";
    website: string = "";
    streetAddress: string = "";
    city: string = "";
    region: string = "";
    country: string = "";
    postalCode: string = "";
    latitude: number = 0;
    longitude: number = 0;
    isBecomeAPartnerRequest: boolean = false;
    partnerRequestStatusId: number = 0;
    partnerTypeId: number = 0;
    notes: string = "";
    createdByUserId: string = Guid.EMPTY;
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = Guid.EMPTY;
    lastUpdatedDate: Date = new Date();
}

export default PartnerRequestData;