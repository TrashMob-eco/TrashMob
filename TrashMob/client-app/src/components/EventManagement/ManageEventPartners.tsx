import * as React from 'react'
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { Button } from 'react-bootstrap';
import * as Constants from '../Models/Constants';
import DisplayEventPartnerLocationData from '../Models/DisplayEventPartnerLocationData';
import EventPartnerLocationServiceStatusData from '../Models/EventPartnerLocationServiceStatusData';
import { getEventPartnerLocationServiceStatus } from '../../store/eventPartnerLocationServiceStatusHelper';
import { Guid } from 'guid-typescript';
import { getServiceType } from '../../store/serviceTypeHelper';
import ServiceTypeData from '../Models/ServiceTypeData';
import EventPartnerLocationServiceData from '../Models/EventPartnerLocationServiceData';
import DisplayEventPartnerLocationServiceData from '../Models/DisplayEventPartnerLocationServiceData';

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

    function handleViewPartnerServices(locationId: string) {
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
                    setEventPartnerLocationServices(data);
                    setIsPartnerLocationServicesDataLoaded(true);
                });
        });
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        setPartnerLocationId(Guid.EMPTY);
        setIsPartnerLocationServicesDataLoaded(false);
    }

    // This will handle the submit form event.
    function requestService(serviceTypeId: number) {

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
            });
        })
    }

    function removeServiceRequest(serviceTypeId: number) {

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
            });
        })
    }

    function renderEventPartnerLocationsTable(eventPartnerLocations: DisplayEventPartnerLocationData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Partner Name</th>
                            <th>Partner Location Name</th>
                            <th>Partner Notes</th>
                            <th>Partner Services Engaged</th>
                        </tr>
                    </thead>
                    <tbody>
                        {eventPartnerLocations.map(eventPartnerLocation =>
                            <tr key={eventPartnerLocation.partnerLocationId.toString()}>
                                <td>{eventPartnerLocation.partnerName}</td>
                                <td>{eventPartnerLocation.partnerLocationName}</td>
                                <td>{eventPartnerLocation.partnerLocationNotes}</td>
                                <td>{eventPartnerLocation.partnerServicesEngaged}</td>
                                <td>
                                    <Button className="action" onClick={() => handleViewPartnerServices(eventPartnerLocation.partnerLocationId)}>View Partner Services</Button>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderEventPartnerLocationServicesTable(services: DisplayEventPartnerLocationServiceData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Service Type</th>
                            <th>Notes</th>
                            <th>Request Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        {services.map(service =>
                            <tr key={service.serviceTypeId}>
                                <td>{getServiceType(serviceTypeList, service.serviceTypeId)}</td>
                                <td>{service.partnerLocationServicePublicNotes}</td>
                                <td>{getEventPartnerLocationServiceStatus(serviceStatusList, service.eventPartnerLocationServiceStatusId)}</td>
                                <td>
                                    <Button hidden={service.eventPartnerLocationServiceStatusId !== 0} className="action" onClick={() => requestService(service.serviceTypeId)}>Request Service</Button>
                                    <Button hidden={service.eventPartnerLocationServiceStatusId === 0} className="action" onClick={() => removeServiceRequest(service.serviceTypeId)}>Remove Service Request</Button>
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
        <>
            <div>
                {props.eventId === Guid.EMPTY && <p> <em>Event must be created first.</em></p>}
                {!isEventPartnerLocationDataLoaded && props.eventId !== Guid.EMPTY && <p><em>Loading...</em></p>}
                {isEventPartnerLocationDataLoaded && eventPartnerLocations.length === 0 && <p> <em>Sorry, there are no registered partners in your area.</em></p>}
                {isEventPartnerLocationDataLoaded && eventPartnerLocations.length !== 0 && renderEventPartnerLocationsTable(eventPartnerLocations)}
                {isPartnerLocationServicesDataLoaded && eventPartnerLocationServices && renderEventPartnerLocationServicesTable(eventPartnerLocationServices)}
            </div>
        </>
    );
}
