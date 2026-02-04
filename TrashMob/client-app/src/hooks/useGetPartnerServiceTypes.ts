import { GetServiceTypes } from '@/services/services';
import { useQuery } from '@tanstack/react-query';

export const useGetPartnerServiceTypes = () => {
    return useQuery({
        queryKey: GetServiceTypes().key,
        queryFn: GetServiceTypes().service,
        select: (res) => res.data,
        staleTime: 5 * 60 * 1000, // 5 minutes - lookup data rarely changes
    });
};
