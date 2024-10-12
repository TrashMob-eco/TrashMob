import { Map, MapProps } from '@vis.gl/react-google-maps'
import { PropsWithChildren } from 'react'
import * as MapStore from '../../../store/MapStore';

export const GoogleMap = (props: PropsWithChildren<MapProps>) => {
  return (
    <Map
      mapId='6f295631d841c617'
      gestureHandling='greedy'
      disableDefaultUI
      style={{ width: '100%', height: '500px' }}
      defaultZoom={MapStore.defaultUserLocationZoom}
      {...props}
    >
      {props.children}
    </Map>
  )
}