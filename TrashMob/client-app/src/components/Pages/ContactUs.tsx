import * as React from 'react'
import ContactRequestData from '../Models/ContactRequestData';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { loadCaptchaEnginge, LoadCanvasTemplateNoReload, validateCaptcha } from 'react-simple-captcha';
import { getDefaultHeaders } from '../../store/AuthStore';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../../store/ToolTips";
import { Button, ButtonGroup, Col, Container, Form, Modal, Row } from 'react-bootstrap';
import * as Constants from '../Models/Constants';

interface ContactUsProps extends RouteComponentProps<any> { }

export const ContactUs: React.FC<ContactUsProps> = (props) => {
    const [name, setName] = React.useState<string>();
    const [email, setEmail] = React.useState<string>();
    const [nameErrors, setNameErrors] = React.useState<string>("");
    const [emailErrors, setEmailErrors] = React.useState<string>("");
    const [message, setMessage] = React.useState<string>("");
    const [messageErrors, setMessageErrors] = React.useState<string>("");
    const [show, setShow] = React.useState<boolean>(false);
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);

    const handleClose = () => setShow(false);
    const handleShow = () => setShow(true);

    React.useEffect(() => {
        window.scrollTo(0, 0);
    })

    // This will handle the submit form event.  
    function handleSave(event: any) {

        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);
        event.preventDefault();

        const form = new FormData(event.target);

        var user_captcha_value = form.get("user_captcha_input")?.toString() ?? "";

        if (validateCaptcha(user_captcha_value) === true) {

            var contactRequestData = new ContactRequestData();
            contactRequestData.name = name ?? "";
            contactRequestData.email = email ?? "";
            contactRequestData.message = message ?? "";

            var data = JSON.stringify(contactRequestData);

            const headers = getDefaultHeaders('POST');

            fetch('/api/ContactRequest', {
                method: 'POST',
                body: data,
                headers: headers,
            }).then(() => {
                setTimeout(() => props.history.push("/"), 2000);
            });

            handleShow();
        }
        else {
            alert('Captcha Does Not Match');
        }
    }

    React.useEffect(() => {
        if (nameErrors !== "" || emailErrors !== "" || messageErrors !== "" || name === "" || email === "" || message === "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }, [nameErrors, emailErrors, messageErrors, name, email, message ]);

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        setIsSaveEnabled(false);
        props.history.push("/");
    }

    function handleNameChanged(val: string) {
        if (val.length <= 0 || val.length > 64) {
            setNameErrors("Please enter a valid name.");
        }
        else {
            setNameErrors("");
            setName(val);
        }
    }

    function handleEmailChanged(val: string) {
        var pattern = new RegExp(Constants.RegexEmail);

        if (!pattern.test(val)) {
            setEmailErrors("Please enter valid email address.");
        }
        else {
            setEmailErrors("");
            setEmail(val);
        }
    }

    function handleMessageChanged(val: string) {
        if (val.length <= 0 || val.length > 1000) {
            setMessageErrors("Message cannot be empty and cannot be more than 1000 characters long.");
        }
        else {
            setMessageErrors("");
            setMessage(val);
        }
    }

    React.useEffect(() => {
        loadCaptchaEnginge(6, 'white', 'black');
    }, []);


    function renderNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.ContactUsName}</Tooltip>
    }

    function renderEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.ContactUsEmail}</Tooltip>
    }

    function renderMessageToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.ContactUsMessage}</Tooltip>
    }

    return (
        <Container>
            <Row className="gx-2 py-5" lg={2}>
                <Col lg={4} className="d-flex">
                    <div className="bg-white py-2 px-5 shadow-sm rounded">
                        <h2 className="color-primary mt-4 mb-5">Contact Us</h2>
                        <p>
                            Have a question for the TrashMob team, want to submit a suggestion for improving the website, or just want to tell us you love us? Drop us a note here and we'll be sure to read it.
                        </p>
                        <p>
                            Can't wait to hear from you!
                        </p>
                    </div>
                </Col>
                <Col lg={8}>
                    <div className="bg-white p-5 shadow-sm rounded">
                        <Form onSubmit={handleSave} >

                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderNameToolTip}>
                                    <Form.Label className="control-label font-weight-bold h5">Name</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={name} maxLength={parseInt('64')} onChange={(val) => handleNameChanged(val.target.value)} required placeholder="Enter Name" />
                                <span style={{ color: "red" }}>{nameErrors}</span>
                            </Form.Group>

                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderEmailToolTip}>
                                    <Form.Label className="control-label font-weight-bold h5">Email</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={email} maxLength={parseInt('64')} onChange={(val) => handleEmailChanged(val.target.value)} required placeholder="Enter Email" />
                                <span style={{ color: "red" }}>{emailErrors}</span>
                            </Form.Group >

                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderMessageToolTip}>
                                    <Form.Label className="control-label font-weight-bold h5">Message</Form.Label>
                                </OverlayTrigger>
                                <Form.Control as="textarea" defaultValue={message} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handleMessageChanged(val.target.value)} required placeholder="Enter Message" />
                                <span style={{ color: "red" }}>{messageErrors}</span>
                            </Form.Group >
                            <Form.Group>
                                <LoadCanvasTemplateNoReload className="border" />
                            </Form.Group>
                            <Form.Group className="required">
                                <Form.Label className="control-label font-weight-bold h5">CAPTCHA Value</Form.Label>
                                <Form.Control type="text" required name="user_captcha_input" placeholder="Enter Captcha" />
                            </Form.Group >
                            <Form.Group className="form-group d-flex justify-content-end">
                                <ButtonGroup className="justify-content-between">
                                    <Button className="action mr-2 event-list-event-type" onClick={(e) => handleCancel(e)}>Cancel</Button>
                                    <Button disabled={!isSaveEnabled} type="submit" className="action btn-default">Submit</Button>
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
                    <br />
                    <small>You'll now be redirected to the TrashMob.eco home page...</small>
                </Modal.Body>
            </Modal>
        </Container >
    )
}

export default withRouter(ContactUs);