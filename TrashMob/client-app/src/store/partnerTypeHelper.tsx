import PartnerTypeData from '../components/Models/PartnerTypeData';
import { GetPartnerTypes } from '../services/partners';

export function getPartnerType(partnerTypeList: PartnerTypeData[], partnerTypeId: number): string {
    const partnerType = partnerTypeList.find((et) => et.id === partnerTypeId);
    return partnerType ? partnerType.name : 'Unknown';
}

export async function getPartnerTypeAsync(partnerTypeId: number): Promise<string> {
    const partnerTypeList = await getPartnerTypes();
    return getPartnerType(partnerTypeList, partnerTypeId);
}

async function getPartnerTypes(): Promise<PartnerTypeData[]> {
    const result = await GetPartnerTypes()
        .service()
        .then((res) => res.data)
        .catch(() => []);
    return result;
}
