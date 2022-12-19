import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Container, Dropdown, Row } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import PartnerLocationData from '../Models/PartnerLocationData';
import { PartnerLocationEdit } from './PartnerLocationEdit';
import { PartnerLocationServices } from './PartnerLocationServices';
import { PartnerLocationContacts } from './PartnerLocationContacts';
import { PartnerLocationEventRequests } from './PartnerLocationEventRequests';
import { Guid } from 'guid-typescript';
import { Pencil, XSquare } from 'react-bootstrap-icons';

export interface PartnerLocationsDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerLocations: React.FC<PartnerLocationsDataProps> = (props) => {

    const [partnerLocations, setPartnerLocations] = React.useState<PartnerLocationData[]>([]);
    const [isPartnerLocationDataLoaded, setIsPartnerLocationDataLoaded] = React.useState<boolean>(false);
    const [partnerLocationId, setPartnerLocationId] = React.useState<string>("");
    const [isEdit, setIsEdit] = React.useState<boolean>(false);
    const [isAdd, setIsAdd] = React.useState<boolean>(false);
    const [isAddEnabled, setIsAddEnabled] = React.useState<boolean>(true);

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

                fetch('/api/partnerlocations/getbypartner/' + props.partnerId, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<PartnerLocationData[]>)
                    .then(data => {
                        setPartnerLocations(data);
                        setIsPartnerLocationDataLoaded(true);
                    });
            });
        }
    }, [props.currentUser, props.isUserLoaded, props.partnerId]);

    function removeLocation(locationId: string, name: string) {
        if (!window.confirm("Please confirm that you want to remove Location with name: '" + name + "' as a location from this Partner?"))
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

                fetch('/api/partnerlocations/' + locationId, {
                    method: 'DELETE',
                    headers: headers,
                })
                    .then(() => {
                        setIsPartnerLocationDataLoaded(false);

                        fetch('/api/partnerlocations/getbypartner/' + props.partnerId, {
                            method: 'GET',
                            headers: headers,
                        })
                            .then(response => response.json() as Promise<PartnerLocationData[]>)
                            .then(data => {
                                setPartnerLocations(data);
                                setIsPartnerLocationDataLoaded(true);
                            });
                    })
            });
        }
    }

    function addLocation() {
        setPartnerLocationId(Guid.EMPTY);
        setIsAdd(true);
        setIsAddEnabled(false);
    }

    // This will handle Cancel button click event.
    function handleCancel() {
        setPartnerLocationId(Guid.EMPTY);
        setIsAdd(false);
        setIsEdit(false);
        setIsAddEnabled(true);
    }

    // This will handle Save button click event.
    function handleSave() {
        setPartnerLocationId(Guid.EMPTY);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerlocations/getbypartner/' + props.partnerId, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<PartnerLocationData[]>)
                .then(data => {
                    setPartnerLocations(data);
                    setIsPartnerLocationDataLoaded(true);
                    setIsAdd(false);
                    setIsEdit(false);
                    setIsAddEnabled(true);
                });
        });
    }

    function editLocation(partnerLocationId: string) {
        setPartnerLocationId(partnerLocationId);
        setIsEdit(true);
        setIsAddEnabled(false);
    }

    const locationActionDropdownList = (locationId: string, locationName: string) => {
        return (
            <>
                <Dropdown.Item onClick={() => editLocation(locationId)}><Pencil />Manage Location</Dropdown.Item>
                <Dropdown.Item onClick={() => removeLocation(locationId, locationName)}><XSquare />Remove Location</Dropdown.Item>
            </>
        )
    }

    function renderPartnerLocationsTable(locations: PartnerLocationData[]) {
        return (
            <div>
                <h2 className="color-primary mt-4 mb-5">Partner Locations</h2>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>City</th>
                            <th>Region</th>
                            <th>Status</th>
                            <th>Ready?</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {locations.map(location =>
                            <tr key={location.id.toString()}>
                                <td>{location.name}</td>
                                <td>{location.city}</td>
                                <td>{location.region}</td>
                                <td>{location.isActive ? 'Active' : 'Inactive' }</td>
                                <td>{location.partnerLocationContacts && location.partnerLocationContacts.length > 0 ? 'Yes' : 'No'}</td>
                                <td className="btn py-0">
                                    <Dropdown role="menuitem">
                                        <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                        <Dropdown.Menu id="share-menu">
                                            {locationActionDropdownList(location.id, location.name)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button disabled={!isAddEnabled} className="action" onClick={() => addLocation()}>Add Location</Button>
            </div>
        );
    }

    function renderEditPartnerLocation() {
        return (
            <div>
                <PartnerLocationEdit partnerId={props.partnerId} partnerLocationId={partnerLocationId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} onCancel={handleCancel} onSave={handleSave} />
                <hr />
            </div >
        )
    }

    function renderPartnerLocationContacts() {
        return (
            <div>
                <PartnerLocationContacts partnerLocationId={partnerLocationId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} onSave={handleSave} />
                <hr />
            </div >
        )
    }

    function renderPartnerLocationContactsHelp() {
        return (
            <div className="bg-white py-2 px-5 shadow-sm rounded">
                <h2 className="color-primary mt-4 mb-5">Edit Partner Location Contacts</h2>
                <p>
                    This page allows you set the contacts for a particular location of your organization. These addresses will be sent emails when a TrashMob.eco user chooses to
                    use the services offered by this location. This will allow you to accept or decline the request so that the user knows the status of their requests.
                </p>
            </div>
        )
    }

    function renderPartnerLocationServices() {
        return (
            <div>
                <PartnerLocationServices partnerLocationId={partnerLocationId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
                <hr />
            </div >
        )
    }

    function renderPartnerLocationServicesHelp() {
        return (
            <div className="bg-white py-2 px-5 shadow-sm rounded">
                <h2 className="color-primary mt-4 mb-5">Edit Partner Location Services</h2>
                <p>
                    This page allows you set up the services offered by a partner location. That is, what capabilities are you willing to provide to TrashMob.eco users to help them
                    clean up the local community? This support is crucial to the success of TrashMob.eco volunteers, and we appreciate your help!
                </p>
            </div>
        )
    }

    function renderPartnerLocationEventRequests() {
        return (
            <div>
                <PartnerLocationEventRequests partnerLocationId={partnerLocationId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
                <hr />
            </div >
        )
    }

    function renderPartnerLocationEventRequestsHelp() {
        return (
            <div className="bg-white py-2 px-5 shadow-sm rounded">
                <h2 className="color-primary mt-4 mb-5">Edit Partner Location Service Requests</h2>
                <p>
                    This page allows you to respond to requests from TrashMob.eco users to help them clean up the local community. When a new event is set up, and a user selects one of your services
                    the location contacts will be notified to accept or decline the request here.
                </p>
            </div>
            )
    }

    return (
        <Container>
            <Row className="gx-2 py-5" lg={2}>
                <Col lg={4} className="d-flex">
                    <div className="bg-white py-2 px-5 shadow-sm rounded">
                        <h2 className="color-primary mt-4 mb-5">Edit Partner Locations</h2>
                        <p>
                            A partner location can be thought of as an instance of a business franchise, or the location of a municipal office or yard. You can have as many locations within a community as you want to
                            set up. Each location can offer different services, and have different contact information associated with it. For instance, City Hall may provide starter kits and supplies, but only the
                            public utilities yard offers hauling and disposal.
                        </p>
                        <p>
                            A partner location must have at least one contact set up in order to be ready for events to use them. It must also be Active.
                        </p>
                    </div>
                </Col>
                <Col lg={8}>
                    <div className="bg-white p-5 shadow-sm rounded">
                        {!isPartnerLocationDataLoaded && <p><em>Loading...</em></p>}
                        {(!isEdit && !isAdd) && isPartnerLocationDataLoaded && partnerLocations && renderPartnerLocationsTable(partnerLocations)}
                        {(isEdit || isAdd) && renderEditPartnerLocation()}
                    </div>
                </Col>
            </Row>
            <Row>
                <Col lg={4} className="d-flex">
                    {(isEdit || isAdd) && renderPartnerLocationContactsHelp()}
                </Col>
                <Col lg={8}>
                    {(isEdit || isAdd) && renderPartnerLocationContacts()}
                </Col>
            </Row>
            <Row>
                <Col lg={4} className="d-flex">
                    {(isEdit || isAdd) && renderPartnerLocationServicesHelp()}
                </Col>
                <Col lg={8}>
                    {(isEdit || isAdd) && renderPartnerLocationServices()}
                </Col>
            </Row>
            <Row>
                <Col lg={4} className="d-flex">
                    {(isEdit || isAdd) && renderPartnerLocationEventRequestsHelp()}
                </Col>
                <Col lg={8}>
                    {(isEdit || isAdd) && renderPartnerLocationEventRequests()}
                </Col>
            </Row>
        </Container >
    );
}