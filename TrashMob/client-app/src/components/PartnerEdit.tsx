import * as React from 'react'
import { RouteComponentProps, useHistory } from 'react-router-dom';
import UserData from './Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import PartnerData from './Models/PartnerData';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import * as Constants from './Models/Constants';
import * as ToolTips from "../store/ToolTips";
import * as MapStore from '../store/MapStore';
import { data } from 'azure-maps-control';
import { IAzureMapOptions } from 'react-azure-maps';
import PartnerStatusData from './Models/PartnerStatusData';

export interface PartnerEditDataProps extends RouteComponentProps {
    partner: PartnerData;
    partnerStatusList: PartnerStatusData[];
    isPartnerDataLoaded: boolean;
    isUserLoaded: boolean;
    currentUser: UserData;
    onPartnerUpdated: any;
    onEditCanceled: any;
};

export const PartnerEdit: React.FC<PartnerEditDataProps> = (props) => {

    const [partnerId, setPartnerId] = React.useState<string>(props.partner.id);
    const [name, setName] = React.useState<string>(props.partner.name);
    const [primaryEmail, setPrimaryEmail] = React.useState<string>(props.partner.primaryEmail);
    const [secondaryEmail, setSecondaryEmail] = React.useState<string>(props.partner.secondaryEmail);
    const [primaryPhone, setPrimaryPhone] = React.useState<string>(props.partner.primaryPhone);
    const [secondaryPhone, setSecondaryPhone] = React.useState<string>(props.partner.secondaryPhone);
    const [partnerStatusId, setPartnerStatusId] = React.useState<number>(props.partner.partnerStatusId);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>(props.partner.createdByUserId);
    const [createdDate, setCreatedDate] = React.useState<Date>(props.partner.createdDate);
    const [lastUpdatedByUserId, setLastUpdatedByUserId] = React.useState<string>(props.partner.lastUpdatedByUserId);
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(props.partner.lastUpdatedDate);
    const [notes, setNotes] = React.useState<string>(props.partner.notes);
    const [nameErrors, setNameErrors] = React.useState<string>("");
    const [primaryEmailErrors, setPrimaryEmailErrors] = React.useState<string>("");
    const [secondaryEmailErrors, setSecondaryEmailErrors] = React.useState<string>("");
    const [primaryPhoneErrors, setPrimaryPhoneErrors] = React.useState<string>("");
    const [secondaryPhoneErrors, setSecondaryPhoneErrors] = React.useState<string>("");
    const [notesErrors, setNotesErrors] = React.useState<string>("");
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [currentUser, setCurrentUser] = React.useState<UserData>(props.currentUser);
    const [isUserLoaded, setIsUserLoaded] = React.useState<boolean>(props.isUserLoaded);
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));

    React.useEffect(() => {
        setCurrentUser(props.currentUser);
        setIsUserLoaded(props.isUserLoaded);

        MapStore.getOption().then(opts => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        })

        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(position => {
                var point = new data.Position(position.coords.longitude, position.coords.latitude);
                setCenter(point)
            });
        } else {
            console.log("Not Available");
        }
    }, [props.currentUser, props.isUserLoaded]);

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        if (nameErrors !== "" ||
            notesErrors !== "" ||
            primaryEmailErrors !== "" ||
            secondaryEmailErrors !== "" ||
            primaryPhoneErrors !== "" ||
            secondaryPhoneErrors !== "") {
            return;
        }

        var partnerData = new PartnerData();
        partnerData.id = partnerId;
        partnerData.name = name ?? "";
        partnerData.primaryEmail = primaryEmail ?? "";
        partnerData.secondaryEmail = secondaryEmail ?? "";
        partnerData.primaryPhone = primaryPhone ?? "";
        partnerData.secondaryPhone = secondaryPhone ?? "";
        partnerData.notes = notes ?? "";
        partnerData.partnerStatusId = partnerStatusId ?? 2;
        partnerData.createdByUserId = createdByUserId ?? props.currentUser.id;
        partnerData.createdDate = createdDate ?? props.partner.createdDate;
        partnerData.lastUpdatedByUserId = props.currentUser.id;

        var data = JSON.stringify(partnerData);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('PUT');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/Partners', {
                method: 'PUT',
                body: data,
                headers: headers,
            })
                .then(() => {
                    props.onPartnerUpdated()
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

    function renderNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestName}</Tooltip>
    }

    function renderPartnerStatusToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerStatus}</Tooltip>
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

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerCreatedDate}</Tooltip>
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLastUpdatedDate}</Tooltip>
    }

    function selectPartnerStatus(val: string) {
        setPartnerStatusId(parseInt(val));
    }
    return (
        <div className="container-fluid card">
            <h1>Edit Partner</h1>
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
                            <OverlayTrigger placement="top" overlay={renderPartnerStatusToolTip}>
                                <Form.Label htmlFor="Partner Status">Partner Status:</Form.Label>
                            </OverlayTrigger>
                            <div>
                                <select data-val="true" name="partnerStatusId" defaultValue={partnerStatusId} onChange={(val) => selectPartnerStatus(val.target.value)} required>
                                    <option value="">-- Select Partner Status --</option>
                                    {props.partnerStatusList.map(status =>
                                        <option key={status.id} value={status.id}>{status.name}</option>
                                    )}
                                </select>
                            </div>
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
                <Form.Group className="form-group">
                    <Button disabled={primaryEmailErrors !== "" || secondaryEmailErrors !== "" || primaryPhoneErrors !== "" || secondaryPhoneErrors !== "" || notesErrors !== ""} type="submit" className="action btn-default">Save</Button>
                    <Button className="action" onClick={(e) => handleCancel(e)}>Cancel</Button>
                </Form.Group >
                <Form.Row>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderCreatedDateToolTip}>
                                <Form.Label htmlFor="createdDate">Created Date:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" disabled defaultValue={createdDate.toString()} />
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderLastUpdatedDateToolTip}>
                                <Form.Label htmlFor="lastUpdatedDate">Last Updated Date:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" disabled defaultValue={lastUpdatedDate.toString()} />
                        </Form.Group>
                    </Col>
                </Form.Row>
            </Form >
        </div>
    )
}