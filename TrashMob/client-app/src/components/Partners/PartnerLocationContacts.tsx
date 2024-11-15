import * as React from 'react';
import { Button, Col, Dropdown, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { Guid } from 'guid-typescript';
import { Pencil, XSquare } from 'react-bootstrap-icons';
import PhoneInput from 'react-phone-input-2';
import { useMutation, useQuery } from '@tanstack/react-query';
import UserData from '../Models/UserData';
import * as ToolTips from '../../store/ToolTips';
import PartnerLocationContactData from '../Models/PartnerLocationContactData';
import * as Constants from '../Models/Constants';
import { Services } from '../../config/services.config';
import {
    CreatePartnerLocationContact,
    DeletePartnerLocationContactByContactId,
    GetPartnerLocationContactByContactId,
    GetPartnerLocationContactsByLocationId,
    UpdatePartnerLocationContact,
} from '../../services/contact';

export interface PartnerLocationContactsDataProps {
    partnerLocationId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
    onSave: any;
}

export const PartnerLocationContacts: React.FC<PartnerLocationContactsDataProps> = (props) => {
    const [partnerLocationContactId, setPartnerLocationContactId] = React.useState<string>(Guid.EMPTY);
    const [name, setName] = React.useState<string>('');
    const [email, setEmail] = React.useState<string>('');
    const [phone, setPhone] = React.useState<string>('');
    const [notes, setNotes] = React.useState<string>('');
    const [nameErrors, setNameErrors] = React.useState<string>('');
    const [emailErrors, setEmailErrors] = React.useState<string>('');
    const [phoneErrors, setPhoneErrors] = React.useState<string>('');
    const [notesErrors, setNotesErrors] = React.useState<string>('');
    const [partnerLocationContacts, setPartnerLocationContacts] = React.useState<PartnerLocationContactData[]>([]);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>(Guid.EMPTY);
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [isPartnerLocationContactsDataLoaded, setIsPartnerLocationContactsDataLoaded] =
        React.useState<boolean>(false);
    const [isEditOrAdd, setIsEditOrAdd] = React.useState<boolean>(false);
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [isAddEnabled, setIsAddEnabled] = React.useState<boolean>(true);

    const getPartnerLocationContactsByLocationId = useQuery({
        queryKey: GetPartnerLocationContactsByLocationId({
            locationId: props.partnerLocationId,
        }).key,
        queryFn: GetPartnerLocationContactsByLocationId({
            locationId: props.partnerLocationId,
        }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getPartnerLocationContactByContactId = useMutation({
        mutationKey: GetPartnerLocationContactByContactId().key,
        mutationFn: GetPartnerLocationContactByContactId().service,
    });

    const createPartnerLocationContact = useMutation({
        mutationKey: CreatePartnerLocationContact().key,
        mutationFn: CreatePartnerLocationContact().service,
    });

    const updatePartnerLocationContact = useMutation({
        mutationKey: UpdatePartnerLocationContact().key,
        mutationFn: UpdatePartnerLocationContact().service,
    });

    const deletePartnerLocationContactByContactId = useMutation({
        mutationKey: DeletePartnerLocationContactByContactId().key,
        mutationFn: DeletePartnerLocationContactByContactId().service,
    });

    React.useEffect(() => {
        if (props.isUserLoaded && props.partnerLocationId && props.partnerLocationId !== Guid.EMPTY) {
            getPartnerLocationContactsByLocationId.refetch().then((res) => {
                setPartnerLocationContacts(res.data?.data || []);
                setIsPartnerLocationContactsDataLoaded(true);
            });
        }
    }, [props.partnerLocationId, props.isUserLoaded]);

    function addContact() {
        setPartnerLocationContactId(Guid.EMPTY);
        setName('');
        setPhone('');
        setEmail('');
        setNotes('');
        setCreatedByUserId(props.currentUser.id);
        setCreatedDate(new Date());
        setLastUpdatedDate(new Date());
        setIsEditOrAdd(true);
        setIsAddEnabled(false);
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        setPartnerLocationContactId(Guid.EMPTY);
        setName('');
        setPhone('');
        setEmail('');
        setNotes('');
        setCreatedByUserId(props.currentUser.id);
        setCreatedDate(new Date());
        setLastUpdatedDate(new Date());
        setIsEditOrAdd(false);
        setIsAddEnabled(true);
    }

    function editContact(partnerLocationContactId: string) {
        getPartnerLocationContactByContactId.mutateAsync({ contactId: partnerLocationContactId }).then((res) => {
            setPartnerLocationContactId(res.data.id);
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

    function removeContact(id: string, name: string) {
        if (!window.confirm(`Please confirm that you want to remove contact: '${name}' from this Partner Location?`))
            return;

        deletePartnerLocationContactByContactId.mutateAsync({ contactId: id }).then(() => {
            setIsPartnerLocationContactsDataLoaded(false);
            getPartnerLocationContactsByLocationId.refetch().then((res) => {
                setPartnerLocationContacts(res.data?.data || []);
                setIsPartnerLocationContactsDataLoaded(true);
            });
        });
    }

    async function handleSave(event: any) {
        event.preventDefault();

        if (!isSaveEnabled) return;
        setIsSaveEnabled(false);

        const body = new PartnerLocationContactData();
        body.id = partnerLocationContactId;
        body.partnerLocationId = props.partnerLocationId;
        body.name = name;
        body.phone = phone;
        body.email = email;
        body.notes = notes;
        body.createdByUserId = createdByUserId;

        if (partnerLocationContactId === Guid.EMPTY) await createPartnerLocationContact.mutateAsync(body);
        else await updatePartnerLocationContact.mutateAsync(body);

        setIsEditOrAdd(false);
        setIsPartnerLocationContactsDataLoaded(false);

        getPartnerLocationContactsByLocationId.refetch().then((res) => {
            setPartnerLocationContacts(res.data?.data || []);
            setIsPartnerLocationContactsDataLoaded(true);
            setIsEditOrAdd(false);
            setIsAddEnabled(true);
            props.onSave();
        });
    }

    React.useEffect(() => {
        if (
            name === '' ||
            nameErrors !== '' ||
            notes === '' ||
            notesErrors !== '' ||
            email === '' ||
            emailErrors !== '' ||
            phoneErrors !== ''
        ) {
            setIsSaveEnabled(false);
        } else {
            setIsSaveEnabled(true);
        }
    }, [name, nameErrors, notes, notesErrors, email, emailErrors, phoneErrors]);

    function handleNameChanged(val: string) {
        if (val === '') {
            setNameErrors('Name cannot be blank.');
        } else {
            setNameErrors('');
            setName(val);
        }
    }

    function handleEmailChanged(val: string) {
        const pattern = new RegExp(Constants.RegexEmail);

        if (!pattern.test(val)) {
            setEmailErrors('Please enter valid email address.');
        } else {
            setEmailErrors('');
            setEmail(val);
        }
    }

    function handlePhoneChanged(val: string) {
        if (val && val !== '') {
            const pattern = new RegExp(Constants.RegexPhoneNumber);

            if (!pattern.test(val)) {
                setPhoneErrors('Please enter a valid phone number.');
            } else {
                setPhoneErrors('');
                setPhone(val);
            }
        } else {
            setPhoneErrors('');
            setPhone(val);
        }
    }

    function handleNotesChanged(val: string) {
        if (val.length < 0 || val.length > 1000) {
            setNotesErrors('Notes cannot be empty and cannot be more than 1000 characters long.');
        } else {
            setNotesErrors('');
            setNotes(val);
        }
    }

    function renderNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationContactName}</Tooltip>;
    }

    function renderEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationContactEmail}</Tooltip>;
    }

    function renderPhoneToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationContactPhone}</Tooltip>;
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationContactNotes}</Tooltip>;
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationContactCreatedDate}</Tooltip>;
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationContactLastUpdatedDate}</Tooltip>;
    }

    const locationContactActionDropdownList = (contactId: string, contactName: string) => (
        <>
            <Dropdown.Item onClick={() => editContact(contactId)}>
                <Pencil />
                Edit Service
            </Dropdown.Item>
            <Dropdown.Item onClick={() => removeContact(contactId, contactName)}>
                <XSquare />
                Remove Service
            </Dropdown.Item>
        </>
    );

    function renderPartnerLocationServicesTable(contacts: PartnerLocationContactData[]) {
        return (
            <div>
                <h2 className='color-primary mt-4 mb-5'>Partner Location Contacts</h2>
                <table className='table table-striped' aria-labelledby='tableLabel' width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Phone</th>
                            <th>Notes</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {contacts.map((contact) => (
                            <tr key={contact.id}>
                                <td>{contact.name}</td>
                                <td>{contact.email}</td>
                                <td>
                                    <PhoneInput value={contact.phone} disabled />
                                </td>
                                <td>{contact.phone}</td>
                                <td>{contact.notes}</td>
                                <td className='btn py-0'>
                                    <Dropdown role='menuitem'>
                                        <Dropdown.Toggle id='share-toggle' variant='outline' className='h-100 border-0'>
                                            ...
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu id='share-menu'>
                                            {locationContactActionDropdownList(contact.id, contact.name)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>{' '}
                            </tr>
                        ))}
                    </tbody>
                </table>
                <Button disabled={!isAddEnabled} className='action' onClick={() => addContact()}>
                    Add Contact
                </Button>
            </div>
        );
    }

    function renderAddPartnerLocationContact() {
        return (
            <div>
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <Col>
                            <Form.Group className='required'>
                                <OverlayTrigger placement='top' overlay={renderNameToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5'>Contact Name</Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    defaultValue={name}
                                    maxLength={parseInt('64')}
                                    onChange={(val) => handleNameChanged(val.target.value)}
                                    required
                                />
                                <span style={{ color: 'red' }}>{nameErrors}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group className='required'>
                                <OverlayTrigger placement='top' overlay={renderEmailToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5'>Email</Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    defaultValue={email}
                                    maxLength={parseInt('64')}
                                    onChange={(val) => handleEmailChanged(val.target.value)}
                                    required
                                />
                                <span style={{ color: 'red' }}>{emailErrors}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement='top' overlay={renderPhoneToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5'>Phone</Form.Label>
                                </OverlayTrigger>
                                <PhoneInput country='us' value={phone} onChange={(val) => handlePhoneChanged(val)} />
                                <span style={{ color: 'red' }}>{phoneErrors}</span>
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Group className='required'>
                        <OverlayTrigger placement='top' overlay={renderNotesToolTip}>
                            <Form.Label className='control-label font-weight-bold h5'>Notes</Form.Label>
                        </OverlayTrigger>
                        <Form.Control
                            as='textarea'
                            defaultValue={notes}
                            maxLength={parseInt('2048')}
                            rows={5}
                            cols={5}
                            onChange={(val) => handleNotesChanged(val.target.value)}
                            required
                        />
                        <span style={{ color: 'red' }}>{notesErrors}</span>
                    </Form.Group>
                    <Form.Group className='form-group'>
                        <Button disabled={!isSaveEnabled} type='submit' className='action btn-default'>
                            Save
                        </Button>
                        <Button className='action' onClick={(e: any) => handleCancel(e)}>
                            Cancel
                        </Button>
                    </Form.Group>
                    <Form.Group className='form-group'>
                        <Col>
                            <OverlayTrigger placement='top' overlay={renderCreatedDateToolTip}>
                                <Form.Label className='control-label font-weight-bold h5'>Created Date</Form.Label>
                            </OverlayTrigger>
                            <Form.Group>
                                <Form.Control
                                    type='text'
                                    disabled
                                    defaultValue={createdDate ? createdDate.toLocaleString() : ''}
                                />
                            </Form.Group>
                        </Col>
                        <Col>
                            <OverlayTrigger placement='top' overlay={renderLastUpdatedDateToolTip}>
                                <Form.Label className='control-label font-weight-bold h5'>Last Updated Date</Form.Label>
                            </OverlayTrigger>
                            <Form.Group>
                                <Form.Control
                                    type='text'
                                    disabled
                                    defaultValue={lastUpdatedDate ? lastUpdatedDate.toLocaleString() : ''}
                                />
                            </Form.Group>
                        </Col>
                    </Form.Group>
                </Form>
            </div>
        );
    }

    return (
        <div className='bg-white p-5 shadow-sm rounded'>
            {props.partnerLocationId === Guid.EMPTY && (
                <p>
                    {' '}
                    <em>Partner location must be created first.</em>
                </p>
            )}
            {!isPartnerLocationContactsDataLoaded && props.partnerLocationId !== Guid.EMPTY && (
                <p>
                    <em>Loading...</em>
                </p>
            )}
            {isPartnerLocationContactsDataLoaded ? renderPartnerLocationServicesTable(partnerLocationContacts) : null}
            {isEditOrAdd ? renderAddPartnerLocationContact() : null}
        </div>
    );
};
