import { useEffect, useState } from 'react';
import * as React from 'react';
import * as MapStore from '../store/MapStore'
import {
    AzureMap,
    AzureMapDataSourceProvider,
    AzureMapFeature,
    AzureMapLayerProvider,
    AzureMapsProvider,
    IAzureDataSourceChildren,
    IAzureMapFeature,
    IAzureMapLayerType,
} from 'react-azure-maps';
import { data, SymbolLayerOptions } from 'azure-maps-control';
import EventData from './Models/EventData';

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

const addMarkers = (eventList: EventData[]): MapStore.pinPoint[] => {
    return eventList.map((mobEvent) => {
        var pin = new MapStore.pinPoint();
        pin.position = new data.Position(mobEvent.latitude, mobEvent.longitude);
        pin.eventName = mobEvent.name;
        return pin;
    })
}

export interface MultipleEventMapDataState {
    eventList: EventData[];
    loading: boolean;
}

const MultipleEventsMap: React.FC<MultipleEventMapDataState> = (props) => {
    const [markers, setMarkers] = useState([]);
    const [markersLayer] = useState<IAzureMapLayerType>('SymbolLayer');
    const [layerOptions, setLayerOptions] = useState<SymbolLayerOptions>(MapStore.memoizedOptions);
    const [isKeyLoaded, setIsKeyLoaded] = useState(false);

    useEffect(() => {
        async function getOpt() {
            MapStore.option.authOptions = await MapStore.getOption();
            setIsKeyLoaded(true);
        }
        getOpt();
    }, [])

    useEffect(() => {
       if (!props.loading) {
            const markerList = addMarkers(props.eventList)
            setMarkers(markerList);
        }
    }, [props.loading, props.eventList])

    const memoizedMarkerRender: IAzureDataSourceChildren = React.useMemo(
        (): any => markers.map((marker) => {
            return renderPoint(marker.position, marker.eventName);
        }),
        [markers],
    );

    return (
        <>
            <AzureMapsProvider>
                <div style={styles.map}>
                    {!isKeyLoaded && <div>Map is loading.</div>}
                    {isKeyLoaded && <AzureMap options={MapStore.option}>
                        <AzureMapDataSourceProvider
                            id={'trashMob AzureMapDataSourceProvider'}
                            options={{ cluster: true, clusterRadius: 2 }}
                        >
                            <AzureMapLayerProvider
                                id={'trashMob AzureMapLayerProvider'}
                                options={layerOptions}
                                type={markersLayer}
                            />
                            {memoizedMarkerRender}
                        </AzureMapDataSourceProvider>
                    </AzureMap>}
                </div>
            </AzureMapsProvider>
        </>
    );
};

const styles = {
    map: {
        height: 300,
    },
    buttonContainer: {
        display: 'grid',
        gridAutoFlow: 'column',
        gridGap: '10px',
        gridAutoColumns: 'max-content',
        padding: '10px 0',
        alignItems: 'center',
    },
    button: {
        height: 35,
        width: 80,
        backgroundColor: '#68aba3',
        'text-align': 'center',
    },
};

export default MultipleEventsMap
