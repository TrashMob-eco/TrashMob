import { useCallback, useEffect, useRef, useState } from 'react';
import {
    AdvancedMarker,
    AdvancedMarkerProps,
    InfoWindow,
    useAdvancedMarkerRef,
    InfoWindowProps,
} from '@vis.gl/react-google-maps';

type MarkerWithInfoWindowProps = AdvancedMarkerProps & {
    infoWindowTrigger: 'click' | 'hover' | 'hover-persist'
    infoWindowProps?: InfoWindowProps
    infoWindowContent: JSX.Element
}

export const GoogleMarkerWithInfoWindow = (props: MarkerWithInfoWindowProps) => {
    const { infoWindowTrigger, infoWindowProps = {}, infoWindowContent, ...markerProps } = props;
    const [markerRef, marker] = useAdvancedMarkerRef();
    const [infoWindowShown, setInfoWindowShown] = useState<boolean>(false);
    const closeInfoWindow = useCallback(() => setInfoWindowShown(false), []);
    const clickMapHandler = useRef<google.maps.MapsEventListener>()

    // on "hover-persist", add Click on map to close infoWindow
    useEffect(() => {
        if (!marker) return
        if (infoWindowTrigger !== 'hover-persist') return

        if (infoWindowShown) {
            clickMapHandler.current = marker.map!.addListener('click', closeInfoWindow)
        } else {
            clickMapHandler.current?.remove()
        }
        return () => {
            clickMapHandler.current?.remove();
        };
    }, [marker, infoWindowShown])

    let triggerProps
    switch (infoWindowTrigger) {
        case 'click': 
            triggerProps = {
                onClick: () => setInfoWindowShown(!infoWindowShown)
            }
            break
        case 'hover': 
            triggerProps = {
                onMouseEnter: () => setInfoWindowShown(true),
                onMouseLeave: () => setInfoWindowShown(false),
            }
            break
        case 'hover-persist':
            triggerProps = {
                onMouseEnter: () => setInfoWindowShown(true)
            }
    }

    return (
        <>
            <AdvancedMarker ref={markerRef} {...markerProps} {...triggerProps} />
            {infoWindowShown ? (
                <InfoWindow anchor={marker} {...infoWindowProps} onClose={closeInfoWindow}>
                    {infoWindowContent}
                </InfoWindow>
            ) : null}
        </>
    );
};
