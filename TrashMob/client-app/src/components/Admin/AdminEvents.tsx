import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
import { Button } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';

interface AdminEventsPropsType extends RouteComponentProps {
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const AdminEvents: React.FC<AdminEventsPropsType> = (props) => {

    const [eventList, setEventList] = React.useState<EventData[]>([]);
    const [isEventDataLoaded, setIsEventDataLoaded] = React.useState<boolean>(false);

    React.useEffect(() => {

        if (props.isUserLoaded) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {

                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                // Load the Partner List
                fetch('/api/events', {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<Array<EventData>>)
                    .then(data => {
                        setEventList(data);
                        setIsEventDataLoaded(true);
                    });
            })
        }
    }, [props.isUserLoaded])

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

    let contents = isEventDataLoaded
        ? renderEventsTable(eventList)
        : <p><em>Loading...</em></p>;

    return (
        <div>
            <h1 id="tableLabel" >All Events</h1>
            {contents}
        </div>
    );
}

