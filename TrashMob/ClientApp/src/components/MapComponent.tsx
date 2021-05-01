import * as React from 'react';
import { AzureMap, IAzureMapOptions } from 'react-azure-maps';

interface MapComponentProps {
    mapOptions: IAzureMapOptions,
    isMapKeyLoaded: boolean,
    onLocationChange: any;
}

const MapComponent: React.FC<MapComponentProps> = (props) => {

    function getCoordinates(e: any) {
        props.onLocationChange(e.position);
    }

    return (
        <div style={{ height: '300px' }}>
            {!props.isMapKeyLoaded && <div>Map is loading.</div>}
            {props.isMapKeyLoaded && <AzureMap options={props.mapOptions} events={{ click: getCoordinates }}/> }
        </div>
    );
};

export default MapComponent;