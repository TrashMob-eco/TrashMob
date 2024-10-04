import { useState } from 'react';
import {
    AdvancedMarker,
    AdvancedMarkerProps,
    InfoWindow,
    useAdvancedMarkerRef,
    InfoWindowProps,
    Pin,
} from '@vis.gl/react-google-maps';

type MarkerWithInfoWindowProps = AdvancedMarkerProps & {
    infoWindowTrigger: 'click' | 'hover';
    infoWindowProps: InfoWindowProps;
    infoWindowContent: JSX.Element;
};
export const GoogleMarkerWithInfoWindow = (props: MarkerWithInfoWindowProps) => {
    const { infoWindowTrigger, infoWindowProps, infoWindowContent, ...markerProps } = props;
    const [markerRef, marker] = useAdvancedMarkerRef();
    const [infoWindowShown, setInfoWindowShown] = useState<boolean>(false);
    const triggerProps =
        infoWindowTrigger === 'click'
            ? {
                  onClick: () => setInfoWindowShown(!infoWindowShown),
              }
            : {
                  onMouseEnter: () => setInfoWindowShown(true),
                  onMouseLeave: () => setInfoWindowShown(false),
              };

    return (
        <>
            <AdvancedMarker ref={markerRef} {...markerProps} {...triggerProps} />
            {infoWindowShown ? (
                <InfoWindow anchor={marker} {...infoWindowProps}>
                    {infoWindowContent}
                </InfoWindow>
            ) : null}
        </>
    );
};
