import { Component } from 'react';
import * as React from 'react'

import { MainEvents } from './MainEvents';
import { MainCarousel } from './MainCarousel';
import MultipleEventsMap from './MultipleEventsMap';
import { Link, RouteComponentProps } from 'react-router-dom';
import EventData from './Models/EventData';
import EventTypeData from './Models/EventTypeData';
import { apiConfig, defaultHeaders, msalClient } from '../store/AuthStore';
import { getUserFromCache } from '../store/accountHandler';

export interface HomeProps extends RouteComponentProps {
}

export interface FetchEventDataState {
    eventList: EventData[];
    eventTypeList: EventTypeData[];
    myAttendanceList: EventData[];
    isLoggedIn: boolean;
    loading: boolean;
}

export class Home extends Component<HomeProps, FetchEventDataState> {
    static displayName = Home.name;

    constructor(props: HomeProps) {
        super(props);
        this.state = { eventList: [], eventTypeList: [], myAttendanceList: [], loading: true, isLoggedIn: false };

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
                        <MultipleEventsMap eventList={this.state.eventList} loading={this.state.loading} />
                    </div>
                </div>
            </div>
        );
    }
}
