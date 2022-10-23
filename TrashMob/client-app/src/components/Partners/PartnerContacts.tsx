import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import { Guid } from 'guid-typescript';
import PartnerContactData from '../Models/PartnerContactData';
import * as Constants from '../Models/Constants';

export interface PartnerContactsDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerContacts: React.FC<PartnerContactsDataProps> = (props) => {

    const [partnerContactId, setPartnerContactId] = React.useState<string>(Guid.EMPTY);
    const [name, setName] = React.useState<string>("");
    const [email, setEmail] = React.useState<string>("");
    const [phone, setPhone] = React.useState<string>("");
    const [notes, setNotes] = React.useState<string>("");
    const [nameErrors, setNameErrors] = React.useState<string>("");
    const [emailErrors, setEmailErrors] = React.useState<string>("");
    const [phoneErrors, setPhoneErrors] = React.useState<string>("");
    const [notesErrors, setNotesErrors] = React.useState<string>("");
    const [partnerContacts, setPartnerContacts] = React.useState<PartnerContactData[]>([]);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>(Guid.EMPTY);
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [isPartnerContactsDataLoaded, setIsPartnerContactsDataLoaded] = React.useState<boolean>(false);
    const [isEditOrAdd, setIsEditOrAdd] = React.useState<boolean>(false);
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);

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

                fetch('/api/partnercontacts/getbypartner/' + props.partnerId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<PartnerContactData[]>)
                    .then(data => {
                        setPartnerContacts(data);
                        setIsPartnerContactsDataLoaded(true);
                    });
            });
        }
    }, [props.partnerId, props.isUserLoaded])

    function addContact() {
        setIsEditOrAdd(true);
    }

    function editContact(partnerContactId: string) {
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnercontacts/' + partnerContactId, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<PartnerContactData>)
                .then(data => {
                    setPartnerContactId(data.id);
                    setName(data.name);
                    setPhone(data.phone);
                    setEmail(data.email);
                    setNotes(data.notes);
                    setCreatedByUserId(data.createdByUserId);
                    setCreatedDate(new Date(data.createdDate));
                    setLastUpdatedDate(new Date(data.lastUpdatedDate));
                    setIsEditOrAdd(true);
                });
        });
    }

    function removeContact(partnerContactId: string, name: string,) {
        if (!window.confirm("Please confirm that you want to remove contact: '" + name + "' from this Partner ?"))
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

                fetch('/api/partnercontacts/' + partnerContactId, {
                    method: 'DELETE',
                    headers: headers,
                })
                    .then(() => {
                        setIsPartnerContactsDataLoaded(false);

                        fetch('/api/partnercontacts/' + props.partnerId, {
                            method: 'GET',
                            headers: headers,
                        })
                            .then(response => response.json() as Promise<PartnerContactData[]>)
                            .then(data => {
                                setPartnerContacts(data);
                                setIsPartnerContactsDataLoaded(true);
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

        var partnerContact = new PartnerContactData();
        partnerContact.id = partnerContactId;
        partnerContact.partnerId = props.partnerId;
        partnerContact.name = name;
        partnerContact.phone = phone;
        partnerContact.email = email;
        partnerContact.notes = notes;
        partnerContact.createdByUserId = createdByUserId;
        partnerContact.lastUpdatedByUserId = props.currentUser.id

        var data = JSON.stringify(partnerContact);

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnercontacts', {
                method: method,
                headers: headers,
                body: data,
            })
                .then(() => {
                    setIsEditOrAdd(false);
                    setIsPartnerContactsDataLoaded(false);
                    var getHeaders = getDefaultHeaders("GET");
                    getHeaders.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                    fetch('/api/partnercontacts/' + props.partnerId, {
                        method: 'GET',
                        headers: getHeaders,
                    })
                        .then(response => response.json() as Promise<PartnerContactData[]>)
                        .then(data => {
                            setPartnerContacts(data);
                            setIsPartnerContactsDataLoaded(true);
                        });
                });
        });
    }

    function validateForm() {
        if (name === "" ||
            nameErrors !== "" ||
            notes === "" ||
            notesErrors !== "" ||
            email === "" ||
            emailErrors !== "" ||
            phoneErrors !== "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }

    function handleNameChanged(val: string) {
        if (val === "") {
            setNameErrors("Name cannot be blank.");
        }
        else {
            setNameErrors("");
            setName(val);
        }

        validateForm();
    }

    function handleEmailChanged(val: string) {
        var pattern = new RegExp(Constants.RegexEmail);

        if (!pattern.test(val)) {
            setEmailErrors("Please enter valid email address.");
        }
        else {
            setEmailErrors("");
            setEmail(val);
        }

        validateForm();
    }

    function handlePhoneChanged(val: string) {

        if (val && val !== "") {
            var pattern = new RegExp(Constants.RegexPhoneNumber);

            if (!pattern.test(val)) {
                setPhoneErrors("Please enter a valid phone number.");
            }
            else {
                setPhoneErrors("");
                setPhone(val);
            }
        }
        else {
            setPhoneErrors("");
            setPhone(val);
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
        return <Tooltip {...props}>{ToolTips.PartnerContactName}</Tooltip>
    }

    function renderEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerContactEmail}</Tooltip>
    }

    function renderPhoneToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerContactPhone}</Tooltip>
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerContactNotes}</Tooltip>
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerContactCreatedDate}</Tooltip>
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerContactLastUpdatedDate}</Tooltip>
    }

    function renderPartnerServicesTable(contacts: PartnerContactData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Phone</th>
                            <th>Notes</th>
                            <th>Created Date</th>
                            <th>Last Updated Date</th>
                        </tr>
                    </thead>
                    <tbody>
                        {contacts.map(contact =>
                            <tr key={contact.id}>
                                <td>{contact.name}</td>
                                <td>{contact.email}</td>
                                <td>{contact.phone}</td>
                                <td>{contact.notes}</td>
                                <td>{contact.createdDate ? contact.createdDate.toLocaleString() : ""}</td>
                                <td>{contact.lastUpdatedDate ? contact.lastUpdatedDate.toLocaleString() : ""}</td>
                                <td>
                                    <Button className="action" onClick={() => editContact(contact.id)}>Edit Contact</Button>
                                    <Button className="action" onClick={() => removeContact(contact.id, contact.name)}>Remove Contact</Button>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button className="action" onClick={() => addContact()}>Add Contact</Button>
            </div>
        );
    }

    function renderAddPartnerContact() {
        return (
            <div>
                <Form onSubmit={handleSave} >
                    <Form.Row>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderNameToolTip}>
                                    <Form.Label className="control-label">Contact Name:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={name} maxLength={parseInt('64')} onChange={(val) => handleNameChanged(val.target.value)} required />
                                <span style={{ color: "red" }}>{nameErrors}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderEmailToolTip}>
                                    <Form.Label className="control-label">Email:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={email} maxLength={parseInt('64')} onChange={(val) => handleEmailChanged(val.target.value)} required />
                                <span style={{ color: "red" }}>{emailErrors}</span>
                            </Form.Group >
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderPhoneToolTip}>
                                    <Form.Label className="control-label">Phone:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={phone} maxLength={parseInt('64')} onChange={(val) => handlePhoneChanged(val.target.value)} />
                                <span style={{ color: "red" }}>{phoneErrors}</span>
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
                    </Form.Group >
                    <Form.Group className="form-group">
                        <Col>
                            <OverlayTrigger placement="top" overlay={renderCreatedDateToolTip}>
                                <Form.Label className="control-label">Created Date:</Form.Label>
                            </OverlayTrigger>
                           <Form.Group>
                                <Form.Control type="text" disabled defaultValue={createdDate ? createdDate.toLocaleString() : ""} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <OverlayTrigger placement="top" overlay={renderLastUpdatedDateToolTip}>
                                <Form.Label className="control-label">Last Updated Date:</Form.Label>
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
        <>
            <div>
                {props.partnerId === Guid.EMPTY && <p> <em>Partner must be created first.</em></p>}
                {!isPartnerContactsDataLoaded && props.partnerId !== Guid.EMPTY && <p><em>Loading...</em></p>}
                {isPartnerContactsDataLoaded && renderPartnerServicesTable(partnerContacts)}
                {isEditOrAdd && renderAddPartnerContact()}
            </div>
        </>
    );
}
