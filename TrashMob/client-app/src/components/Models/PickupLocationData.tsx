import { Guid } from "guid-typescript";

class PickupLocationData {
    id: string = Guid.createEmpty().toString();
    eventId: string = Guid.createEmpty().toString();
    streetAddress: string = "";
    city: string = "";
    region: string = "";
    country: string = "";
    latitude: number = 0;
    longitude: number = 0;
    postalCode: string = "";
    notes: string = "";
    hasBeenSubmitted: boolean = false;
    hasBeenPickedUp: boolean = true;
    createdByUserId: string = Guid.EMPTY;
    createdDate: Date = new Date();
    lastUpdatedByUserId: string = Guid.EMPTY;
    lastUpdatedDate: Date = new Date();
}

export default PickupLocationData;