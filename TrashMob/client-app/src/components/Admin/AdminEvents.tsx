import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
import { Col, Container, Dropdown, Row } from 'react-bootstrap';
import { getApiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import { Eye, Pencil, XSquare } from 'react-bootstrap-icons';

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
            var apiConfig = getApiConfig();

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

    const eventActionDropdownList = (eventId: string) => {
        return (
            <>
                <Dropdown.Item onClick={() => props.history.push('/manageeventdashboard/' + eventId)}><Pencil />Manage Event</Dropdown.Item>
                <Dropdown.Item onClick={() => props.history.push('/cancelevent/' + eventId)}><XSquare />Delete Event</Dropdown.Item>
                <Dropdown.Item onClick={() => props.history.push('/eventdetails/' + eventId)}><Eye />View Event</Dropdown.Item>
            </>
        )
    }

    function renderEventsTable(events: EventData[]) {
        return (
            <div>
                <h2 className="color-primary mt-4 mb-5">Events</h2>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Date</th>
                            <th>City</th>
                            <th>Region</th>
                            <th>Country</th>
                            <th>Postal Code</th>
                            <th>Actions</th>
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
                                    <td className="btn py-0">
                                        <Dropdown role="menuitem">
                                            <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                            <Dropdown.Menu id="share-menu">
                                                {eventActionDropdownList(mobEvent.id)}
                                            </Dropdown.Menu>
                                        </Dropdown>
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
        <Container>
            <Row className="gx-2 py-5" lg={2}>
                <Col lg={12}>
                    <div className="bg-white p-5 shadow-sm rounded">
                        {contents}
                    </div>
                </Col>
            </Row>
        </Container >
    );
}

