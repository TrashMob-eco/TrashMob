import { GetLocationsByPartner } from '@/services/locations';
import { useQuery } from '@tanstack/react-query';

type GetPartnerLocationsParams = {
    partnerId: string;
};

export const useGetPartnerLocations = (params: GetPartnerLocationsParams) => {
    const { partnerId } = params;
    return useQuery({
        queryKey: GetLocationsByPartner({ partnerId }).key,
        queryFn: GetLocationsByPartner({ partnerId }).service,
        select: (res) => res.data,
    });
};
