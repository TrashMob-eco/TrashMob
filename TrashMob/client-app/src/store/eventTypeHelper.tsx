import EventTypeData from "../components/Models/EventTypeData";

export function getEventType(eventTypeList: EventTypeData[], eventTypeId: any): string {
    const eventType = eventTypeList.find(et => et.id === eventTypeId)
    return eventType ? eventType.name : "Unknown"
}
