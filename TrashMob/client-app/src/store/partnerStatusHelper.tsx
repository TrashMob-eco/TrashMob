import PartnerStatusData from '../components/Models/PartnerStatusData';
import { GetPartnerStatuses } from '../services/partners';

export function getPartnerStatus(partnerStatusList: PartnerStatusData[], partnerStatusId: any): string {
    const partnerStatus = partnerStatusList.find((et) => et.id === partnerStatusId);
    return partnerStatus ? partnerStatus.name : 'Unknown';
}

export async function getPartnerStatusAsync(partnerStatusId: any): Promise<string> {
    const partnerStatusList = await getPartnerStatuses();
    return getPartnerStatus(partnerStatusList, partnerStatusId);
}

async function getPartnerStatuses(): Promise<PartnerStatusData[]> {
    const result = await GetPartnerStatuses()
        .service()
        .then((res) => res.data)
        .catch((err) => []);
    return result;
}
