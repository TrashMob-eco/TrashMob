import JobOpportunityData from '@/components/Models/JobOpportunity';
import { ApiService } from '.';

export type GetAllJobOpportunities_Response = JobOpportunityData[];
export const GetAllJobOpportunities = () => ({
    key: ['/jobopportunities', 'active'],
    service: async () => {
        return ApiService('public').fetchData<GetAllJobOpportunities_Response>({
            url: '/jobopportunities?isActive=true',
            method: 'get',
        });
    },
});
