import { ApiService } from '.';
import ServiceTypeData from '../components/Models/ServiceTypeData';

export type GetServiceTypes_Response = ServiceTypeData[];
export const GetServiceTypes = () => ({
    key: ['/servicetypes'],
    service: async () =>
        ApiService('public').fetchData<GetServiceTypes_Response>({
            url: '/servicetypes',
            method: 'get',
        }),
});
