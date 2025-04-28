import { useCallback, useEffect, useRef, useState } from 'react';
import {
    AdvancedMarker,
    AdvancedMarkerProps,
    InfoWindow,
    useAdvancedMarkerRef,
    InfoWindowProps,
} from '@vis.gl/react-google-maps';

type MarkerWithInfoWindowProps = AdvancedMarkerProps & {
    readonly infoWindowTrigger: 'click' | 'hover' | 'hover-persist';
    readonly infoWindowProps?: InfoWindowProps;
    readonly infoWindowContent: JSX.Element;
};

export const GoogleMarkerWithInfoWindow = (props: MarkerWithInfoWindowProps) => {
    const { infoWindowTrigger, infoWindowProps = {}, infoWindowContent, ...markerProps } = props;
    const [markerRef, marker] = useAdvancedMarkerRef();
    const [infoWindowShown, setInfoWindowShown] = useState<boolean>(false);
    const closeInfoWindow = useCallback(() => setInfoWindowShown(false), []);

    let triggerProps;
    switch (infoWindowTrigger) {
        case 'click':
            triggerProps = {
                onClick: () => setInfoWindowShown(!infoWindowShown),
            };
            break;
        case 'hover':
            triggerProps = {
                onMouseEnter: () => setInfoWindowShown(true),
                onMouseLeave: () => setInfoWindowShown(false),
            };
            break;
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
