import { GetServiceTypes } from '@/services/services';
import { useQuery } from '@tanstack/react-query';

export const useGetPartnerServiceTypes = () => {
    return useQuery({
        queryKey: GetServiceTypes().key,
        queryFn: GetServiceTypes().service,
        select: (res) => res.data,
    });
};
