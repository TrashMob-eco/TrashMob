import React, {useState} from 'react';
import {
  AdvancedMarker,
  InfoWindow,
  useAdvancedMarkerRef
} from '@vis.gl/react-google-maps';

type TrashMarkerProps = {
  gps: {
    latitude: number
    longitude: number
  }
  previewUrl: string
}

export const TrashMarker = (props: TrashMarkerProps) => {
  const [infowindowOpen, setInfowindowOpen] = useState(false);
  const [markerRef, marker] = useAdvancedMarkerRef();

  return (
    <>
      <AdvancedMarker
        ref={markerRef}
        onClick={() => setInfowindowOpen(true)}
        position={{ lat: props.gps.latitude, lng: props.gps.longitude }}
      >
        <div
          style={{
            width: 16,
            height: 16,
            position: 'absolute',
            top: 0,
            left: 0,
            background: '#1dbe80',
            border: '2px solid #0e6443',
            borderRadius: '50%',
            transform: 'translate(-50%, -50%)'
          }}
        />
      </AdvancedMarker>
      {infowindowOpen && (
        <InfoWindow
          anchor={marker}
          maxWidth={250}
          onCloseClick={() => setInfowindowOpen(false)}
        >
          <img src={props.previewUrl} style={{ width: 250, height: 'auto' }} />
        </InfoWindow>
      )}
    </>
  )
}