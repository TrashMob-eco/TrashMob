import { Component } from 'react';
import * as React from 'react'

import GoogleMapReact from 'google-map-react';

interface MapProps {
    center: JSX.IntrinsicAttributes;
    zoom: JSX.IntrinsicAttributes;
};

interface MapState {
    mapKey: string;
    loading: boolean;
}

export class NearbyEventsMap extends Component<MapProps, MapState> {
    static defaultProps = {
        center: {
            lat: 59.95,
            lng: 30.33
        },
        zoom: 11
    };

    constructor(props: any) {
        super(props);
        this.state = { mapKey: "", loading: true };

        fetch('api/Maps', {
            method: 'GET',
            headers: {
                Allow: 'GET',
                Accept: 'application/json',
                'Content-Type': 'application/json'
            },
        })
            .then(response => response.json() as Promise<string>)
            .then(data => {
                this.setState({ mapKey: data, loading: false });
            });
    }

    render() {
        return (
            // Important! Always set the container height explicitly
            <div style={{ height: '100vh', width: '100%' }}>
                <GoogleMapReact
                    bootstrapURLKeys={{ key: this.state.mapKey }}
                    defaultCenter={this.props.center}
                    defaultZoom={this.props.zoom}>
                </GoogleMapReact>
            </div>
        );
    }
}
