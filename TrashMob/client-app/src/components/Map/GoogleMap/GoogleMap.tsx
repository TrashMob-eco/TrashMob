import { Map, MapProps, useMap } from '@vis.gl/react-google-maps';
import { PropsWithChildren, useEffect } from 'react';
import * as MapStore from '@/store/MapStore';
import { useGetDefaultMapCenter } from '@/hooks/useGetDefaultMapCenter';

export const GoogleMap = (props: PropsWithChildren<MapProps>) => {
    const { defaultCenter: defaultCenterProps, defaultZoom: defaultZoomProps, children, ...rest } = props;
    const defaultCenter = useGetDefaultMapCenter();

    // Move Map when receiving new defaultCenterProps
    const map = useMap();
    useEffect(() => {
        if (map && defaultCenterProps) {
            map.panTo(defaultCenterProps);
        }
    }, [map, defaultCenterProps]);

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
