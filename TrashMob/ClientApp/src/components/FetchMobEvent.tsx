import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';

interface FetchMobEventDataState {
    eventList: MobEventData[];
    loading: boolean;
}  

export class FetchMobEvent extends React.Component<RouteComponentProps<{}>, FetchMobEventDataState> {

    constructor(props: RouteComponentProps<{}>) {
        super(props);
        this.state = { eventList: [], loading: true };

        fetch('api/MobEvents')
            .then(response => response.json() as Promise<MobEventData[]>)
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
            : this.renderMobEventsTable(this.state.eventList);

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
    private handleDelete(id: number) {
        if (!window.confirm("Do you want to delete mob event with Id: " + id))
            return;
        else {
            fetch('api/MobEvents/' + id, {
                method: 'delete'
            }).then(data => {
                this.setState(
                    {
                        eventList: this.state.eventList.filter((rec) => {
                            return (rec.mobEventId !== id);
                        })
                    });
            });
        }
    }

    handleEdit(id: number) {
        this.props.history.push("/mobevent/edit/" + id);
    }

    private renderMobEventsTable(mobEvents: MobEventData[]) {
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
                    {mobEvents.map(mobEvent =>
                        <tr key={mobEvent.mobEventId}>
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
                                <a className="action" onClick={(id) => this.handleEdit(mobEvent.mobEventId)}>Edit</a>  |
                                <a className="action" onClick={(id) => this.handleDelete(mobEvent.mobEventId)}>Delete</a>
                            </td> 
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }
}

export class MobEventData {
    mobEventId: number = 0;
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