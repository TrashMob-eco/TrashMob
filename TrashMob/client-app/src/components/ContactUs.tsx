import * as React from 'react'
import ContactRequestData from './Models/ContactRequestData';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { loadCaptchaEnginge, LoadCanvasTemplateNoReload, validateCaptcha } from 'react-simple-captcha';
import { getDefaultHeaders } from '../store/AuthStore';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../store/ToolTips";
import { Button, Col, Form } from 'react-bootstrap';

interface ContactUsProps extends RouteComponentProps<any> { }

export const ContactUs: React.FC<ContactUsProps> = (props) => {
    const [name, setName] = React.useState<string>();
    const [email, setEmail] = React.useState<string>();
    const [emailErrors, setEmailErrors] = React.useState<string>("");
    const [message, setMessage] = React.useState<string>("");
    const [messageErrors, setMessageErrors] = React.useState<string>("");

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        const form = new FormData(event.target);

        if (messageErrors !== "" || emailErrors !== "") {
            return;
        }

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
                props.history.push("/");
            })
        }

        else {
            alert('Captcha Does Not Match');
        }
    }

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.push("/");
    }

    function handleNameChanged(val: string) {
        setName(val);
    }

    function handleEmailChanged(val: string) {
        var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);

        if (!pattern.test(val)) {
            setEmailErrors("Please enter valid email address.");
        }
        else {
            setEmailErrors("");
            setEmail(val);
        }
    }

    function handleMessageChanged(val: string) {
        if (val.length < 0 || val.length > 1000) {
            setMessageErrors("Message cannot be empty and cannot be more than 1000 characters long.");
        }
        else {
            setMessageErrors("");
            setMessage(val);
        }
    }

    React.useEffect(() => {
        loadCaptchaEnginge(6);
    });


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
        <div className="container-fluid">
            <h1>Contact Us</h1>
            <div>
                Have a question for the TrashMob team? Or an idea to make this site better? Or just want to tell us you love us? Drop us a note!
                </div>
            <Form onSubmit={handleSave} >
                <Form.Row>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderNameToolTip}>
                                <Form.Label>Name:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={name} maxLength={parseInt('64')} onChange={(val) => handleNameChanged(val.target.value)} required />
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderEmailToolTip}>
                                <Form.Label>Email:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={email} maxLength={parseInt('64')} onChange={(val) => handleEmailChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{emailErrors}</span>
                        </Form.Group >
                    </Col>
                </Form.Row>
                <Form.Group>
                    <OverlayTrigger placement="top" overlay={renderMessageToolTip}>
                        <Form.Label>Message:</Form.Label>
                    </OverlayTrigger>
                    <Form.Control as="textarea" defaultValue={message} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handleMessageChanged(val.target.value)} required />
                    <span style={{ color: "red" }}>{messageErrors}</span>
                </Form.Group >
                <Form.Group>
                    <LoadCanvasTemplateNoReload />
                </Form.Group>
                <Form.Group>
                    <Form.Label>CAPTCHA Value:</Form.Label>
                    <Form.Control type="text" required name="user_captcha_input" />
                </Form.Group >
                <Form.Group className="form-group">
                    <Button disabled={emailErrors !== "" || messageErrors !== ""} type="submit" className="action btn-default">Save</Button>
                    <Button className="action" onClick={(e) => handleCancel(e)}>Cancel</Button>
                </Form.Group >
            </Form >
        </div>
    )
}

export default withRouter(ContactUs);