import { useState } from "react"
import { Marker, InfoWindow, MarkerProps, useMarkerRef, InfoWindowProps } from "@vis.gl/react-google-maps"

type MarkerWithInfoWindowProps = MarkerProps & {
  infoWindowTrigger: "click" | "hover"
  infoWindowProps: InfoWindowProps
  infoWindowContent: JSX.Element
}
export const MarkerWithInfoWindow = (props: MarkerWithInfoWindowProps) => {
  const { infoWindowTrigger, infoWindowProps, infoWindowContent, ...markerProps } = props
  const [markerRef, marker] = useMarkerRef();
  const [infoWindowShown, setInfoWindowShown] = useState<boolean>(false);

  const triggerProps = infoWindowTrigger === "click" ? {
    onClick: () => setInfoWindowShown(!infoWindowShown)
  } : {
    onMouseOver: () => setInfoWindowShown(true),
    onMouseOut: () => setInfoWindowShown(false)
  }
                        
  return (
    <>
      <Marker
        ref={markerRef}
        {...markerProps}
        {...triggerProps}
      />

      {infoWindowShown && (
        <InfoWindow anchor={marker} {...infoWindowProps}>
          {infoWindowContent}
        </InfoWindow>
      )}
    </>
  );
};