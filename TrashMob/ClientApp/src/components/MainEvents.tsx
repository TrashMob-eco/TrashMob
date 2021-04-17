import { Component } from 'react';
import * as React from 'react'
import { Link } from 'react-router-dom';
import EventData from './Models/EventData';
import { getUserFromCache } from '../store/accountHandler';
import EventAttendeeData from './Models/EventAttendeeData';
import { apiConfig, defaultHeaders, msalClient } from '../store/AuthStore';
import { getEventType } from '../store/eventTypeHelper';
import { FetchEventDataState } from './Home';


export class MainEvents extends Component<FetchEventDataState> {

    constructor(props: FetchEventDataState) {
        super(props);
        this.state = { eventList: this.props.eventList, eventTypeList: this.props.eventTypeList, loading: this.props.loading };

        // This binding is necessary to make "this" work in the callback  
        this.handleAttend = this.handleAttend.bind(this);
    }

    private addAttendee(eventId: string) {

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {

            var user = getUserFromCache();
            var eventAttendee = new EventAttendeeData();
            eventAttendee.userId = user.id;
            eventAttendee.eventId = eventId;

            var data = JSON.stringify(eventAttendee);

            const headers = defaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            // POST request for Add EventAttendee.  
            fetch('api/EventAttendees', {
                method: 'POST',
                body: data,
                headers: headers,
            }).then((response) => response.json())
        })

    }

    private handleAttend(eventId: string) {

        var accounts = msalClient.getAllAccounts();

        if (accounts === null || accounts.length === 0) {
            msalClient.loginRedirect().then(() => {
                this.addAttendee(eventId);
            })
        }
        else {
            this.addAttendee(eventId);
        }
    }

    public render() {
        let contents = this.props.loading
            ? <p><em>Loading...</em></p>
            : this.renderEventsTable(this.props.eventList);

        return (
            <div>
                <h1 id="tabelLabel" >Upcoming Events</h1>
                {contents}
            </div>
        );
    }

    private renderEventsTable(events: EventData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Date</th>
                            <th>Event Type</th>
                            <th>City</th>
                            <th>Region</th>
                            <th>Country</th>
                        </tr>
                    </thead>
                    <tbody>
                        {events.map(mobEvent =>
                            <tr key={mobEvent.id.toString()}>
                                <td>{mobEvent.name}</td>
                                <td>{mobEvent.eventDate}</td>
                                <td>{getEventType(this.props.eventTypeList, mobEvent.eventTypeId)}</td>
                                <td>{mobEvent.city}</td>
                                <td>{mobEvent.region}</td>
                                <td>{mobEvent.country}</td>
                                <td>
                                    <Link to={`/eventdetails/${mobEvent.id}`}>Details</Link>
                                    <button className="btn" onClick={() => this.handleAttend(mobEvent.id)}>Attend</button>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }
}
