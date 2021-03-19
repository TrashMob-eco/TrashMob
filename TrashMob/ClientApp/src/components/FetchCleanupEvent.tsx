import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';

interface FetchCleanupEventDataState {
    eventList: CleanupEventData[];
    loading: boolean;
}  

export class FetchCleanupEvent extends React.Component<RouteComponentProps<{}>, FetchCleanupEventDataState> {

    constructor(props) {
        super(props);
        this.state = { eventList: [], loading: true };

        fetch('api/CleanupEvents')
            .then(response => response.json() as Promise<CleanupEventData[]>)
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
            : this.renderCleanupEventsTable(this.state.eventList);

        return (
            <div>
                <h1 id="tabelLabel" >Cleanup Events</h1>
                <p>
                    <Link to="/addcleanupevent">Create New</Link>
                </p>
                {contents}
            </div>
        );
    }

    // Handle Delete request for an cleanup event  
    private handleDelete(id: number) {
        if (!window.confirm("Do you want to delete cleanup event with Id: " + id))
            return;
        else {
            fetch('api/CleanupEvents/' + id, {
                method: 'delete'
            }).then(data => {
                this.setState(
                    {
                        eventList: this.state.eventList.filter((rec) => {
                            return (rec.cleanupEventId !== id);
                        })
                    });
            });
        }
    }

    handleEdit(id) {
        this.props.history.push("/cleanupevent/edit/" + id);
    }

    private renderCleanupEventsTable(cleanupEvents: CleanupEventData[]) {
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
                    {cleanupEvents.map(cleanupEvent =>
                        <tr key={cleanupEvent.cleanupEventId}>
                            <td>{cleanupEvent.eventDate}</td>
                            <td>{cleanupEvent.name}</td>
                            <td>{cleanupEvent.address}</td>
                            <td>{cleanupEvent.country}</td>
                            <td>{cleanupEvent.description}</td>
                            <td>{cleanupEvent.contactPhone}</td>
                            <td>{cleanupEvent.latitude}</td>
                            <td>{cleanupEvent.longitude}</td>
                            <td>{cleanupEvent.maxNumberOfParticipants}</td>
                            <td>{cleanupEvent.userName}</td>
                            <td>
                                <a className="action" onClick={(id) => this.handleEdit(cleanupEvent.cleanupEventId)}>Edit</a>  |
                                <a className="action" onClick={(id) => this.handleDelete(cleanupEvent.cleanupEventId)}>Delete</a>
                            </td> 
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }
}

export class CleanupEventData {
    cleanupEventId: number = 0;
    name: string = "";
    eventDate: Date;
    description: string = "";
    userName: string = "";
    contactPhone: string = "";
    country: string = "";
    address: string = "";
    latitude: string = "";
    longitude: string = "";
    maxNumberOfParticipants: number;
}