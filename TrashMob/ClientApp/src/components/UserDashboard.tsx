import { Component } from 'react';
import * as React from 'react'
import { Link } from 'react-router-dom';

export class UserDashboard extends Component {

    render() {
        return (
            <div>
                <div>
                    <Link to="/addevent">Create a New Event</Link>
                    </div>
                <div>
                    My Upcoming Events
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
