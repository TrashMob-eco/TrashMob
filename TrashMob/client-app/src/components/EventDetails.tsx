import * as React from 'react'
import { RouteComponentProps } from 'react-router';
import EventData from './Models/EventData';
import UserData from './Models/UserData';
import EventTypeData from './Models/EventTypeData';
import { getDefaultHeaders } from '../store/AuthStore';
import { getEventType } from '../store/eventTypeHelper';
import { data } from 'azure-maps-control';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import { Container, Dropdown } from 'react-bootstrap';
import MapControllerSinglePoint from './MapControllerSinglePoint';
import AddToCalendar from '@culturehq/add-to-calendar';
import moment from 'moment';
import { Calendar, Facebook, GeoAlt, Link, Share, Stopwatch, Twitter } from 'react-bootstrap-icons';
import { RegisterBtn } from './RegisterBtn';

export interface DetailsMatchParams {
    eventId: string;
}

export interface EventDetailsProps extends RouteComponentProps<DetailsMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const EventDetails: React.FC<EventDetailsProps> = (props) => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [eventId, setEventId] = React.useState<string>(props.match.params["eventId"]);
    const [eventName, setEventName] = React.useState<string>("New Event");
    const [description, setDescription] = React.useState<string>("");
    const [eventDate, setEventDate] = React.useState<Date>(new Date());
    const [durationHours, setDurationHours] = React.useState<number>(1);
    const [durationMinutes, setDurationMinutes] = React.useState<number>(0);
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
    const [createdById, setCreatedById] = React.useState<string>("");
    const [copied, setCopied] = React.useState(false);

    const startDateTime = moment(eventDate);
    const endDateTime = moment(startDateTime).add(durationHours, 'hours').add(durationMinutes, 'minutes');

    const event = {
        name: eventName,
        details: description,
        location: streetAddress + ', ' + city,
        startsAt: moment(eventDate).format(),
        endsAt: moment(endDateTime).format()
    }

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
                    setDurationHours(eventData.durationHours);
                    setDurationMinutes(eventData.durationMinutes);
                    setEventTypeId(eventData.eventTypeId);
                    setStreetAddress(eventData.streetAddress);
                    setCity(eventData.city);
                    setCountry(eventData.country);
                    setRegion(eventData.region);
                    setPostalCode(eventData.postalCode);
                    setLatitude(eventData.latitude);
                    setLongitude(eventData.longitude);
                    setCreatedById(eventData.createdByUserId);
                    const shareMessage = "Help clean up Planet Earth! Sign up for this TrashMob.eco event in " + eventData.city + ", " + eventData.region + " on " + (new Date(eventData.eventDate)).toLocaleDateString() + "! via @TrashMobEco";
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

    const handleLocationChange = (point: data.Position) => {
        // do nothing
    }

    const handleCopyLink = () => {
        navigator.clipboard.writeText(window.location.href);
        setCopied(true);
        setTimeout(() => {
            setCopied(false);
        }, 5000)
    }

    const UsersTable = () => {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr className="bg-ice">
                            <th>User Name</th>
                            <th>City</th>
                            <th>Country</th>
                            <th>Member Since</th>
                        </tr>
                    </thead>
                    <tbody>
                        {userList.map(user => {
                            var uName = user.userName ? user.userName : user.sourceSystemUserName;
                            if (user.id === createdById) {
                                uName += " (Lead)";
                            }

                            return (
                                <tr key={user.id.toString()}>
                                    <td>{uName}</td>
                                    <td>{user.city}</td>
                                    <td>{user.country}</td>
                                    <td>{new Date(user.memberSince).toLocaleDateString()}</td>
                                </tr>
                            )
                        }
                        )}
                    </tbody>
                </table>
            </div>
        );
    };

    const renderEvent = () => {
        return (
            <>
                <AzureMapsProvider>
                    <MapControllerSinglePoint center={center} isEventDataLoaded={isDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={eventName} eventDate={eventDate} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} isDraggable={false} />
                </AzureMapsProvider>
                <Container>
                    <div className="d-flex justify-content-between align-items-end">
                        <h4 className="font-weight-bold">{eventName}</h4>
                        <div className="d-flex">
                            <div id="addToCalendarBtn"><AddToCalendar event={event} /></div>
                            <Dropdown role="menuitem">
                                <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100"><Share className="mr-2" aria-hidden="true" />Share</Dropdown.Toggle>
                                <Dropdown.Menu id="share-menu">
                                    <Dropdown.Item className="share-link" onClick={handleCopyLink}><Link className="mr-2" aria-hidden="true" />{!copied ? "Copy link" : "Copied!"}</Dropdown.Item>
                                    <Dropdown.Item className="share-link"><Facebook className="mr-2" aria-hidden="true" /><a target="_blank" rel="noopener noreferrer" href={facebookUrl} className="fb-xfbml-parse-ignore">Share to Facebook</a></Dropdown.Item>
                                    <Dropdown.Item className="share-link"><Twitter className="mr-2" aria-hidden="true" /><a target="_blank" rel="noopener noreferrer" href={twitterUrl} className="twitter-share-button">Share to Twitter</a></Dropdown.Item>
                                </Dropdown.Menu>
                            </Dropdown>
                            <RegisterBtn eventId={eventId} isAttending={isAttending} currentUser={currentUser} onAttendanceChanged={onAttendanceChanged} isUserLoaded={isUserLoaded}></RegisterBtn>
                        </div>
                    </div>
                    <span className="my-2 event-list-event-type p-2 rounded d-block">{getEventType(eventTypeList, eventTypeId)}</span>
                    <p>{description}</p>
                    <p><Calendar className="mr-2" />{moment(startDateTime).local().format('L')}</p>
                    <p><Stopwatch className="mr-2" />{moment(startDateTime).local().format('LT')}</p>
                    <p><GeoAlt className="mr-2" /><a href={`https://google.com/maps/place/${streetAddress}+${city}+${region}+${postalCode}+${country}`}>{streetAddress}, {city}, {region} - {postalCode} {country}</a></p>
                    <div className="d-flex">
                        <span className="font-weight-bold mr-2">Latitude:</span>
                        <span className="mr-5">{latitude}</span>
                        <span className="font-weight-bold mr-2">Longitude:</span>
                        <span>{longitude}</span>
                    </div>
                    <div hidden={maxNumberOfParticipants === 0}>
                        <span className="font-weight-bold mr-2">Max Number of Participants:</span>
                        <span>{maxNumberOfParticipants}</span>
                    </div>
                </Container>
                <Container>
                    <h5 className="font-weight-bold mr-2 mt-5 mb-4 text-decoration-underline">Attendees ({userList.length})</h5>
                    <UsersTable />
                </Container>
            </>
        )
    }

    const contents = isDataLoaded
        ? renderEvent()
        : <p><em>Loading...</em></p>;

    return <div>
        <h3>Event Details</h3>
        <hr />
        {contents}
    </div>;
}
