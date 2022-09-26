import PartnerRequestStatusData from "../components/Models/PartnerRequestStatusData";
import PartnerStatusData from "../components/Models/PartnerStatusData";
import { getDefaultHeaders } from "./AuthStore";

export function getDisplayPartnershipStatus(partnerStatusList: PartnerStatusData[], partnerRequestStatusList: PartnerRequestStatusData[], partnerStatusId: any, partnerRequestStatusId: any): string {
    if (partnerStatusList === null || partnerStatusList.length === 0) {
        partnerStatusList = getPartnerStatuses();
    }

    if (partnerRequestStatusList === null || partnerRequestStatusList.length === 0) {
        partnerRequestStatusList = getPartnerRequestStatuses();
    }

    var partnerStatus = partnerStatusList.find(et => et.id === partnerStatusId)
    var partnerRequestStatus = partnerRequestStatusList.find(et => et.id === partnerRequestStatusId)

    if (partnerRequestStatus) {
        return partnerRequestStatus.name;
    }
    else if (partnerStatus) {
        return partnerStatus.name;
    }

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
