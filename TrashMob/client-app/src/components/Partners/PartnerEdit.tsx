import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Container, Form, OverlayTrigger, Row, Tooltip } from 'react-bootstrap';
import PartnerData from '../Models/PartnerData';
import { getApiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import PartnerStatusData from '../Models/PartnerStatusData';
import PartnerTypeData from '../Models/PartnerTypeData';

export interface PartnerEditDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerEdit: React.FC<PartnerEditDataProps> = (props) => {

    const [isPartnerDataLoaded, setIsPartnerDataLoaded] = React.useState<boolean>(false);
    const [partnerStatusList, setPartnerStatusList] = React.useState<PartnerStatusData[]>([]);
    const [partnerTypeList, setPartnerTypeList] = React.useState<PartnerTypeData[]>([]);
    const [name, setName] = React.useState<string>("");
    const [website, setWebsite] = React.useState<string>("");
    const [partnerStatusId, setPartnerStatusId] = React.useState<number>(0);
    const [partnerTypeId, setPartnerTypeId] = React.useState<number>(0);
    const [nameErrors, setNameErrors] = React.useState<string>("");
    const [publicNotes, setPublicNotes] = React.useState<string>("");
    const [publicNotesErrors, setPublicNotesErrors] = React.useState<string>();
    const [privateNotes, setPrivateNotes] = React.useState<string>("");

    const [createdByUserId, setCreatedByUserId] = React.useState<string>("");
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);

    React.useEffect(() => {
        if (props.isUserLoaded) {
            const account = msalClient.getAllAccounts()[0];
            var apiConfig = getApiConfig();

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
                    .then(() => {
                        fetch('/api/partnertypes', {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => response.json() as Promise<PartnerTypeData[]>)
                            .then(data => {
                                setPartnerTypeList(data)
                            })
                            .then(_ => {
                                setIsPartnerDataLoaded(false);

                                fetch('/api/partners/' + props.partnerId, {
                                    method: 'GET',
                                    headers: headers
                                })
                                    .then(response => response.json() as Promise<PartnerData>)
                                    .then(data => {
                                        setPartnerStatusId(data.partnerStatusId);
                                        setPartnerTypeId(data.partnerTypeId);
                                        setName(data.name);
                                        setPublicNotes(data.publicNotes);
                                        setPrivateNotes(data.privateNotes);
                                        setWebsite(data.website);
                                        setCreatedByUserId(data.createdByUserId);
                                        setCreatedDate(new Date(data.createdDate));
                                        setLastUpdatedDate(new Date(data.lastUpdatedDate));
                                        setIsPartnerDataLoaded(true);
                                    })
                            })
                    })
            });
        }
    }, [props.currentUser, props.isUserLoaded, props.partnerId]);

    React.useEffect(() => {
        if (name === "" ||
            nameErrors !== "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }, [name, nameErrors]);

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
        partnerData.website = website ?? "";
        partnerData.partnerStatusId = partnerStatusId ?? 2;
        partnerData.publicNotes = publicNotes;
        partnerData.privateNotes = privateNotes;
        partnerData.partnerTypeId = partnerTypeId;
        partnerData.createdByUserId = createdByUserId ?? props.currentUser.id;
        partnerData.createdDate = createdDate;

        var data = JSON.stringify(partnerData);

        const account = msalClient.getAllAccounts()[0];
        var apiConfig = getApiConfig();

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
        if (val === "") {
            setNameErrors("Name cannot be blank.");
        }
        else {
            setNameErrors("");
            setName(val);
        }
    }

    function handlePublicNotesChanged(notes: string) {
        if (notes === "") {
            setPublicNotesErrors("Notes cannot be empty.");
        }
        else {
            setPublicNotes(notes);
            setPublicNotesErrors("");
        }
    }

    function handlePrivateNotesChanged(notes: string) {
        setPrivateNotes(notes);
    }

    function handleWebsiteChanged(val: string) {
        setWebsite(val);
    }

    function selectPartnerStatus(val: string) {
        setPartnerStatusId(parseInt(val));
    }

    function selectPartnerType(val: string) {
        setPartnerTypeId(parseInt(val));
    }

    function renderNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerName}</Tooltip>
    }

    function renderWebsiteToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerWebsite}</Tooltip>
    }

    function renderPublicNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerPublicNotes}</Tooltip>
    }

    function renderPrivateNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerPrivateNotes}</Tooltip>
    }

    function renderPartnerStatusToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerStatus}</Tooltip>
    }

    function renderPartnerTypeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerType}</Tooltip>
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerCreatedDate}</Tooltip>
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLastUpdatedDate}</Tooltip>
    }

    // Returns the HTML Form to the render() method.  
    function renderCreateForm(statusList: Array<PartnerStatusData>, typeList: Array<PartnerTypeData>) {
        return (
            <Container>
                <Row className="gx-2 py-5" lg={2}>
                    <Col lg={4} className="d-flex">
                        <div className="bg-white py-2 px-5 shadow-sm rounded">
                            <h2 className="color-primary mt-4 mb-5">Edit Partner Information</h2>
                            <p>
                                This page allows you to add basic details about your organization. Public notes may be shown to TrashMob.eco users on the partnership page. Think of this as a blurb or a tag line you may want to add to let users know more about your organization in general.
                            </p>
                        </div>
                    </Col>
                    <Col lg={8}>
                        <div className="bg-white p-5 shadow-sm rounded">
                            <h2 className="color-primary mt-4 mb-5">Edit Partner</h2>
                            <Form onSubmit={handleSave} >
                                <Form.Row>
                                    <Col>
                                        <Form.Group className="required">
                                            <OverlayTrigger placement="top" overlay={renderNameToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5">Partner Name</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" defaultValue={name} maxLength={parseInt('64')} onChange={(val) => handleNameChanged(val.target.value)} required />
                                            <span style={{ color: "red" }}>{nameErrors}</span>
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderWebsiteToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5">Website</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" defaultValue={website} maxLength={parseInt('1024')} onChange={(val) => handleWebsiteChanged(val.target.value)} />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group className="required">
                                            <OverlayTrigger placement="top" overlay={renderPartnerStatusToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5" htmlFor="Partner Status">Partner Status</Form.Label>
                                            </OverlayTrigger>
                                            <div>
                                                <select data-val="true" name="partnerStatusId" defaultValue={partnerStatusId} onChange={(val) => selectPartnerStatus(val.target.value)} required>
                                                    <option value="">-- Select Partner Status --</option>
                                                    {statusList.map(status =>
                                                        <option key={status.id} value={status.id}>{status.name}</option>
                                                    )}
                                                </select>
                                            </div>
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group className="required">
                                            <OverlayTrigger placement="top" overlay={renderPartnerTypeToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5" htmlFor="PartnerType">Partner Type</Form.Label>
                                            </OverlayTrigger>
                                            <div>
                                                <select data-val="true" name="partnerTypeId" defaultValue={partnerTypeId} onChange={(val) => selectPartnerType(val.target.value)} required>
                                                    <option value="">-- Select Partner Type --</option>
                                                    {typeList.map(partnerType =>
                                                        <option key={partnerType.id} value={partnerType.id}>{partnerType.name}</option>
                                                    )}
                                                </select>
                                            </div>
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderPublicNotesToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5">Public Notes</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control as="textarea" defaultValue={publicNotes} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handlePublicNotesChanged(val.target.value)} required />
                                    <span style={{ color: "red" }}>{publicNotesErrors}</span>
                                </Form.Group >
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderPrivateNotesToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5">Private Notes</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control as="textarea" defaultValue={privateNotes} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handlePrivateNotesChanged(val.target.value)} />
                                </Form.Group >
                                <Form.Group className="form-group">
                                    <Button disabled={!isSaveEnabled} type="submit" className="action btn-default">Save</Button>
                                    <Button className="action" onClick={(e) => handleCancel(e)}>Cancel</Button>
                                </Form.Group >
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderCreatedDateToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5" htmlFor="createdDate">Created Date</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="createdDate" value={createdDate ? createdDate.toLocaleString() : ""} />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderLastUpdatedDateToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5" htmlFor="lastUpdatedDate">Last Updated Date</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="lastUpdatedDate" value={lastUpdatedDate ? lastUpdatedDate.toLocaleString() : ""} />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                            </Form >
                        </div>
                    </Col>
                </Row>
            </Container>
        )
    }

    var contents = isPartnerDataLoaded && props.partnerId
        ? renderCreateForm(partnerStatusList, partnerTypeList)
        : <p><em>Loading...</em></p>;

    return <div>
        <hr />
        {contents}
    </div>;
}