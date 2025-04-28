import PartnerRequestStatusData from '../components/Models/PartnerRequestStatusData';
import { GetPartnerRequestStatuses } from '../services/partners';

export function getPartnerRequestStatus(
    partnerRequestStatusList: PartnerRequestStatusData[],
    partnerRequestStatusId: any,
): string {
    const partnerRequestStatus = partnerRequestStatusList.find((et) => et.id === partnerRequestStatusId);
    return partnerRequestStatus ? partnerRequestStatus.name : 'Unknown';
}

export async function getPartnerRequestStatusAsync(partnerRequestStatusId: any): Promise<string> {
    const partnerRequestStatusList = await getPartnerRequestStatuses();
    return getPartnerRequestStatus(partnerRequestStatusList, partnerRequestStatusId);
}

async function getPartnerRequestStatuses(): Promise<PartnerRequestStatusData[]> {
    const result = await GetPartnerRequestStatuses()
        .service()
        .then((res) => res.data)
        .catch((err) => []);
    return result;
}
