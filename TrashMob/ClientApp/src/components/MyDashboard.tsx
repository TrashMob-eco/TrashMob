import { Component } from 'react';
import * as React from 'react'

import { Link } from 'react-router-dom';
import { FetchUserEvents } from './FetchUserEvents'

export class MyDashboard extends Component {
    render() {
        return (
            <div>
                <div>
                    <Link to="/addevent">Create a New Event</Link>
                </div>
                <div>
                    My Upcoming Events
                    <div>
                    {/*    <FetchUserEvents />*/}
                    </div>
                </div>
                <div>
                    My Completed Events
                </div>
                <div>
                    My Stats
                </div>
            </div>
        );
    }
}
