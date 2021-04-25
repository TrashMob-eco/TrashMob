import { useContext, useEffect } from 'react';
import * as React from 'react';
import { AzureMapsContext, IAzureMapOptions, IAzureMapsContextProps } from 'react-azure-maps';
import { data, layer, source } from 'azure-maps-control';
import MapComponent from './MapComponent';
import EventData from './Models/EventData';
import * as MapStore from '../store/MapStore'

const dataSourceRef = new source.DataSource();
const layerRef = new layer.SymbolLayer(dataSourceRef);

interface MapControllerState {
    mapOptions: IAzureMapOptions
    center: data.Position;
    multipleEvents: EventData[],
    loading: boolean,
    isKeyLoaded: boolean
}

const MapController = (props: MapControllerState) => {
    // Here you use mapRef from context
    const { mapRef, isMapReady } = useContext<IAzureMapsContextProps>(AzureMapsContext);

    useEffect(() => {
        if (mapRef && !props.loading && props.isKeyLoaded) {
            // Simple Camera options modification
            mapRef.setCamera({ center: props.center, zoom: MapStore.defaultUserLocationZoom });
        }
    }, [mapRef, props.center, props.loading, props.isKeyLoaded]);

    useEffect(() => {
        if (mapRef && !props.loading && props.isKeyLoaded) {
            props.multipleEvents.forEach(mobEvent => {
                var pin = new MapStore.markerPoint();
                pin.position = new data.Point(new data.Position(mobEvent.longitude, mobEvent.latitude));
                pin.properties = {
                    title: mobEvent.name, icon: 'pin-round-blue', type: 'Point'
                } 
                addMarker(pin);
            })
        }
    }, [props.multipleEvents, mapRef, props.loading, props.isKeyLoaded]);

    useEffect(() => {
        if (isMapReady && mapRef && props.isKeyLoaded) {
            // Need to add source and layer to map on init and ready
            mapRef.sources.add(dataSourceRef);
            layerRef.setOptions(MapStore.memoizedOptions);
            mapRef.layers.add(layerRef);
        }
    }, [isMapReady, mapRef, props.isKeyLoaded]);

    // Util function to add pin
    const addMarker = (point: MapStore.markerPoint) => {
        dataSourceRef.add(new data.Feature(point.position, point.properties));
    };

    return (
        <>
            <MapComponent mapOptions={props.mapOptions} isKeyLoaded={props.isKeyLoaded} />
        </>
    );
};

export default MapController;
