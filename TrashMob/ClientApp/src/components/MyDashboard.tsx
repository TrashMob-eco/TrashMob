import { Component } from 'react';
import * as React from 'react'

import { Link } from 'react-router-dom';
import { FetchUserEvents } from './FetchUserEvents'
import { MsalAuthenticationTemplate, MsalProvider } from '@azure/msal-react';
import { InteractionType } from '@azure/msal-browser';
import { msalClient } from '../store/AuthStore';

export class MyDashboard extends Component {
    render() {
        return (
            <MsalProvider instance={msalClient} >
            <MsalAuthenticationTemplate interactionType={InteractionType.Popup}>

            <div>
                <div>
                    <Link to="/addevent">Create a New Event</Link>
                </div>
                <div>
                    My Upcoming Events
                    <div>
                        <FetchUserEvents />
                    </div>
                </div>
                <div>
                    My Completed Events
                </div>
                <div>
                    My Stats
                </div>
                </div>
                </MsalAuthenticationTemplate >
            </MsalProvider>
        );
    }
}
