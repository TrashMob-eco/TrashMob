import { useContext, useEffect } from 'react';
import * as React from 'react';
import { AzureMapsContext, IAzureMapOptions, IAzureMapsContextProps } from 'react-azure-maps';
import { data, source, Popup, HtmlMarker } from 'azure-maps-control';
import MapComponent from './MapComponent';
import * as MapStore from '../store/MapStore'
import UserData from './Models/UserData';
import { HtmlMarkerLayer } from './HtmlMarkerLayer/SimpleHtmlMarkerLayer'
import { AsyncTypeahead } from 'react-bootstrap-typeahead';
import { getDefaultHeaders } from '../store/AuthStore';
import AddressData from './Models/AddressData';
import SearchAddressData from './Models/SearchAddressData';

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

export const EventCollectionMapController: React.FC<MapControllerProps> = (props) => {
    // Here you use mapRef from context
    const { mapRef, isMapReady } = useContext<IAzureMapsContextProps>(AzureMapsContext);
    const [isDataSourceLoaded, setIsDataSourceLoaded] = React.useState(false);
    const { onLocationChange } = props.onLocationChange;
    const CACHE = {};
    const PER_PAGE = 50;
    const [isLoading, setIsLoading] = React.useState(false);
    const [options, setOptions] = React.useState([]);
    const [query, setQuery] = React.useState('');
    const [mapKey, setMapKey] = React.useState('');

    const handleInputChange = (q: string) => {
        setQuery(q);
    };

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
                        draggable: props.isDraggable
                    });

                    mapRef.events.add('mouseover', marker, (event: any) => {
                        const marker = event.target as HtmlMarker & { properties: any };
                        const content = marker.properties.cluster
                            ? `Cluster of ${marker.properties.point_count_abbreviated} markers`
                            : `<div className="container-fluid card">
                                <h4>${marker.properties.name}</h4>
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>Event Date:</td>
                                            <td>${new Date(marker.properties.eventDate).toLocaleDateString("en-US", { month: "long", day: "numeric", year: 'numeric', hour: 'numeric', minute: 'numeric' })}</td>
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

                    mapRef.events.add('drag', marker, (e: any) => {
                        var pos = e.target.options.position;
                        onLocationChange(pos);
                    });

                    mapRef.events.add('mouseout', marker, () => popup.close());

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
        onLocationChange,
        isMapReady]);

    //useEffect(() => {
    //    const getmapkey = async () => {
    //        var key = await MapStore.getKey();
    //        setMapKey(key);
    //    }

    //    getmapkey();
    //});

    function handleLocationChange(e: any) {
        props.onLocationChange(e);
    }

    const handlePagination = (e: any, shownResults: any) => {
        const cachedQuery = CACHE[query];

        // Don't make another request if:
        // - the cached results exceed the shown results
        // - we've already fetched all possible results
        if (
            cachedQuery.options.length > shownResults ||
            cachedQuery.options.length === cachedQuery.total_count
        ) {
            return;
        }

        setIsLoading(true);

        const page = cachedQuery.page + 1;

        makeAndHandleRequest(query, page)
            .then((resp: any) => {
            const options = cachedQuery.options.concat(resp.options);
            CACHE[query] = { ...cachedQuery, options, page };

            setIsLoading(false);
            setOptions(options);
        });
    };

    // `handleInputChange` updates state and triggers a re-render, so
    // use `useCallback` to prevent the debounced search handler from
    // being cancelled.
    const handleSearch = React.useCallback((q) => {
        if (CACHE[q]) {
            setOptions(CACHE[q].options);
            return;
        }

        setIsLoading(true);
        makeAndHandleRequest(q)
            .then((resp: any) => {
            CACHE[q] = { ...resp, page: 1 };

            setIsLoading(false);
            setOptions(resp.options);
        });
    }, []);

    function makeAndHandleRequest(query: string, page: number = 1) {

        var headers = getDefaultHeaders('GET');
        var kk = "5p5HTkSxyEJQS3Jo5n6uVbdtY_zmhItA4QpxWaQh0x8";

        return fetch('https://atlas.microsoft.com/search/address/json?typeahead=true&subscription-key=' + kk + '&api-version=1.0&query=' + query, {
            method: 'GET',
            mode: 'cors',
            headers: headers
        })
            .then((resp) => resp.json() as Promise<SearchAddressData>)
            .then( (addressData) => {
                const options = addressData.results.map((i: any) => ({
                    id: i.id,
                    address: i.address.freeformAddress,
                }));
                const totalResults = addressData.summary.totalResults;
                return { options, totalResults };
            });
    }

    return (
        <>
            <AsyncTypeahead
                id="async-pagination-example"
                isLoading={isLoading}
                labelKey="address"
                maxResults={PER_PAGE - 1}
                minLength={2}
                onInputChange={handleInputChange}
                onPaginate={handlePagination}
                onSearch={handleSearch}
                options={options}
                paginate
                placeholder="Search for a location..."
                renderMenuItemChildren={(option: any) => (
                    <div key={option.id}>
                        <span>{option.address}</span>
                    </div>
                )}
                useCache={false}
            />
            <MapComponent mapOptions={props.mapOptions} isMapKeyLoaded={props.isMapKeyLoaded} onLocationChange={handleLocationChange} />
        </>
    );
};

export default EventCollectionMapController;