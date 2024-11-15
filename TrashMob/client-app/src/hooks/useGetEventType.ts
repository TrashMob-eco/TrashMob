import { useQuery } from '@tanstack/react-query';
import { GetEventTypes } from '../services/events';
import { useGetEventTypes } from './useGetEventTypes';

export const useGetEventType = (id: number) => {
    const { data: eventTypes, isLoading: isEventTypesLoading } = useGetEventTypes();
    return useQuery({
        queryKey: [...GetEventTypes().key, id],
        queryFn: () => {
            return (eventTypes || []).find((et) => et.id === id);
        },
        // Prevent query from running if eventTypes aren't loaded yet
        enabled: !!eventTypes && !isEventTypesLoading,
    });
};
