import { useQuery } from "@tanstack/react-query";
import { GetEventTypes } from "../services/events";

export const useGetEventTypes = () => {
  	return useQuery({
        queryKey: GetEventTypes().key,
        queryFn: GetEventTypes().service,
		select: res => res.data,
		staleTime: 76800
    })
}