import { Guid } from "guid-typescript";

class DisplayPartnershipData {
    id: string = Guid.createEmpty().toString();
    name: string = "";
    city: string = "";
    region: string = "";
    country: string = "";
    partnerRequestStatusId: number = 0;
    partnerStatusId: number = 0;
}

export default DisplayPartnershipData;