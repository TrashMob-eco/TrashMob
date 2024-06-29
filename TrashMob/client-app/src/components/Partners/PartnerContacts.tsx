import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Container, Dropdown, Form, OverlayTrigger, Row, Tooltip } from 'react-bootstrap';
import * as ToolTips from "../../store/ToolTips";
import { Guid } from 'guid-typescript';
import PartnerContactData from '../Models/PartnerContactData';
import * as Constants from '../Models/Constants';
import { Pencil, XSquare } from 'react-bootstrap-icons';
import PhoneInput from 'react-phone-input-2'
import { useMutation, useQuery } from '@tanstack/react-query';
import { Services } from '../../config/services.config';
import { CreatePartnerContact, DeletePartnerContactByContactId, GetPartnerContactsByContactId, GetPartnerContactsByPartnerId, UpdatePartnerContact } from '../../services/contact';

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
    const [isAddEnabled, setIsAddEnabled] = React.useState<boolean>(true);

    const getPartnerContactsByPartnerId = useQuery({
        queryKey: GetPartnerContactsByPartnerId({ partnerId: props.partnerId }).key,
        queryFn: GetPartnerContactsByPartnerId({ partnerId: props.partnerId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false
    });

    const getPartnerContactsByContactId = useMutation({
        mutationKey: GetPartnerContactsByContactId().key,
        mutationFn: GetPartnerContactsByContactId().service
    });

    const createPartnerContact = useMutation({
        mutationKey: CreatePartnerContact().key,
        mutationFn: CreatePartnerContact().service
    });

    const updatePartnerContact = useMutation({
        mutationKey: UpdatePartnerContact().key,
        mutationFn: UpdatePartnerContact().service
    });

    const deletePartnerContactByContactId = useMutation({
        mutationKey: DeletePartnerContactByContactId().key,
        mutationFn: DeletePartnerContactByContactId().service
    });

    React.useEffect(() => {
        if (props.isUserLoaded && props.partnerId && props.partnerId !== Guid.EMPTY) {
            getPartnerContactsByPartnerId.refetch().then(res => {
                setPartnerContacts(res.data?.data || []);
                setIsPartnerContactsDataLoaded(true);
            });
        }
    }, [props.partnerId, props.isUserLoaded])

    function addContact() {
        setPartnerContactId(Guid.EMPTY);
        setName("");
        setPhone("");
        setEmail("");
        setNotes("");
        setCreatedByUserId(props.currentUser.id);
        setCreatedDate(new Date());
        setLastUpdatedDate(new Date());
        setIsEditOrAdd(true);
        setIsAddEnabled(false);
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        setPartnerContactId(Guid.EMPTY);
        setName("");
        setPhone("");
        setEmail("");
        setNotes("");
        setCreatedByUserId(props.currentUser.id);
        setCreatedDate(new Date());
        setLastUpdatedDate(new Date());
        setIsEditOrAdd(false);
        setIsAddEnabled(true);
    }

    function editContact(partnerContactId: string) {
        getPartnerContactsByContactId.mutateAsync({ contactId: partnerContactId }).then((res) => {
            setPartnerContactId(res.data.id);
            setName(res.data.name);
            setPhone(res.data.phone);
            setEmail(res.data.email);
            setNotes(res.data.notes);
            setCreatedByUserId(res.data.createdByUserId);
            setCreatedDate(new Date(res.data.createdDate));
            setLastUpdatedDate(new Date(res.data.lastUpdatedDate));
            setIsEditOrAdd(true);
            setIsAddEnabled(false);
        });
    }

    function removeContact(partnerContactId: string, name: string,) {
        if (!window.confirm("Please confirm that you want to remove contact: '" + name + "' from this Partner ?")) return;
        else {
            deletePartnerContactByContactId.mutateAsync({ contactId: partnerContactId }).then(() => {
                setIsPartnerContactsDataLoaded(false);
                getPartnerContactsByPartnerId.refetch().then(res => {
                    setPartnerContacts(res.data?.data || []);
                    setIsPartnerContactsDataLoaded(true);
                })
            });
        }
    }

    async function handleSave(event: any) {
        event.preventDefault();

        if (!isSaveEnabled) return;
        setIsSaveEnabled(false);

        const body = new PartnerContactData();
        body.id = partnerContactId;
        body.partnerId = props.partnerId;
        body.name = name;
        body.phone = phone;
        body.email = email;
        body.notes = notes;
        body.createdByUserId = createdByUserId;

        if (partnerContactId === Guid.EMPTY) await createPartnerContact.mutateAsync(body);
        else await updatePartnerContact.mutateAsync(body);

        setIsEditOrAdd(false);
        setIsPartnerContactsDataLoaded(false);

        getPartnerContactsByPartnerId.refetch().then(res => {
            setPartnerContacts(res.data?.data || []);
            setIsPartnerContactsDataLoaded(true);
            setIsEditOrAdd(false);
            setIsAddEnabled(true);
        });
    }

    React.useEffect(() => {
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
    }, [name, nameErrors, notes, notesErrors, email, emailErrors, phoneErrors]);

    function handleNameChanged(val: string) {
        if (val === "") {
            setNameErrors("Name cannot be blank.");
        }
        else {
            setNameErrors("");
            setName(val);
        }
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

    const contactActionDropdownList = (contactId: string, contactName: string) => {
        return (
            <>
                <Dropdown.Item onClick={() => editContact(contactId)}><Pencil />Edit Contact</Dropdown.Item>
                <Dropdown.Item onClick={() => removeContact(contactId, contactName)}><XSquare />Remove Contact</Dropdown.Item>
            </>
        )
    }

    function renderPartnerContactsTable(contacts: PartnerContactData[]) {
        return (
            <div>
                <h2 className="color-primary mt-4 mb-5">Partner Contacts</h2>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Phone</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {contacts.map(contact =>
                            <tr key={contact.id}>
                                <td>{contact.name}</td>
                                <td>{contact.email}</td>
                                <td><PhoneInput
                                    value={contact.phone}
                                    disabled
                                /></td>
                                <td className="btn py-0">
                                    <Dropdown role="menuitem">
                                        <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                        <Dropdown.Menu id="share-menu">
                                            {contactActionDropdownList(contact.id, contact.name)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button disabled={!isAddEnabled} className="action" onClick={() => addContact()}>Add Contact</Button>
            </div >
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
                                    <Form.Label className="control-label font-weight-bold h5">Contact Name</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={name} maxLength={parseInt('64')} onChange={(val) => handleNameChanged(val.target.value)} required />
                                <span style={{ color: "red" }}>{nameErrors}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderEmailToolTip}>
                                    <Form.Label className="control-label font-weight-bold h5">Email</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={email} maxLength={parseInt('64')} onChange={(val) => handleEmailChanged(val.target.value)} required />
                                <span style={{ color: "red" }}>{emailErrors}</span>
                            </Form.Group >
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderPhoneToolTip}>
                                    <Form.Label className="control-label font-weight-bold h5">Phone</Form.Label>
                                </OverlayTrigger>
                                <PhoneInput
                                    country={'us'}
                                    value={phone}
                                    onChange={(val) => handlePhoneChanged(val)}
                                />
                                <span style={{ color: "red" }}>{phoneErrors}</span>
                            </Form.Group >
                        </Col>
                    </Form.Row>
                    <Form.Group className="required">
                        <OverlayTrigger placement="top" overlay={renderNotesToolTip}>
                            <Form.Label className="control-label font-weight-bold h5">Notes</Form.Label>
                        </OverlayTrigger>
                        <Form.Control as="textarea" defaultValue={notes} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handleNotesChanged(val.target.value)} required />
                        <span style={{ color: "red" }}>{notesErrors}</span>
                    </Form.Group >
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
                        <h2 className="color-primary mt-4 mb-5">Edit Partner Contacts</h2>
                        <p>
                            This page allows you to add more contacts to this partner so you can share the load of responding to questions for this partner. This information may be displayed in
                            the partnerships page on TrashMob.eco, but is also used by the TrashMob site administrators to contact your organization during setup and during times where issues have arisen.
                        </p>
                    </div>
                </Col>
                <Col lg={8}>
                    <div className="bg-white p-5 shadow-sm rounded">
                        {props.partnerId === Guid.EMPTY && <p> <em>Partner must be created first.</em></p>}
                        {!isPartnerContactsDataLoaded && props.partnerId !== Guid.EMPTY && <p><em>Loading...</em></p>}
                        {isPartnerContactsDataLoaded && renderPartnerContactsTable(partnerContacts)}
                        {isEditOrAdd && renderAddPartnerContact()}
                    </div>
                </Col>
            </Row>
        </Container>
    );
}
