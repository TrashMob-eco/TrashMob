import { Guid } from 'guid-typescript';

class PartnerAdminInvitationData {
    id: string = Guid.createEmpty().toString();

    partnerId: string = Guid.createEmpty().toString();

    email: string = '';

    invitationStatusId: number = 0;

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default PartnerAdminInvitationData;
