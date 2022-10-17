import EventPartnerLocationStatusData from "../components/Models/EventPartnerLocationStatusData";
import { getDefaultHeaders } from "./AuthStore";

export function getEventPartnerStatus(eventPartnerLocationStatusList: EventPartnerLocationStatusData[], eventPartnerLocationStatusId: any): string {
    if (eventPartnerLocationStatusList === null || eventPartnerLocationStatusList.length === 0) {
        eventPartnerLocationStatusList = getEventPartnerLocationStatuses();
    }

    var eventPartnerStatus = eventPartnerLocationStatusList.find(et => et.id === eventPartnerLocationStatusId)
    if (eventPartnerStatus)
        return eventPartnerStatus.name;
    return "None";
}

function getEventPartnerLocationStatuses(): EventPartnerLocationStatusData[] {
    const headers = getDefaultHeaders('GET');

    fetch('/api/eventPartnerLocationStatuses', {
        method: 'GET',
        headers: headers
    })
        .then(response => response.json() as Promise<Array<any>>)
        .then(data => {
            return data;
        });
    return [];
}
