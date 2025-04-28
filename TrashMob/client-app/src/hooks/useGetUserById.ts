import { GetUserById } from '@/services/users';
import { useQuery } from '@tanstack/react-query';

export const useGetUserById = (userId: string) => {
    return useQuery({
        queryKey: GetUserById({ userId }).key,
        queryFn: GetUserById({ userId }).service,
        select: (res) => res.data,
    });
};
