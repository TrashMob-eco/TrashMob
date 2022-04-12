import * as React from 'react'
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { UserEvents } from './UserEvents'
import EventData from './Models/EventData';
import EventTypeData from './Models/EventTypeData';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import { data } from 'azure-maps-control';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapControllerPointCollection from './MapControllerPointCollection';
import UserData from './Models/UserData';
import { Col, Form, ToggleButton } from 'react-bootstrap';

interface MyDashboardProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const MyDashboard: React.FC<MyDashboardProps> = (props) => {
    const [myEventList, setMyEventList] = React.useState<EventData[]>([]);
    const [eventTypeList, setEventTypeList] = React.useState<EventTypeData[]>([]);
    const [isEventDataLoaded, setIsEventDataLoaded] = React.useState<boolean>(false);
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [currentUser, setCurrentUser] = React.useState<UserData>(props.currentUser);
    const [isUserLoaded, setIsUserLoaded] = React.useState<boolean>(props.isUserLoaded);
    const [showFutureEventsOnly, setShowFutureEventsOnly] = React.useState<boolean>(false);
    const [showCanceledEventsOnly, setShowCanceledEventsOnly] = React.useState<boolean>(false);
    const [reloadEvents, setReloadEvents] = React.useState<number>(0);

    React.useEffect(() => {
        const headers = getDefaultHeaders('GET');

        fetch('/api/eventtypes', {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                setEventTypeList(data);
            });

        MapStore.getOption().then(opts => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        })

        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(position => {
                var point = new data.Position(position.coords.longitude, position.coords.latitude);
                setCenter(point)
            });
        } else {
            console.log("Not Available");
        }
    }, []);

    React.useEffect(() => {
        if (props.isUserLoaded) {

            setCurrentUser(props.currentUser);
            setIsUserLoaded(props.isUserLoaded);

            setIsEventDataLoaded(false);
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                if (!showCanceledEventsOnly) {
                    fetch('/api/events/userevents/' + props.currentUser.id + '/' + showFutureEventsOnly, {
                        method: 'GET',
                        headers: headers
                    })
                        .then(response => response.json() as Promise<EventData[]>)
                        .then(data => {
                            setMyEventList(data);
                            setIsEventDataLoaded(true);
                        });
                }
                else {
                    fetch('/api/events/canceleduserevents/' + props.currentUser.id + '/' + showFutureEventsOnly, {
                        method: 'GET',
                        headers: headers
                    })
                        .then(response => response.json() as Promise<EventData[]>)
                        .then(data => {
                            setMyEventList(data);
                            setIsEventDataLoaded(true);
                        });
                }
            });
        }
    }, [showFutureEventsOnly, showCanceledEventsOnly, reloadEvents, props.currentUser.id, props.isUserLoaded]);

    function handleLocationChange(point: data.Position) {
        // do nothing
    }

    function handleAttendanceChanged(point: data.Position) {
        // do nothing
    }

    function handleDetailsSelected(eventId: string) {
        props.history.push("eventdetails/" + eventId);
    }

    function handleReloadEvents() {
        // A trick to force the reload as needed.
        setReloadEvents(reloadEvents + 1);
    }

    return (
        <div className="card pop">
            <div>
                <Link to="/manageeventdashboard">Create a New Event</Link>
            </div>
            <div>
                <Form>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <ToggleButton
                                    type="checkbox"
                                    variant="outline-dark"
                                    checked={showFutureEventsOnly}
                                    value="1"
                                    onChange={(e) => setShowFutureEventsOnly(e.currentTarget.checked)}
                                >
                                    Show Future Events Only:
                                    </ToggleButton>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <ToggleButton
                                    type="checkbox"
                                    variant="outline-dark"
                                    checked={showCanceledEventsOnly}
                                    value="1"
                                    onChange={(e) => setShowCanceledEventsOnly(e.currentTarget.checked)}
                                >
                                    Show Canceled Events Only:
                                </ToggleButton>
                            </Form.Group>
                        </Col>
                    </Form.Row>
                </Form>
            </div>
            <div>
                <div>
                    <UserEvents history={props.history} location={props.location} match={props.match} eventList={myEventList} eventTypeList={eventTypeList} isEventDataLoaded={isEventDataLoaded} onEventListChanged={handleReloadEvents} currentUser={currentUser} isUserLoaded={isUserLoaded} />
                </div>
            </div>
            <div>
                <AzureMapsProvider>
                    <>
                        <MapControllerPointCollection center={center} multipleEvents={myEventList} isEventDataLoaded={isEventDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} onAttendanceChanged={handleAttendanceChanged} myAttendanceList={myEventList} isUserEventDataLoaded={isEventDataLoaded} onDetailsSelected={handleDetailsSelected} />
                    </>
                </AzureMapsProvider>
            </div>
        </div>
    );
}

export default withRouter(MyDashboard);