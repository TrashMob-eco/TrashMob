import { Component } from 'react';
import * as React from 'react'

import { MainEvents } from './MainEvents';
import { MainCarousel } from './MainCarousel';
import NearbyEventsMap from './NearbyEventsMap';
import { Link, RouteComponentProps } from 'react-router-dom';
import EventData from './Models/EventData';
import EventTypeData from './Models/EventTypeData';
import { defaultHeaders } from '../store/AuthStore';

export interface HomeProps extends RouteComponentProps {
}

export interface FetchEventDataState {
    eventList: EventData[];
    eventTypeList: EventTypeData[];
    loading: boolean;
}

export class Home extends Component<HomeProps, FetchEventDataState> {
    static displayName = Home.name;

    constructor(props: HomeProps) {
        super(props);
        this.state = { eventList: [], eventTypeList: [], loading: true };

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
                    <div style={{ width: 50+'%' }}>
                        <MainEvents eventList={data.eventList} eventTypeList={data.eventTypeList} loading={data.loading} />
                    </div>
                    <div style={{ width: 50+'%' }}>
                        <NearbyEventsMap eventList={data.eventList} />
                    </div>
                </div>
            </div>
        );
    }
}
