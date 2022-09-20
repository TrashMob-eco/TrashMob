import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import PartnerData from '../Models/PartnerData';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import PartnerStatusData from '../Models/PartnerStatusData';

export interface PartnerEditDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerEdit: React.FC<PartnerEditDataProps> = (props) => {

    const [isPartnerDataLoaded, setIsPartnerDataLoaded] = React.useState<boolean>(false);
    const [partnerMessage, setPartnerMessage] = React.useState<string>("");
    const [partnerStatusList, setPartnerStatusList] = React.useState<PartnerStatusData[]>([]);
    const [name, setName] = React.useState<string>();
    const [partnerStatusId, setPartnerStatusId] = React.useState<number>(0);
    const [city, setCity] = React.useState<string>();
    const [country, setCountry] = React.useState<string>();
    const [region, setRegion] = React.useState<string>();
    const [postalCode, setPostalCode] = React.useState<string>();
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [nameErrors, setNameErrors] = React.useState<string>("");
    const [createdByUserId, setCreatedByUserId] = React.useState<string>("");
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedByUserId, setLastUpdatedByUserId] = React.useState<string>("");
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);

    React.useEffect(() => {
        if (props.isUserLoaded) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partnerstatuses', {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<PartnerStatusData[]>)
                    .then(data => {
                        setPartnerStatusList(data)
                    })
                    .then(_ => {
                        setIsPartnerDataLoaded(false);
                        setPartnerMessage("Loading");

                        fetch('/api/partners/' + props.partnerId, {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => response.json() as Promise<PartnerData>)
                            .then(data => {
                                setPartnerStatusId(data.partnerStatusId);
                                setCity(data.city);
                                setRegion(data.region);
                                setCountry(data.country);
                                setPostalCode(data.postalCode);
                                setLatitude(data.latitude)
                                setLongitude(data.longitude)
                                setIsPartnerDataLoaded(true);
                            })
                    })
            });
        }
    }, [props.currentUser, props.isUserLoaded]);

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
        partnerData.id = props.partnerId;
        partnerData.name = name ?? "";
        partnerData.partnerStatusId = partnerStatusId ?? 2;
        partnerData.createdByUserId = createdByUserId ?? props.currentUser.id;
        partnerData.createdDate = createdDate;
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
        });
    }

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
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
                                    {partnerStatusList.map(status =>
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
                            <Form.Control type="text" disabled defaultValue={createdDate.toString()} />
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderLastUpdatedDateToolTip}>
                                <Form.Label className="control-label" htmlFor="lastUpdatedDate">Last Updated Date:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" disabled defaultValue={lastUpdatedDate.toString()} />
                        </Form.Group>
                    </Col>
                </Form.Row>
            </Form >
        </div>
    )
}