import { Guid } from "guid-typescript";

class PartnerLocationData {
    id: string = Guid.createEmpty().toString();
    partnerId: string = Guid.createEmpty().toString();
    name: string = "";
    streetAddress: string = "";
    city: string = "";
    region: string = "";
    country: string = "";
    latitude: number = 0;
    longitude: number = 0;
    postalCode: string = "";
    primaryPhone: string = "";
    secondaryPhone: string = "";
    primaryEmail: string = "";
    secondaryEmail: string = "";
    notes: string = "";
    isActive: boolean = true;
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default PartnerLocationData;