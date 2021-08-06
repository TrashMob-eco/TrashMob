import EventPartnerStatusData from "../components/Models/EventPartnerStatusData";
import { getDefaultHeaders } from "./AuthStore";

export function getEventPartnerStatus(eventPartnerStatusList: EventPartnerStatusData[], eventPartnerStatusId: any): string {
    if (eventPartnerStatusList === null || eventPartnerStatusList.length === 0) {
        eventPartnerStatusList = getEventPartnerStatuses();
    }

    var eventPartnerStatus = eventPartnerStatusList.find(et => et.id === eventPartnerStatusId)
    if (eventPartnerStatus)
        return eventPartnerStatus.name;
    return "None";
}

function getEventPartnerStatuses(): EventPartnerStatusData[] {
    const headers = getDefaultHeaders('GET');

    fetch('/api/eventPartnerStatuses', {
        method: 'GET',
        headers: headers
    })
        .then(response => response.json() as Promise<Array<any>>)
        .then(data => {
            return data;
        });
    return [];
}
