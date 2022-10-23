import * as React from 'react'
import UserData from './Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from './../store/AuthStore';
import * as ToolTips from "./../store/ToolTips";
import EventSummaryData from './Models/EventSummaryData';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import EventData from './Models/EventData';

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

    React.useEffect(() => {

        const headers = getDefaultHeaders('GET');

        fetch('/api/Events/' + loadedEventId, {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<EventData>)
            .then(eventData => {
                setEventName(eventData.name);
                setEventDate(new Date(eventData.eventDate));
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

    function validateForm() {
        if (notesErrors !== "" || actualNumberOfAttendeesErrors !== "" || numberOfBagsErrors !== "" || numberOfBucketsErrors !== "" || durationInMinutesErrors !== "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }

    // This will handle the submit form event.  
    function handleSave(event: any) {

        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);

        var method = 'PUT';

        if (!createdByUserId) {
            method = 'POST';
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
        eventSummaryData.lastUpdatedByUserId = props.currentUser.id;

        var data = JSON.stringify(eventSummaryData);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/eventsummaries', {
                method: method,
                body: data,
                headers: headers,
            })
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

        validateForm();
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

        validateForm();
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

        validateForm();
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

        validateForm();
    }

    function handleNotesChanged(val: string) {
        if (val.length < 0 || val.length > 1000) {
            setNotesErrors("Notes cannot be empty and cannot be more than 1000 characters long.");
        }
        else {
            setNotesErrors("");
            setNotes(val);
        }

        validateForm();
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

    return (
        <div className="container-fluid card">
            <h1>Event Summary</h1>
            <h2>Name: {eventName}</h2>
            <h3>Date: {eventDate.toLocaleDateString()}</h3>

            <Form onSubmit={handleSave} >
                <Form.Row>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderActualNumberOfAttendeesToolTip}>
                                <Form.Label className="control-label">Actual Number of Attendees:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" disabled={!isOwner} value={actualNumberOfAttendees} maxLength={parseInt('3')} onChange={(val) => handleActualNumberOfAttendeesChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{actualNumberOfAttendeesErrors}</span>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderNumberOfBagsToolTip}>
                                <Form.Label className="control-label">Number of Bags:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" disabled={!isOwner} value={numberOfBags} maxLength={parseInt('3')} onChange={(val) => handleNumberOfBagsChanged(val.target.value)} />
                            <span style={{ color: "red" }}>{numberOfBagsErrors}</span>
                        </Form.Group >
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderNumberOfBucketsToolTip}>
                                <Form.Label className="control-label">Number of Buckets:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" disabled={!isOwner} value={numberOfBuckets} maxLength={parseInt('3')} onChange={(val) => handleNumberOfBucketsChanged(val.target.value)} />
                            <span style={{ color: "red" }}>{numberOfBucketsErrors}</span>
                        </Form.Group >
                    </Col>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderDurationInMinutesToolTip}>
                                <Form.Label className="control-label">Actual Duration in Minutes:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" disabled={!isOwner} value={durationInMinutes} maxLength={parseInt('3')} onChange={(val) => handleDurationInMinutesChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{durationInMinutesErrors}</span>
                        </Form.Group >
                    </Col>
                </Form.Row>
                <Form.Group>
                    <OverlayTrigger placement="top" overlay={renderNotesToolTip}>
                        <Form.Label className="control-label">Notes:</Form.Label>
                    </OverlayTrigger>
                    <Form.Control as="textarea" disabled={!isOwner} defaultValue={notes} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handleNotesChanged(val.target.value)} />
                    <span style={{ color: "red" }}>{notesErrors}</span>
                </Form.Group >
                <Form.Group className="form-group">
                    <Button disabled={!isSaveEnabled || !isOwner} type="submit" className="action btn-default">Save</Button>
                </Form.Group >
            </Form >
        </div>
    )
}

export default withRouter(EventSummary);