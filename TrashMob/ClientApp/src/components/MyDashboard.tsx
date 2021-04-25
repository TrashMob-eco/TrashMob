import { Component } from 'react';
import * as React from 'react'

import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { EventsUserOwns } from './EventsUserOwns'
import { EventsUserIsAttending } from './EventsUserIsAttending';
import EventData from './Models/EventData';
import EventTypeData from './Models/EventTypeData';
import { apiConfig, defaultHeaders, msalClient } from '../store/AuthStore';
import { getUserFromCache } from '../store/accountHandler';
import { data } from 'azure-maps-control';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapController from './MapController';

interface Props extends RouteComponentProps<any> {
}

interface MyDashboardDataState {
    title: string;
    myEventList: EventData[];
    myAttendanceList: EventData[];
    eventTypeList: EventTypeData[];
    loading: boolean;
    center: data.Position;
    isKeyLoaded: boolean;
    mapOptions: IAzureMapOptions;
}

class MyDashboard extends Component<Props, MyDashboardDataState> {
    constructor(props: Props) {
        super(props);

        this.state = {
            title: "My Dashboard", loading: false, myEventList: [], myAttendanceList: [], eventTypeList: [], center: new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude), isKeyLoaded: false, mapOptions: null
        };

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = defaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('api/eventtypes', {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<Array<any>>)
                .then(data => {
                    this.setState({ eventTypeList: data });
                });

            fetch('api/events/eventsowned/' + getUserFromCache().id, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<EventData[]>)
                .then(data => {
                    this.setState({ myEventList: data, loading: false });
                });

            fetch('api/events/eventsuserisattending/' + getUserFromCache().id, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<EventData[]>)
                .then(data => {
                    this.setState({ myAttendanceList: data, loading: false });
                })

        });

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

    handleLocationChange = (point: data.Position) => {
        // do nothing
    }

    render() {
        const data = this.state;
        return (
            <div>
                <div>
                    <Link to="/createevent">Create a New Event</Link>
                </div>
                <div>
                    Events I Own
                    <div>
                        <EventsUserOwns eventList={data.myEventList} eventTypeList={this.state.eventTypeList} loading={data.loading} />
                    </div>
                </div>
                <div>
                    <AzureMapsProvider>
                        <>
                            <MapController center={this.state.center} multipleEvents={data.myEventList} loading={this.state.loading} mapOptions={this.state.mapOptions} isKeyLoaded={this.state.isKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={this.handleLocationChange} />
                        </>
                    </AzureMapsProvider>
                </div>
                <div>
                    Events I am Attending
                    <div>
                        <EventsUserIsAttending eventList={data.myAttendanceList} eventTypeList={this.state.eventTypeList} loading={data.loading} />
                    </div>
                </div>
                <AzureMapsProvider>
                    <>
                        <MapController center={this.state.center} multipleEvents={data.myAttendanceList} loading={this.state.loading} mapOptions={this.state.mapOptions} isKeyLoaded={this.state.isKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={this.handleLocationChange} />
                    </>
                </AzureMapsProvider>
                <div>
                    My Stats
                </div>
            </div>
        );
    }
}

export default withRouter(MyDashboard);