import { Guid } from "guid-typescript";

class PartnerUserData {
    id: string = Guid.createEmpty().toString();
    username: string = "";
    email: string = "";
}

export default PartnerUserData;