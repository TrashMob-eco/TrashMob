import { useContext, useEffect } from 'react';
import * as React from 'react';
import { AzureMapsContext, IAzureMapOptions, IAzureMapsContextProps } from 'react-azure-maps';
import { data, layer, source } from 'azure-maps-control';
import MapComponent from './MapComponent';
import EventData from './Models/EventData';
import * as MapStore from '../store/MapStore'
import UserData from './Models/UserData';

const dataSourceRef = new source.DataSource();
const layerRef = new layer.SymbolLayer(dataSourceRef);

interface MapControllerProps {
    mapOptions: IAzureMapOptions | undefined
    center: data.Position;
    multipleEvents: EventData[],
    isEventDataLoaded: boolean,
    isMapKeyLoaded: boolean
    eventName: string;
    latitude: number;
    longitude: number;
    onLocationChange: any;
    currentUser: UserData;
    isUserLoaded: boolean;
}

export const MapController: React.FC<MapControllerProps> = (props) => {
    // Here you use mapRef from context
    const { mapRef, isMapReady } = useContext<IAzureMapsContextProps>(AzureMapsContext);

    useEffect(() => {
        if (mapRef && props.isEventDataLoaded && props.isMapKeyLoaded) {
            // Simple Camera options modification
            mapRef.setCamera({ center: props.center, zoom: MapStore.defaultUserLocationZoom });
        }
    }, [mapRef, props.center, props.isEventDataLoaded, props.isMapKeyLoaded]);

    // This is used for maps with multiple events
    useEffect(() => {
        if (mapRef && props.isEventDataLoaded && props.isMapKeyLoaded) {
            clearMarkers();
            props.multipleEvents.forEach(mobEvent => {
                var pin = new MapStore.markerPoint();
                var pinColor ='pin-round-green';
                if (props.isUserLoaded && props.currentUser && mobEvent.createdByUserId === props.currentUser.id) {
                    pinColor = 'pin-round-blue';
                }

                pin.position = new data.Point(new data.Position(mobEvent.longitude, mobEvent.latitude));
                pin.properties = {
                    title: mobEvent.name, icon: pinColor, type: 'Point'
                }
                addMarker(pin);
            })
        }
    }, [props.multipleEvents, mapRef, props.isEventDataLoaded, props.isMapKeyLoaded, props.currentUser, props.isUserLoaded]);

    // This is only used for maps with a single event
    useEffect(() => {
        if (props.isEventDataLoaded && props.eventName !== '' && props.isMapKeyLoaded && mapRef && isMapReady) {
            var pin = new MapStore.markerPoint();
            pin.position = new data.Point(new data.Position(props.longitude, props.latitude));
            pin.properties = {
                title: props.eventName, icon: 'pin-round-blue', type: 'Point'
            }
            clearMarkers();
            addMarker(pin);
        }
    }, [props.isEventDataLoaded, props.latitude, props.longitude, props.eventName, props.isMapKeyLoaded, mapRef, isMapReady])


    useEffect(() => {
        if (isMapReady && mapRef && props.isMapKeyLoaded) {
            // Need to add source and layer to map on init and ready
            mapRef.sources.add(dataSourceRef);
            layerRef.setOptions(MapStore.memoizedOptions);
            mapRef.layers.add(layerRef);
        }
    }, [isMapReady, mapRef, props.isMapKeyLoaded]);

    function handleLocationChange(e: any) {
        props.onLocationChange(e);
    }

    function clearMarkers() {
        dataSourceRef.clear();
    };

    // Util function to add pin
    function addMarker(point: MapStore.markerPoint) {
        dataSourceRef.add(new data.Feature(point.position, point.properties));
    };

    return (
        <>
            <MapComponent mapOptions={props.mapOptions} isMapKeyLoaded={props.isMapKeyLoaded} onLocationChange={handleLocationChange} />
        </>
    );
};

export default MapController;
