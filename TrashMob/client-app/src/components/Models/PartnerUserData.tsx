import { Guid } from 'guid-typescript';

class PartnerUserData {
    id: string = Guid.createEmpty().toString();

    userName: string = '';

    email: string = '';
}

export default PartnerUserData;
