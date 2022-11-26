import * as React from 'react'
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { Button, Container, Dropdown } from 'react-bootstrap';
import * as Constants from '../Models/Constants';
import DisplayEventPartnerLocationData from '../Models/DisplayEventPartnerLocationData';
import EventPartnerLocationServiceStatusData from '../Models/EventPartnerLocationServiceStatusData';
import { getEventPartnerLocationServiceStatus } from '../../store/eventPartnerLocationServiceStatusHelper';
import { Guid } from 'guid-typescript';
import { getServiceType } from '../../store/serviceTypeHelper';
import ServiceTypeData from '../Models/ServiceTypeData';
import EventPartnerLocationServiceData from '../Models/EventPartnerLocationServiceData';
import DisplayEventPartnerLocationServiceData from '../Models/DisplayEventPartnerLocationServiceData';
import { Building, Envelope, Eye, ThreeDots } from 'react-bootstrap-icons';

export interface ManageEventPartnersProps {
    eventId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const ManageEventPartners: React.FC<ManageEventPartnersProps> = (props) => {
    const [isEventPartnerLocationDataLoaded, setIsEventPartnerLocationDataLoaded] = React.useState<boolean>(false);
    const [eventPartnerLocations, setEventPartnerLocations] = React.useState<DisplayEventPartnerLocationData[]>([]);
    const [eventPartnerLocationServices, setEventPartnerLocationServices] = React.useState<DisplayEventPartnerLocationServiceData[]>([]);
    const [isPartnerLocationServicesDataLoaded, setIsPartnerLocationServicesDataLoaded] = React.useState<boolean>(false);
    const [serviceTypeList, setServiceTypeList] = React.useState<ServiceTypeData[]>([]);
    const [serviceStatusList, setServiceStatusList] = React.useState<EventPartnerLocationServiceStatusData[]>([]);
    const [partnerLocationId, setPartnerLocationId] = React.useState<string>(Guid.EMPTY);
    const [partnerLocationName, setPartnerLocationName] = React.useState<string>("");

    React.useEffect(() => {

        const headers = getDefaultHeaders('GET');

        fetch('/api/servicetypes', {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                setServiceTypeList(data);
            })
            .then(() => {

                fetch('/api/eventpartnerlocationservicestatuses', {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<Array<any>>)
                    .then(data => {
                        setServiceStatusList(data);
                    });
            });


        if (props.isUserLoaded && props.eventId && props.eventId !== Guid.EMPTY) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/eventpartnerlocationservices/' + props.eventId, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<DisplayEventPartnerLocationData[]>)
                    .then(data => {
                        setEventPartnerLocations(data);
                        setIsEventPartnerLocationDataLoaded(true)
                    })
            });
        }
    }, [props.eventId, props.isUserLoaded])

    function OnEventPartnerLocationsUpdated() {
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/eventpartnerlocationservices/' + props.eventId, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<DisplayEventPartnerLocationData[]>)
                .then(data => {
                    setEventPartnerLocations(data);
                    setIsEventPartnerLocationDataLoaded(true)
                })
        });
    }

    function handleViewPartnerServices(locationId: string, partnerLocName: string) {
        const account = msalClient.getAllAccounts()[0];


        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/eventpartnerlocationservices/' + props.eventId + "/" + locationId, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<DisplayEventPartnerLocationServiceData[]>)
                .then(data => {
                    setPartnerLocationId(locationId);
                    setPartnerLocationName(partnerLocName);
                    setEventPartnerLocationServices(data);
                    setIsPartnerLocationServicesDataLoaded(true);
                });
        });
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        setPartnerLocationId(Guid.EMPTY);
        setPartnerLocationName("");
        setIsPartnerLocationServicesDataLoaded(false);
    }

    //  This will handle the submit form event.
    function requestService(serviceTypeId: number, partnerLocName: string) {

        var eventData = new EventPartnerLocationServiceData();
        eventData.eventId = props.eventId;
        eventData.partnerLocationId = partnerLocationId;
        eventData.serviceTypeId = serviceTypeId;
        eventData.eventPartnerLocationServiceStatusId = Constants.EventPartnerLocationServiceStatusRequested;

        var method = "POST";

        var evtdata = JSON.stringify(eventData);

        // PUT request for Edit Event.  
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/eventpartnerlocationservices', {
                method: method,
                headers: headers,
                body: evtdata,
            }).then(() => {
                OnEventPartnerLocationsUpdated();
                handleViewPartnerServices(partnerLocationId, partnerLocName);
            });
        })
    }

    function removeServiceRequest(serviceTypeId: number, partnerLocName: string) {

        var method = "DELETE";

        // PUT request for Edit Event.  
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/eventpartnerlocationservices/' + props.eventId + "/" + partnerLocationId + "/" + serviceTypeId, {
                method: method,
                headers: headers
            }).then(() => {
                OnEventPartnerLocationsUpdated();
                handleViewPartnerServices(partnerLocationId, partnerLocName);
            });
        })
    }

    const partnerActionDropdownList = (partnerLocationId: string, partnerLocName: string) => {
        return (
            <>
                <Dropdown.Item onClick={() => handleViewPartnerServices(partnerLocationId, partnerLocName)}><Eye aria-hidden="true" color='#96ba00' size={24} className="mr-2" />View partner services</Dropdown.Item>
            </>
        )
    }

    const partnerServiceActionDropdownList = (serviceTypeId: number, partnerLocName: string) => {
        return (
            <>
                <Dropdown.Item onClick={() => requestService(serviceTypeId, partnerLocName)}><Eye aria-hidden="true" color='#96ba00' size={24} className="mr-2" />Request partner service</Dropdown.Item>
            </>
        )
    }

    const existingPartnerServiceActionDropdownList = (serviceTypeId: number, partnerLocName: string) => {
        return (
            <>
                <Dropdown.Item onClick={() => removeServiceRequest(serviceTypeId, partnerLocName)}><Eye aria-hidden="true" color='#96ba00' size={24} className="mr-2" />Remove service request</Dropdown.Item>
            </>
        )
    }

    function renderEventPartnerLocationsTable(eventPartnerLocations: DisplayEventPartnerLocationData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th className='h5 py-4'>Partner Name</th>
                            <th className='h5 py-4'>Location</th>
                            <th className='h5 py-4'>Notes</th>
                            <th className='h5 py-4'>Requested Services</th>
                            <th className='h5 py-4'>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {eventPartnerLocations.map(eventPartnerLocation =>
                            <tr key={eventPartnerLocation.partnerLocationId.toString()}>
                                <td className='color-grey p-18 py-3'>{eventPartnerLocation.partnerName}</td>
                                <td className='color-grey p-18 py-3'>{eventPartnerLocation.partnerLocationName}</td>
                                <td className='color-grey p-18 py-3'>{eventPartnerLocation.partnerLocationNotes}</td>
                                <td className='color-grey p-18 py-3'>{eventPartnerLocation.partnerServicesEngaged}</td>
                                <td className='color-grey p-18 py-3'>
                                    <Dropdown role="menuitem">
                                        <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                        <Dropdown.Menu id="share-menu">
                                            {partnerActionDropdownList(eventPartnerLocation.partnerLocationId, eventPartnerLocation.partnerLocationName)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderEventPartnerLocationServicesTable(services: DisplayEventPartnerLocationServiceData[], partnerLocationName: string) {
        return (
            <div>
                <div className='d-flex align-items-center justify-content-between'>
                    <h4 className='fw-600 color-primary my-4'>Services Available from Partner: {partnerLocationName}</h4>
                </div>                
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Service Type</th>
                            <th>Notes</th>
                            <th>Request Status</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {services.map(service =>
                            <tr key={service.serviceTypeId}>
                                <td>{getServiceType(serviceTypeList, service.serviceTypeId)}</td>
                                <td>{service.partnerLocationServicePublicNotes}</td>
                                <td>{getEventPartnerLocationServiceStatus(serviceStatusList, service.eventPartnerLocationServiceStatusId)}</td>
                                <td className="btn py-0">
                                    <Dropdown role="menuitem">
                                        <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                        <Dropdown.Menu id="share-menu">
                                            {service.eventPartnerLocationServiceStatusId === 0 ? partnerServiceActionDropdownList(service.serviceTypeId, partnerLocationName) : existingPartnerServiceActionDropdownList(service.serviceTypeId, partnerLocationName)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button className="action" onClick={(e: any) => handleCancel(e)}>Cancel</Button>
            </div>
        );
    }

    return (
        <Container className='p-4 bg-white rounded my-5'>
            <div className='d-flex align-items-center justify-content-between'>
                <h4 className='fw-600 color-primary my-4'>Available Partners ({eventPartnerLocations?.length})</h4>
            </div>
            {props.eventId === Guid.EMPTY && <p> <em>Event must be created first.</em></p>}
            {!isEventPartnerLocationDataLoaded && props.eventId !== Guid.EMPTY && <p><em>Loading...</em></p>}
            {isEventPartnerLocationDataLoaded && eventPartnerLocations.length === 0 && <p> <em>Sorry, there are no registered partners in your area.</em></p>}
            {isEventPartnerLocationDataLoaded && eventPartnerLocations.length !== 0 && renderEventPartnerLocationsTable(eventPartnerLocations)}
            {isPartnerLocationServicesDataLoaded && eventPartnerLocationServices && renderEventPartnerLocationServicesTable(eventPartnerLocationServices, partnerLocationName)}
        </Container>
    );
}
