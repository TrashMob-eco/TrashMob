import { APIProvider, Map, MapProps } from '@vis.gl/react-google-maps';
import { PropsWithChildren } from 'react';
import * as MapStore from '../../../store/MapStore';
import { useGetDefaultMapCenter } from '../../../hooks/useGetDefaultMapCenter';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';

export const GoogleMap = (props: PropsWithChildren<MapProps>) => {
    const { defaultCenter: defaultCenterProps, defaultZoom: defaultZoomProps, children, ...rest } = props;
    const defaultCenter = useGetDefaultMapCenter();
    
    return (
        <Map
            mapId='6f295631d841c617'
            gestureHandling={props.gestureHandling ?? 'greedy'}
            disableDefaultUI
            style={{ width: '100%', height: '500px' }}
            defaultZoom={defaultZoomProps || MapStore.defaultUserLocationZoom}
            defaultCenter={defaultCenterProps || defaultCenter}
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
