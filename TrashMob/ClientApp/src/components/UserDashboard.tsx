import { Component } from 'react';
import * as React from 'react'
import { Link } from 'react-router-dom';

export class UserDashboard extends Component {


    private GetUserEvents(endpoint, token) {

        const headers = new Headers();
        const bearer = `Bearer ${token}`;

        headers.append("Authorization", bearer);

        const options = {
            method: "GET",
            headers: headers
        };

        fetch(endpoint, options)
            .then(response => response.json())
            .then(response => {
                return response;
            }).catch(error => {
                console.error(error);
            });
    }

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
