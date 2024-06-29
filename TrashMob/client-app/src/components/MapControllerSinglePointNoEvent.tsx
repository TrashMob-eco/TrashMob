import { useContext, useEffect } from 'react';
import * as React from 'react';
import { AzureMapsContext, IAzureMapOptions, IAzureMapsContextProps } from 'react-azure-maps';
import { data, source, HtmlMarker } from 'azure-maps-control';
import MapComponent from './MapComponent';
import * as MapStore from '../store/MapStore'
import UserData from './Models/UserData';
import { HtmlMarkerLayer } from './HtmlMarkerLayer/src/layer/HtmlMarkerLayer'
import { AsyncTypeahead } from 'react-bootstrap-typeahead';
import { getDefaultHeaders } from '../store/AuthStore';
import SearchAddressData from './Models/SearchAddressData';
import { useMutation } from '@tanstack/react-query';
import { AzureMapSearchAddress } from '../services/maps';

interface MapControllerProps {
    mapOptions: IAzureMapOptions | undefined
    center: data.Position;
    isMapKeyLoaded: boolean
    latitude: number;
    longitude: number;
    onLocationChange: any;
    currentUser: UserData;
    isUserLoaded: boolean;
    isDraggable: boolean;
}

export const MapControllerSinglePointNoEvent: React.FC<MapControllerProps> = (props) => {
    // Here you use mapRef from context
    const { mapRef, isMapReady } = useContext<IAzureMapsContextProps>(AzureMapsContext);
    const [isDataSourceLoaded, setIsDataSourceLoaded] = React.useState(false);
    const CACHE = {};
    const PER_PAGE = 50;
    const [isLoading, setIsLoading] = React.useState(false);
    const [options, setOptions] = React.useState([]);
    const [query, setQuery] = React.useState('');
    const mapKeyRef = React.useRef('');
    const [isPrevLoaded, setIsPrevLoaded] = React.useState<boolean>(false);

    const azureMapSearchAddress = useMutation({
        mutationKey: AzureMapSearchAddress().key,
        mutationFn: AzureMapSearchAddress().service,
    });

    const handleInputChange = (q: string) => {
        setQuery(q);
    };

    useEffect(() => {
        if (props.mapOptions) {
            const key = props.mapOptions?.subscriptionKey ?? "";
            mapKeyRef.current = key;
        }
    }, [props.mapOptions]);

    useEffect(() => {
        if (mapRef && props.isMapKeyLoaded && !isDataSourceLoaded && isMapReady) {

            // Simple Camera options modification
            mapRef.setCamera({ center: props.center, zoom: MapStore.defaultUserLocationZoom });

            var dataSourceRef = new source.DataSource("mainDataSource", { cluster: true });

            mapRef.sources.add(dataSourceRef);
            setIsDataSourceLoaded(true);

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

                    mapRef.events.add('dragend', marker, (e: any) => {
                        var pos = e.target.options.position;
                        handleLocationChange(pos);
                    });

                    mapRef.events.add('click', marker, (e: any) => {
                        var pos = e.target.options.position;
                        handleLocationChange(pos);
                    });

                    return marker
                }
            });

            // Add marker layer to the map.
            mapRef.layers.add(markerLayer);

            var position = new data.Point(new data.Position(props.longitude, props.latitude));

            dataSourceRef.add(new data.Feature(position));
        }
    }, [mapRef,
        props.center,
        props.isMapKeyLoaded,
        props.currentUser,
        props.isUserLoaded,
        props.longitude,
        props.latitude,
        isDataSourceLoaded,
        props.isDraggable,
        // eslint-disable-next-line
        handleLocationChange,
        isMapReady]);

    useEffect(() => {
        if (mapRef && props.isMapKeyLoaded && isDataSourceLoaded && isMapReady && !isPrevLoaded) {
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
        // eslint-disable-next-line
    }, []);

    async function makeAndHandleRequest(query: string, page: number = 1) {
        const res = await azureMapSearchAddress.mutateAsync({ azureKey: mapKeyRef.current, query });
        const options = res.data.results.map((i: any) => ({ id: i.id, displayAddress: i.address.freeformAddress, position: i.position }));
        const totalResults = res.data.summary.totalResults;
        return { options, totalResults };
    }

    function handleSelectedChanged(val: any) {
        if (val && val.length > 0) {
            var position = val[0].position;
            var point = new data.Position(position.lon, position.lat)
            handleLocationChange(point);
        }
    }

    return (
        <>
            {props.isDraggable ? <div >
                <AsyncTypeahead
                    id="async-pagination-example"
                    isLoading={isLoading}
                    labelKey="displayAddress"
                    maxResults={PER_PAGE - 1}
                    minLength={2}
                    onInputChange={handleInputChange}
                    onPaginate={handlePagination}
                    onSearch={handleSearch}
                    onChange={(selected) => handleSelectedChanged(selected)}
                    options={options}
                    paginate
                    placeholder="Search for a location..."
                    renderMenuItemChildren={(option: any) => (
                        <div key={option.id}>
                            <span>{option.displayAddress}</span>
                        </div>
                    )}
                    useCache={false}
                />
            </div> : null}
            <MapComponent mapOptions={props.mapOptions} isMapKeyLoaded={props.isMapKeyLoaded} onLocationChange={handleLocationChange} />
        </>
    );
};

export default MapControllerSinglePointNoEvent;