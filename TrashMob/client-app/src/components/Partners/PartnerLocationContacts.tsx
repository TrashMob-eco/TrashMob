import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import { Guid } from 'guid-typescript';
import PartnerLocationContactData from '../Models/PartnerLocationContactData';

export interface PartnerLocationContactsDataProps {
    partnerLocationId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerLocationContacts: React.FC<PartnerLocationContactsDataProps> = (props) => {

    const [notes, setNotes] = React.useState<string>("");
    const [partnerLocationContacts, setPartnerLocationContacts] = React.useState<PartnerLocationContactData[]>([]);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>(Guid.EMPTY);
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [isPartnerLocationContactsDataLoaded, setIsPartnerLocationContactsDataLoaded] = React.useState<boolean>(false);
    const [isEditOrAdd, setIsEditOrAdd] = React.useState<boolean>(false);

    React.useEffect(() => {

        const headers = getDefaultHeaders('GET');

        if (props.isUserLoaded && props.partnerLocationId && props.partnerLocationId !== Guid.EMPTY) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partnerlocationcontacts/getbypartnerlocation/' + props.partnerLocationId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<PartnerLocationContactData[]>)
                    .then(data => {
                        setPartnerLocationContacts(data);
                        setIsPartnerLocationContactsDataLoaded(true);
                    });
            });
        }
    }, [props.partnerLocationId, props.isUserLoaded])

    function addContact() {
        setIsEditOrAdd(true);
    }

    function editContact(partnerLocationContactId: string) {
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerlocationcontacts/' + props.partnerLocationId + '/' + partnerLocationContactId, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<PartnerLocationContactData>)
                .then(data => {
                    setNotes(data.notes);
                    setCreatedByUserId(data.createdByUserId);
                    setCreatedDate(data.createdDate);
                    setLastUpdatedDate(data.lastUpdatedDate);
                    setIsEditOrAdd(true);
                });
        });
    }

    function removeContact(id: string, name: string, ) {
        if (!window.confirm("Please confirm that you want to remove contact: '" + name + "' from this Partner Location?"))
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

                fetch('/api/partnerlocationcontacts/' + props.partnerLocationId + '/' + id, {
                    method: 'DELETE',
                    headers: headers,
                })
                    .then(() => {
                        setIsPartnerLocationContactsDataLoaded(false);

                        fetch('/api/partnerlocationcontacts/' + props.partnerLocationId, {
                            method: 'GET',
                            headers: headers,
                        })
                            .then(response => response.json() as Promise<PartnerLocationContactData[]>)
                            .then(data => {
                                setPartnerLocationContacts(data);
                                setIsPartnerLocationContactsDataLoaded(true);
                            });
                    })
            });
        }
    }

    function handleSave() {

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        var method = "PUT";

        if (createdByUserId === Guid.EMPTY) {
            method = "POST";
        }

        var partnerLocationContact = new PartnerLocationContactData();
        partnerLocationContact.partnerLocationId = props.partnerLocationId;
        partnerLocationContact.notes = notes;
        partnerLocationContact.createdByUserId = createdByUserId;
        partnerLocationContact.lastUpdatedByUserId = props.currentUser.id

        var data = JSON.stringify(partnerLocationContact);

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerlocationcontacts', {
                method: method,
                headers: headers,
                body: data,
            })
                .then(() => {
                    setIsEditOrAdd(false);
                    setIsPartnerLocationContactsDataLoaded(false);

                    fetch('/api/partnerlocationcontacts/' + props.partnerLocationId, {
                        method: 'GET',
                        headers: headers,
                    })
                        .then(response => response.json() as Promise<PartnerLocationContactData[]>)
                        .then(data => {
                            setPartnerLocationContacts(data);
                            setIsPartnerLocationContactsDataLoaded(true);
                        });
                });
        });
    }

    function handleNotesChanged(notes: string) {
        setNotes(notes);
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerServiceNotes}</Tooltip>
    }

    function renderPartnerLocationServicesTable(contacts: PartnerLocationContactData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Notes</th>
                            <th>Created Date</th>
                            <th>Last Updated Date</th>
                        </tr>
                    </thead>
                    <tbody>
                        {contacts.map(contact =>
                            <tr key={contact.id}>
                                <td>{contact.notes}</td>
                                <td>{createdDate ? createdDate.toLocaleString() : ""}</td>
                                <td>{lastUpdatedDate ? lastUpdatedDate.toLocaleString() : ""}</td>
                                <td>
                                    <Button className="action" onClick={() => editContact(contact.id)}>Edit Contact</Button>
                                    <Button className="action" onClick={() => removeContact(contact.name, contact.id)}>Remove Contact</Button>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button className="action" onClick={() => addContact()}>Add Contact</Button>
            </div>
        );
    }

    function renderAddPartnerLocationContact() {
        return (
            <div>
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderNotesToolTip}>
                                    <Form.Label className="control-label" htmlFor="serviceType">Notes</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="notes" defaultValue={notes} onChange={val => handleNotesChanged(val.target.value)} maxLength={parseInt('64')} required />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label className="control-label" htmlFor="createdDate">Created Date</Form.Label>
                                <Form.Label defaultValue={createdDate ? createdDate.toLocaleString() : ""} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label className="control-label" htmlFor="lastUpdatedDate">Last Updated Date</Form.Label>
                                <Form.Label defaultValue={lastUpdatedDate ? lastUpdatedDate.toLocaleString() : ""} />
                            </Form.Group>
                        </Col>
                        <Button className="action" onClick={() => handleSave()}>Save</Button>
                    </Form.Row>
                </Form>
            </div>
        );
    }

    return (
        <>
            <div>
                {props.partnerLocationId === Guid.EMPTY && <p> <em>Partner Location must be created first.</em></p>}
                {!isPartnerLocationContactsDataLoaded && props.partnerLocationId !== Guid.EMPTY && <p><em>Loading...</em></p>}
                {isPartnerLocationContactsDataLoaded && renderPartnerLocationServicesTable(partnerLocationContacts)}
                {isEditOrAdd && renderAddPartnerLocationContact()}
            </div>
        </>
    );
}
