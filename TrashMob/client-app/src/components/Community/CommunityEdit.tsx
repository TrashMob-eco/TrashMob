import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import CommunityData from '../Models/CommunityData';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as Constants from '../Models/Constants';
import * as ToolTips from "../../store/ToolTips";
import CommunityStatusData from '../Models/CommunityStatusData';

export interface CommunityEditDataProps {
    community: CommunityData;
    communityStatusList: CommunityStatusData[];
    isCommunityDataLoaded: boolean;
    isUserLoaded: boolean;
    currentUser: UserData;
    onCommunityUpdated: any;
    onEditCanceled: any;
};

export const CommunityEdit: React.FC<CommunityEditDataProps> = (props) => {

    const [city, setCity] = React.useState<string>(props.community.city);
    const [region, setRegion] = React.useState<string>(props.community.region);
    const [country, setCountry] = React.useState<string>(props.community.country);
    const [postalCode, setPostalCode] = React.useState<string>(props.community.postalCode);
    const [secondaryPhone, setSecondaryPhone] = React.useState<string>(props.community.secondaryPhone);
    const [communityStatusId, setCommunityStatusId] = React.useState<number>(props.community.communityStatusId);
    const [notes, setNotes] = React.useState<string>(props.community.notes);
    const [nameErrors, setNameErrors] = React.useState<string>("");
    const [primaryEmailErrors, setPrimaryEmailErrors] = React.useState<string>("");
    const [secondaryEmailErrors, setSecondaryEmailErrors] = React.useState<string>("");
    const [primaryPhoneErrors, setPrimaryPhoneErrors] = React.useState<string>("");
    const [secondaryPhoneErrors, setSecondaryPhoneErrors] = React.useState<string>("");
    const [notesErrors, setNotesErrors] = React.useState<string>("");
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);

    function validateForm() {
        if (name === "" ||
            nameErrors !== "" ||
            notes === "" ||
            notesErrors !== "" ||
            primaryEmail === "" ||
            primaryEmailErrors !== "" ||
            secondaryEmail === "" ||
            secondaryEmailErrors !== "" ||
            primaryPhone === "" ||
            primaryPhoneErrors !== "" ||
            secondaryPhone === "" ||
            secondaryPhoneErrors !== "") {
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

        var communityData = new CommunityData();
        communityData.id = props.community.id;
        communityData.name = name ?? "";
        communityData.primaryEmail = primaryEmail ?? "";
        communityData.secondaryEmail = secondaryEmail ?? "";
        communityData.primaryPhone = primaryPhone ?? "";
        communityData.secondaryPhone = secondaryPhone ?? "";
        communityData.notes = notes ?? "";
        communityData.communityStatusId = communityStatusId ?? 2;
        communityData.createdByUserId = props.community.createdByUserId ?? props.currentUser.id;
        communityData.createdDate = props.community.createdDate;
        communityData.lastUpdatedByUserId = props.currentUser.id;

        var data = JSON.stringify(communityData);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('PUT');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/Communitys', {
                method: 'PUT',
                body: data,
                headers: headers,
            })
                .then(() => {
                    props.onCommunityUpdated()
                });
        });
    }

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        props.onEditCanceled();
    }

    function handleNameChanged(val: string) {
        if (name === "") {
            setNameErrors("Name cannot be blank.");
        }
        else {
            setNameErrors("");
            setName(val);
        }

        validateForm();
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

        validateForm();
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

        validateForm();
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

        validateForm();
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

    function renderNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestName}</Tooltip>
    }

    function renderCommunityStatusToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityStatus}</Tooltip>
    }

    function renderPrimaryEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestPrimaryEmail}</Tooltip>
    }

    function renderSecondaryEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestSecondaryEmail}</Tooltip>
    }

    function renderPrimaryPhoneToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestPrimaryPhone}</Tooltip>
    }

    function renderSecondaryPhoneToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestSecondaryPhone}</Tooltip>
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestNotes}</Tooltip>
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityCreatedDate}</Tooltip>
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityLastUpdatedDate}</Tooltip>
    }

    function selectCommunityStatus(val: string) {
        setCommunityStatusId(parseInt(val));
    }

    return (
        <div className="container-fluid card">
            <h1>Edit Community</h1>
            <Form onSubmit={handleSave} >
                <Form.Row>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderNameToolTip}>
                                <Form.Label className="control-label">Community Name:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={name} maxLength={parseInt('64')} onChange={(val) => handleNameChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{nameErrors}</span>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderCommunityStatusToolTip}>
                                <Form.Label className="control-label" htmlFor="Community Status">Community Status:</Form.Label>
                            </OverlayTrigger>
                            <div>
                                <select data-val="true" name="communityStatusId" defaultValue={communityStatusId} onChange={(val) => selectCommunityStatus(val.target.value)} required>
                                    <option value="">-- Select Community Status --</option>
                                    {props.communityStatusList.map(status =>
                                        <option key={status.id} value={status.id}>{status.name}</option>
                                    )}
                                </select>
                            </div>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderPrimaryEmailToolTip}>
                                <Form.Label className="control-label">Primary Email:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={primaryEmail} maxLength={parseInt('64')} onChange={(val) => handlePrimaryEmailChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{primaryEmailErrors}</span>
                        </Form.Group >
                    </Col>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderSecondaryEmailToolTip}>
                                <Form.Label className="control-label">Secondary Email:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={secondaryEmail} maxLength={parseInt('64')} onChange={(val) => handleSecondaryEmailChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{secondaryEmailErrors}</span>
                        </Form.Group >
                    </Col>
                </Form.Row>
                <Form.Row>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderPrimaryPhoneToolTip}>
                                <Form.Label className="control-label">Primary Phone:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={primaryPhone} maxLength={parseInt('64')} onChange={(val) => handlePrimaryPhoneChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{primaryPhoneErrors}</span>
                        </Form.Group >
                    </Col>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderSecondaryPhoneToolTip}>
                                <Form.Label className="control-label">Secondary Phone:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={secondaryPhone} maxLength={parseInt('64')} onChange={(val) => handleSecondaryPhoneChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{secondaryPhoneErrors}</span>
                        </Form.Group >
                    </Col>
                </Form.Row>
                <Form.Group className="required">
                    <OverlayTrigger placement="top" overlay={renderNotesToolTip}>
                        <Form.Label className="control-label">Notes:</Form.Label>
                    </OverlayTrigger>
                    <Form.Control as="textarea" defaultValue={notes} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handleNotesChanged(val.target.value)} required />
                    <span style={{ color: "red" }}>{notesErrors}</span>
                </Form.Group >
                <Form.Group className="form-group">
                    <Button disabled={!isSaveEnabled} type="submit" className="action btn-default">Save</Button>
                    <Button className="action" onClick={(e) => handleCancel(e)}>Cancel</Button>
                </Form.Group >
                <Form.Row>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderCreatedDateToolTip}>
                                <Form.Label className="control-label" htmlFor="createdDate">Created Date:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" disabled defaultValue={props.community.createdDate.toString()} />
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderLastUpdatedDateToolTip}>
                                <Form.Label className="control-label" htmlFor="lastUpdatedDate">Last Updated Date:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" disabled defaultValue={props.community.lastUpdatedDate.toString()} />
                        </Form.Group>
                    </Col>
                </Form.Row>
            </Form >
        </div>
    )
}