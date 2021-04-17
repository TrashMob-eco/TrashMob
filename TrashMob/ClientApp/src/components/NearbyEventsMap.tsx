import { useEffect, memo, useMemo, useState } from 'react';
import * as React from 'react';
import {
    AzureMap,
    AzureMapDataSourceProvider,
    AzureMapFeature,
    AzureMapHtmlMarker,
    AzureMapLayerProvider,
    AzureMapsProvider,
    IAzureDataSourceChildren,
    IAzureMapFeature,
    IAzureMapHtmlMarkerEvent,
    IAzureMapLayerType,
    IAzureMapOptions,
} from 'react-azure-maps';
import { data, HtmlMarkerOptions, SymbolLayerOptions } from 'azure-maps-control';
import { Button, Chip } from '@material-ui/core';
import { getOption } from '../store/MapStore';

const point1 = new data.Position(-100.01, 45.01);
const point2 = new data.Position(-120.2, 45.1);
const point3 = new data.Position(-120.2, 50.1);
const point4 = new data.Position(-126.2, 55.1);

function clusterClicked(e: any) {
    console.log('clusterClicked', e);
}

const onClick = (e: any) => {
    console.log('You click on: ', e);
};

function azureHtmlMapMarkerOptions(coordinates: data.Position): HtmlMarkerOptions {
    return {
        position: coordinates,
        text: 'My text',
        title: 'Title',
    };
}

const memoizedOptions: SymbolLayerOptions = {
    textOptions: {
        textField: ['get', 'title'], //Specify the property name that contains the text you want to appear with the symbol.
        offset: [0, 1.2],
    },
};

const eventToMarker: Array<IAzureMapHtmlMarkerEvent> = [{ eventName: 'click', callback: onClick }];

const renderPoint = (coordinates: data.Position): IAzureMapFeature => {
    const rendId = Math.random();
    return (
        <AzureMapFeature
            key={rendId}
            id={rendId.toString()}
            type="Point"
            coordinate={coordinates}
            properties={{
                title: 'Pin',
                icon: 'pin-round-blue',
            }}
        />
    );
};

function renderHTMLPoint(coordinates: data.Position): any {
    const rendId = Math.random();
    return (
        <AzureMapHtmlMarker
            key={rendId}
            markerContent={<div className="pulseIcon"></div>}
            options={{ ...azureHtmlMapMarkerOptions(coordinates) } as any}
            events={eventToMarker}
        />
    );
}

const colorValue = () =>
    '#000000'.replace(/0/g, function () {
        return (~~(Math.random() * 16)).toString(16);
    });
const markersStandardImages = [
    `marker-black`,
    `marker-blue`,
    `marker-darkblue`,
    `marker-red`,
    `marker-yellow`,
    `pin-blue`,
    `pin-darkblue`,
    `pin-red`,
    `pin-round-blue`,
    `pin-round-darkblue`,
    `pin-round-red`,
];

const rand = () => (markersStandardImages[Math.floor(Math.random() * markersStandardImages.length)])

const option: IAzureMapOptions = {
    authOptions: {},
    center: [-100.01, 45.01],
    zoom: 2,
    view: 'Auto',
}

const NearbyEventsMap: React.FC = () => {
    const [markers, setMarkers] = useState([point1, point2, point3]);
    const [htmlMarkers, setHtmlMarkers] = useState([point4]);
    const [markersLayer] = useState<IAzureMapLayerType>('SymbolLayer');
    const [layerOptions, setLayerOptions] = useState<SymbolLayerOptions>(memoizedOptions);
    const [isKeyLoaded, setIsKeyLoaded] = useState(false);

    useEffect(() => {
        async function getOpt() {
            option.authOptions = await getOption();
            setIsKeyLoaded(true);
        }
        getOpt();
    }, [])

    const addRandomMarker = () => {
        const randomLongitude = Math.floor(Math.random() * (-80 - -120) + -120);
        const randomLatitude = Math.floor(Math.random() * (30 - 65) + 65);
        const newPoint = new data.Position(randomLongitude, randomLatitude);
        setMarkers([...markers, newPoint]);
    };

    const addRandomHTMLMarker = () => {
        const randomLongitude = Math.floor(Math.random() * (-80 - -120) + -120);
        const randomLatitude = Math.floor(Math.random() * (30 - 65) + 65);
        const newPoint = new data.Position(randomLongitude, randomLatitude);
        setHtmlMarkers([...htmlMarkers, newPoint]);
    };

    const removeAllMarkers = () => {
        setMarkers([]);
        setHtmlMarkers([]);
    };

    const memoizedMarkerRender: IAzureDataSourceChildren = useMemo(
        (): any => markers.map((marker) => renderPoint(marker)),
        [markers],
    );

    const memoizedHtmlMarkerRender: IAzureDataSourceChildren = useMemo(
        (): any => htmlMarkers.map((marker) => renderHTMLPoint(marker)),
        [htmlMarkers],
    );

    return (
        <>
            <div style={styles.buttonContainer}>
                <Button size="small" variant="contained" color="primary" onClick={addRandomMarker}>
                    {' '}
          MARKER POINT
        </Button>
                <Button size="small" variant="contained" color="primary" onClick={addRandomHTMLMarker}>
                    {' '}
          HTML MARKER
        </Button>
                <Button
                    size="small"
                    variant="contained"
                    color="primary"
                    onClick={() =>
                        setLayerOptions({
                            textOptions: {
                                color: colorValue(),
                                size: 16,
                            },
                        })
                    }
                >
                    {' '}
          Text Options
        </Button>
                <Button
                    size="small"
                    variant="contained"
                    color="primary"
                    onClick={() =>
                        setLayerOptions({
                            iconOptions: {
                                image: rand(),
                            },
                        })
                    }
                >
                    {' '}
          ICON OPTIONS
        </Button>
                <Button size="small" variant="contained" color="primary" onClick={removeAllMarkers}>
                    {' '}
          REMOVE ALL
        </Button>
                <Chip label={`Markers Point on map: ${markers.length}`} />
                <Chip label={`Markers HTML on map: ${htmlMarkers.length}`} />
            </div>
            <AzureMapsProvider>
                <div style={styles.map}>
                    {!isKeyLoaded && <div>Map is loading.</div>}
                    {isKeyLoaded && <AzureMap options={option}>
                        <AzureMapDataSourceProvider
                            events={{
                                dataadded: (e: any) => {
                                    console.log('Data on source added', e);
                                },
                            }}
                            id={'markersExample AzureMapDataSourceProvider'}
                            options={{ cluster: true, clusterRadius: 2 }}
                        >
                            <AzureMapLayerProvider
                                id={'markersExample AzureMapLayerProvider'}
                                options={layerOptions}
                                events={{
                                    click: clusterClicked,
                                    dbclick: clusterClicked,
                                }}
                                lifecycleEvents={{
                                    layeradded: () => {
                                        console.log('LAYER ADDED TO MAP');
                                    },
                                }}
                                type={markersLayer}
                            />
                            {memoizedMarkerRender}
                            {memoizedHtmlMarkerRender}
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

export default NearbyEventsMap
