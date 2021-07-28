import * as React from 'react'
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { loadCaptchaEnginge, LoadCanvasTemplateNoReload, validateCaptcha } from 'react-simple-captcha';
import { getDefaultHeaders } from '../store/AuthStore';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../store/ToolTips";
import { Button, Col, Form } from 'react-bootstrap';
import PartnerRequestData from './Models/PartnerRequestData';
import UserData from './Models/UserData';
import * as Constants from './Models/Constants';

interface BecomeAPartnerProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const BecomeAPartner: React.FC<BecomeAPartnerProps> = (props) => {
    const [name, setName] = React.useState<string>();
    const [primaryEmail, setPrimaryEmail] = React.useState<string>();
    const [secondaryEmail, setSecondaryEmail] = React.useState<string>();
    const [primaryPhone, setPrimaryPhone] = React.useState<string>();
    const [secondaryPhone, setSecondaryPhone] = React.useState<string>();
    const [notes, setNotes] = React.useState<string>("");
    const [nameErrors, setNameErrors] = React.useState<string>("");
    const [primaryEmailErrors, setPrimaryEmailErrors] = React.useState<string>("");
    const [secondaryEmailErrors, setSecondaryEmailErrors] = React.useState<string>("");
    const [primaryPhoneErrors, setPrimaryPhoneErrors] = React.useState<string>("");
    const [secondaryPhoneErrors, setSecondaryPhoneErrors] = React.useState<string>("");
    const [notesErrors, setNotesErrors] = React.useState<string>("");

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        const form = new FormData(event.target);

        if (nameErrors !== "" ||
            notesErrors !== "" ||
            primaryEmailErrors !== "" ||
            secondaryEmailErrors !== "" ||
            primaryPhoneErrors !== "" ||
            secondaryPhoneErrors !== "") {
            return;
        }

        var user_captcha_value = form.get("user_captcha_input")?.toString() ?? "";

        if (validateCaptcha(user_captcha_value) === true) {

            var partnerRequestData = new PartnerRequestData();
            partnerRequestData.name = name ?? "";
            partnerRequestData.primaryEmail = primaryEmail ?? "";
            partnerRequestData.secondaryEmail = secondaryEmail ?? "";
            partnerRequestData.primaryPhone = primaryPhone ?? "";
            partnerRequestData.secondaryPhone = secondaryPhone ?? "";
            partnerRequestData.partnerRequestStatusId = 1;
            partnerRequestData.notes = notes ?? "";
            partnerRequestData.createdByUserId = props.currentUser.id;
            partnerRequestData.lastUpdatedByUserId = props.currentUser.id;

            var data = JSON.stringify(partnerRequestData);

            const headers = getDefaultHeaders('POST');

            fetch('/api/PartnerRequests', {
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
        if (name === "") {
            setNameErrors("Name cannot be blank.");
        }
        else {
            setNameErrors("");
            setName(val);
        }
    }

    function handlePrimaryEmailChanged(val: string) {
        var pattern = new RegExp(Constants.RegexEmail);

        if (!pattern.test(val)) {
            setPrimaryEmailErrors("Please enter valid email address.");
        }
        else {
            setPrimaryEmailErrors("");
            setPrimaryEmail(val);
        }
    }

    function handleSecondaryEmailChanged(val: string) {
        var pattern = new RegExp(Constants.RegexEmail);

        if (!pattern.test(val)) {
            setSecondaryEmailErrors("Please enter valid email address.");
        }
        else {
            setSecondaryEmailErrors("");
            setSecondaryEmail(val);
        }
    }

    function handlePrimaryPhoneChanged(val: string) {
        var pattern = new RegExp(Constants.RegexPhoneNumber);

        if (!pattern.test(val)) {
            setPrimaryPhoneErrors("Please enter a valid phone number.");
        }
        else {
            setPrimaryPhoneErrors("");
            setPrimaryPhone(val);
        }
    }

    function handleSecondaryPhoneChanged(val: string) {
        var pattern = new RegExp(Constants.RegexPhoneNumber);

        if (!pattern.test(val)) {
            setSecondaryPhoneErrors("Please enter a valid phone number.");
        }
        else {
            setSecondaryPhoneErrors("");
            setSecondaryPhone(val);
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

    React.useEffect(() => {
        loadCaptchaEnginge(6);
    }, []);


    function renderNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.ContactUsName}</Tooltip>
    }

    function renderPrimaryEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestPrimaryEmail}</Tooltip>
    }

    function renderSecondaryEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestSecondaryEmail}</Tooltip>
    }

    function renderPrimaryPhoneToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestPrimaryPhone}</Tooltip>
    }

    function renderSecondaryPhoneToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestSecondaryPhone}</Tooltip>
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestNotes}</Tooltip>
    }

    return (
        <div className="container-fluid card">
            <h1>Become a Partner!</h1>
            <Form onSubmit={handleSave} >
                <Form.Row>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderNameToolTip}>
                                <Form.Label>Partner Name:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={name} maxLength={parseInt('64')} onChange={(val) => handleNameChanged(val.target.value)} required />
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderPrimaryEmailToolTip}>
                                <Form.Label>Primary Email:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={primaryEmail} maxLength={parseInt('64')} onChange={(val) => handlePrimaryEmailChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{primaryEmailErrors}</span>
                        </Form.Group >
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderSecondaryEmailToolTip}>
                                <Form.Label>Secondary Email:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={secondaryEmail} maxLength={parseInt('64')} onChange={(val) => handleSecondaryEmailChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{secondaryEmailErrors}</span>
                        </Form.Group >
                    </Col>
                </Form.Row>
                <Form.Row>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderPrimaryPhoneToolTip}>
                                <Form.Label>Primary Phone:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={primaryPhone} maxLength={parseInt('64')} onChange={(val) => handlePrimaryPhoneChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{primaryPhoneErrors}</span>
                        </Form.Group >
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderSecondaryPhoneToolTip}>
                                <Form.Label>Secondary Phone:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={secondaryPhone} maxLength={parseInt('64')} onChange={(val) => handleSecondaryPhoneChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{secondaryPhoneErrors}</span>
                        </Form.Group >
                    </Col>
                </Form.Row>
                <Form.Group>
                    <OverlayTrigger placement="top" overlay={renderNotesToolTip}>
                        <Form.Label>Notes:</Form.Label>
                    </OverlayTrigger>
                    <Form.Control as="textarea" defaultValue={notes} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handleNotesChanged(val.target.value)} required />
                    <span style={{ color: "red" }}>{notesErrors}</span>
                </Form.Group >
                <Form.Group>
                    <LoadCanvasTemplateNoReload />
                </Form.Group>
                <Form.Group>
                    <Form.Label>CAPTCHA Value:</Form.Label>
                    <Form.Control type="text" required name="user_captcha_input" />
                </Form.Group >
                <Form.Group className="form-group">
                    <Button disabled={primaryEmailErrors !== "" || secondaryEmailErrors !== "" || primaryPhoneErrors !== "" || secondaryPhoneErrors !== "" || notesErrors !== ""} type="submit" className="action btn-default">Save</Button>
                    <Button className="action" onClick={(e) => handleCancel(e)}>Cancel</Button>
                </Form.Group >
            </Form >
        </div>
    )
}

export default withRouter(BecomeAPartner);