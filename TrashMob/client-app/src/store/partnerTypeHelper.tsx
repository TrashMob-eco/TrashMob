import PartnerTypeData from "../components/Models/PartnerTypeData";
import { getDefaultHeaders } from "./AuthStore";

export function getPartnerType(partnerTypeList: PartnerTypeData[], partnerTypeId: any): string {
    if (partnerTypeList === null || partnerTypeList.length === 0) {
        partnerTypeList = getPartnerTypees();
    }

    var partnerType = partnerTypeList.find(et => et.id === partnerTypeId)
    if (partnerType)
        return partnerType.name;
    return "Unknown";
}

function getPartnerTypees(): PartnerTypeData[] {
    const headers = getDefaultHeaders('GET');

    fetch('/api/partnerTypees', {
        method: 'GET',
        headers: headers
    })
        .then(response => response.json() as Promise<Array<any>>)
        .then(data => {
            return data;
        });
    return [];
}
