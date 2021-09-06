import { useContext, useEffect } from 'react';
import * as React from 'react';
import { AzureMapsContext, IAzureMapOptions, IAzureMapsContextProps } from '@ambientlight/react-azure-maps';
import { data, source, Popup, HtmlMarker } from 'azure-maps-control';
import MapComponent from './MapComponent';
import * as MapStore from '../store/MapStore'
import UserData from './Models/UserData';
import { HtmlMarkerLayer } from './HtmlMarkerLayer/SimpleHtmlMarkerLayer'
import { renderToStaticMarkup } from "react-dom/server"

interface MapControllerProps {
    mapOptions: IAzureMapOptions | undefined
    center: data.Position;
    isEventDataLoaded: boolean,
    isMapKeyLoaded: boolean
    eventName: string;
    eventDate: Date;
    latitude: number;
    longitude: number;
    onLocationChange: any;
    currentUser: UserData;
    isUserLoaded: boolean;
}

export const EventCollectionMapController: React.FC<MapControllerProps> = (props) => {
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
                            : `<div className="container-fluid card">
                                <h1>${marker.properties.name}</h1>
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>Event Date:</td>
                                            <td>${marker.properties.eventDate}</td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>`;
                        popup.setOptions({
                            content: content,
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

            //Add marker layer to the map.
            mapRef.layers.add(markerLayer);

            var position = new data.Point(new data.Position(props.longitude, props.latitude));

            var featureProperties = ({
                name: props.eventName,
                eventDate: props.eventDate,
            });

            dataSourceRef.add(new data.Feature(position, featureProperties));
        }
    }, [mapRef,
        props.center,
        props.isEventDataLoaded,
        props.isMapKeyLoaded,
        props.currentUser,
        props.isUserLoaded,
        props.eventName,
        props.eventDate,
        props.longitude,
        props.latitude,
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

export default EventCollectionMapController;