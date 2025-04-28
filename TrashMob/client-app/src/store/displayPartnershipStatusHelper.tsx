import PartnerRequestStatusData from '../components/Models/PartnerRequestStatusData';
import PartnerStatusData from '../components/Models/PartnerStatusData';
import { GetPartnerRequestStatuses, GetPartnerStatuses } from '../services/partners';

export function getDisplayPartnershipStatus(
    partnerStatusList: PartnerStatusData[],
    partnerRequestStatusList: PartnerRequestStatusData[],
    partnerStatusId: any,
    partnerRequestStatusId: any,
): string {
    const partnerStatus = partnerStatusList.find((et) => et.id === partnerStatusId);
    const partnerRequestStatus = partnerRequestStatusList.find((et) => et.id === partnerRequestStatusId);
    if (partnerRequestStatus) return partnerRequestStatus.name;
    if (partnerStatus) return partnerStatus.name;
    return 'Unknown';
}

export async function getDisplayPartnershipStatusAsync(
    partnerStatusId: any,
    partnerRequestStatusId: any,
): Promise<string> {
    const partnerStatusList = await getPartnerStatuses();
    const partnerRequestStatusList = await getPartnerRequestStatuses();
    return getDisplayPartnershipStatus(
        partnerStatusList,
        partnerRequestStatusList,
        partnerStatusId,
        partnerRequestStatusId,
    );
}

async function getPartnerRequestStatuses(): Promise<PartnerRequestStatusData[]> {
    const result = await GetPartnerRequestStatuses()
        .service()
        .then((res) => res.data)
        .catch((err) => []);
    return result;
}

async function getPartnerStatuses(): Promise<PartnerStatusData[]> {
    const result = await GetPartnerStatuses()
        .service()
        .then((res) => res.data)
        .catch((err) => []);
    return result;
}
