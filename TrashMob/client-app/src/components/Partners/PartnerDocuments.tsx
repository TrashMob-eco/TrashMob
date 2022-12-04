import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Container, Dropdown, Form, OverlayTrigger, Row, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import { Guid } from 'guid-typescript';
import PartnerDocumentData from '../Models/PartnerDocumentData';
import { Pencil, XSquare } from 'react-bootstrap-icons';

export interface PartnerDocumentsDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerDocuments: React.FC<PartnerDocumentsDataProps> = (props) => {

    const [partnerDocumentId, setPartnerDocumentId] = React.useState<string>(Guid.EMPTY);
    const [documentName, setDocumentName] = React.useState<string>("");
    const [documentUrl, setDocumentUrl] = React.useState<string>("");
    const [createdByUserId, setCreatedByUserId] = React.useState<string>(Guid.EMPTY);
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [documentNameErrors, setDocumentNameErrors] = React.useState<string>("");
    const [partnerDocuments, setPartnerDocuments] = React.useState<PartnerDocumentData[]>([]);
    const [isPartnerDocumentsDataLoaded, setIsPartnerDocumentsDataLoaded] = React.useState<boolean>(false);
    const [isEditOrAdd, setIsEditOrAdd] = React.useState<boolean>(false);
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [isAddEnabled, setIsAddEnabled] = React.useState<boolean>(true);

    React.useEffect(() => {

        const headers = getDefaultHeaders('GET');

        if (props.isUserLoaded && props.partnerId && props.partnerId !== Guid.EMPTY) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partnerdocuments/getbypartner/' + props.partnerId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<PartnerDocumentData[]>)
                    .then(data => {
                        setPartnerDocuments(data);
                        setIsPartnerDocumentsDataLoaded(true);
                    });
            });
        }
    }, [props.partnerId, props.isUserLoaded])

    function addDocument() {
        setPartnerDocumentId(Guid.EMPTY);
        setDocumentName("");
        setDocumentUrl("");
        setCreatedByUserId(props.currentUser.id);
        setCreatedDate(new Date());
        setLastUpdatedDate(new Date());
        setIsEditOrAdd(true);
        setIsAddEnabled(false);
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();

        setPartnerDocumentId(Guid.EMPTY);
        setDocumentName("");
        setDocumentUrl("");
        setCreatedByUserId(props.currentUser.id);
        setCreatedDate(new Date());
        setLastUpdatedDate(new Date());
        setIsEditOrAdd(false);
        setIsAddEnabled(true);
    }
    function editDocument(partnerDocumentId: string) {
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerdocuments/' + partnerDocumentId, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<PartnerDocumentData>)
                .then(data => {
                    setPartnerDocumentId(data.id);
                    setDocumentName(data.name);
                    setDocumentUrl(data.url);
                    setCreatedByUserId(data.createdByUserId);
                    setCreatedDate(new Date(data.createdDate));
                    setLastUpdatedDate(new Date(data.lastUpdatedDate));
                    setIsEditOrAdd(true);
                    setIsAddEnabled(false);
                });
        });
    }

    function removeDocument(documentId: string, documentName: string) {
        if (!window.confirm("Please confirm that you want to remove document with name: '" + documentName + "' as a document from this Partner?"))
            return;
        else {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('DELETE');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partnerdocuments/' + documentId, {
                    method: 'DELETE',
                    headers: headers,
                })
                    .then(() => {
                        setIsPartnerDocumentsDataLoaded(false);

                        fetch('/api/partnerdocuments/getbypartner/' + props.partnerId, {
                            method: 'GET',
                            headers: headers,
                        })
                            .then(response => response.json() as Promise<PartnerDocumentData[]>)
                            .then(data => {
                                setPartnerDocuments(data);
                                setIsPartnerDocumentsDataLoaded(true);
                            });
                    })
            });
        }
    }

    function handleSave(event: any) {

        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        var method = "PUT";

        if (partnerDocumentId === Guid.EMPTY) {
            method = "POST";
        }

        var documentData = new PartnerDocumentData();
        documentData.id = partnerDocumentId;
        documentData.partnerId = props.partnerId;
        documentData.name = documentName;
        documentData.url = documentUrl ?? 0;
        documentData.createdDate = createdDate;
        documentData.createdByUserId = createdByUserId;

        var data = JSON.stringify(documentData);

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerdocuments', {
                method: method,
                headers: headers,
                body: data,
            })
                .then(() => {
                    setIsEditOrAdd(false);
                    setIsPartnerDocumentsDataLoaded(false);
                    var getHeaders = getDefaultHeaders("GET");
                    getHeaders.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                    fetch('/api/partnerdocuments/getbypartner/' + props.partnerId, {
                        method: 'GET',
                        headers: getHeaders,
                    })
                        .then(response => response.json() as Promise<PartnerDocumentData[]>)
                        .then(data => {
                            setPartnerDocuments(data);
                            setIsPartnerDocumentsDataLoaded(true);
                            setIsEditOrAdd(false);
                            setIsAddEnabled(true);
                        });
                });
        });
    }

    function validateForm() {
        if (documentName === "" ||
            documentNameErrors !== "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }


    function handleDocumentNameChanged(val: string) {
        if (val === "") {
            setDocumentNameErrors("Name cannot be blank.");
        }
        else {
            setDocumentNameErrors("");
            setDocumentName(val);
        }

        validateForm();
    }

    function handleDocumentUrlChanged(val: string) {
        setDocumentUrl(val);
        validateForm();
    }


    function renderDocumentNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerDocumentName}</Tooltip>
    }

    function renderDocumentUrlToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerDocumentUrl}</Tooltip>
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerDocumentCreatedDate}</Tooltip>
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerDocumentLastUpdatedDate}</Tooltip>
    }

    const documentActionDropdownList = (documentId: string, documentName: string) => {
        return (
            <>
                <Dropdown.Item onClick={() => editDocument(documentId)}><Pencil />Edit Document</Dropdown.Item>
                <Dropdown.Item onClick={() => removeDocument(documentId, documentName)}><XSquare />Remove Document</Dropdown.Item>
            </>
        )
    }

    function renderPartnerDocumentsTable(documents: PartnerDocumentData[]) {
        return (
            <div>
                <h2 className="color-primary mt-4 mb-5">Partner Documents</h2>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Url</th>
                        </tr>
                    </thead>
                    <tbody>
                        {documents.map(document =>
                            <tr key={document.id.toString()}>
                                <td>{document.name}</td>
                                <td>{document.url}</td>
                                <td className="btn py-0">
                                    <Dropdown role="menuitem">
                                        <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                        <Dropdown.Menu id="share-menu">
                                            {documentActionDropdownList(document.id, document.name)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button disabled={!isAddEnabled} className="action" onClick={() => addDocument()}>Add Document</Button>
            </div>
        );
    }

    function renderAddDocument() {
        return (
            <div>
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderDocumentNameToolTip}>
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="DocumentName">Document Name</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="DocumentName" defaultValue={documentName} onChange={val => handleDocumentNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderDocumentUrlToolTip}>
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="DocumentUrl">DocumentUrl</Form.Label>
                                </OverlayTrigger>
                                <div>
                                    <Form.Control type="text" name="DocumentUrl" defaultValue={documentUrl} onChange={val => handleDocumentUrlChanged(val.target.value)} maxLength={parseInt('64')} required />
                                </div>
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Group className="form-group">
                        <Button disabled={!isSaveEnabled} type="submit" className="action btn-default">Save</Button>
                        <Button className="action" onClick={(e: any) => handleCancel(e)}>Cancel</Button>
                    </Form.Group >
                    <Form.Group className="form-group">
                        <Col>
                            <OverlayTrigger placement="top" overlay={renderCreatedDateToolTip}>
                                <Form.Label className="control-label font-weight-bold h5">Created Date</Form.Label>
                            </OverlayTrigger>
                            <Form.Group>
                                <Form.Control type="text" disabled defaultValue={createdDate ? createdDate.toLocaleString() : ""} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <OverlayTrigger placement="top" overlay={renderLastUpdatedDateToolTip}>
                                <Form.Label className="control-label font-weight-bold h5">Last Updated Date</Form.Label>
                            </OverlayTrigger>
                            <Form.Group>
                                <Form.Control type="text" disabled defaultValue={lastUpdatedDate ? lastUpdatedDate.toLocaleString() : ""} />
                            </Form.Group>
                        </Col>
                    </Form.Group >
                </Form>
            </div>
        );
    }

    return (
        <Container>
            <Row className="gx-2 py-5" lg={2}>
                <Col lg={4} className="d-flex">
                    <div className="bg-white py-2 px-5 shadow-sm rounded">
                        <h2 className="color-primary mt-4 mb-5">Edit Partner Documents</h2>
                        <p>
                            This page allows you and the TrashMob administrators to track documents relevant to the partnership. i.e. Volunteer Organizational Agreements or special waivers if needed.
                            Note that this page will have more functionality added in the future to allow uploading filled out documents or to allow usage of docusign.
                        </p>
                    </div>
                </Col>
                <Col lg={8}>
                    <div className="bg-white p-5 shadow-sm rounded">
                        {props.partnerId === Guid.EMPTY && <p> <em>Partner must be created first.</em></p>}
                        {!isPartnerDocumentsDataLoaded && props.partnerId !== Guid.EMPTY && <p><em>Loading...</em></p>}
                        {isPartnerDocumentsDataLoaded && renderPartnerDocumentsTable(partnerDocuments)}
                        {isEditOrAdd && renderAddDocument()}
                    </div>
                </Col>
            </Row>
        </Container >
    );
}
