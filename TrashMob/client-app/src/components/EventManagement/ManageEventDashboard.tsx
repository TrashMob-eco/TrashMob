import * as React from 'react';

import { useNavigate } from 'react-router';
import { Col, Container, Row } from 'react-bootstrap';
import { Guid } from 'guid-typescript';
import { useQuery } from '@tanstack/react-query';
import UserData from '../Models/UserData';
import EditEvent from './EditEvent';
import { ManageEventPartners } from './ManageEventPartners';
import { ManageEventAttendees } from './ManageEventAttendees';
import { HeroSection } from '../Customization/HeroSection';
import { GetEventById } from '../../services/events';
import { Services } from '../../config/services.config';
import { useParams } from 'react-router';

export interface ManageEventDashboardMatchParams {
    eventId?: string;
}

export interface ManageEventDashboardProps {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const ManageEventDashboard: React.FC<ManageEventDashboardProps> = (props) => {
    const navigate = useNavigate();
    const params = useParams<'eventId'>();
    const [eventId, setEventId] = React.useState<string>('');
    const [isEventIdReady, setIsEventIdReady] = React.useState<boolean>();
    const [loadedEventId, setLoadedEventId] = React.useState<string | undefined>(params.eventId!);
    const [isEventComplete, setIsEventComplete] = React.useState<boolean>(false);

    const getEventById = useQuery({
        queryKey: GetEventById({ eventId }).key,
        queryFn: GetEventById({ eventId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    React.useEffect(() => {
        const evId = loadedEventId;
        if (!evId) {
            setEventId(Guid.createEmpty().toString());
            setLoadedEventId(Guid.createEmpty().toString());
        } else if (evId !== Guid.EMPTY) {
            setEventId(evId);
            // Check to see if this event has been completed
            getEventById.refetch().then((res) => {
                if (res.data !== undefined && new Date(res.data?.data.eventDate) < new Date()) setIsEventComplete(true);
            });
        }

        setIsEventIdReady(true);
    }, [loadedEventId]);

    function handleEditCancel() {
        navigate('/mydashboard');
    }

    function handleEditSave() {
        navigate('/mydashboard', {
            state: {
                newEventCreated: true,
            },
        });
    }

    function renderEventDashboard() {
        return (
            <>
                <EditEvent
                    eventId={eventId}
                    currentUser={props.currentUser}
                    isUserLoaded={props.isUserLoaded}
                    onEditCancel={handleEditCancel}
                    onEditSave={handleEditSave}
                    history={props.history}
                    location={props.location}
                    match={props.match}
                />
                <ManageEventAttendees
                    eventId={eventId}
                    currentUser={props.currentUser}
                    isUserLoaded={props.isUserLoaded}
                />
                <ManageEventPartners
                    eventId={eventId}
                    currentUser={props.currentUser}
                    isUserLoaded={props.isUserLoaded}
                    isEventComplete={isEventComplete}
                />
            </>
        );
    }

    const contents = isEventIdReady ? (
        renderEventDashboard()
    ) : (
        <p>
            <em>Loading...</em>
        </p>
    );

    return (
        <div>
            <HeroSection Title='Manage Event' Description='We can’t wait to see the results.' />
            <Container>
                <Row className='gx-2 py-5' lg={2}>
                    <Col lg={4} className='d-flex'>
                        <div className='bg-white py-2 px-5 shadow-sm rounded'>
                            <h2 className='color-primary mt-4 mb-5'>Manage Event</h2>
                            <p>
                                This page allows you to create a new event or edit an existing event. You can set the
                                name, time, and location for the event, and then request services from TrashMob.eco
                                Partners.
                            </p>
                        </div>
                    </Col>
                    <Col lg={8}>
                        <div className='bg-white p-5 shadow-sm rounded'>{contents}</div>
                    </Col>
                </Row>
            </Container>
        </div>
    );
};

export default ManageEventDashboard;
