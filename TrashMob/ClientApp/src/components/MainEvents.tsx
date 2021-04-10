import { Component } from 'react';
import * as React from 'react'
import { Link } from 'react-router-dom';
import EventData from './Models/EventData';  

interface PropsType { };

interface FetchEventDataState {
    eventList: EventData[];
    loading: boolean;
}

export class MainEvents extends Component<PropsType, FetchEventDataState> {

    constructor(props: FetchEventDataState) {
        super(props);
        this.state = { eventList: [], loading: true };

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
                            <th>Description</th>
                            <th>Date</th>
                            <th>Event Type</th>
                            <th>City</th>
                            <th>State / Province</th>
                            <th>Country</th>
                        </tr>
                    </thead>
                    <tbody>
                        {events.map(mobEvent =>
                            <tr key={mobEvent.id.toString()}>
                                <td>{mobEvent.name}</td>
                                <td>{mobEvent.description}</td>
                                <td>{mobEvent.eventDate}</td>
                                <td>{mobEvent.eventTypeId}</td>
                                <td>{mobEvent.city}</td>
                                <td>{mobEvent.stateProvince}</td>
                                <td>{mobEvent.country}</td>
                                <td>
                                    <Link to={`/eventdetails/${mobEvent.id}`}>Details</Link>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }
}
