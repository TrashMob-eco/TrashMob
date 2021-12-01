import * as React from 'react'
import EventData from '../Models/EventData';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../../store/ToolTips";
import { Button, Col, Form, ToggleButton } from 'react-bootstrap';

export interface CancelEventProps {
    eventId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
    onCancelCancel: any;
    onCancelSave: any;
}

export const CancelEvent: React.FC<CancelEventProps> = (props) => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [eventId, setEventId] = React.useState<string>(props.eventId);
    const [eventName, setEventName] = React.useState<string>("New Event");
    const [cancellationReason, setCancellationReason] = React.useState<string>("");

    React.useEffect(() => {
        const headers = getDefaultHeaders('GET');

        fetch('/api/Events/' + eventId, {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<EventData>)
            .then(eventData => {
                setEventId(eventData.id);
                setEventName(eventData.name);
                setIsDataLoaded(true);
            });
    }, [eventId])


    function handleCancellationReasonChanged(val: string) {
        setCancellationReason(val);
    }

    function renderCancellationReasonToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventCancellationReason}</Tooltip>
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();

        props.onCancelCancel();
    }

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        var method = "DELETE";

        // PUT request for Edit Event.  
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/Events', {
                method: method,
                headers: headers,
                body: cancellationReason,
            }).then(() => {
                props.onCancelSave();
            });
        })
    }

    // Returns the HTML Form to the render() method.  
    function renderCancelForm() {
        return (
            <div className="container-fluid" >
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <input type="hidden" name="Id" value={eventId.toString()} />
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderCancellationReasonToolTip}>
                                    <Form.Label htmlFor="Name">Cancellation Reason:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="cancellationReason" defaultValue={cancellationReason} onChange={(val) => handleCancellationReasonChanged(val.target.value)} maxLength={parseInt('200')} required />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Form.Group>
                            <Button type="submit" className="btn btn-default">Save</Button>
                            <Button className="action" onClick={(e: any) => handleCancel(e)}>Cancel</Button>
                        </Form.Group>
                    </Form.Row>
                </Form >
            </div>
        )
    }

    var contents = isDataLoaded && eventId
        ? renderCancelForm()
        : <p><em>Loading...</em></p>;

    return <div>
        <h3>Cancel Event</h3>
        <h4>Are you sure you want to cancel the Event {eventName}?</h4>
        <hr />
        {contents}
    </div>;
}