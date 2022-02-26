import * as React from 'react'
import { Link, useHistory } from 'react-router-dom';
import EventAttendeeData from './Models/EventAttendeeData';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import { getEventType } from '../store/eventTypeHelper';
import EventData from './Models/EventData';
import EventTypeData from './Models/EventTypeData';
import UserData from './Models/UserData';
import { Button, Form, Modal } from 'react-bootstrap';
import UserWaiverData from './Models/UserWaiverData';

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
    const [isOpen, setIsOpen] = React.useState(false);
    const [agree, setAgree] = React.useState(false);
    const [selectedEventId, setSelectedEventId] = React.useState("");

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

    function addAttendee() {

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {

            var headers = getDefaultHeaders('GET');

            // Check to see if the user has a current TrashMob.eco waiver on file
            fetch('api/userwaivers/current/' + props.currentUser.id, {
                method: 'GET',
                headers: headers,
            }).then(response => {
                if (response.status === 404) {
                    // User has no signed waivers on file. Pop up waiver signing form
                    setIsOpen(true);
                }
                else {
                    var eventAttendee = new EventAttendeeData();
                    eventAttendee.userId = props.currentUser.id;
                    eventAttendee.eventId = selectedEventId;

                    var data = JSON.stringify(eventAttendee);

                    headers = getDefaultHeaders('POST');
                    headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                    // POST request for Add EventAttendee.  
                    fetch('/api/EventAttendees', {
                        method: 'POST',
                        body: data,
                        headers: headers,
                    }).then((response) => response.json())
                        .then(props.onAttendanceChanged())
                }
            })
        })
    }

    function handleAttend(eventId: string) {

        var accounts = msalClient.getAllAccounts();
        setSelectedEventId(eventId);

        if (accounts === null || accounts.length === 0) {
            msalClient.loginRedirect().then(() => {
                addAttendee();
            })
        }
        else {
            addAttendee();
        }
    }

    function togglemodal() {
        setIsOpen(!isOpen);
    }

    function checkboxhandler() {
        // if agree === true, it will be set to false
        // if agree === false, it will be set to true
        setAgree(!agree);
    }

    function updateWaiver() {

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/userwaivers/' + props.currentUser.id, {
                method: 'POST',
                headers: headers
            })
                .then(response => response.json() as Promise<UserWaiverData>)
                .then(_ => {
                    addAttendee();
                }
                )
        })
    }

    function renderAcceptWaiver() {
        return (
            <div>
                <Modal isOpen={isOpen} onrequestclose={togglemodal} contentlabel="Accept Terms of Use" fade={true} style={{ width: "500px", display: "block" }}>
                    <div className="container">
                        <Form>
                            <Form.Row>
                                <Form.Group>
                                    <Form.Label className="control-label">I have reviewed and I agree to the TrashMob.eco <Link to='./waiver'>Liability Waiver</Link>.</Form.Label>
                                    <Form.Check id="agree" onChange={checkboxhandler} label="Yes" />
                                </Form.Group>
                            </Form.Row>
                            <Form.Row>
                                <Button disabled={!agree} className="action" onClick={() => {
                                    updateWaiver();
                                    togglemodal();
                                }
                                }>
                                    I Agree
                                </Button>
                            </Form.Row>
                        </Form>
                    </div>
                </Modal>
            </div>
        )
    }

    function renderEventsTable(events: DisplayEvent[]) {
        return (
            <div>
                { renderAcceptWaiver() }
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
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
                        {events.sort((a, b) => (a.eventDate > b.eventDate) ? 1 : -1).map(mobEvent =>
                            <tr key={mobEvent.id.toString()}>
                                <td>{mobEvent.name}</td>
                                <td>{new Date(mobEvent.eventDate).toLocaleDateString("en-US", { month: "long", day: "numeric", year: 'numeric', hour: 'numeric', minute: 'numeric' })}</td>
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