import EventTypeData from "../components/Models/EventTypeData";
import { defaultHeaders } from "./AuthStore";

export function getEventType(eventTypeList: EventTypeData[], eventTypeId: any): string {
    if (eventTypeList === null || eventTypeList.length === 0) {
        eventTypeList = getEventTypes();
    }

    var eventType = eventTypeList.find(et => et.id === eventTypeId)
    if (eventType)
        return eventType.name;
    return "Unknown";
}

function getEventTypes() : EventTypeData[] {
    const headers = defaultHeaders('GET');

    fetch('api/eventtypes', {
        method: 'GET',
        headers: headers
    })
        .then(response => response.json() as Promise<Array<any>>)
        .then(data => {
            return data;
        });
    return [];
}
