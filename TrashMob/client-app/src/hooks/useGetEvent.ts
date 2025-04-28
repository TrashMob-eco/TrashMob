import { useQuery } from '@tanstack/react-query';
import { GetEventById } from '@/services/events';

export const useGetEvent = (eventId: string) => {
    return useQuery({
        queryKey: GetEventById({ eventId }).key,
        queryFn: GetEventById({ eventId }).service,
        select: (res) => res.data,
    });
};
