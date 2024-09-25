import { useState } from "react"
import { AdvancedMarker, AdvancedMarkerProps, InfoWindow, useAdvancedMarkerRef, InfoWindowProps, Pin } from "@vis.gl/react-google-maps"

type MarkerWithInfoWindowProps = AdvancedMarkerProps & {
  infoWindowTrigger: "click" | "hover"
  infoWindowProps: InfoWindowProps
  infoWindowContent: JSX.Element
}
export const MarkerWithInfoWindow = (props: MarkerWithInfoWindowProps) => {
  const { infoWindowTrigger, infoWindowProps, infoWindowContent, ...markerProps } = props
  const [markerRef, marker] = useAdvancedMarkerRef();
  const [infoWindowShown, setInfoWindowShown] = useState<boolean>(false);

  const triggerProps = infoWindowTrigger === "click" ? {
    onClick: () => setInfoWindowShown(!infoWindowShown)
  } : {
    onMouseOver: () => setInfoWindowShown(true),
    onMouseOut: () => setInfoWindowShown(false)
  }
                        
  return (
    <>
      <AdvancedMarker
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