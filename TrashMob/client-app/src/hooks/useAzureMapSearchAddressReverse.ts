import { useQuery, UseQueryOptions } from "@tanstack/react-query";
import { AzureMapSearchAddressReverse, AzureMapSearchAddressReverse_Params } from "../services/maps";
import { AzureMapSearchAddressReverseResult } from "../components/Models/AzureMapSearchAddressReverse";

export const useAzureMapSearchAddressReverse = (params: AzureMapSearchAddressReverse_Params, options: Pick<UseQueryOptions, "enabled"> = {}) => {
  return useQuery<AzureMapSearchAddressReverseResult>({ 
    queryKey: [AzureMapSearchAddressReverse().key, params.lat, params.long],
    queryFn: async () => {
      const response = await AzureMapSearchAddressReverse().service(params)
      return response.data
    },
    ...options,
  });
}