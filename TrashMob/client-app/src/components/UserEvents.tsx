import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import EventData from './Models/EventData';
import EventTypeData from './Models/EventTypeData';
import { getEventType } from '../store/eventTypeHelper';
import UserData from './Models/UserData';
import { Button } from 'react-bootstrap';
import { useMutation } from '@tanstack/react-query';
import { DeleteEventAttendee } from '../services/events';

interface UserEventsPropsType extends RouteComponentProps {
    eventList: EventData[];
    eventTypeList: EventTypeData[];
    isEventDataLoaded: boolean;
    onEventListChanged: any;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const UserEvents: React.FC<UserEventsPropsType> = (props) => {

    const deleteEventAttendee = useMutation({
        mutationKey: DeleteEventAttendee().key,
        mutationFn: DeleteEventAttendee().service
    })

    // Handle Remove request for an event
    function handleRemove(id: string, name: string) {
        if (!window.confirm("Do you want to remove yourself from this event: " + name + "?")) return;
        else deleteEventAttendee.mutateAsync({ eventId: id, userId: props.currentUser.id }).then(() => props.onEventListChanged());
    }

    function renderEventsTable(events: EventData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Date</th>
                            <th>Event Type</th>
                            <th>City</th>
                            <th>Region</th>
                            <th>Country</th>
                            <th>Postal Code</th>
                        </tr>
                    </thead>
                    <tbody>
                        {events.sort((a, b) => (a.eventDate < b.eventDate) ? 1 : -1).map(mobEvent => {
                            var isOwner = mobEvent.createdByUserId === props.currentUser.id;

                            return (
                                <tr key={mobEvent.id.toString()}>
                                    <td>{mobEvent.name}</td>
                                    <td>{new Date(mobEvent.eventDate).toLocaleString()}</td>
                                    <td>{getEventType(props.eventTypeList, mobEvent.eventTypeId)}</td>
                                    <td>{mobEvent.city}</td>
                                    <td>{mobEvent.region}</td>
                                    <td>{mobEvent.country}</td>
                                    <td>{mobEvent.postalCode}</td>
                                    <td>
                                        <Button hidden={!isOwner} className="action" onClick={() => props.history.push('/manageeventdashboard/' + mobEvent.id)}>Manage Event</Button>
                                        <Button hidden={!isOwner} className="action" onClick={() => props.history.push('/cancelevent/' + mobEvent.id)}>Cancel Event</Button>
                                        <Button className="action" onClick={() => props.history.push('/eventdetails/' + mobEvent.id)}>View Details</Button>
                                        <Button className="action" onClick={() => props.history.push('/eventsummary/' + mobEvent.id)}>View Summary</Button>
                                        <Button hidden={isOwner} className="action" onClick={() => handleRemove(mobEvent.id, mobEvent.name)}>Remove Me from Event</Button>
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
            <h1 id="tableLabel" >My Events</h1>
            {contents}
        </div>
    );
}

