import { useContext, useEffect } from 'react';
import * as React from 'react';
import { AzureMapsContext, IAzureMapOptions, IAzureMapsContextProps } from '@ambientlight/react-azure-maps';
import { data, source, Popup, HtmlMarker } from 'azure-maps-control';
import MapComponent from './MapComponent';
import EventData from './Models/EventData';
import * as MapStore from '../store/MapStore'
import UserData from './Models/UserData';
import { HtmlMarkerLayer } from './HtmlMarkerLayer/SimpleHtmlMarkerLayer'

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
    const [isDataSourceLoaded, setIsDataSourceLoaded] = React.useState(false);

    useEffect(() => {
        if (mapRef && props.isEventDataLoaded && props.isMapKeyLoaded && !isDataSourceLoaded && isMapReady) {

            // Simple Camera options modification
            mapRef.setCamera({ center: props.center, zoom: MapStore.defaultUserLocationZoom });

            var dataSourceRef = new source.DataSource("mainDataSource", { cluster: true });
            mapRef.sources.add(dataSourceRef);
            setIsDataSourceLoaded(true);

            // Create a reusable popup.
            const popup = new Popup({
                pixelOffset: [0, -20],
                closeButton: false
            });

            // Create a HTML marker layer for rendering data points.
            var markerLayer = new HtmlMarkerLayer(dataSourceRef, "marker1", {
                markerRenderCallback: (id: any, position: data.Position, properties: any) => {
                    // Create an HtmlMarker.
                    const marker = new HtmlMarker({
                        position: position,
                        text: properties.name
                    });

                    mapRef.events.add('mouseover', marker, (event: any) => {
                        const marker = event.target as HtmlMarker & { properties: any };
                        const content = marker.properties.cluster
                            ? `Cluster of ${marker.properties.point_count_abbreviated} markers`
                            : marker.properties.name;

                        popup.setOptions({
                            content: `<div style="padding:10px;">${content}</div>`,
                            position: marker.getOptions().position
                        });

                        // Open the popup.
                        if (mapRef) {
                            popup.open(mapRef);
                        }
                    });

                    mapRef.events.add('mouseout', marker, (event: any) => popup.close());
                    return marker
                },
                clusterRenderCallback: function (id: any, position: any, properties: any) {
                    const markerCluster = new HtmlMarker({
                        position: position,
                        color: 'DarkViolet',
                        text: properties.point_count_abbreviated,
                    });

                    return markerCluster;
                },
                source: dataSourceRef
            });

            // markerLayer.setOptions(MapStore.memoizedOptions);

            // Add mouse events to the layer to show/hide a popup when hovering over a marker.
            // mapRef.events.add('mouseover', markerLayer, onHover);
            // mapRef.events.add('mouseout', markerLayer, closePopup);

            //Add marker layer to the map.
            mapRef.layers.add(markerLayer);

            props.multipleEvents.forEach(mobEvent => {
                var position = new data.Point(new data.Position(mobEvent.longitude, mobEvent.latitude));
                var properties = {
                    name: mobEvent.name,
                }
                dataSourceRef.add(new data.Feature(position, properties));
            })
        }
    }, [mapRef,
        props.center,
        props.isEventDataLoaded,
        props.isMapKeyLoaded,
        props.multipleEvents,
        props.currentUser,
        props.isUserLoaded,
        isDataSourceLoaded,
        isMapReady]);

    function handleLocationChange(e: any) {
        props.onLocationChange(e);
    }

    return (
        <>
            <MapComponent mapOptions={props.mapOptions} isMapKeyLoaded={props.isMapKeyLoaded} onLocationChange={handleLocationChange} />
        </>
    );
};

export default MapController;