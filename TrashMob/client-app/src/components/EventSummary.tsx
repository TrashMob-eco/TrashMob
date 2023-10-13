import * as React from 'react'
import UserData from './Models/UserData';
import { Button, Col, Container, Form, OverlayTrigger, Row, Tooltip } from 'react-bootstrap';
import { getApiConfig, getDefaultHeaders, msalClient, validateToken } from './../store/AuthStore';
import * as ToolTips from "./../store/ToolTips";
import EventSummaryData from './Models/EventSummaryData';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import EventData from './Models/EventData';
import { PickupLocations } from './PickupLocations';
import { SocialsModal } from './EventManagement/ShareToSocialsModal';
import { Guid } from 'guid-typescript';

export interface EventSummaryMatchParams {
    eventId: string;
}

export interface EventSummaryDashboardProps extends RouteComponentProps<EventSummaryMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}


const EventSummary: React.FC<EventSummaryDashboardProps> = (props) => {
    const [actualNumberOfAttendees, setActualNumberOfAttendees] = React.useState<number>(0);
    const [numberOfBuckets, setNumberOfBuckets] = React.useState<number>(0);
    const [numberOfBags, setNumberOfBags] = React.useState<number>(0);
    const [durationInMinutes, setDurationInMinutes] = React.useState<number>(0);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>();
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [notes, setNotes] = React.useState<string>("");
    const [notesErrors, setNotesErrors] = React.useState<string>("");
    const [actualNumberOfAttendeesErrors, setactualNumberOfAttendeesErrors] = React.useState<string>("");
    const [numberOfBagsErrors, setNumberOfBagsErrors] = React.useState<string>("");
    const [numberOfBucketsErrors, setNumberOfBucketsErrors] = React.useState<string>("");
    const [durationInMinutesErrors, setDurationInMinutesErrors] = React.useState<string>("");
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [loadedEventId] = React.useState<string>(props.match.params["eventId"]);
    const [isOwner, setIsOwner] = React.useState<boolean>(false);
    const [eventName, setEventName] = React.useState<string>("New Event");
    const [eventDate, setEventDate] = React.useState<Date>(new Date());
    const [showModal, setShowSocialsModal] = React.useState<boolean>(false);
    const [eventToShare, setEventToShare] = React.useState<EventData>();
    const [shareMessage, setShareMessage] = React.useState<string>(""); 

    React.useEffect(() => {

        window.scrollTo(0, 0);

        const headers = getDefaultHeaders('GET');

        fetch('/api/Events/' + loadedEventId, {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<EventData>)
            .then(eventData => {
                setEventName(eventData.name);
                setEventDate(new Date(eventData.eventDate));
                setEventToShare(eventData)
                if (eventData.createdByUserId === props.currentUser.id) {
                    setIsOwner(true);
                }
            })
            .then(() => {
                fetch('/api/eventsummaries/' + loadedEventId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => {
                        if (response.status === 200) {
                            return response.json() as Promise<EventSummaryData>;
                        }
                        else {
                            throw Error(response.statusText);
                        }
                    })
                    .then(data => {
                        setActualNumberOfAttendees(data.actualNumberOfAttendees);
                        setCreatedByUserId(data.createdByUserId);
                        setCreatedDate(new Date(data.createdDate));
                        setDurationInMinutes(data.durationInMinutes);
                        setNotes(data.notes);
                        setNumberOfBags(data.numberOfBags);
                        setNumberOfBuckets(data.numberOfBuckets);
                    })
                    .catch((error) => {
                    });
            })
            .catch((error) => {
            });
    }, [loadedEventId, props.currentUser.id]);

    React.useEffect(() => {
        if (notesErrors !== "" || actualNumberOfAttendeesErrors !== "" || numberOfBagsErrors !== "" || numberOfBucketsErrors !== "" || durationInMinutesErrors !== "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }, [notesErrors, actualNumberOfAttendeesErrors, numberOfBagsErrors, numberOfBucketsErrors, durationInMinutesErrors]);

    // This will handle the submit form event.  
    function handleSave(event: any) {

        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);

        var method = 'POST';

        if (createdByUserId && createdByUserId !== Guid.EMPTY) {
            method = 'PUT';
        }

        var eventSummaryData = new EventSummaryData();
        eventSummaryData.eventId = loadedEventId;
        eventSummaryData.actualNumberOfAttendees = actualNumberOfAttendees;
        eventSummaryData.numberOfBags = numberOfBags;
        eventSummaryData.numberOfBuckets = numberOfBuckets;
        eventSummaryData.durationInMinutes = durationInMinutes;
        eventSummaryData.notes = notes ?? "";
        eventSummaryData.createdByUserId = createdByUserId ?? props.currentUser.id;
        eventSummaryData.createdDate = createdDate;

        if (eventToShare) {
            setShareMessage(`We just finished a {{TrashMob}} event in ${eventToShare.city}. ${actualNumberOfAttendees} attendees picked up ${numberOfBags} bags of #litter. ` +
            `Sign up using the link to get notified the next time we are having an event. Help us clean up the planet!`)
        }

        var data = JSON.stringify(eventSummaryData);

        const account = msalClient.getAllAccounts()[0];
        var apiConfig = getApiConfig();

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {

            if (!validateToken(tokenResponse.idTokenClaims)) {
                return;
            }

            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/eventsummaries', {
                method: method,
                body: data,
                headers: headers,
            }).then(() => {
                handleShowModal(true)
            });
        });
    }

    function handleActualNumberOfAttendeesChanged(val: string) {
        try {
            if (val) {
                var attendees = parseInt(val);

                if (attendees < 0) {
                    setactualNumberOfAttendeesErrors("Actual attendee count must be greater than or equal to zero.");
                }
                else {
                    setactualNumberOfAttendeesErrors("");
                    setActualNumberOfAttendees(attendees);
                }
            }
            else {
                setactualNumberOfAttendeesErrors("");
                setActualNumberOfAttendees(0);
            }
        }
        catch {
            setactualNumberOfAttendeesErrors("Actual attendee count must be a number.");
        }
    }

    function handleNumberOfBagsChanged(val: string) {
        try {
            if (val) {
                var bags = parseInt(val);

                if (bags < 0) {
                    setNumberOfBagsErrors("Number of bags must be greater than or equal to 0.")
                }
                else {
                    setNumberOfBagsErrors("")
                    setNumberOfBags(bags);
                }
            }
            else {
                setNumberOfBagsErrors("")
                setNumberOfBags(0);
            }
        }
        catch {
            setNumberOfBagsErrors("Number of bags must be a number.")
        }
    }

    function handleNumberOfBucketsChanged(val: string) {
        try {
            if (val) {
                var buckets = parseInt(val);

                if (buckets < 0) {
                    setNumberOfBagsErrors("Number of buckets must be greater than or equal to 0.")
                }
                else {
                    setNumberOfBagsErrors("")
                    setNumberOfBuckets(buckets);
                }
            }
            else {
                setNumberOfBucketsErrors("")
                setNumberOfBuckets(0);
            }
        }
        catch {
            setNumberOfBucketsErrors("Number of buckets must be a number.")
        }
    }

    function handleDurationInMinutesChanged(val: string) {
        try {
            if (val) {
                var duration = parseInt(val);

                if (duration < 0) {
                    setDurationInMinutesErrors("Actual duration in minutes must be greater than or equal to 0.")
                }
                else {
                    setDurationInMinutesErrors("")
                    setDurationInMinutes(duration);
                }
            }
            else {
                setDurationInMinutes(0);
                setDurationInMinutesErrors("")
            }
        }
        catch {
            setDurationInMinutesErrors("Actual Number of minutes must be a number.")
        }
    }

    function handleNotesChanged(val: string) {
        if (val.length < 0 || val.length > 1000) {
            setNotesErrors("Notes cannot be empty and cannot be more than 1000 characters long.");
        }
        else {
            setNotesErrors("");
            setNotes(val);
        }
    }

    function renderActualNumberOfAttendeesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventSummaryActualNumberOfAttendees}</Tooltip>
    }

    function renderNumberOfBagsToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventSummaryNumberOfBags}</Tooltip>
    }

    function renderNumberOfBucketsToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventSummaryNumberOfBuckets}</Tooltip>
    }

    function renderDurationInMinutesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventSummaryDurationInMinutes}</Tooltip>
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestNotes}</Tooltip>
    }

    function renderSummary() {
        return (
            <Form onSubmit={handleSave} >
                <Form.Row>
                    <Col>
                        <Form.Label className="control-label font-weight-bold h5" htmlFor="eventName">Event Name</Form.Label>
                        <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="name" value={eventName} />
                    </Col>
                    <Col>
                        <Form.Label className="control-label font-weight-bold h5" htmlFor="eventDate">Event Date</Form.Label>
                        <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="eventDate" value={eventDate.toDateString()} />
                    </Col>
                </Form.Row>
                <Form.Row>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderActualNumberOfAttendeesToolTip}>
                                <Form.Label className="control-label font-weight-bold h5">Actual Number of Attendees</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled={!isOwner} value={actualNumberOfAttendees} maxLength={parseInt('3')} onChange={(val) => handleActualNumberOfAttendeesChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{actualNumberOfAttendeesErrors}</span>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderDurationInMinutesToolTip}>
                                <Form.Label className="control-label font-weight-bold h5">Actual Duration in Minutes</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled={!isOwner} value={durationInMinutes} maxLength={parseInt('3')} onChange={(val) => handleDurationInMinutesChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{durationInMinutesErrors}</span>
                        </Form.Group >
                    </Col>
                </Form.Row>
                <Form.Row>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderNumberOfBagsToolTip}>
                                <Form.Label className="control-label font-weight-bold h5">Number of Bags</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled={!isOwner} value={numberOfBags} maxLength={parseInt('3')} onChange={(val) => handleNumberOfBagsChanged(val.target.value)} />
                            <span style={{ color: "red" }}>{numberOfBagsErrors}</span>
                        </Form.Group >
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderNumberOfBucketsToolTip}>
                                <Form.Label className="control-label font-weight-bold h5">Number of Buckets</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled={!isOwner} value={numberOfBuckets} maxLength={parseInt('3')} onChange={(val) => handleNumberOfBucketsChanged(val.target.value)} />
                            <span style={{ color: "red" }}>{numberOfBucketsErrors}</span>
                        </Form.Group >
                    </Col>
                </Form.Row>
                <Form.Group>
                    <OverlayTrigger placement="top" overlay={renderNotesToolTip}>
                        <Form.Label className="control-label font-weight-bold h5">Notes</Form.Label>
                    </OverlayTrigger>
                    <Form.Control as="textarea" className='border-0 bg-light h-60 p-18' disabled={!isOwner} defaultValue={notes} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handleNotesChanged(val.target.value)} />
                    <span style={{ color: "red" }}>{notesErrors}</span>
                </Form.Group >
                <Form.Group className="form-group">
                    <Button disabled={!isSaveEnabled || !isOwner} type="submit" className="action btn-default">Save</Button>
                </Form.Group >
            </Form >
        );
    }

    function renderPickupLocations() {
        return (
            <PickupLocations eventId={loadedEventId} isUserLoaded={props.isUserLoaded} currentUser={props.currentUser} />
        );
    }

    const handleShowModal = (showModal: boolean) => {
        setShowSocialsModal(showModal)
    }

    return (
        <Container>
            { eventToShare &&
                <SocialsModal eventToShare={eventToShare} show={showModal} handleShow={handleShowModal} modalTitle="Event Summary Saved" eventLink='https://www.trashmob.eco' message={shareMessage} emailSubject='TrashMob Event Summary' />
            }
            <Row className="gx-2 py-5" lg={2}>
                <Col lg={4} className="d-flex">
                    <div className="bg-white py-2 px-5 shadow-sm rounded">
                        <h2 className="color-primary mt-4 mb-5">Enter Event Summary Information</h2>
                        <p>
                            Please enter information about how the event went.
                        </p>
                        <p>
                            If you have garbage that needs to be hauled, and have previously requested help from a partner with hauling, enter the locations
                            as accurately as possible of the piles you need hauled. Leave additional notes as needed to help the partner locate the trash. You can
                            add as many locations as needed, but the request will not be sent until you have saved the entries and then hit the Submit button!
                        </p>
                    </div>
                </Col>
                <Col lg={8}>
                    <div className="bg-white p-5 shadow-sm rounded">
                        <h2 className="color-primary mt-4 mb-5">Event Summary</h2>
                        {renderSummary()}
                        {renderPickupLocations()}
                    </div>
                </Col>
            </Row>
        </Container>
    );
}

export default withRouter(EventSummary);