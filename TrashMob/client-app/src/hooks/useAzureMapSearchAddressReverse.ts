import { useQuery, UseQueryOptions } from '@tanstack/react-query';
import { ReverseGeocode, ReverseGeocode_Params } from '../services/maps';
import { AzureMapSearchAddressReverseResult } from '../components/Models/AzureMapSearchAddressReverse';

export const useAzureMapSearchAddressReverse = (
    params: ReverseGeocode_Params,
    options: Pick<UseQueryOptions, 'enabled'> = {},
) => {
    return useQuery<AzureMapSearchAddressReverseResult>({
        queryKey: [ReverseGeocode().key, params.lat, params.long],
        queryFn: async () => {
            const response = await ReverseGeocode().service(params);
            return response.data;
        },
        ...options,
    });
};
