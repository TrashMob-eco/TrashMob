import { useQuery } from '@tanstack/react-query';
import { GetUserEvents } from '@/services/events';

export const useGetUserEvents = (userId: string) => {
    return useQuery({
        queryKey: GetUserEvents({ userId }).key,
        queryFn: GetUserEvents({ userId }).service,
        select: (res) => res.data,
    });
};
