import { useEffect, useState } from 'react';
import * as MapStore from '../store/MapStore';
import { useGetUserById } from './useGetUserById';
import { useLogin } from './useLogin';

export const useGetDefaultMapCenter = () => {
    const { currentUser } = useLogin();
    const { data: user } = useGetUserById(currentUser.id)
    console.log({ user })
    
    const [defaultCenter, setDefaultCenter] = useState<google.maps.LatLngLiteral>({
        lat: MapStore.defaultLatitude,
        lng: MapStore.defaultLongitude,
    });
    useEffect(() => {
        if ('geolocation' in navigator) {
            navigator.geolocation.getCurrentPosition((position) => {
                setDefaultCenter({ lat: position.coords.latitude, lng: position.coords.longitude });
            });
        }
    }, []);

    return defaultCenter;
};
