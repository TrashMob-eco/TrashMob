import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import PartnerData from '../Models/PartnerData';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as Constants from '../Models/Constants';
import * as ToolTips from "../../store/ToolTips";
import PartnerStatusData from '../Models/PartnerStatusData';

export interface PartnerEditDataProps {
    partner: PartnerData;
    partnerStatusList: PartnerStatusData[];
    isPartnerDataLoaded: boolean;
    isUserLoaded: boolean;
    currentUser: UserData;
    onPartnerUpdated: any;
    onEditCanceled: any;
};

export const PartnerEdit: React.FC<PartnerEditDataProps> = (props) => {

    const [name, setName] = React.useState<string>(props.partner.name);
    const [partnerStatusId, setPartnerStatusId] = React.useState<number>(props.partner.partnerStatusId);
    const [nameErrors, setNameErrors] = React.useState<string>("");
    const [primaryEmailErrors, setPrimaryEmailErrors] = React.useState<string>("");
    const [secondaryEmailErrors, setSecondaryEmailErrors] = React.useState<string>("");
    const [primaryPhoneErrors, setPrimaryPhoneErrors] = React.useState<string>("");
    const [secondaryPhoneErrors, setSecondaryPhoneErrors] = React.useState<string>("");
    const [notesErrors, setNotesErrors] = React.useState<string>("");
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);

    function validateForm() {
        if (name === "" ||
            nameErrors !== "") {
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

        var partnerData = new PartnerData();
        partnerData.id = props.partner.id;
        partnerData.name = name ?? "";
        partnerData.partnerStatusId = partnerStatusId ?? 2;
        partnerData.createdByUserId = props.partner.createdByUserId ?? props.currentUser.id;
        partnerData.createdDate = props.partner.createdDate;
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

        validateForm();
    }

    function renderNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestName}</Tooltip>
    }

    function renderPartnerStatusToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerStatus}</Tooltip>
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
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderNameToolTip}>
                                <Form.Label className="control-label">Partner Name:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={name} maxLength={parseInt('64')} onChange={(val) => handleNameChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{nameErrors}</span>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderPartnerStatusToolTip}>
                                <Form.Label className="control-label" htmlFor="Partner Status">Partner Status:</Form.Label>
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
                </Form.Row>
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
                            <Form.Control type="text" disabled defaultValue={props.partner.createdDate.toString()} />
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderLastUpdatedDateToolTip}>
                                <Form.Label className="control-label" htmlFor="lastUpdatedDate">Last Updated Date:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" disabled defaultValue={props.partner.lastUpdatedDate.toString()} />
                        </Form.Group>
                    </Col>
                </Form.Row>
            </Form >
        </div>
    )
}