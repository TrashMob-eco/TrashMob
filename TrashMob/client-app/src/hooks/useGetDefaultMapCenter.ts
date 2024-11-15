import { useEffect, useState } from 'react';
import * as MapStore from '../store/MapStore';

export const useGetDefaultMapCenter = () => {
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
