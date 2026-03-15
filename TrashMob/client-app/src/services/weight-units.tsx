import { ApiService } from '.';
import WeightUnit from '../components/Models/WeightUnit';

export type WeightUnit_Response = WeightUnit[];
export const GetWeightUnits = () => ({
    key: ['/weightunits'],
    service: async () =>
        ApiService('public').fetchData<WeightUnit_Response>({
            url: `/v2/weightunits`,
            method: 'get',
        }),
});
