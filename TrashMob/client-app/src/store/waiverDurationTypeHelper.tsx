import WaiverDurationTypeData from "../components/Models/WaiverDurationTypeData";
import { getDefaultHeaders } from "./AuthStore";

export function getWaiverDurationType(waiverDurationTypeList: WaiverDurationTypeData[], waiverDurationTypeId: any): string {
    if (waiverDurationTypeList === null || waiverDurationTypeList.length === 0) {
        waiverDurationTypeList = getWaiverDurationTypes();
    }

    var waiverDurationType = waiverDurationTypeList.find(et => et.id === waiverDurationTypeId)
    if (waiverDurationType)
        return waiverDurationType.name;
    return "Unknown";
}

function getWaiverDurationTypes() : WaiverDurationTypeData[] {
    const headers = getDefaultHeaders('GET');

    fetch('/api/waiverDurationtypes', {
        method: 'GET',
        headers: headers
    })
        .then(response => response.json() as Promise<Array<any>>)
        .then(data => {
            return data;
        });
    return [];
}
