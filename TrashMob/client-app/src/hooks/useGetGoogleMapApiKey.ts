import { useQuery } from "@tanstack/react-query";
import { GetGoogleMapApiKey } from "../services/maps";

export const useGetGoogleMapApiKey = () => {
    return useQuery({
        queryKey: GetGoogleMapApiKey().key,
        queryFn: GetGoogleMapApiKey().service,
        select: (res) => res.data,
        staleTime: 24 * 60 * 60 * 1000, // 24hr 
    });
}