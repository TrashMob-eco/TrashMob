import ServiceTypeData from "../components/Models/ServiceTypeData";
import { getDefaultHeaders } from "./AuthStore";

export function getServiceType(serviceTypeList: ServiceTypeData[], serviceTypeId: any): string {
    if (serviceTypeList === null || serviceTypeList.length === 0) {
        serviceTypeList = getServiceTypes();
    }

    var serviceType = serviceTypeList.find(et => et.id === serviceTypeId)
    if (serviceType)
        return serviceType.name;
    return "Unknown";
}

function getServiceTypes(): ServiceTypeData[] {
    const headers = getDefaultHeaders('GET');

    fetch('/api/serviceTypes', {
        method: 'GET',
        headers: headers
    })
        .then(response => response.json() as Promise<Array<any>>)
        .then(data => {
            return data;
        });
    return [];
}
