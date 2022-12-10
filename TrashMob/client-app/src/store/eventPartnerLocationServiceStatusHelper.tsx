import EventPartnerLocationServiceStatusData from "../components/Models/EventPartnerLocationServiceStatusData";
import { getDefaultHeaders } from "./AuthStore";

export function getEventPartnerLocationServiceStatus(eventPartnerLocationServiceStatusList: EventPartnerLocationServiceStatusData[], eventPartnerLocationServiceStatusId: any): string {
    if (eventPartnerLocationServiceStatusList === null || eventPartnerLocationServiceStatusList.length === 0) {
        eventPartnerLocationServiceStatusList = getEventPartnerLocationServiceStatuses();
    }

    var eventPartnerStatus = eventPartnerLocationServiceStatusList.find(et => et.id === eventPartnerLocationServiceStatusId)
    if (eventPartnerStatus)
        return eventPartnerStatus.name;
    return "None";
}

function getEventPartnerLocationServiceStatuses(): EventPartnerLocationServiceStatusData[] {
    const headers = getDefaultHeaders('GET');

    fetch('/api/eventPartnerLocationServiceStatuses', {
        method: 'GET',
        headers: headers
    })
        .then(response => response.json() as Promise<Array<any>>)
        .then(data => {
            return data;
        });
    return [];
}
