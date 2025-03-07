import { APIProvider, Map, MapProps, useMap } from '@vis.gl/react-google-maps';
import { PropsWithChildren, useEffect } from 'react';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import * as MapStore from '@/store/MapStore';
import { useGetDefaultMapCenter } from '@/hooks/useGetDefaultMapCenter';

export const GoogleMap = (props: PropsWithChildren<MapProps>) => {
    const { defaultCenter: defaultCenterProps, defaultZoom: defaultZoomProps, children, id, ...rest } = props;
    const userDefaultCenter = useGetDefaultMapCenter();

    const defaultCenter = defaultCenterProps || userDefaultCenter;

    // Move Map when receiving new defaultCenterProps
    const map = useMap(id);
    useEffect(() => {
        if (map && defaultCenter) {
            map.panTo(defaultCenter);
        }
    }, [map, defaultCenter]);

    return (
        <Map
            mapId='6f295631d841c617'
            gestureHandling={props.gestureHandling ?? 'greedy'}
            disableDefaultUI
            style={{ width: '100%', height: '500px' }}
            defaultZoom={defaultZoomProps || MapStore.defaultUserLocationZoom}
            defaultCenter={defaultCenter}
            {...rest}
        >
            {children}
        </Map>
    );
};

export const GoogleMapWithKey = (props: PropsWithChildren<MapProps>) => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();

    if (isLoading) return null;

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <GoogleMap {...props} />
        </APIProvider>
    );
};
