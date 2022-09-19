import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import { Guid } from 'guid-typescript';
import CommunityDocumentData from '../Models/CommunityDocumentData';

export interface CommunityDocumentsDataProps {
    communityId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const CommunityDocuments: React.FC<CommunityDocumentsDataProps> = (props) => {

    const [documentName, setDocumentName] = React.useState<string>("");
    const [documentUrl, setDocumentUrl] = React.useState<string>("");
    const [documents, setDocuments] = React.useState<CommunityDocumentData[]>([]);
    const [isDocumentsDataLoaded, setIsDocumentsDataLoaded] = React.useState<boolean>(false);

    React.useEffect(() => {

        const headers = getDefaultHeaders('GET');

        if (props.isUserLoaded && props.communityId && props.communityId !== Guid.EMPTY) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/communitydocuments/' + props.communityId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<CommunityDocumentData[]>)
                    .then(data => {
                        setDocuments(data);
                        setIsDocumentsDataLoaded(true);
                    });
            });
        }
    }, [props.communityId, props.isUserLoaded])

    function removeDocument(documentId: string, documentName: string) {
        if (!window.confirm("Please confirm that you want to remove document with name: '" + documentName + "' as a document from this Community?"))
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

                fetch('/api/communitydocuments/' + props.communityId + '/' + documentId, {
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

        var documentData = new CommunityDocumentData();
        documentData.name = documentName;
        documentData.url = documentUrl ?? 0;

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/communitydocuments/' + props.communityId, {
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
        return <Tooltip {...props}>{ToolTips.CommunityDocumentName}</Tooltip>
    }

    function renderDocumentUrlToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityDocumentUrl}</Tooltip>
    }

    function renderCommunityDocumentsTable(documents: CommunityDocumentData[]) {
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

    function renderAddCommunitySocialMediaAccount() {
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
                {props.communityId === Guid.EMPTY && <p> <em>Community must be created first.</em></p>}
                {!isDocumentsDataLoaded && props.communityId !== Guid.EMPTY && <p><em>Loading...</em></p>}
                {isDocumentsDataLoaded && renderCommunityDocumentsTable(documents)}
                {renderAddCommunitySocialMediaAccount()}
            </div>
        </>
    );
}
