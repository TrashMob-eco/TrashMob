import { Component } from 'react';
import * as React from 'react'

import { Link } from 'react-router-dom';
import EventData from './Models/EventData';  
import { getUserFromCache } from '../store/accountHandler';

interface PropsType { };

interface FetchEventDataState {
    eventList: EventData[];
    loading: boolean;
    token: string;
}

export class EventsUserOwns extends Component<PropsType, FetchEventDataState> {

    constructor(props: FetchEventDataState) {
        super(props);
        this.state = { eventList: [], loading: true, token: "" };

        var token = "";
        const headers = new Headers();
        const bearer = `Bearer ${token}`;

        headers.append("Authorization", bearer);
        headers.append("Allow", 'GET');
        headers.append("Accept", 'application/json');
        headers.append("Content-Type", 'application/json');

        fetch('api/events/eventsowned/' + getUserFromCache().id, {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<EventData[]>)
            .then(data => {
                this.setState({ eventList: data, loading: false });
            });

        // This binding is necessary to make "this" work in the callback  
        this.handleDelete = this.handleDelete.bind(this);
    }

    // Handle Delete request for an event  
    private handleDelete(id: string, name: string) {
        if (!window.confirm("Do you want to delete event with name: " + name))
            return;
        else {
            fetch('api/Events/' + id, {
                method: 'delete'
            }).then(data => {
                this.setState(
                    {
                        eventList: this.state.eventList.filter((rec) => {
                            return (rec.id !== id);
                        })
                    });
            });
        }
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderEventsTable(this.state.eventList);

        return (
            <div>
                <h1 id="tabelLabel" >Mob Events</h1>
                <p>
                    <Link to="/createevent">Create Event</Link>
                </p>
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
                            <th>Description</th>
                            <th>Date</th>
                            <th>Event Type</th>
                            <th>Address</th>
                            <th>City</th>
                            <th>State / Province</th>
                            <th>Country</th>
                            <th>ZipCode</th>
                            <th>Created By</th>
                            <th>Created Date</th>
                            <th>Latitude</th>
                            <th>Longitude</th>
                            <th>GPS Coords</th>
                            <th>MaximumNumberOfParticpants</th>
                            <th>Last Updated By</th>
                            <th>Last Updated Date</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        {events.map(mobEvent =>
                            <tr key={mobEvent.id.toString()}>
                                <td>{mobEvent.name}</td>
                                <td>{mobEvent.description}</td>
                                <td>{mobEvent.eventDate}</td>
                                <td>{mobEvent.eventTypeId}</td>
                                <td>{mobEvent.streetAddress}</td>
                                <td>{mobEvent.city}</td>
                                <td>{mobEvent.stateProvince}</td>
                                <td>{mobEvent.country}</td>
                                <td>{mobEvent.zipCode}</td>
                                <td>{mobEvent.createdByUserId}</td>
                                <td>{mobEvent.createdDate}</td>
                                <td>{mobEvent.latitude}</td>
                                <td>{mobEvent.longitude}</td>
                                <td>{mobEvent.gpscoords}</td>
                                <td>{mobEvent.maxNumberOfParticipants}</td>
                                <td>{mobEvent.lastUpdatedByUserId}</td>
                                <td>{mobEvent.lastUpdatedDate}</td>
                                <td>{mobEvent.eventStatusId}</td>
                                <td>
                                    <Link to={`/editevent/${mobEvent.id}`}>Edit Event</Link>
                                    <a className="action" onClick={() => this.handleDelete(mobEvent.id, mobEvent.name)}>Delete Event</a>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }
}
