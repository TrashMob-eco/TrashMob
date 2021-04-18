import { Component } from 'react';
import * as React from 'react'

import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { EventsUserOwns } from './EventsUserOwns'
import { EventsUserIsAttending } from './EventsUserIsAttending';
import MultipleEventsMap from './MultipleEventsMap';
import EventData from './Models/EventData';
import EventTypeData from './Models/EventTypeData';
import { apiConfig, defaultHeaders, msalClient } from '../store/AuthStore';
import { getUserFromCache } from '../store/accountHandler';

interface Props extends RouteComponentProps<any> {
}

interface MyDashboardDataState {
    title: string;
    myEventList: EventData[];
    myAttendanceList: EventData[];
    eventTypeList: EventTypeData[];
    loading: boolean;
}

class MyDashboard extends Component<Props, MyDashboardDataState> {
    constructor(props: Props) {
        super(props);

        this.state = {
            title: "My Dashboard", loading: false, myEventList: [], myAttendanceList: [], eventTypeList: [],
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
                        <EventsUserOwns eventList={data.myEventList} eventTypeList={data.eventTypeList} loading={data.loading} />
                    </div>
                </div>
                <div>
                    <MultipleEventsMap eventList={data.myEventList} loading={data.loading} />
                </div>
                <div>
                    Events I am Attending
                    <div>
                        <EventsUserIsAttending eventList={data.myAttendanceList} eventTypeList={data.eventTypeList} loading={data.loading} />
                    </div>
                </div>
                <div>
                    <MultipleEventsMap eventList={data.myAttendanceList} loading={data.loading} />
                </div>
                <div>
                    My Stats
                </div>
            </div>
        );
    }
}

export default withRouter(MyDashboard);