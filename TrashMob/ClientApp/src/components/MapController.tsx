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
    eventName: string;
    latitude: number;
    longitude: number;
    onLocationChange: any;
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

    function createPin(eventData: EventData): MapStore.markerPoint {
        var pin = new MapStore.markerPoint();
        pin.position = new data.Point(new data.Position(eventData.longitude, eventData.latitude));
        pin.properties = {
            title: eventData.name, icon: 'pin-round-blue', type: 'Point'
        } 
        return pin;
    }

    // This is used for maps with multiple events
    useEffect(() => {
        if (mapRef && !props.loading && props.isKeyLoaded) {
            clearMarkers();
            props.multipleEvents.forEach(mobEvent => {
                var pin = createPin(mobEvent);
                addMarker(pin);
            })
        }
    }, [props.multipleEvents, mapRef, props.loading, props.isKeyLoaded]);

    // This is only used for maps with a single event
    useEffect(() => {
        if (!props.loading && props.eventName !== '') {
            var pin = new MapStore.markerPoint();
            pin.position = new data.Point(new data.Position(props.longitude, props.latitude));
            pin.properties = {
                title: props.eventName, icon: 'pin-round-blue', type: 'Point'
            }
            clearMarkers();
            addMarker(pin);
        }
    }, [props.loading, props.latitude, props.longitude, props.eventName])


    useEffect(() => {
        if (isMapReady && mapRef && props.isKeyLoaded) {
            // Need to add source and layer to map on init and ready
            mapRef.sources.add(dataSourceRef);
            layerRef.setOptions(MapStore.memoizedOptions);
            mapRef.layers.add(layerRef);
        }
    }, [isMapReady, mapRef, props.isKeyLoaded]);


    function handleLocationChange(e: any) {
        props.onLocationChange(e);
    }

    const clearMarkers = () => {
        dataSourceRef.clear();
    };

    // Util function to add pin
    const addMarker = (point: MapStore.markerPoint) => {
        dataSourceRef.add(new data.Feature(point.position, point.properties));
    };

    return (
        <>
            <MapComponent mapOptions={props.mapOptions} isKeyLoaded={props.isKeyLoaded} onLocationChange={handleLocationChange} />
        </>
    );
};

export default MapController;
