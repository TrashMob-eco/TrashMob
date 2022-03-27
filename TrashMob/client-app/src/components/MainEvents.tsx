import * as React from 'react'
import { Link, useHistory } from 'react-router-dom';
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
    creator: string = "";
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
                dispEvent.creator = mobEvent.createdByUserName;

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

    const renderEventsList = (events: DisplayEvent[]) => {
        const sortedEvents = events.sort((a, b) => (a.eventDate > b.eventDate) ? 1 : -1)
        return (
            <>
                <ol className="px-1 px-md-5">
                    {sortedEvents.map((mobEvent, i) =>
                        <li className={`d-flex flex-column justify-content-center mb-4 ${i !== sortedEvents.length - 1 ? "border-bottom" : ""}`} key={`event-${i}`}>
                            <div className="d-flex justify-content-between align-items-start align-items-sm-end flex-column flex-sm-row">
                                <h5 className="font-weight-bold font-size-xl">{mobEvent.name}</h5>
                                <span className="font-grey">Created by: {mobEvent.creator}</span>
                            </div>
                            <span className="my-2 event-list-event-type p-2 rounded">{getEventType(props.eventTypeList, mobEvent.eventTypeId)}</span>
                            <div className="d-flex justify-content-between align-items-start align-items-sm-end mb-4 flex-column flex-sm-row">
                                <div className="d-inline-block font-grey">
                                    <p>{new Date(mobEvent.eventDate).toLocaleDateString("en-US", { month: "long", day: "numeric", year: 'numeric', hour: 'numeric', minute: 'numeric' })}</p>
                                    <span>{mobEvent.city}, {mobEvent.region}, {mobEvent.country}</span>
                                </div>
                                <div className="mt-3 mt-sm-0">
                                    <Link to={'/eventdetails/' + mobEvent.id}><button className="btn btn-outline mr-2 font-weight-bold btn-128">View</button></Link>
                                    <Button className="btn btn-primary action btn-128" hidden={!props.isUserLoaded || mobEvent.isAttending === "Yes"} onClick={() => handleAttend(mobEvent.id)}>Register</Button>
                                </div>
                            </div>
                        </li>
                    )}
                </ol>
                <div className="d-flex justify-content-center">
                    <Link to="/eventsummaries"><Button className="btn btn-primary my-5">View all events</Button></Link>
                </div>
            </>
        )
    }

    return (
        <>
            {!props.isEventDataLoaded && <p><em>Loading...</em></p>}
            {props.isEventDataLoaded && renderEventsList(displayEvents)}
        </>
    );
}