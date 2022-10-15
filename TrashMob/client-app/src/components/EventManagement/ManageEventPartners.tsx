import * as React from 'react'
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { Button } from 'react-bootstrap';
import * as Constants from '../Models/Constants';
import DisplayEventPartnerLocationData from '../Models/DisplayEventPartnerLocationData';
import EventPartnerStatusData from '../Models/EventPartnerStatusData';
import { getEventPartnerStatus } from '../../store/eventPartnerStatusHelper';
import EventPartnerLocationData from '../Models/EventPartnerLocationData';
import { Guid } from 'guid-typescript';

export interface ManageEventPartnersProps {
    eventId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const ManageEventPartners: React.FC<ManageEventPartnersProps> = (props) => {
    const [isEventPartnerDataLoaded, setIsEventPartnerDataLoaded] = React.useState<boolean>(false);
    const [eventPartnerStatusList, setEventPartnerStatusList] = React.useState<EventPartnerStatusData[]>([]);
    const [eventPartners, setEventPartners] = React.useState<DisplayEventPartnerLocationData[]>([]);

    React.useEffect(() => {
        if (props.isUserLoaded && props.eventId && props.eventId !== Guid.EMPTY) {
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

                        fetch('/api/eventpartners/' + props.eventId, {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => response.json() as Promise<DisplayEventPartnerLocationData[]>)
                            .then(data => {
                                setEventPartners(data);
                                setIsEventPartnerDataLoaded(true)
                            })
                    });
            });
        }
    }, [props.eventId, props.isUserLoaded])

    function OnEventPartnersUpdated() {
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/eventpartners/' + props.eventId, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<DisplayEventPartnerLocationData[]>)
                .then(data => {
                    setEventPartners(data);
                    setIsEventPartnerDataLoaded(true)
                })
        });
    }

    // This will handle the submit form event.  
    function handleRequestPartnerAssistance(eventId: string, partnerId: string, partnerLocationId: string) {

        var eventData = new EventPartnerLocationData();
        eventData.eventId = eventId;
        eventData.partnerId = partnerId;
        eventData.partnerLocationId = partnerLocationId;
        eventData.eventPartnerStatusId = Constants.EventPartnerStatusRequested;

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

            fetch('/api/eventpartners', {
                method: method,
                headers: headers,
                body: evtdata,
            }).then(() => {
                OnEventPartnersUpdated();
            });
        })
    }

    function renderEventPartnersTable(eventPartners: DisplayEventPartnerLocationData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Partner Name</th>
                            <th>Partner Location Name</th>
                            <th>Partner Notes</th>
                            <th>Partner Status for this Event</th>
                        </tr>
                    </thead>
                    <tbody>
                        {eventPartners.map(eventPartner =>
                            <tr key={eventPartner.partnerLocationId.toString()}>
                                <td>{eventPartner.partnerName}</td>
                                <td>{eventPartner.partnerLocationName}</td>
                                <td>{eventPartner.partnerLocationNotes}</td>
                                <td>{getEventPartnerStatus(eventPartnerStatusList, eventPartner.eventPartnerStatusId)}</td>
                                <td>
                                    <Button hidden={eventPartner.eventPartnerStatusId !== Constants.EventPartnerStatusNone} className="action" onClick={() => handleRequestPartnerAssistance(eventPartner.eventId, eventPartner.partnerId, eventPartner.partnerLocationId)}>Request Partner Assistance</Button>
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
                {props.eventId === Guid.EMPTY && <p> <em>Event must be created first.</em></p>}
                {!isEventPartnerDataLoaded && props.eventId !== Guid.EMPTY && <p><em>Loading...</em></p>}
                {isEventPartnerDataLoaded && eventPartners.length === 0 && <p> <em>Sorry, there are no registered partners in your area.</em></p>}
                {isEventPartnerDataLoaded && eventPartners.length !== 0 && renderEventPartnersTable(eventPartners)}
            </div>
        </>
    );
}
