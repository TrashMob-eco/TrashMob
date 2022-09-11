import { Guid } from "guid-typescript";

class CommunityRequestData {
    id: string = Guid.createEmpty().toString();
    email: string = "";
    contactName: string = "";
    phone: string = "";
    website: string = "";
    city: string = "";
    region: string = "";
    country: string = "";
    postalCode: string = "";
    latitude: number = 0;
    longitude: number = 0;
}

export default CommunityRequestData;