import { Component } from 'react';
import * as React from 'react'

import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { EventsUserOwns } from './EventsUserOwns'
import { EventsUserIsAttending } from './EventsUserIsAttending';
import NearbyEventsMap from './NearbyEventsMap';

interface Props extends RouteComponentProps<any> {
}

interface MyDashboardDataState {
    title: string;
    loading: boolean;
}

class MyDashboard extends Component<Props, MyDashboardDataState> {
    constructor(props: Props) {
        super(props);

        this.state = {
            title: "My Dashboard", loading: false
        };
    }

    render() {
        return (
            <div>
                <div>
                    <Link to="/createevent">Create a New Event</Link>
                </div>
                <div>
                    Events I Own
                    <div>
                        <EventsUserOwns />
                    </div>
                </div>
                <div>
                    Events I am Attending
                    <div>
                        <EventsUserIsAttending />
                    </div>
                </div>
                <div>
                    <NearbyEventsMap />
                </div>
                <div>
                    My Stats
                </div>
            </div>
        );
    }
}

export default withRouter(MyDashboard);