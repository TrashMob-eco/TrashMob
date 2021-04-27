import { Component } from 'react';
import * as React from 'react'

import { MainEvents } from './MainEvents';
import { MainCarousel } from './MainCarousel';
import { Link, RouteComponentProps } from 'react-router-dom';
import EventData from './Models/EventData';
import EventTypeData from './Models/EventTypeData';
import { apiConfig, defaultHeaders, msalClient } from '../store/AuthStore';
import { getUserFromCache } from '../store/accountHandler';
import { data } from 'azure-maps-control';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapController from './MapController';

export interface HomeProps extends RouteComponentProps {
}

export interface HomeDataState {
    eventList: EventData[];
    eventTypeList: EventTypeData[];
    myAttendanceList: EventData[];
    isLoggedIn: boolean;
    loading: boolean;
    isKeyLoaded: boolean;
    center: data.Position;
    mapOptions: IAzureMapOptions;
    currentUserId: string;
}

export class Home extends Component<HomeProps, HomeDataState> {
    static displayName = Home.name;

    constructor(props: HomeProps) {
        super(props);
        this.state = { eventList: [], eventTypeList: [], myAttendanceList: [], loading: true, isLoggedIn: false, center: new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude), isKeyLoaded: false, mapOptions: null, currentUserId: ""};

        const headers = defaultHeaders('GET');
        this.getEventTypes();

        fetch('api/Events/active', {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<EventData[]>)
            .then(data => {
                this.setState({ eventList: data, loading: false });
            });

        // If the user is logged in, get the events they are attending
        var accounts = msalClient.getAllAccounts();

        if (accounts !== null && accounts.length > 0) {
            var userId = getUserFromCache().id;
            this.setState({ currentUserId: userId })
            var request = {
                scopes: apiConfig.b2cScopes,
                account: accounts[0]
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = defaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('api/events/eventsuserisattending/' + getUserFromCache().id, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<EventData[]>)
                    .then(data => {
                        this.setState({ myAttendanceList: data, loading: false, isLoggedIn: true });
                    })
            });
        }

        MapStore.getOption().then(opts => {
            this.setState({ mapOptions: opts });
            this.setState({ isKeyLoaded: true });
        })
    }

    componentDidMount() {
        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(position => {
                var point = new data.Position(position.coords.longitude, position.coords.latitude);
                this.setState({ center: point })
            });
        } else {
            console.log("Not Available");
        }
    }

    private getEventTypes() {
        const headers = defaultHeaders('GET');

        fetch('api/eventtypes', {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                this.setState({ eventTypeList: data });
            });
    }

    handleLocationChange = (point: data.Position) => {
        // do nothing
    }

    render() {
        const data = this.state;
        return (

            <div>
                <div>
                    <MainCarousel />
                </div>
                <div>
                    <div>
                        <Link to="/createevent">Create a New Event</Link>
                    </div>
                    <div style={{ width: 50 + '%' }}>
                        <MainEvents eventList={data.eventList} eventTypeList={data.eventTypeList} myAttendanceList={data.myAttendanceList} loading={data.loading} isLoggedIn={data.isLoggedIn} />
                    </div>
                    <div style={{ width: 50 + '%' }}>
                        <AzureMapsProvider>
                            <>
                                <MapController center={this.state.center} multipleEvents={this.state.eventList} loading={this.state.loading} mapOptions={this.state.mapOptions} isKeyLoaded={this.state.isKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={this.handleLocationChange} currentUserId={this.state.currentUserId}  />
                            </>
                        </AzureMapsProvider>
                    </div>
                </div>
            </div>
        );
    }
}
