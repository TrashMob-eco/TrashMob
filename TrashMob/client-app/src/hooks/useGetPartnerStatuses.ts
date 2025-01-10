import { useQuery } from '@tanstack/react-query';
import { GetPartnerStatuses } from '@/services/partners';
import PartnerStatusData from '@/components/Models/PartnerStatusData';
import { AxiosResponse } from 'axios';

export const useGetPartnerStatuses = () => {
    return useQuery<AxiosResponse<PartnerStatusData[]>, unknown, PartnerStatusData[]>({
        queryKey: GetPartnerStatuses().key,
        queryFn: GetPartnerStatuses().service,
        select: (res) => res.data,
    });
};
