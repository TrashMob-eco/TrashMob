import * as React from 'react'
import UserData from '../Models/UserData';
import { Dropdown } from 'react-bootstrap';
import { getApiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as Constants from '../Models/Constants';
import EventPartnerLocationServiceStatusData from '../Models/EventPartnerLocationServiceStatusData';
import EventPartnerLocationServiceData from '../Models/EventPartnerLocationServiceData';
import { getEventPartnerLocationServiceStatus } from '../../store/eventPartnerLocationServiceStatusHelper';
import DisplayPartnerLocationEventData from '../Models/DisplayPartnerLocationEventServiceData';
import { Guid } from 'guid-typescript';
import DisplayPartnerLocationEventServiceData from '../Models/DisplayPartnerLocationEventServiceData';
import ServiceTypeData from '../Models/ServiceTypeData';
import { getServiceType } from '../../store/serviceTypeHelper';
import { CheckSquare, XSquare } from 'react-bootstrap-icons';

export interface PartnerLocationEventRequestsDataProps {
    partnerLocationId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerLocationEventRequests: React.FC<PartnerLocationEventRequestsDataProps> = (props) => {

    const [isPartnerLocationEventDataLoaded, setIsPartnerLocationEventDataLoaded] = React.useState<boolean>(false);
    const [eventPartnerStatusList, setEventPartnerStatusList] = React.useState<EventPartnerLocationServiceStatusData[]>([]);
    const [serviceTypeList, setServiceTypeList] = React.useState<ServiceTypeData[]>([]);
    const [partnerLocationEvents, setPartnerLocationEvents] = React.useState<DisplayPartnerLocationEventData[]>([]);

    React.useEffect(() => {
        if (props.isUserLoaded && props.partnerLocationId) {
            const account = msalClient.getAllAccounts()[0];
            var apiConfig = getApiConfig();

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/eventpartnerlocationservicestatuses', {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<EventPartnerLocationServiceStatusData[]>)
                    .then(data => {
                        setEventPartnerStatusList(data)
                    })
                    .then(() => {
                        if (props.partnerLocationId !== Guid.EMPTY) {
                            fetch('/api/partnerlocationeventservices/' + props.partnerLocationId, {
                                method: 'GET',
                                headers: headers
                            })
                                .then(response => response.json() as Promise<DisplayPartnerLocationEventData[]>)
                                .then(data => {
                                    setPartnerLocationEvents(data);
                                })
                        }
                        else {
                            fetch('/api/partnerlocationeventservices/getbyuser/' + props.currentUser.id, {
                                method: 'GET',
                                headers: headers
                            })
                                .then(response => response.json() as Promise<DisplayPartnerLocationEventData[]>)
                                .then(data => {
                                    setPartnerLocationEvents(data);
                                })
                        }

                    })
                    .then(() => {
                        fetch('/api/servicetypes/', {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => response.json() as Promise<ServiceTypeData[]>)
                            .then(data => {
                                setServiceTypeList(data);
                                setIsPartnerLocationEventDataLoaded(true)
                            })
                    });
            });
        }
    }, [props.partnerLocationId, props.isUserLoaded, props.currentUser.id])

    // This will handle the submit form event.  
    function handleRequestPartnerAssistance(eventId: string, partnerLocationId: string, serviceTypeId: number, eventPartnerLocationServiceStatusId: number) {

        var eventData = new EventPartnerLocationServiceData();
        eventData.eventId = eventId;
        eventData.partnerLocationId = partnerLocationId;
        eventData.serviceTypeId = serviceTypeId;
        eventData.eventPartnerLocationServiceStatusId = eventPartnerLocationServiceStatusId;

        var method = "PUT";

        var evtdata = JSON.stringify(eventData);

        const account = msalClient.getAllAccounts()[0];
        var apiConfig = getApiConfig();

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
                fetch('/api/partnerlocationeventservices/' + partnerLocationId, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<DisplayPartnerLocationEventData[]>)
                    .then(data => {
                        setPartnerLocationEvents(data);
                        setIsPartnerLocationEventDataLoaded(true)
                    })
            });
        })
    }

    const eventPartnerServiceRequestActionDropdownList = (partnerEvent: DisplayPartnerLocationEventServiceData) => {
        return (
            <>
                <Dropdown.Item onClick={() => handleRequestPartnerAssistance(partnerEvent.eventId, partnerEvent.partnerLocationId, partnerEvent.serviceTypeId, Constants.EventPartnerLocationServiceStatusAccepted)}><CheckSquare />Accept Partner Assistance Request</Dropdown.Item>
                <Dropdown.Item onClick={() => handleRequestPartnerAssistance(partnerEvent.eventId, partnerEvent.partnerLocationId, partnerEvent.serviceTypeId, Constants.EventPartnerLocationServiceStatusDeclined)}><XSquare />Decline Partner Assistance Request</Dropdown.Item>
            </>
        )
    }

    function renderPartnerLocationEventServicesTable(partnerLocationEventServices: DisplayPartnerLocationEventServiceData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Location Name</th>
                            <th>Event Name</th>
                            <th>Event Date</th>
                            <th>Event Address</th>
                            <th>Service Type</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        {partnerLocationEventServices.map(partnerEvent =>
                            <tr key={partnerEvent.eventId.toString()}>
                                <td>{partnerEvent.partnerLocationName}</td>
                                <td>{partnerEvent.eventName}</td>
                                <td>{new Date(partnerEvent.eventDate).toDateString()}</td>
                                <td>{partnerEvent.eventStreetAddress}, {partnerEvent.eventCity}</td>
                                <td>{getServiceType(serviceTypeList, partnerEvent.serviceTypeId)}</td>
                                <td>{getEventPartnerLocationServiceStatus(eventPartnerStatusList, partnerEvent.eventPartnerLocationStatusId)}</td>
                                <td className="btn py-0">
                                    <Dropdown role="menuitem">
                                        <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                        <Dropdown.Menu id="share-menu">
                                            {eventPartnerServiceRequestActionDropdownList(partnerEvent)}
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

    return (
        <div className="bg-white p-5 shadow-sm rounded">
            {!isPartnerLocationEventDataLoaded && props.partnerLocationId !== Guid.EMPTY && <p><em>Loading...</em></p>}
            {isPartnerLocationEventDataLoaded && partnerLocationEvents.length === 0 && <p> <em>There are no event requests for this location.</em></p>}
            {isPartnerLocationEventDataLoaded && partnerLocationEvents.length !== 0 && renderPartnerLocationEventServicesTable(partnerLocationEvents)}
        </div>
    );
}