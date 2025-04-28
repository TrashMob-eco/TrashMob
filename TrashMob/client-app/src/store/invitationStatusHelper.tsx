import InvitationStatusData from '../components/Models/InvitationStatusData';
import { GetInvitationStatuses } from '../services/invitations';

export function getInvitationStatus(invitationStatusList: InvitationStatusData[], invitationStatusId: any): string {
    const invitationStatus = invitationStatusList.find((et) => et.id === invitationStatusId);
    return invitationStatus ? invitationStatus.name : 'Unknown';
}

export async function getInvitationStatusAsync(invitationStatusId: any): Promise<string> {
    const invitationStatusList = await getInvitationStatuses();
    return getInvitationStatus(invitationStatusList, invitationStatusId);
}

async function getInvitationStatuses(): Promise<InvitationStatusData[]> {
    const result = await GetInvitationStatuses()
        .service()
        .then((res) => res.data)
        .catch((err) => []);
    return result;
}
