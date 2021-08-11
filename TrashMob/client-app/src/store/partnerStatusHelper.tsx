import PartnerStatusData from "../components/Models/PartnerStatusData";
import { getDefaultHeaders } from "./AuthStore";

export function getPartnerStatus(partnerStatusList: PartnerStatusData[], partnerStatusId: any): string {
    if (partnerStatusList === null || partnerStatusList.length === 0) {
        partnerStatusList = getPartnerStatuses();
    }

    var partnerStatus = partnerStatusList.find(et => et.id === partnerStatusId)
    if (partnerStatus)
        return partnerStatus.name;
    return "Unknown";
}

function getPartnerStatuses(): PartnerStatusData[] {
    const headers = getDefaultHeaders('GET');

    fetch('/api/partnerStatuses', {
        method: 'GET',
        headers: headers
    })
        .then(response => response.json() as Promise<Array<any>>)
        .then(data => {
            return data;
        });
    return [];
}
