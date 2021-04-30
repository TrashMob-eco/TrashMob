import { Component } from 'react';
import * as React from 'react'

import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { UserEvents } from './UserEvents'
import EventData from './Models/EventData';
import EventTypeData from './Models/EventTypeData';
import { apiConfig, defaultHeaders, msalClient } from '../store/AuthStore';
import { getUserFromCache, verifyAccount } from '../store/accountHandler';
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
    currentUserId: string;
}

class MyDashboard extends Component<Props, MyDashboardDataState> {
    constructor(props: Props) {
        super(props);

        this.state = {
            title: "My Dashboard", loading: false, myEventList: [], myAttendanceList: [], eventTypeList: [], center: new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude), isKeyLoaded: false, mapOptions: null, currentUserId: getUserFromCache().id
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

            var user = getUserFromCache();

            fetch('api/events/userevents/' + user.id, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<EventData[]>)
                .then(data => {
                    this.setState({ myEventList: data, loading: false });
                });
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

    private loadEvents = () => {
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = defaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);
            var user = getUserFromCache();

            fetch('api/events/userevents/' + user.id, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<EventData[]>)
                .then(data => {
                    this.setState({ myEventList: data, loading: false });
                });
        });
    }

    private handleLocationChange = (point: data.Position) => {
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
                    <div>
                        <UserEvents history={this.props.history} location={this.props.location} match={this.props.match} eventList={data.myEventList} eventTypeList={this.state.eventTypeList} loading={data.loading} onEventListChanged={this.loadEvents} />
                    </div>
                </div>
                <div>
                    <AzureMapsProvider>
                        <>
                            <MapController center={this.state.center} multipleEvents={data.myEventList} loading={this.state.loading} mapOptions={this.state.mapOptions} isKeyLoaded={this.state.isKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={this.handleLocationChange} currentUserId={this.state.currentUserId} />
                        </>
                    </AzureMapsProvider>
                </div>
                <div>
                    My Stats
                </div>
            </div>
        );
    }
}

export default withRouter(MyDashboard);