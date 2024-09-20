import EventTypeData from "../components/Models/EventTypeData";
import { GetEventTypes } from "../services/events";

export function getEventType(eventTypeList: EventTypeData[], eventTypeId: any): string {
    const eventType = eventTypeList.find(et => et.id === eventTypeId)
    return eventType ? eventType.name : "Unknown"
}

export async function getEventTypeAsync(eventTypeId: any): Promise<string> {
    const eventTypeList = await getEventTypes();
    return getEventType(eventTypeList, eventTypeId);
}

async function getEventTypes(): Promise<EventTypeData[]> {
    const result = await GetEventTypes().service().then(res => res.data).catch(err => [])
    return result;
}
