import * as React from 'react'
import { useHistory } from 'react-router-dom';
import EventAttendeeData from './Models/EventAttendeeData';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import { getEventType } from '../store/eventTypeHelper';
import EventData from './Models/EventData';
import EventTypeData from './Models/EventTypeData';
import UserData from './Models/UserData';
import { Button } from 'react-bootstrap';

class DisplayEvent {
    id: string = "";
    name: string = "";
    eventDate: Date = new Date();
    eventTypeId: number = 0;
    city: string = "";
    region: string = "";
    country: string = "";
    isAttending: string = "";
}


export interface MainEventsDataProps {
    eventList: EventData[];
    eventTypeList: EventTypeData[];
    myAttendanceList: EventData[];
    isEventDataLoaded: boolean;
    isUserEventDataLoaded: boolean;
    isUserLoaded: boolean;
    currentUser: UserData;
    onAttendanceChanged: any;
};

export const MainEvents: React.FC<MainEventsDataProps> = (props) => {
    const [displayEvents, setDisplayEvents] = React.useState<DisplayEvent[]>([]);
    const history = useHistory();

    React.useEffect(() => {
        if (props.isEventDataLoaded && props.eventList) {
            const list = props.eventList.map((mobEvent) => {
                var dispEvent = new DisplayEvent()
                dispEvent.id = mobEvent.id;
                dispEvent.city = mobEvent.city;
                dispEvent.region = mobEvent.region;
                dispEvent.country = mobEvent.country;
                dispEvent.eventDate = mobEvent.eventDate;
                dispEvent.eventTypeId = mobEvent.eventTypeId;
                dispEvent.name = mobEvent.name;
                if (props.isUserEventDataLoaded) {
                    var isAttending = props.myAttendanceList && (props.myAttendanceList.findIndex((e) => e.id === mobEvent.id) >= 0);
                    dispEvent.isAttending = (isAttending ? 'Yes' : 'No');
                }
                else {
                    dispEvent.isAttending = 'Log in to see your status';
                }
                return dispEvent;
            });
            setDisplayEvents(list);
        }
    }, [props.isEventDataLoaded, props.eventList, props.myAttendanceList, props.isUserLoaded, props.isUserEventDataLoaded])

    function addAttendee(eventId: string) {

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {

            var eventAttendee = new EventAttendeeData();
            eventAttendee.userId = props.currentUser.id;
            eventAttendee.eventId = eventId;

            var data = JSON.stringify(eventAttendee);

            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            // POST request for Add EventAttendee.  
            fetch('/api/EventAttendees', {
                method: 'POST',
                body: data,
                headers: headers,
            }).then((response) => response.json())
                .then(props.onAttendanceChanged())
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
                <table className='table table-striped' aria-labelledby="tabelLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Date</th>
                            <th>Event Type</th>
                            {/* <th>City</th> */}
                            {/* <th>Region</th> */}
                            {/* <th>Country</th> */}
                            <th>Am I Attending?</th>
                            <th />
                        </tr>
                    </thead>
                    <tbody>
                        {events.map(mobEvent =>
                            <tr key={mobEvent.id.toString()}>
                                <td>{mobEvent.name}</td>
                                <td>{new Date(mobEvent.eventDate).toLocaleDateString("en-US", {month:"long", day:"numeric", hour: 'numeric', minute: 'numeric' })}</td>
                                <td>{getEventType(props.eventTypeList, mobEvent.eventTypeId)}</td>
                                {/* <td>{mobEvent.city}</td> */}
                                {/* <td>{mobEvent.region}</td> */}
                                {/* <td>{mobEvent.country}</td> */}
                                <td>
                                    <Button hidden={!props.isUserLoaded || mobEvent.isAttending === "Yes"} className="action" onClick={() => handleAttend(mobEvent.id)}>Register to Attend Event</Button>
                                    <label hidden={props.isUserLoaded}>Sign-in required</label>
                                    <label hidden={!props.isUserLoaded || mobEvent.isAttending !== 'Yes'}>Yes</label>
                                </td>
                                <td>
                                    <Button className="action" onClick={() => history.push('/eventdetails/' + mobEvent.id)}>View Details</Button>
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
                {!props.isEventDataLoaded && <p><em>Loading...</em></p>}
                {props.isEventDataLoaded && renderEventsTable(displayEvents)}
            </div>
        </>
    );
}