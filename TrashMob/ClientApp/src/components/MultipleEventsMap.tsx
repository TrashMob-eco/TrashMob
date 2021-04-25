import * as React from 'react';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapController from './MapController';
import * as MapStore from '../store/MapStore'
import EventData from './Models/EventData';
import { useEffect, useState } from 'react';
import { data } from 'azure-maps-control';

export interface MultipleEventMapDataState {
    eventList: EventData[];
    loading: boolean;
    center: data.Position;
}

const MultipleEventsMap: React.FC<MultipleEventMapDataState> = (props) => {

    const [isKeyLoaded, setIsKeyLoaded] = useState(false);
    const [mapOptions, setMapOptions] = useState<IAzureMapOptions>();

    // componentDidMount()
    useEffect(() => {
        // simulate fetching subscriptionKey from Key Vault
        async function GetMap() {
            setMapOptions(await MapStore.getOption());
            setIsKeyLoaded(true);
        }

        GetMap();
    }, []);

    return (
        <AzureMapsProvider>
            <>
                <MapController center={props.center} multipleEvents={props.eventList} loading={props.loading} mapOptions={mapOptions} isKeyLoaded={isKeyLoaded} />
            </>
        </AzureMapsProvider>
    );
};

export default MultipleEventsMap
