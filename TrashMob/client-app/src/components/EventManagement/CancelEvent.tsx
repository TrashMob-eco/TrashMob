import * as React from 'react'
import EventData from '../Models/EventData';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../../store/ToolTips";
import { Button, Col, Form } from 'react-bootstrap';
import { RouteComponentProps } from 'react-router-dom';

export interface CancelEventMatchParams {
    eventId: string;
}

export interface CancelEventProps extends RouteComponentProps<CancelEventMatchParams>{
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const CancelEvent: React.FC<CancelEventProps> = (props) => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [eventId, setEventId] = React.useState<string>(props.match.params["eventId"]);
    const [eventName, setEventName] = React.useState<string>("New Event");
    const [cancellationReason, setCancellationReason] = React.useState<string>("");
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);

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

    function validateForm() {
        if (cancellationReason === "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }

    function handleCancellationReasonChanged(val: string) {
        setCancellationReason(val);
        validateForm();
    }

    function renderCancellationReasonToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventCancellationReason}</Tooltip>
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.goBack();
    }

    // This will handle the submit form event.  
    function handleSave(event: any) {

        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);

        var method = "DELETE";

        // PUT request for Edit Event.  
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        var content = { eventId: eventId, cancellationReason: cancellationReason };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/Events', {
                method: method,
                headers: headers,
                body: JSON.stringify(content),
            }).then(() => {
                props.history.goBack();
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
                            <Form.Group className="required" >
                                <OverlayTrigger placement="top" overlay={renderCancellationReasonToolTip}>
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="Name">Reason for Cancelling the Event?</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="cancellationReason" defaultValue={cancellationReason} onChange={(val) => handleCancellationReasonChanged(val.target.value)} maxLength={parseInt('200')} required />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <h4>Are you sure you want to cancel the Event {eventName}?</h4>
                    </Form.Row>
                    <Form.Row>
                        <Form.Group>
                            <Button disabled={!isSaveEnabled} type="submit" className="btn btn-default">Yes</Button>
                            <Button className="action" onClick={(e: any) => handleCancel(e)}>No</Button>
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
        <hr />
        {contents}
    </div>;
}