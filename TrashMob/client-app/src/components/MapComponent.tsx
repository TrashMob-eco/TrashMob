import * as React from 'react';
import { AzureMap, IAzureMapOptions } from 'react-azure-maps';

interface MapComponentProps {
    mapOptions: IAzureMapOptions | undefined;
    isMapKeyLoaded: boolean;
    onLocationChange: any;
}

const MapComponent: React.FC<MapComponentProps> = (props) => (
    <div style={{ height: '500px', width: `${100}%` }}>
        {(!props.isMapKeyLoaded || !props.mapOptions) && <div>Map is loading.</div>}
        {props.isMapKeyLoaded && props.mapOptions ? <AzureMap options={props.mapOptions} /> : null}
    </div>
);

export default MapComponent;
