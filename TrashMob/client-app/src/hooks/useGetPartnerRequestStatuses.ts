import PartnerRequestStatusData from '@/components/Models/PartnerRequestStatusData';
import { GetPartnerRequestStatuses } from '@/services/partners';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';

export const useGetPartnerRequestStatuses = () => {
    return useQuery<AxiosResponse<PartnerRequestStatusData[]>, unknown, PartnerRequestStatusData[]>({
        queryKey: GetPartnerRequestStatuses().key,
        queryFn: GetPartnerRequestStatuses().service,
        select: (res) => res.data,
    });
};
