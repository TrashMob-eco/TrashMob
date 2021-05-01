import * as React from 'react';
import { AzureMap, IAzureMapOptions } from 'react-azure-maps';

interface MapComponentProps {
    mapOptions: IAzureMapOptions,
    isKeyLoaded: boolean,
    onLocationChange: any;
}

const MapComponent: React.FC<MapComponentProps> = (props) => {

    function getCoordinates(e: any) {
        props.onLocationChange(e.position);
    }

    return (
        <div style={{ height: '300px' }}>
            {!props.isKeyLoaded && <div>Map is loading.</div>}
            {props.isKeyLoaded && <AzureMap options={props.mapOptions} events={{ click: getCoordinates }}/> }
        </div>
    );
};

export default MapComponent;