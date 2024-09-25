import { FC, useState, useEffect } from 'react';
import { Button, Col, Container, Form, OverlayTrigger, Row, Tooltip } from 'react-bootstrap';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { Guid } from 'guid-typescript';
import { useMutation, useQuery } from '@tanstack/react-query';
import UserData from './Models/UserData';
import * as ToolTips from '../store/ToolTips';
import EventSummaryData from './Models/EventSummaryData';
import EventData from './Models/EventData';
import { PickupLocations } from './PickupLocations';
import { SocialsModal } from './EventManagement/ShareToSocialsModal';
import * as SharingMessages from '../store/SharingMessages';
import { CreateEventSummary, GetEventById, GetEventSummaryById, UpdateEventSummary } from '../services/events';
import { Services } from '../config/services.config';

export interface EventSummaryMatchParams {
    eventId: string;
}

export interface EventSummaryDashboardProps extends RouteComponentProps<EventSummaryMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const EventSummary: FC<EventSummaryDashboardProps> = (props) => {
    const [actualNumberOfAttendees, setActualNumberOfAttendees] = useState<number>(0);
    const [numberOfBuckets, setNumberOfBuckets] = useState<number>(0);
    const [numberOfBags, setNumberOfBags] = useState<number>(0);
    const [durationInMinutes, setDurationInMinutes] = useState<number>(0);
    const [createdByUserId, setCreatedByUserId] = useState<string>();
    const [createdDate, setCreatedDate] = useState<Date>(new Date());
    const [notes, setNotes] = useState<string>('');
    const [notesErrors, setNotesErrors] = useState<string>('');
    const [actualNumberOfAttendeesErrors, setactualNumberOfAttendeesErrors] = useState<string>('');
    const [numberOfBagsErrors, setNumberOfBagsErrors] = useState<string>('');
    const [numberOfBucketsErrors, setNumberOfBucketsErrors] = useState<string>('');
    const [durationInMinutesErrors, setDurationInMinutesErrors] = useState<string>('');
    const [isSaveEnabled, setIsSaveEnabled] = useState<boolean>(false);
    const [loadedEventId] = useState<string>(props.match.params.eventId);
    const [isOwner, setIsOwner] = useState<boolean>(false);
    const [eventName, setEventName] = useState<string>('New Event');
    const [eventDate, setEventDate] = useState<Date>(new Date());
    const [showModal, setShowSocialsModal] = useState<boolean>(false);
    const [eventToShare, setEventToShare] = useState<EventData>();

    const getEventById = useQuery({
        queryKey: GetEventById({ eventId: loadedEventId }).key,
        queryFn: GetEventById({ eventId: loadedEventId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getEventSummaryById = useQuery({
        queryKey: GetEventSummaryById({ eventId: loadedEventId }).key,
        queryFn: GetEventSummaryById({ eventId: loadedEventId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const createEventSummary = useMutation({
        mutationKey: CreateEventSummary().key,
        mutationFn: CreateEventSummary().service,
    });

    const updateEventSummary = useMutation({
        mutationKey: UpdateEventSummary().key,
        mutationFn: UpdateEventSummary().service,
    });

    useEffect(() => {
        window.scrollTo(0, 0);
        getEventById.refetch().then((res) => {
            if (res.data === undefined) return;
            setEventName(res.data.data.name);
            setEventDate(new Date(res.data.data.eventDate));
            setEventToShare(res.data.data);
            setCreatedByUserId(res.data.data.createdByUserId);
            if (res.data.data.createdByUserId === props.currentUser.id) setIsOwner(true);
        });

        getEventSummaryById.refetch().then((res) => {
            if (res.data === undefined) {
                setActualNumberOfAttendees(0);
                setDurationInMinutes(0);
                setNotes('');
                setNumberOfBags(0);
                setNumberOfBuckets(0);
            } else {
                setActualNumberOfAttendees(res.data.data.actualNumberOfAttendees);
                setCreatedByUserId(res.data.data.createdByUserId);
                setCreatedDate(new Date(res.data.data.createdDate));
                setDurationInMinutes(res.data.data.durationInMinutes);
                setNotes(res.data.data.notes);
                setNumberOfBags(res.data.data.numberOfBags);
                setNumberOfBuckets(res.data.data.numberOfBuckets);
            }
        });
    }, [loadedEventId, props.currentUser.id]);

    useEffect(() => {
        if (
            notesErrors !== '' ||
            actualNumberOfAttendeesErrors !== '' ||
            numberOfBagsErrors !== '' ||
            numberOfBucketsErrors !== '' ||
            durationInMinutesErrors !== ''
        ) {
            setIsSaveEnabled(false);
        } else {
            setIsSaveEnabled(true);
        }
    }, [
        notesErrors,
        actualNumberOfAttendeesErrors,
        numberOfBagsErrors,
        numberOfBucketsErrors,
        durationInMinutesErrors,
    ]);

    // This will handle the submit form event.
    function handleSave(event: any) {
        event.preventDefault();

        if (!isSaveEnabled) return;
        setIsSaveEnabled(false);

        const body = new EventSummaryData();
        body.eventId = loadedEventId;
        body.actualNumberOfAttendees = actualNumberOfAttendees;
        body.numberOfBags = numberOfBags;
        body.numberOfBuckets = numberOfBuckets;
        body.durationInMinutes = durationInMinutes;
        body.notes = notes ?? '';
        body.createdByUserId = createdByUserId ?? props.currentUser.id;
        body.createdDate = createdDate;

        if (createdByUserId && createdByUserId !== Guid.EMPTY) updateEventSummary.mutateAsync(body);
        else {
            createEventSummary.mutateAsync(body).then(() => handleShowModal(true));
        }
    }

    function handleActualNumberOfAttendeesChanged(val: string) {
        try {
            if (val) {
                const attendees = parseInt(val);

                if (attendees < 0) {
                    setactualNumberOfAttendeesErrors('Actual attendee count must be greater than or equal to zero.');
                } else {
                    setactualNumberOfAttendeesErrors('');
                    setActualNumberOfAttendees(attendees);
                }
            } else {
                setactualNumberOfAttendeesErrors('');
                setActualNumberOfAttendees(0);
            }
        } catch {
            setactualNumberOfAttendeesErrors('Actual attendee count must be a number.');
        }
    }

    function handleNumberOfBagsChanged(val: string) {
        try {
            if (val) {
                const bags = parseInt(val);

                if (bags < 0) {
                    setNumberOfBagsErrors('Number of bags must be greater than or equal to 0.');
                } else {
                    setNumberOfBagsErrors('');
                    setNumberOfBags(bags);
                }
            } else {
                setNumberOfBagsErrors('');
                setNumberOfBags(0);
            }
        } catch {
            setNumberOfBagsErrors('Number of bags must be a number.');
        }
    }

    function handleNumberOfBucketsChanged(val: string) {
        try {
            if (val) {
                const buckets = parseInt(val);

                if (buckets < 0) {
                    setNumberOfBagsErrors('Number of buckets must be greater than or equal to 0.');
                } else {
                    setNumberOfBagsErrors('');
                    setNumberOfBuckets(buckets);
                }
            } else {
                setNumberOfBucketsErrors('');
                setNumberOfBuckets(0);
            }
        } catch {
            setNumberOfBucketsErrors('Number of buckets must be a number.');
        }
    }

    function handleDurationInMinutesChanged(val: string) {
        try {
            if (val) {
                const duration = parseInt(val);

                if (duration < 0) {
                    setDurationInMinutesErrors('Actual duration in minutes must be greater than or equal to 0.');
                } else {
                    setDurationInMinutesErrors('');
                    setDurationInMinutes(duration);
                }
            } else {
                setDurationInMinutes(0);
                setDurationInMinutesErrors('');
            }
        } catch {
            setDurationInMinutesErrors('Actual Number of minutes must be a number.');
        }
    }

    function handleNotesChanged(val: string) {
        if (val.length < 0 || val.length > 1000) {
            setNotesErrors('Notes cannot be empty and cannot be more than 1000 characters long.');
        } else {
            setNotesErrors('');
            setNotes(val);
        }
    }

    function renderActualNumberOfAttendeesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventSummaryActualNumberOfAttendees}</Tooltip>;
    }

    function renderNumberOfBagsToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventSummaryNumberOfBags}</Tooltip>;
    }

    function renderNumberOfBucketsToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventSummaryNumberOfBuckets}</Tooltip>;
    }

    function renderDurationInMinutesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventSummaryDurationInMinutes}</Tooltip>;
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestNotes}</Tooltip>;
    }

    function renderSummary() {
        return (
            <Form onSubmit={handleSave}>
                <Form.Row>
                    <Col>
                        <Form.Label className='control-label font-weight-bold h5' htmlFor='eventName'>
                            Event Name
                        </Form.Label>
                        <Form.Control
                            type='text'
                            className='border-0 bg-light h-60 p-18'
                            disabled
                            name='name'
                            value={eventName}
                        />
                    </Col>
                    <Col>
                        <Form.Label className='control-label font-weight-bold h5' htmlFor='eventDate'>
                            Event Date
                        </Form.Label>
                        <Form.Control
                            type='text'
                            className='border-0 bg-light h-60 p-18'
                            disabled
                            name='eventDate'
                            value={eventDate.toDateString()}
                        />
                    </Col>
                </Form.Row>
                <Form.Row>
                    <Col>
                        <Form.Group className='required'>
                            <OverlayTrigger placement='top' overlay={renderActualNumberOfAttendeesToolTip}>
                                <Form.Label className='control-label font-weight-bold h5'>
                                    Actual Number of Attendees
                                </Form.Label>
                            </OverlayTrigger>
                            <Form.Control
                                type='text'
                                className='border-0 bg-light h-60 p-18'
                                disabled={!isOwner}
                                value={actualNumberOfAttendees}
                                maxLength={parseInt('3')}
                                onChange={(val) => handleActualNumberOfAttendeesChanged(val.target.value)}
                                required
                            />
                            <span style={{ color: 'red' }}>{actualNumberOfAttendeesErrors}</span>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group className='required'>
                            <OverlayTrigger placement='top' overlay={renderDurationInMinutesToolTip}>
                                <Form.Label className='control-label font-weight-bold h5'>
                                    Actual Duration in Minutes
                                </Form.Label>
                            </OverlayTrigger>
                            <Form.Control
                                type='text'
                                className='border-0 bg-light h-60 p-18'
                                disabled={!isOwner}
                                value={durationInMinutes}
                                maxLength={parseInt('3')}
                                onChange={(val) => handleDurationInMinutesChanged(val.target.value)}
                                required
                            />
                            <span style={{ color: 'red' }}>{durationInMinutesErrors}</span>
                        </Form.Group>
                    </Col>
                </Form.Row>
                <Form.Row>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement='top' overlay={renderNumberOfBagsToolTip}>
                                <Form.Label className='control-label font-weight-bold h5'>Number of Bags</Form.Label>
                            </OverlayTrigger>
                            <Form.Control
                                type='text'
                                className='border-0 bg-light h-60 p-18'
                                disabled={!isOwner}
                                value={numberOfBags}
                                maxLength={parseInt('3')}
                                onChange={(val) => handleNumberOfBagsChanged(val.target.value)}
                            />
                            <span style={{ color: 'red' }}>{numberOfBagsErrors}</span>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement='top' overlay={renderNumberOfBucketsToolTip}>
                                <Form.Label className='control-label font-weight-bold h5'>Number of Buckets</Form.Label>
                            </OverlayTrigger>
                            <Form.Control
                                type='text'
                                className='border-0 bg-light h-60 p-18'
                                disabled={!isOwner}
                                value={numberOfBuckets}
                                maxLength={parseInt('3')}
                                onChange={(val) => handleNumberOfBucketsChanged(val.target.value)}
                            />
                            <span style={{ color: 'red' }}>{numberOfBucketsErrors}</span>
                        </Form.Group>
                    </Col>
                </Form.Row>
                <Form.Group>
                    <OverlayTrigger placement='top' overlay={renderNotesToolTip}>
                        <Form.Label className='control-label font-weight-bold h5'>Notes</Form.Label>
                    </OverlayTrigger>
                    <Form.Control
                        as='textarea'
                        className='border-0 bg-light h-60 p-18'
                        disabled={!isOwner}
                        defaultValue={notes}
                        maxLength={parseInt('2048')}
                        rows={5}
                        cols={5}
                        onChange={(val) => handleNotesChanged(val.target.value)}
                    />
                    <span style={{ color: 'red' }}>{notesErrors}</span>
                </Form.Group>
                <Form.Group className='form-group'>
                    <Button disabled={!isSaveEnabled || !isOwner} type='submit' className='action btn-default'>
                        Save
                    </Button>
                </Form.Group>
            </Form>
        );
    }

    function renderPickupLocations() {
        return (
            <PickupLocations
                eventId={loadedEventId}
                isUserLoaded={props.isUserLoaded}
                currentUser={props.currentUser}
            />
        );
    }

    const handleShowModal = (showModal: boolean) => {
        setShowSocialsModal(showModal);
    };

    return (
        <Container>
            {eventToShare ? (
                <SocialsModal
                    eventToShare={eventToShare}
                    show={showModal}
                    handleShow={handleShowModal}
                    modalTitle='Event Summary Saved'
                    eventLink='https://www.trashmob.eco'
                    message={SharingMessages.getEventSummaryMessage(
                        eventToShare.city,
                        actualNumberOfAttendees,
                        numberOfBags,
                    )}
                    emailSubject='TrashMob Event Summary'
                />
            ) : null}
            <Row className='gx-2 py-5' lg={2}>
                <Col lg={4} className='d-flex'>
                    <div className='bg-white py-2 px-5 shadow-sm rounded'>
                        <h2 className='color-primary mt-4 mb-5'>Enter Event Summary Information</h2>
                        <p>Please enter information about how the event went.</p>
                        <p>
                            If you have garbage that needs to be hauled, and have previously requested help from a
                            partner with hauling, enter the locations as accurately as possible of the piles you need
                            hauled. Leave additional notes as needed to help the partner locate the trash. You can add
                            as many locations as needed, but the request will not be sent until you have saved the
                            entries and then hit the Submit button!
                        </p>
                    </div>
                </Col>
                <Col lg={8}>
                    <div className='bg-white p-5 shadow-sm rounded'>
                        <h2 className='color-primary mt-4 mb-5'>Event Summary</h2>
                        {renderSummary()}
                        {renderPickupLocations()}
                    </div>
                </Col>
            </Row>
        </Container>
    );
};

export default withRouter(EventSummary);
