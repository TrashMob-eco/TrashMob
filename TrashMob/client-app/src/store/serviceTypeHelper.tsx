import ServiceTypeData from "../components/Models/ServiceTypeData";
import { GetServiceTypes } from "../services/services";

export function getServiceType(serviceTypeList: ServiceTypeData[], serviceTypeId: any): string {
    const serviceType = serviceTypeList.find(et => et.id === serviceTypeId)
    return (serviceType) ? serviceType.name : "Unknown";
}

export async function getServiceTypeAsync(serviceTypeId: any): Promise<string> {
    const serviceTypeList = await getServiceTypes();
    return getServiceType(serviceTypeList, serviceTypeId)
}

async function getServiceTypes(): Promise<ServiceTypeData[]> {
    const result = await GetServiceTypes().service().then(res => res.data).catch(err => [])
    return result
}
