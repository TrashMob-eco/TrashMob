import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import { Guid } from 'guid-typescript';
import PartnerDocumentData from '../Models/PartnerDocumentData';

export interface PartnerDocumentsDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerDocuments: React.FC<PartnerDocumentsDataProps> = (props) => {

    const [documentName, setDocumentName] = React.useState<string>("");
    const [documentUrl, setDocumentUrl] = React.useState<string>("");
    const [documents, setDocuments] = React.useState<PartnerDocumentData[]>([]);
    const [isDocumentsDataLoaded, setIsDocumentsDataLoaded] = React.useState<boolean>(false);

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
                        setDocuments(data);
                        setIsDocumentsDataLoaded(true);
                    });
            });
        }
    }, [props.partnerId, props.isUserLoaded])

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

                fetch('/api/partnerdocuments/' + props.partnerId + '/' + documentId, {
                    method: 'DELETE',
                    headers: headers,
                })
            });
        }
    }

    function handleAddDocument() {
        
        if (documentName === "" || documentUrl === "")
            return;

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        var documentData = new PartnerDocumentData();
        documentData.name = documentName;
        documentData.url = documentUrl ?? 0;

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerdocuments/' + props.partnerId, {
                  method: 'POST',
                  headers: headers,
                 })
        });
    }

    function handleDocumentNameChanged(name: string) {
        setDocumentName(documentName);
    }

    function handleDocumentUrlChanged(url: string) {
        setDocumentUrl(url);
    }


    function renderDocumentNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerDocumentName}</Tooltip>
    }

    function renderDocumentUrlToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerDocumentUrl}</Tooltip>
    }

    function renderPartnerDocumentsTable(documents: PartnerDocumentData[]) {
        return (
            <div>
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
                                        <td>
                                            <Button className="action" onClick={() => removeDocument(document.id, document.url)}>Remove Document</Button>
                                        </td>
                                    </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderAddDocument() {
        return (
            <div>
                <Form onSubmit={handleAddDocument}>
                    <Form.Row>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderDocumentNameToolTip}>
                                    <Form.Label className="control-label" htmlFor="DocumentName">Document Name</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="DocumentName" defaultValue={documentName} onChange={val => handleDocumentNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderDocumentUrlToolTip}>
                                    <Form.Label className="control-label" htmlFor="DocumentUrl">DocumentUrl</Form.Label>
                                </OverlayTrigger>
                                <div>
                                    <Form.Control type="text" name="DocumentUrl" defaultValue={documentUrl} onChange={val => handleDocumentUrlChanged(val.target.value)} maxLength={parseInt('64')} required />
                                 </div>
                            </Form.Group>
                        </Col>
                        <Button className="action" onClick={() => handleAddDocument()}>Add Document</Button>
                    </Form.Row>
                </Form>
            </div>
        );
    }

    return (
        <>
            <div>
                {props.partnerId === Guid.EMPTY && <p> <em>Partner must be created first.</em></p>}
                {!isDocumentsDataLoaded && props.partnerId !== Guid.EMPTY && <p><em>Loading...</em></p>}
                {isDocumentsDataLoaded && renderPartnerDocumentsTable(documents)}
                {renderAddDocument()}
            </div>
        </>
    );
}
