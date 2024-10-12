import { useState } from 'react';
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
                <InfoWindow anchor={marker} style={{ fontFamily: 'Poppins'}} {...infoWindowProps}>
                    {infoWindowContent}
                </InfoWindow>
            ) : null}
        </>
    );
};
