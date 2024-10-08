import EventPartnerLocationServiceStatusData from '../components/Models/EventPartnerLocationServiceStatusData';
import { GetEventPartnerLocationServiceStatuses } from '../services/locations';

export function getEventPartnerLocationServiceStatus(
    eventPartnerLocationServiceStatusList: EventPartnerLocationServiceStatusData[],
    eventPartnerLocationServiceStatusId: any,
): string {
    const eventPartnerStatus = eventPartnerLocationServiceStatusList.find(
        (et) => et.id === eventPartnerLocationServiceStatusId,
    );
    return eventPartnerStatus ? eventPartnerStatus.name : 'None';
}

export async function getEventPartnerLocationServiceStatusAsync(
    eventPartnerLocationServiceStatusId: any,
): Promise<string> {
    const eventPartnerLocationServiceStatusList = await getEventPartnerLocationServiceStatuses();
    return getEventPartnerLocationServiceStatus(
        eventPartnerLocationServiceStatusList,
        eventPartnerLocationServiceStatusId,
    );
}

async function getEventPartnerLocationServiceStatuses(): Promise<EventPartnerLocationServiceStatusData[]> {
    const result = await GetEventPartnerLocationServiceStatuses()
        .service()
        .then((res) => res.data)
        .catch((err) => []);
    return result;
}
