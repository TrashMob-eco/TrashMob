import * as React from 'react';
import { AzureMap, IAzureMapOptions } from 'react-azure-maps';

interface MapComponentState {
    mapOptions: IAzureMapOptions,
    isKeyLoaded: boolean
}

const MapComponent: React.FC<MapComponentState> = (props) => {
    return (
        <div style={{ height: '300px' }}>
            {!props.isKeyLoaded && <div>Map is loading.</div>}
            {props.isKeyLoaded && <AzureMap options={props.mapOptions}/> }
        </div>
    );
};

export default MapComponent;