import * as React from 'react'
import UserData from '../Models/UserData';
import { Button } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as Constants from '../Models/Constants';
import EventPartnerStatusData from '../Models/EventPartnerStatusData';
import DisplayPartnerEventData from '../Models/DisplayPartnerEventData';
import EventPartnerData from '../Models/EventPartnerData';
import { getEventPartnerStatus } from '../../store/eventPartnerStatusHelper';

export interface PartnerEventRequestsDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerEventRequests: React.FC<PartnerEventRequestsDataProps> = (props) => {

    const [isPartnerEventDataLoaded, setIsPartnerEventDataLoaded] = React.useState<boolean>(false);
    const [eventPartnerStatusList, setEventPartnerStatusList] = React.useState<EventPartnerStatusData[]>([]);
    const [partnerEvents, setPartnerEvents] = React.useState<DisplayPartnerEventData[]>([]);

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

                fetch('/api/eventpartnerstatuses', {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<EventPartnerStatusData[]>)
                    .then(data => {
                        setEventPartnerStatusList(data)
                    })
                    .then(() => {

                        fetch('/api/partnerevents/' + props.partnerId, {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => response.json() as Promise<DisplayPartnerEventData[]>)
                            .then(data => {
                                setPartnerEvents(data);
                                setIsPartnerEventDataLoaded(true)
                            })
                    });
            });
        }
    }, [props.partnerId, props.isUserLoaded])

    function OnEventPartnersUpdated() {
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerevents/' + props.partnerId, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<DisplayPartnerEventData[]>)
                .then(data => {
                    setPartnerEvents(data);
                    setIsPartnerEventDataLoaded(true)
                })
        });
    }

    // This will handle the submit form event.  
    function handleRequestPartnerAssistance(eventId: string, partnerId: string, partnerLocationId: string, eventPartnerStatusId: number) {

        var eventData = new EventPartnerData();
        eventData.eventId = eventId;
        eventData.partnerId = partnerId;
        eventData.partnerLocationId = partnerLocationId;
        eventData.eventPartnerStatusId = eventPartnerStatusId;

        var method = "PUT";

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

            fetch('/api/eventpartners', {
                method: method,
                headers: headers,
                body: evtdata,
            }).then(() => {
                OnEventPartnersUpdated();
            });
        })
    }

    function renderPartnerEventsTable(partnerEvents: DisplayPartnerEventData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Location Name</th>
                            <th>Event Name</th>
                            <th>Event Date</th>
                            <th>Event Address</th>
                            <th>Event Description</th>
                            <th>Partner Status for this Event</th>
                        </tr>
                    </thead>
                    <tbody>
                        {partnerEvents.map(partnerEvent =>
                            <tr key={partnerEvent.eventId.toString()}>
                                <td>{partnerEvent.partnerLocationName}</td>
                                <td>{partnerEvent.eventName}</td>
                                <td>{new Date(partnerEvent.eventDate).toLocaleString()}</td>
                                <td>{partnerEvent.eventStreetAddress}, {partnerEvent.eventCity}</td>
                                <td>{partnerEvent.eventDescription}</td>
                                <td>{getEventPartnerStatus(eventPartnerStatusList, partnerEvent.eventPartnerStatusId)}</td>
                                <td>
                                    <Button hidden={partnerEvent.eventPartnerStatusId === Constants.EventPartnerStatusAccepted} className="action" onClick={() => handleRequestPartnerAssistance(partnerEvent.eventId, partnerEvent.partnerId, partnerEvent.partnerLocationId, Constants.EventPartnerStatusAccepted)}>Accept Partner Assistance Request</Button>
                                    <Button hidden={partnerEvent.eventPartnerStatusId === Constants.EventPartnerStatusDeclined} className="action" onClick={() => handleRequestPartnerAssistance(partnerEvent.eventId, partnerEvent.partnerId, partnerEvent.partnerLocationId, Constants.EventPartnerStatusDeclined)}>Decline Partner Assistance Request</Button>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    return (
        <>
            <div>
                {!isPartnerEventDataLoaded && <p><em>Loading...</em></p>}
                {isPartnerEventDataLoaded && partnerEvents.length === 0 && <p> <em>Sorry, there are no registered partners in your area.</em></p>}
                {isPartnerEventDataLoaded && partnerEvents.length !== 0 && renderPartnerEventsTable(partnerEvents)}
            </div>
        </>
    );
}