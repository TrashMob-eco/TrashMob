import { Guid } from "guid-typescript";

class PartnerData {
    id: string = Guid.createEmpty().toString();
    name: string = "";
    city: string = "";
    region: string = "";
    country: string = "";
    postalCode: string = "";
    latitude: number = 0;
    longitude: number = 0;
    partnerStatusId: number = 0;
    isActive: boolean = true;
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default PartnerData;