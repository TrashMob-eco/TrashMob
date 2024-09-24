import { useContext, useEffect } from 'react';
import * as React from 'react';
import { AzureMapsContext, IAzureMapOptions, IAzureMapsContextProps } from 'react-azure-maps';
import { data, source, Popup, HtmlMarker } from 'azure-maps-control';
import MapComponent from './MapComponent';
import * as MapStore from '../store/MapStore'
import UserData from './Models/UserData';
import { HtmlMarkerLayer } from './HtmlMarkerLayer/src/layer/HtmlMarkerLayer'
import { AzureSearchLocationInput, SearchLocationOption } from './Map/AzureSearchLocationInput';

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
    isDraggable: boolean;
}

export const MapControllerSinglePoint: React.FC<MapControllerProps> = (props) => {
    // Here you use mapRef from context
    const { mapRef, isMapReady } = useContext<IAzureMapsContextProps>(AzureMapsContext);
    const [isDataSourceLoaded, setIsDataSourceLoaded] = React.useState(false);

    const mapKeyRef = React.useRef('');
    const [isPrevLoaded, setIsPrevLoaded] = React.useState<boolean>(false);

    useEffect(() => {
        if (props.mapOptions) {
            const key = props.mapOptions?.subscriptionKey ?? "";
            mapKeyRef.current = key;
        }
    }, [props.mapOptions]);

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
                markerCallback: (id: any, position: data.Position, properties: any) => {

                    dataSourceRef.clear();

                    // Create an HtmlMarker.
                    const marker = new HtmlMarker({
                        draggable: props.isDraggable,
                        color: "#96ba00"
                    });

                    marker.setOptions({
                        position: position
                    });

                    mapRef.events.add('mouseover', marker, (event: any) => {
                        const marker = event.target as HtmlMarker & { properties: any };
                        const date = new Date(marker.properties.eventDate).toLocaleDateString([], { month: "long", day: "2-digit", year: "numeric" });
                        const time = new Date(marker.properties.eventDate).toLocaleTimeString([], { timeZoneName: 'short' });
                        const content = marker.properties.cluster
                            ? `Cluster of ${marker.properties.point_count_abbreviated} markers`
                            : `<div className="map-popup-container" style="padding:0.5rem;">
                                <h5 style="font-weight: 500; font-size: 18px; margin-top: 0.5rem;">${marker.properties.name}</h5>
                                <div><span className="font-weight-bold">Event Date: </span><span>${date}</span></div>
                                <div><span className="font-weight-bold">Time: </span><span>${time}</span></div>

                                <div>
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

                    mapRef.events.add('dragend', marker, (e: any) => {
                        var pos = e.target.options.position;
                        handleLocationChange(pos);
                    });

                    //mapRef.events.add('click', marker, (e: any) => {
                    //    var pos = e.target.options.position;
                    //    handleLocationChange(pos);
                    //});

                    mapRef.events.add('mouseout', marker, () => popup.close());

                    return marker
                }
            });

            // Add marker layer to the map.
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
        props.isDraggable,
        // eslint-disable-next-line
        handleLocationChange,
        isMapReady]);

    useEffect(() => {
        if (mapRef && props.isEventDataLoaded && props.isMapKeyLoaded && isDataSourceLoaded && isMapReady && !isPrevLoaded) {
            var dsr = mapRef.sources.getById("mainDataSource") as source.DataSource;
            var feature = dsr.getShapes()[0];

            var position = new data.Position(props.longitude, props.latitude);

            // if the value is (0,0) this is a default. Use the user's position instead if available
            if (props.latitude === 0 && props.longitude === 0) {
                position = props.center;
            }

            feature.setCoordinates(position);

            // Simple Camera options modification
            mapRef.setCamera({ center: position, zoom: MapStore.defaultUserLocationZoom });
            setIsPrevLoaded(true);
        }
    }, [mapRef,
        props.center,
        props.isEventDataLoaded,
        props.isMapKeyLoaded,
        props.longitude,
        props.latitude,
        isDataSourceLoaded,
        isMapReady,
        isPrevLoaded]);

    // eslint-disable-next-line
    function handleLocationChange(e: any) {
        props.onLocationChange(e);
    }

    function handleSelectSearchLocation(location: SearchLocationOption) {
        const position = location.position;
        const point = new data.Position(position.lon, position.lat)
        props.onLocationChange(point)
    }

    return (
        <>
            {props.isDraggable && props.mapOptions?.subscriptionKey && (
                <div>
                    <AzureSearchLocationInput
                        azureKey={props.mapOptions.subscriptionKey}
                        onSelectLocation={handleSelectSearchLocation}
                    />
                </div>
            )}
            <MapComponent mapOptions={props.mapOptions} isMapKeyLoaded={props.isMapKeyLoaded} onLocationChange={handleLocationChange} />
        </>
    );
};

export default MapControllerSinglePoint;