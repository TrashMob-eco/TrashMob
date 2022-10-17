import * as React from 'react'
import UserData from '../Models/UserData';
import { Button } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as Constants from '../Models/Constants';
import EventPartnerLocationStatusData from '../Models/EventPartnerLocationStatusData';
import EventPartnerLocationData from '../Models/EventPartnerLocationData';
import { getEventPartnerStatus } from '../../store/eventPartnerStatusHelper';
import DisplayPartnerLocationEventData from '../Models/DisplayPartnerLocationEventData';

export interface PartnerLocationEventRequestsDataProps {
    partnerLocationId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerLocationEventRequests: React.FC<PartnerLocationEventRequestsDataProps> = (props) => {

    const [isPartnerLocationEventDataLoaded, setIsPartnerLocationEventDataLoaded] = React.useState<boolean>(false);
    const [eventPartnerStatusList, setEventPartnerStatusList] = React.useState<EventPartnerLocationStatusData[]>([]);
    const [partnerLocationEvents, setPartnerLocationEvents] = React.useState<DisplayPartnerLocationEventData[]>([]);

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

                fetch('/api/eventpartnerlocationstatuses', {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<EventPartnerLocationStatusData[]>)
                    .then(data => {
                        setEventPartnerStatusList(data)
                    })
                    .then(() => {

                        fetch('/api/partnerlocationevents/' + props.partnerLocationId, {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => response.json() as Promise<DisplayPartnerLocationEventData[]>)
                            .then(data => {
                                setPartnerLocationEvents(data);
                                setIsPartnerLocationEventDataLoaded(true)
                            })
                    });
            });
        }
    }, [props.partnerLocationId, props.isUserLoaded])

    function OnEventPartnerLocationsUpdated() {
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerlocationevents/' + props.partnerLocationId, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<DisplayPartnerLocationEventData[]>)
                .then(data => {
                    setPartnerLocationEvents(data);
                    setIsPartnerLocationEventDataLoaded(true)
                })
        });
    }

    // This will handle the submit form event.  
    function handleRequestPartnerAssistance(eventId: string, partnerLocationId: string, eventPartnerLocationStatusId: number) {

        var eventData = new EventPartnerLocationData();
        eventData.eventId = eventId;
        eventData.partnerLocationId = partnerLocationId;
        eventData.eventPartnerLocationStatusId = eventPartnerLocationStatusId;

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

            fetch('/api/eventpartnerlocations', {
                method: method,
                headers: headers,
                body: evtdata,
            }).then(() => {
                OnEventPartnerLocationsUpdated();
            });
        })
    }

    function renderPartnerLocationEventsTable(partnerEvents: DisplayPartnerLocationEventData[]) {
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
                                <td>{getEventPartnerStatus(eventPartnerStatusList, partnerEvent.eventPartnerLocationStatusId)}</td>
                                <td>
                                    <Button hidden={partnerEvent.eventPartnerLocationStatusId === Constants.EventPartnerLocationStatusAccepted} className="action" onClick={() => handleRequestPartnerAssistance(partnerEvent.eventId, partnerEvent.partnerLocationId, Constants.EventPartnerLocationStatusAccepted)}>Accept Partner Assistance Request</Button>
                                    <Button hidden={partnerEvent.eventPartnerLocationStatusId === Constants.EventPartnerLocationStatusDeclined} className="action" onClick={() => handleRequestPartnerAssistance(partnerEvent.eventId, partnerEvent.partnerLocationId, Constants.EventPartnerLocationStatusDeclined)}>Decline Partner Assistance Request</Button>
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
                {!isPartnerLocationEventDataLoaded && <p><em>Loading...</em></p>}
                {isPartnerLocationEventDataLoaded && partnerLocationEvents.length === 0 && <p> <em>Sorry, there are no registered partners in your area.</em></p>}
                {isPartnerLocationEventDataLoaded && partnerLocationEvents.length !== 0 && renderPartnerLocationEventsTable(partnerLocationEvents)}
            </div>
        </>
    );
}