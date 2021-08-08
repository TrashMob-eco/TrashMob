import PartnerRequestStatusData from "../components/Models/PartnerRequestStatusData";
import { getDefaultHeaders } from "./AuthStore";

export function getPartnerRequestStatus(partnerRequestStatusList: PartnerRequestStatusData[], partnerRequestStatusId: any): string {
    if (partnerRequestStatusList === null || partnerRequestStatusList.length === 0) {
        partnerRequestStatusList = getPartnerRequestStatuses();
    }

    var partnerRequestStatus = partnerRequestStatusList.find(et => et.id === partnerRequestStatusId)
    if (partnerRequestStatus)
        return partnerRequestStatus.name;
    return "Unknown";
}

function getPartnerRequestStatuses(): PartnerRequestStatusData[] {
    const headers = getDefaultHeaders('GET');

    fetch('/api/partnerRequestStatuses', {
        method: 'GET',
        headers: headers
    })
        .then(response => response.json() as Promise<Array<any>>)
        .then(data => {
            return data;
        });
    return [];
}
