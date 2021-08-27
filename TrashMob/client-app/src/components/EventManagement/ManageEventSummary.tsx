import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import EventSummaryData from '../Models/EventSummaryData';

export interface ManageEventSummaryDataProps {
    eventId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const ManageEventSummary: React.FC<ManageEventSummaryDataProps> = (props) => {
    const [actualNumberOfAttendees, setActualNumberOfAttendees] = React.useState<number>(0);
    const [numberOfBuckets, setNumberOfBuckets] = React.useState<number>(0);
    const [numberOfBags, setNumberOfBags] = React.useState<number>(0);
    const [durationInMinutes, setDurationInMinutes] = React.useState<number>(0);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>();
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [notes, setNotes] = React.useState<string>("");
    const [notesErrors, setNotesErrors] = React.useState<string>("");

    React.useEffect(() => {

        const headers = getDefaultHeaders('GET');

        fetch('/api/eventsummaries/' + props.eventId, {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<EventSummaryData>)
            .then(data => {
                setActualNumberOfAttendees(data.actualNumberOfAttendees);
                setCreatedByUserId(data.createdByUserId);
                setCreatedDate(data.createdDate);
                setDurationInMinutes(data.durationInMinutes);
                setNotes(data.notes);
                setNumberOfBags(data.numberOfBags);
                setNumberOfBuckets(data.numberOfBuckets);
            });
    }, [props.eventId]);

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        if (notesErrors !== "") {
            return;
        }

        var method = 'PUT';

        if (!createdByUserId) {
            method = 'POST';
        }

        var eventSummaryData = new EventSummaryData();
        eventSummaryData.eventId = props.eventId;
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
                setActualNumberOfAttendees(attendees);
            }
            else {
                setActualNumberOfAttendees(0);
            }
        }
        catch { }
    }

    function handleNumberOfBagsChanged(val: string) {
        try {
            if (val) {
                var bags = parseInt(val);
                setNumberOfBags(bags);
            }
            else {
                setNumberOfBags(0);
            }
        }
        catch { }
    }

    function handleNumberOfBucketsChanged(val: string) {
        try {
            if (val) {
                var buckets = parseInt(val);
                setNumberOfBuckets(buckets);
            }
            else {
                setNumberOfBuckets(0);
            }
        }
        catch { }
    }

    function handleDurationInMinutesChanged(val: string) {
        try {
            if (val) {
                var duration = parseInt(val);
                setDurationInMinutes(duration);
            }
            else {
                setDurationInMinutes(0);
            }
        }
        catch { }
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

    return (
        <div className="container-fluid card">
            <Form onSubmit={handleSave} >
                <Form.Row>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderActualNumberOfAttendeesToolTip}>
                                <Form.Label>Actual Number of Attendees:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" value={actualNumberOfAttendees} maxLength={parseInt('3')} onChange={(val) => handleActualNumberOfAttendeesChanged(val.target.value)} required />
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderNumberOfBagsToolTip}>
                                <Form.Label>Number of Bags:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" value={numberOfBags} maxLength={parseInt('3')} onChange={(val) => handleNumberOfBagsChanged(val.target.value)} />
                        </Form.Group >
                    </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderNumberOfBucketsToolTip}>
                                    <Form.Label>Number of Buckets:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" value={numberOfBuckets} maxLength={parseInt('3')} onChange={(val) => handleNumberOfBucketsChanged(val.target.value)} />
                            </Form.Group >
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderDurationInMinutesToolTip}>
                                    <Form.Label>Actual Duration in Minutes:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" value={durationInMinutes} maxLength={parseInt('3')} onChange={(val) => handleDurationInMinutesChanged(val.target.value)} />
                            </Form.Group >
                        </Col>
                </Form.Row>
                <Form.Group>
                    <OverlayTrigger placement="top" overlay={renderNotesToolTip}>
                        <Form.Label>Notes:</Form.Label>
                    </OverlayTrigger>
                    <Form.Control as="textarea" defaultValue={notes} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handleNotesChanged(val.target.value)} />
                    <span style={{ color: "red" }}>{notesErrors}</span>
                </Form.Group >
                <Form.Group className="form-group">
                    <Button disabled={notesErrors !== ""} type="submit" className="action btn-default">Save</Button>
                </Form.Group >
            </Form >
        </div>
    )
}