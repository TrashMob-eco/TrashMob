import { Guid } from "guid-typescript";

class CommunityData {
    id: string = Guid.createEmpty().toString();
    city: string = "";
    region: string = "";
    country: string = "";
    postalCode: string = "";
    latitude: number = 0;
    longitude: number = 0;
    communityStatusId: number = 0;
    createdByUserId: string = "";
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = "";
    lastUpdatedDate: Date = new Date();
}

export default CommunityData;