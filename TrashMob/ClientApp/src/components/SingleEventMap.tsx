import { useEffect, useState } from 'react';
import * as React from 'react';
import { AzureMap, AzureMapDataSourceProvider, AzureMapFeature, AzureMapLayerProvider, AzureMapsProvider, IAzureDataSourceChildren, IAzureMapFeature, IAzureMapLayerType } from 'react-azure-maps'
import { data, SymbolLayerOptions } from 'azure-maps-control';
import * as MapStore from '../store/MapStore'

const renderPoint = (coordinates: data.Position, eventName: string): IAzureMapFeature => {
    const rendId = Math.random();

    return (
        <AzureMapFeature
            key={rendId}
            id={rendId.toString()}
            type="Point"
            coordinate={coordinates}
            properties={{
                title: eventName,
                icon: 'pin-round-blue',
            }}
        />
    );
};

export interface SingleEventMapDataState {
    eventName: string;
    latitude: number;
    longitude: number;
    loading: boolean;
    onLocationChange: any;
}

const SingleEventMap: React.FC<SingleEventMapDataState> = (props) => {
    const [marker, setMarker] = useState<MapStore.pinPoint>();
    const [markersLayer] = useState<IAzureMapLayerType>('SymbolLayer');
    const [layerOptions, setLayerOptions] = useState<SymbolLayerOptions>(MapStore.memoizedOptions);
    const [isKeyLoaded, setIsKeyLoaded] = useState(false);
    const [markerRender, setMarkerRender] = useState<IAzureMapFeature[]>();

    // componentDidMount()
    useEffect(() => {
        // simulate fetching subscriptionKey from Key Vault
        async function GetMap() {
            MapStore.option.authOptions = await MapStore.getOption()
            setIsKeyLoaded(true);
        }

        GetMap();
    }, []);

    useEffect(() => {
        if (!props.loading && props.eventName !== '') {
            var pin = new MapStore.pinPoint();
            pin.position = new data.Position(props.longitude, props.latitude);
            pin.eventName = props.eventName
            setMarker(pin);
        }
    }, [props.loading, props.latitude, props.longitude, props.eventName])


    useEffect((): any => {
        var isCancelled = false;

        const renderP = () => {
            if (!isCancelled && !props.loading && marker) {
                var point = renderPoint(marker.position, marker.eventName);
                var points: IAzureMapFeature[] = [];
                points.push(point);
                setMarkerRender(points)
            }
        }

        renderP();

        return () => { isCancelled = true; };
    },
        [marker, props.loading],
    );

    function getCoordinates(e: any) {
        console.log('Clicked on:', e.position);
        props.onLocationChange(e.position);
    }

    return (
        <>
            <AzureMapsProvider>
                <div style={styles.map}>
                    {!isKeyLoaded && <div>Map is loading.</div>}
                    {isKeyLoaded && <AzureMap options={MapStore.option} events={{ click: getCoordinates }}>
                        <AzureMapDataSourceProvider
                            id={'trashMob AzureMapDataSourceProvider'}
                            options={{ cluster: true, clusterRadius: 2 }}
                        >
                            <AzureMapLayerProvider
                                id={'trashMob AzureMapLayerProvider'}
                                options={layerOptions}
                                type={markersLayer}
                            />
                            {markerRender}
                        </AzureMapDataSourceProvider>
                    </AzureMap>}
                </div>
            </AzureMapsProvider>
        </>
    );
}

const styles = {
    map: {
        height: 300,
    },
};

export default SingleEventMap
