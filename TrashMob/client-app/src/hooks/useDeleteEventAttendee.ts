import { DeleteEventAttendee, GetUserEvents } from '@/services/events';
import { useMutation, useQueryClient } from '@tanstack/react-query';

export const useDeleteEventAttendee = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationKey: DeleteEventAttendee().key,
        mutationFn: DeleteEventAttendee().service,
        onSuccess: async (data, variables) => {
            // on delete attendee success
            const { userId } = variables;

            // refetch current user's eventlist
            await queryClient.refetchQueries({ queryKey: GetUserEvents({ userId }).key });
        },
    });
};
