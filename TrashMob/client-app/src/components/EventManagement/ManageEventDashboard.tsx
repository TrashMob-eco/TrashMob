import * as React from 'react'

import { RouteComponentProps, withRouter } from 'react-router-dom';
import UserData from '../Models/UserData';
import { Col, Container, Row } from 'react-bootstrap';
import { EditEvent } from './EditEvent';
import { ManageEventPartners } from './ManageEventPartners';
import { ManageEventAttendees } from './ManageEventAttendees';
import { Guid } from 'guid-typescript';
import { getDefaultHeaders } from '../../store/AuthStore';
import EventData from '../Models/EventData';
import { HeroSection } from '../Customization/HeroSection'

export interface ManageEventDashboardMatchParams {
    eventId?: string;
}

export interface ManageEventDashboardProps extends RouteComponentProps<ManageEventDashboardMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const ManageEventDashboard: React.FC<ManageEventDashboardProps> = (props) => {
    const [eventId, setEventId] = React.useState<string>("");
    const [isEventIdReady, setIsEventIdReady] = React.useState<boolean>();
    const [loadedEventId, setLoadedEventId] = React.useState<string | undefined>(props.match?.params["eventId"]);
    const [isEventComplete, setIsEventComplete] = React.useState<boolean>(false);

    React.useEffect(() => {
        var evId = loadedEventId;
        if (!evId) {
            setEventId(Guid.createEmpty().toString());
            setLoadedEventId(Guid.createEmpty().toString())
        }
        else {
            setEventId(evId);

            const headers = getDefaultHeaders('GET');

            // Check to see if this event has been completed
            fetch('/api/Events/' + evId, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<EventData>)
                .then(eventData => {
                    if (new Date(eventData.eventDate) < new Date()) {
                        setIsEventComplete(true);
                    }
                })
        }

        setIsEventIdReady(true);
    }, [loadedEventId]);

    function handleEditCancel() {
        props.history.push("/mydashboard");
    }

    function handleEditSave() {
        props.history.push({
            pathname: '/mydashboard',
            state: {
                newEventCreated: true
            }
        });
    }

    function renderEventDashboard() {
        return (
            <>
                <EditEvent eventId={eventId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} onEditCancel={handleEditCancel} onEditSave={handleEditSave} history={props.history} location={props.location} match={props.match} />
                <ManageEventAttendees eventId={eventId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
                <ManageEventPartners eventId={eventId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} isEventComplete={isEventComplete} />
            </>
        );
    }

    let contents = isEventIdReady
        ? renderEventDashboard()
        : <p><em>Loading...</em></p>;

    return <div>
        <HeroSection Title='Manage Event' Description='We can’t wait to see the results.'></HeroSection>
        <Container>
            <Row className="gx-2 py-5" lg={2}>
                <Col lg={4} className="d-flex">
                    <div className="bg-white py-2 px-5 shadow-sm rounded">
                        <h2 className="color-primary mt-4 mb-5">Manage Event</h2>
                        <p>
                            This page allows you to create a new event or edit an existing event. You can set the name, time, and location for the event, and then request services from TrashMob.eco Partners.
                        </p>
                    </div>
                </Col>
                <Col lg={8}>
                    <div className="bg-white p-5 shadow-sm rounded">
                        {contents}
                    </div>
                </Col>
            </Row>
        </Container>
    </div>;
}

export default withRouter(ManageEventDashboard);
