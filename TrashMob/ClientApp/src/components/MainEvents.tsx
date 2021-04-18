import * as React from 'react'
import { Link } from 'react-router-dom';
import { getUserFromCache } from '../store/accountHandler';
import EventAttendeeData from './Models/EventAttendeeData';
import { apiConfig, defaultHeaders, msalClient } from '../store/AuthStore';
import { getEventType } from '../store/eventTypeHelper';
import { FetchEventDataState } from './Home';

class DisplayEvent {
    id: string;
    name: string;
    eventDate: Date;
    eventTypeId: number;
    city: string;
    region: string;
    country: string;
    isAttending: string;
}

export const MainEvents: React.FC<FetchEventDataState> = (props) => {
    const [displayEvents, setDisplayEvents] = React.useState([]);

    React.useEffect(() => {
        if (!props.loading && props.eventList && props.myAttendanceList) {
            const list = props.eventList.map((mobEvent) => {
                var dispEvent = new DisplayEvent()
                dispEvent.id = mobEvent.id;
                dispEvent.city = mobEvent.city;
                dispEvent.region = mobEvent.region;
                dispEvent.country = mobEvent.country;
                dispEvent.eventDate = mobEvent.eventDate;
                dispEvent.eventTypeId = mobEvent.eventTypeId;
                dispEvent.name = mobEvent.name;
                var isAttending = props.myAttendanceList.findIndex((e) => e.id === mobEvent.id) >= 0;
                dispEvent.isAttending = !props.isLoggedIn ? 'Log in to see your status' : (isAttending ? 'Yes' : 'No');
                return dispEvent;
            });
            setDisplayEvents(list);
        }
    }, [props.loading, props.eventList, props.myAttendanceList, props.isLoggedIn])

    function addAttendee(eventId: string) {

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {

            var user = getUserFromCache();
            var eventAttendee = new EventAttendeeData();
            eventAttendee.userId = user.id;
            eventAttendee.eventId = eventId;

            var data = JSON.stringify(eventAttendee);

            const headers = defaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            // POST request for Add EventAttendee.  
            fetch('api/EventAttendees', {
                method: 'POST',
                body: data,
                headers: headers,
            }).then((response) => response.json())
        })
    }

    function handleAttend(eventId: string) {

        var accounts = msalClient.getAllAccounts();

        if (accounts === null || accounts.length === 0) {
            msalClient.loginRedirect().then(() => {
                addAttendee(eventId);
            })
        }
        else {
            addAttendee(eventId);
        }
    }

    function renderEventsTable(events: DisplayEvent[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Date</th>
                            <th>Event Type</th>
                            <th>City</th>
                            <th>Region</th>
                            <th>Country</th>
                            <th>Am I Attending?</th>
                        </tr>
                    </thead>
                    <tbody>
                        {events.map(mobEvent =>
                            <tr key={mobEvent.id.toString()}>
                                <td>{mobEvent.name}</td>
                                <td>{mobEvent.eventDate}</td>
                                <td>{getEventType(props.eventTypeList, mobEvent.eventTypeId)}</td>
                                <td>{mobEvent.city}</td>
                                <td>{mobEvent.region}</td>
                                <td>{mobEvent.country}</td>
                                <td>{mobEvent.isAttending}</td>
                                <td>
                                    <Link to={`/eventdetails/${mobEvent.id}`}>Details</Link>
                                    <button className="btn" onClick={() => handleAttend(mobEvent.id)}>Attend</button>
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
                {props.loading && <p><em>Loading...</em></p>}
                {!props.loading && renderEventsTable(displayEvents)}
            </div>
        </>
    );
}