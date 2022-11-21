import InvitationStatusData from "../components/Models/InvitationStatusData";
import { getDefaultHeaders } from "./AuthStore";

export function getInvitationStatus(invitationStatusList: InvitationStatusData[], invitationStatusId: any): string {
    if (invitationStatusList === null || invitationStatusList.length === 0) {
        invitationStatusList = getInvitationStatuses();
    }

    var invitationStatus = invitationStatusList.find(et => et.id === invitationStatusId)
    if (invitationStatus)
        return invitationStatus.name;
    return "Unknown";
}

function getInvitationStatuses(): InvitationStatusData[] {
    const headers = getDefaultHeaders('GET');

    fetch('/api/invitationStatuses', {
        method: 'GET',
        headers: headers
    })
        .then(response => response.json() as Promise<Array<any>>)
        .then(data => {
            return data;
        });
    return [];
}
