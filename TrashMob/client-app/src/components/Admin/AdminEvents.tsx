import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
import { Button } from 'react-bootstrap';

interface AdminEventsPropsType extends RouteComponentProps {
    eventList: EventData[];
    isEventDataLoaded: boolean;
    onEventListChanged: any;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const AdminEvents: React.FC<AdminEventsPropsType> = (props) => {

    function renderEventsTable(events: EventData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Date</th>
                            <th>City</th>
                            <th>Region</th>
                            <th>Country</th>
                            <th>Postal Code</th>
                        </tr>
                    </thead>
                    <tbody>
                        {events.map(mobEvent => {
                            return (
                                <tr key={mobEvent.id.toString()}>
                                    <td>{mobEvent.name}</td>
                                    <td>{new Date(mobEvent.eventDate).toLocaleString()}</td>
                                    <td>{mobEvent.city}</td>
                                    <td>{mobEvent.region}</td>
                                    <td>{mobEvent.country}</td>
                                    <td>{mobEvent.postalCode}</td>
                                    <td>
                                        <Button className="action" onClick={() => props.history.push('/manageeventdashboard/' + mobEvent.id)}>Edit Event</Button>
                                        <Button className="action" onClick={() => props.history.push('/cancelevent/' + mobEvent.id)}>Delete Event</Button>
                                        <Button className="action" onClick={() => props.history.push('/eventdetails/' + mobEvent.id)}>View Details</Button>
                                    </td>
                                </tr>)
                        }
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    let contents = props.isEventDataLoaded
        ? renderEventsTable(props.eventList)
        : <p><em>Loading...</em></p>;

    return (
        <div>
            <h1 id="tableLabel" >All Events</h1>
            {contents}
        </div>
    );
}

