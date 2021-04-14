import { Component } from 'react';
import * as React from 'react'
import { Link } from 'react-router-dom';
import EventData from './Models/EventData';
import { getUserFromCache } from '../store/accountHandler';
import EventAttendeeData from './Models/EventAttendeeData';
import EventTypeData from './Models/EventTypeData';

interface PropsType { };

interface FetchEventDataState {
    eventList: EventData[];
    eventTypeList: EventTypeData[];
    loading: boolean;
}

export class MainEvents extends Component<PropsType, FetchEventDataState> {

    constructor(props: FetchEventDataState) {
        super(props);
        this.state = { eventList: [], eventTypeList: [], loading: true };


        fetch('api/eventtypes', {
            method: 'GET',
            headers: {
                Allow: 'GET',
                Accept: 'application/json',
                'Content-Type': 'application/json'
            },
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                this.setState({ eventTypeList: data });
            });

        fetch('api/Events', {
            method: 'GET',
            headers: {
                Allow: 'GET',
                Accept: 'application/json',
                'Content-Type': 'application/json'
            },
        })
            .then(response => response.json() as Promise<EventData[]>)
            .then(data => {
                this.setState({ eventList: data, loading: false });
            });

        // This binding is necessary to make "this" work in the callback  
        this.handleAttend = this.handleAttend.bind(this);
    }

    private handleAttend(eventId: string) {
        var user = getUserFromCache();

        var eventAttendee = new EventAttendeeData();
        eventAttendee.attendeeId = user.Id;
        eventAttendee.eventId = eventId;

        var data = JSON.stringify(eventAttendee);

        // POST request for Add EventAttendee.  
        fetch('api/EventAttendees', {
            method: 'POST',
            body: data,
            headers: {
                Allow: 'POST',
                Accept: 'application/json, text/plain',
                'Content-Type': 'application/json'
            },
        }).then((response) => response.json())
    }

    private getEventType(eventTypeId: any): string {
        return this.state.eventTypeList.find(et => et.id === eventTypeId).name;
    }


    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderEventsTable(this.state.eventList);

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
                                <td>{this.getEventType(mobEvent.eventTypeId)}</td>
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
