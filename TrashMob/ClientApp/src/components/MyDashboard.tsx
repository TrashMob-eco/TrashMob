import { Component } from 'react';
import * as React from 'react'

import { Link } from 'react-router-dom';
import { EventsUserOwns } from './EventsUserOwns'
import { EventsUserIsAttending } from './EventsUserIsAttending';
import NearbyEventsMap from './NearbyEventsMap';

export class MyDashboard extends Component {
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
