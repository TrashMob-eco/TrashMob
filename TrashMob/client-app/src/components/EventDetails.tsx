import * as React from 'react'
import { RouteComponentProps } from 'react-router';
import EventData from './Models/EventData';
import UserData from './Models/UserData';
import EventTypeData from './Models/EventTypeData';
import { getDefaultHeaders } from '../store/AuthStore';
import { getEventType } from '../store/eventTypeHelper';
import { data } from 'azure-maps-control';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from '@ambientlight/react-azure-maps';
import MapController from './MapController';
import { Col, Form } from 'react-bootstrap';

export interface MatchParams {
    eventId: string;
}

export interface EventDetailsProps extends RouteComponentProps<MatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const EventDetails: React.FC<EventDetailsProps> = (props) => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [eventId, setEventId] = React.useState<string>(props.match.params["eventId"]);
    const [eventName, setEventName] = React.useState<string>("New Event");
    const [description, setDescription] = React.useState<string>();
    const [eventDate, setEventDate] = React.useState<Date>(new Date());
    const [eventTypeId, setEventTypeId] = React.useState<number>(0);
    const [streetAddress, setStreetAddress] = React.useState<string>();
    const [city, setCity] = React.useState<string>();
    const [country, setCountry] = React.useState<string>();
    const [region, setRegion] = React.useState<string>();
    const [postalCode, setPostalCode] = React.useState<string>();
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [maxNumberOfParticipants, setMaxNumberOfParticipants] = React.useState<number>(0);
    const [eventTypeList, setEventTypeList] = React.useState<EventTypeData[]>([]);
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [userList, setUserList] = React.useState<UserData[]>([]);
    const [currentUser, setCurrentUser] = React.useState<UserData>(props.currentUser);
    const [isUserLoaded, setIsUserLoaded] = React.useState<boolean>(props.isUserLoaded);
    const [eventUrl, setEventUrl] = React.useState<string>();
    const [twitterUrl, setTwitterUrl] = React.useState<string>();
    const [facebookUrl, setFacebookUrl] = React.useState<string>();

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

        if (eventId != null) {
            setEventUrl("https://www.trashmob.eco/eventdetails/" + eventId);

            fetch('/api/eventattendees/' + eventId, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<UserData[]>)
                .then(data => {
                    setUserList(data);
                });

            fetch('/api/Events/' + eventId, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<EventData>)
                .then(eventData => {
                    setEventId(eventData.id);
                    setEventName(eventData.name);
                    setDescription(eventData.description);
                    setEventDate(new Date(eventData.eventDate));
                    setEventTypeId(eventData.eventTypeId);
                    setStreetAddress(eventData.streetAddress);
                    setCity(eventData.city);
                    setCountry(eventData.country);
                    setRegion(eventData.region);
                    setPostalCode(eventData.postalCode);
                    setLatitude(eventData.latitude);
                    setLongitude(eventData.longitude);
                    var shareMessage = "Help clean up Planet Earth! Sign up for this TrashMob.eco event in " + eventData.city + ", " + eventData.region + " on " + (new Date(eventData.eventDate)).toLocaleDateString() +"! via @TrashMobEco";
                    setTwitterUrl("https://twitter.com/intent/tweet?text=" + encodeURI(shareMessage) + "&ref_src=twsrc%5Etfw");
                    setFacebookUrl("https://www.facebook.com/sharer/sharer.php?u=https%3A%2F%2Fwww.trashmob.eco%2Feventdetails%2" + eventId + "&amp;src=sdkpreparse");
                    setMaxNumberOfParticipants(eventData.maxNumberOfParticipants);
                    setCenter(new data.Position(eventData.longitude, eventData.latitude));
                    setIsDataLoaded(true);
                });
        }

        MapStore.getOption().then(opts => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        })
    }, [eventId, eventUrl]);

    React.useEffect(() => {
        setCurrentUser(props.currentUser);
        setIsUserLoaded(props.isUserLoaded);
    }, [props.currentUser, props.isUserLoaded])

    function handleLocationChange(point: data.Position) {
        // do nothing
    }

    function renderUsersTable(users: UserData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>User Name</th>
                            <th>City</th>
                            <th>Country</th>
                            <th>Member Since</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map(user =>
                            <tr key={user.id.toString()}>
                                <td>{user.userName ? user.userName : user.sourceSystemUserName}</td>
                                <td>{user.city}</td>
                                <td>{user.country}</td>
                                <td>{new Date(user.memberSince).toLocaleDateString()}</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderEvent() {

        return (
            <div>
                <Form>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <div><a target="_blank" rel="noopener noreferrer" href={twitterUrl} className="twitter-share-button" data-size="large">Share on Twitter</a></div>
                                <div className="fb-share-button" data-href={eventUrl} data-layout="button" data-size="small"><a target="_blank" rel="noreferrer" href={facebookUrl} className="fb-xfbml-parse-ignore">Share</a></div>
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="Name">Name:</Form.Label>
                                <Form.Control disabled type="text" defaultValue={eventName} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="EventDate">EventDate:</Form.Label>
                                <Form.Control disabled type="text" defaultValue={eventDate.toLocaleString()} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="EventType">Event Type:</Form.Label>
                                <Form.Control disabled type="text" defaultValue={getEventType(eventTypeList, eventTypeId)} />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="Description">Description:</Form.Label>
                                <Form.Control disabled as="textarea" defaultValue={description} rows={5} cols={5} />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="StreetAddress">Street Address:</Form.Label>
                                <Form.Control disabled type="text" defaultValue={streetAddress} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="City">City:</Form.Label>
                                <Form.Control disabled type="text" defaultValue={city} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="postalCode">Postal Code:</Form.Label>
                                <Form.Control disabled type="text" defaultValue={postalCode} />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="stateProvince">Region:</Form.Label>
                                <Form.Control disabled type="text" defaultValue={region} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="Country">Country:</Form.Label>
                                <Form.Control disabled type="text" defaultValue={country} />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="Latitude">Latitude:</Form.Label>
                                <Form.Control disabled type="text" defaultValue={latitude} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="Longitude">Longitude:</Form.Label>
                                <Form.Control disabled type="text" defaultValue={longitude} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="MaxNumberOfParticipants">Max Number Of Participants:</Form.Label>
                                <Form.Control disabled type="text" defaultValue={maxNumberOfParticipants} />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                </Form >
                <div>
                    <h2>Attendees</h2>
                    {renderUsersTable(userList)}
                </div>
                <div>
                    <AzureMapsProvider>
                        <>
                            <MapController center={center} multipleEvents={[]} isEventDataLoaded={isDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={eventName} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} />
                        </>
                    </AzureMapsProvider>
                </div>
            </div>
        )
    }

    let contents = isDataLoaded
        ? renderEvent()
        : <p><em>Loading...</em></p>;

    return <div>
        <h3>Event Details</h3>
        <hr />
        {contents}
    </div>;
}
