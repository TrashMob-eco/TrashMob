import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { Button, ButtonGroup, Col, Container, Form, Modal, OverlayTrigger, Row, Tooltip } from 'react-bootstrap';
import * as ToolTips from "../../store/ToolTips";
import MessageRequestData from '../Models/MessageRequestData';

interface AdminSendNotificationsPropsType extends RouteComponentProps {
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const AdminSendNotifications: React.FC<AdminSendNotificationsPropsType> = (props) => {
    const [name, setName] = React.useState<string>();
    const [nameErrors, setNameErrors] = React.useState<string>("");
    const [message, setMessage] = React.useState<string>("");
    const [messageErrors, setMessageErrors] = React.useState<string>("");
    const [isSendEnabled, setIsSendEnabled] = React.useState<boolean>(false);
    const [show, setShow] = React.useState<boolean>(false);

    const handleClose = () => setShow(false);
    const handleShow = () => setShow(true);

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.push("/");
    }

    // Handle Delete request for a user  
    function handleSendNotification(event: any) {

        if (!isSendEnabled) {
            return;
        }

        setIsSendEnabled(false);
        event.preventDefault();

        var messageRequestData = new MessageRequestData();
        messageRequestData.userName = name ?? "";
        messageRequestData.message = message ?? "";
        var data = JSON.stringify(messageRequestData);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('api/messagerequest/', {
                method: 'post',
                body: data,
                headers: headers
            }).then(() => {
                handleShow();
                setIsSendEnabled(false);
            });
        });
    }

    function validateForm() {
        if (nameErrors !== "" || messageErrors !== "" || name === "" || message === "") {
            setIsSendEnabled(false);
        }
        else {
            setIsSendEnabled(true);
        }
    }

    function handleNameChanged(val: string) {
        if (val.length <= 0 || val.length > 64) {
            setNameErrors("Please enter a valid name.");
        }
        else {
            setNameErrors("");
            setName(val);
        }

        validateForm();
    }

    function handleMessageChanged(val: string) {
        if (val.length <= 0 || val.length > 1000) {
            setMessageErrors("Message cannot be empty and cannot be more than 1000 characters long.");
        }
        else {
            setMessageErrors("");
            setMessage(val);
        }

        validateForm();
    }

    function renderNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.AdminSendNotificationUserName}</Tooltip>
    }

    function renderMessageToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.AdminSendNotificationMessage}</Tooltip>
    }

    function renderMessageForm() {
        return (
            <Container>
                <Row className="gx-2 py-5" lg={2}>
                    <Col lg={4} className="d-flex">
                        <div className="bg-white py-2 px-5 shadow-sm rounded">
                            <h2 className="color-primary mt-4 mb-5">Send Notification</h2>
                            <p>
                                Enter the User Name you wish to message and the message you wish to send them.
                            </p>
                        </div>
                    </Col>
                    <Col lg={{ span: 7, offset: 1 }}>
                        <div className="bg-white p-5 shadow-sm rounded">
                            <Form onSubmit={handleSendNotification} >

                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderNameToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5">User Name</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" defaultValue={name} maxLength={parseInt('64')} onChange={(val) => handleNameChanged(val.target.value)} required placeholder="Enter Name" />
                                    <span style={{ color: "red" }}>{nameErrors}</span>
                                </Form.Group>

                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderMessageToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5">Message</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control as="textarea" defaultValue={message} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handleMessageChanged(val.target.value)} required placeholder="Enter Message" />
                                    <span style={{ color: "red" }}>{messageErrors}</span>
                                </Form.Group >
                                <Form.Group className="form-group d-flex justify-content-end">
                                    <ButtonGroup className="justify-content-between">
                                        <Button id="sendMessageFormCancelBtn" className="action mr-2" onClick={(e) => handleCancel(e)}>Cancel</Button>
                                        <Button disabled={!isSendEnabled} type="submit" className="action btn-default">Send</Button>
                                    </ButtonGroup>
                                </Form.Group >
                            </Form >
                        </div>
                    </Col>
                </Row >
                <Modal show={show} onHide={handleClose}>
                    <Modal.Header closeButton>
                        <Modal.Title>Confirmation</Modal.Title>
                    </Modal.Header>
                    <Modal.Body className="text-center">
                        <b>Message was successfully sent!</b>
                    </Modal.Body>
                </Modal>
            </Container >
        )
    }

    let contents = props.isUserLoaded
        ? renderMessageForm()
        : <p><em>Loading...</em></p>;

    return (
        <div>
            <h1 id="tableLabel" >Send Notification</h1>
            {contents}
        </div>
    );
}

