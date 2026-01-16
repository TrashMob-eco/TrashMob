import { ApiService } from '.';
import WeightUnit from '../components/Models/WeightUnit'

export type WeightUnit_Response = WeightUnit[];
export const GetWeightUnits = () => ({
    key: ['/weightunits'],
    service: async () =>
        ApiService('public').fetchData<WeightUnit_Response>({
            url: `/weightunits`,
            method: 'get',
        }),
});