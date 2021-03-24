import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';
import { Guid } from "guid-typescript";
import authService from './api-authorization/AuthorizeService'

interface FetchEventDataState {
    eventList: EventData[];
    loading: boolean;
}  

export class FetchEvent extends React.Component<RouteComponentProps<{}>, FetchEventDataState> {

    constructor(props: RouteComponentProps<{}>) {
        super(props);
        this.state = { eventList: [], loading: true };

        const token = authService.getAccessToken();

        fetch('api/Events', {
            method: 'GET',
            headers: {
                Allow: 'GET',
                Accept: 'application/json',
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
                },
            })
            .then(response => response.json() as Promise<EventData[]>)
            .then(data => {
                this.setState({ eventList: data, loading: false });
            });

        // This binding is necessary to make "this" work in the callback  
        this.handleDelete = this.handleDelete.bind(this);
        this.handleEdit = this.handleEdit.bind(this);
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderEventsTable(this.state.eventList);

        return (
            <div>
                <h1 id="tabelLabel" >Mob Events</h1>
                <p>
                    <Link to="/addmobevent">Create New</Link>
                </p>
                {contents}
            </div>
        );
    }

    // Handle Delete request for an mob event  
    private handleDelete(id: Guid) {
        if (!window.confirm("Do you want to delete mob event with Id: " + id))
            return;
        else {
            const token = authService.getAccessToken();
            fetch('api/Events/' + id, {
                method: 'delete',
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
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

    handleEdit(id: Guid) {
        this.props.history.push("/mobevent/edit/" + id);
    }

    private renderEventsTable(events: EventData[]) {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Name</th>
                        <th>Address</th>
                        <th>Country</th>
                        <th>Description</th>
                        <th>Contact Phone</th>
                        <th>Latitude</th>
                        <th>Longitude</th>
                        <th>MaximumNumberOfParticpants</th>
                        <th>EventLead</th>
                    </tr>
                </thead>
                <tbody>
                    {events.map(mobEvent =>
                        <tr key={mobEvent.id.toString()}>
                            <td>{mobEvent.eventDate}</td>
                            <td>{mobEvent.name}</td>
                            <td>{mobEvent.address}</td>
                            <td>{mobEvent.country}</td>
                            <td>{mobEvent.description}</td>
                            <td>{mobEvent.contactPhone}</td>
                            <td>{mobEvent.latitude}</td>
                            <td>{mobEvent.longitude}</td>
                            <td>{mobEvent.maxNumberOfParticipants}</td>
                            <td>{mobEvent.userName}</td>
                            <td>
                                <a className="action" onClick={(id) => this.handleEdit(mobEvent.id)}>Edit</a>  |
                                <a className="action" onClick={(id) => this.handleDelete(mobEvent.id)}>Delete</a>
                            </td> 
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }
}

export class EventData {
    id: Guid = Guid.create();
    name: string = "";
    eventDate: Date = new Date();
    description: string = "";
    userName: string = "";
    contactPhone: string = "";
    country: string = "";
    address: string = "";
    latitude: string = "";
    longitude: string = "";
    maxNumberOfParticipants: number = 0;
}