import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
import { Col, Container, Dropdown, Row } from 'react-bootstrap';
import { getApiConfig, getDefaultHeaders, msalClient, validateToken } from '../../store/AuthStore';
import { Eye, Pencil, XSquare, SortDown } from 'react-bootstrap-icons';
import { useQuery } from '@tanstack/react-query';
import { GetAllEvents } from '../../services/events';
import { Services } from '../../config/services.config';

interface AdminEventsPropsType extends RouteComponentProps {
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const AdminEvents: React.FC<AdminEventsPropsType> = (props) => {
    const [eventList, setEventList] = React.useState<EventData[]>([]);
    const [isEventDataLoaded, setIsEventDataLoaded] = React.useState<boolean>(false);

    const getAllEvents = useQuery({ 
        queryKey: GetAllEvents().key,
        queryFn: GetAllEvents().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false
    });

    const eventStatus = {
        1: "Active",
        2: "Full",
        3: "Canceled",
        4: "Complete"
    }

    React.useEffect(() => {
        if (props.isUserLoaded) {
            // Load the Partner List
            getAllEvents.refetch().then((res) => {
                setEventList(res.data?.data || []);
                setIsEventDataLoaded(true);
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

    const sortEvents = (sortField: string) => {

        var eventListCopy = [...eventList]

        // remove leading whitespace if sorting by name
        if (sortField === 'name') {
            eventListCopy = eventListCopy.map(function (el) {
                return { ...el, name: el.name.trim() }
            })
        }

        const sortedList = eventListCopy.sort(sortObjsList(sortField))
        setEventList(sortedList)
    }

    const sortObjsList = (sortBy: string) => (a: object, b: object) => {
        if (a[sortBy] > b[sortBy]) {
            return 1;
        } else if (a[sortBy] < b[sortBy]) {
            return -1;
        }
        return 0;
    }

    const SortDropdown = () => {
        return (
            <Dropdown>
                <Dropdown.Toggle id="userBtn" className="mt-4 mb-5" variant="light">
                    <SortDown className="mr-3" size={32} color="#96ba00" aria-labelledby="sort-icon" />
                    Sort
                </Dropdown.Toggle>
                <Dropdown.Menu className="shadow border-0">
                    <Dropdown.Item eventKey="1" onClick={() => sortEvents("name")}>Alphabetical</Dropdown.Item>
                    <Dropdown.Divider />
                    <Dropdown.Item eventKey="2" onClick={() => sortEvents("eventDate")}>Date</Dropdown.Item>
                    <Dropdown.Divider />
                    <Dropdown.Item eventKey="3" onClick={() => sortEvents("eventStatusId")}>Status</Dropdown.Item>
                </Dropdown.Menu>
            </Dropdown>
        )
    }

    function renderEventsTable(events: EventData[]) {
        return (
            <div>
                <div className="d-flex flex-row align-items-center justify-content-between">
                    <h2 className="color-primary mt-4 mb-5">Events</h2>
                    <SortDropdown />
                </div>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Status</th>
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
                                    <td>{eventStatus[mobEvent.eventStatusId]}</td>
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

