import EventPartnerLocationServiceStatusData from '../components/Models/EventPartnerLocationServiceStatusData';
import { GetEventPartnerLocationServiceStatuses } from '../services/locations';

export function getEventPartnerLocationServiceStatus(
    eventPartnerLocationServiceStatusList: EventPartnerLocationServiceStatusData[],
    eventPartnerLocationServiceStatusId: number,
): string {
    const eventPartnerStatus = eventPartnerLocationServiceStatusList.find(
        (et) => et.id === eventPartnerLocationServiceStatusId,
    );
    return eventPartnerStatus ? eventPartnerStatus.name : 'None';
}

export async function getEventPartnerLocationServiceStatusAsync(
    eventPartnerLocationServiceStatusId: number,
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
        .catch(() => []);
    return result;
}
