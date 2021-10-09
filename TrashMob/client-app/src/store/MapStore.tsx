import { AuthenticationType, SymbolLayerOptions, data } from 'azure-maps-control'
import { getDefaultHeaders } from './AuthStore';
import {IAzureMapOptions} from 'react-azure-maps';

export async function getOption(): Promise<IAzureMapOptions> {
    var authOptions = {
        authType: AuthenticationType.subscriptionKey,
        subscriptionKey: await getKey()
    }
    return authOptions;
}

export async function getKey(): Promise<string> {
    var key = '';

    const headers = getDefaultHeaders('GET');

    await fetch('/api/maps', {
        method: 'GET',
        headers: headers,
    })
        .then(response => response.json() as Promise<string>)
        .then(data => {
            key = data;
        });
    return key;
}

export const memoizedOptions: SymbolLayerOptions = {
    textOptions: {
        textField: ['get', 'title'], //Specify the property name that contains the text you want to appear with the symbol.
        offset: [0, 1.2],
        color: '#6642f5',
        size: 16,
        ignorePlacement: true, //To skip label collision detection for better performance.
        allowOverlap: true    //To ensure smooth rendering when dragging, allow symbol to overlap all other symbols on the map.
    },
};

export const defaultLongitude: number = -100.01;
export const defaultLatitude: number = 45.01;
export const defaultUserLocationZoom: number = 10;

export const option: IAzureMapOptions = {
    authOptions: {},
    center: [defaultLongitude, defaultLatitude],
    zoom: defaultUserLocationZoom,
    view: 'Auto',
}

export class pinPoint {
    position: data.Position = new data.Position(0,0);
    eventName: string = "";
}

export class markerPoint {
    position: data.Point = new data.Point(new data.Position(0,0));
    properties: {
        title: string,
        icon: string,
        type: string,
    } = { title: "", icon: "", type: ""};
}