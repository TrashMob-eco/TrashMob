import { FC, useEffect, useState } from 'react'
import { RouteComponentProps } from 'react-router';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
import EventTypeData from '../Models/EventTypeData';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import { getEventType } from '../../store/eventTypeHelper';
import { data } from 'azure-maps-control';
import * as MapStore from '../../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import { Col, Container, Dropdown, Image, Row } from 'react-bootstrap';
import MapControllerSinglePoint from '../MapControllerSinglePoint';
import AddToCalendar from '@culturehq/add-to-calendar';
import moment from 'moment';
import { Calendar, Facebook, GeoAlt, Link, Share, Stopwatch, Twitter } from 'react-bootstrap-icons';
import { RegisterBtn } from '../RegisterBtn';
import globes from '../assets/gettingStarted/globes.png';

export interface DetailsMatchParams {
    eventId: string;
}

export interface EventDetailsProps extends RouteComponentProps<DetailsMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
    onAttendanceChanged: () => void;
    myAttendanceList: EventData[];
    handleUpdateAttendanceList: (data: EventData[]) => void;
    isUserEventDataLoaded: boolean;
    handleUpdateIsUserEventDataLoaded: (status: boolean) => void;
    isAttending: string;
    handleUpdateIsAttending: (status: string) => void;
}

export const EventDetails: FC<EventDetailsProps> = ({ match, currentUser, isUserLoaded, handleUpdateAttendanceList,
    handleUpdateIsUserEventDataLoaded, myAttendanceList, handleUpdateIsAttending, onAttendanceChanged, isAttending }) => {
    const [isDataLoaded, setIsDataLoaded] = useState<boolean>(false);
    const [eventId, setEventId] = useState<string>(match.params["eventId"]);
    const [eventName, setEventName] = useState<string>("New Event");
    const [description, setDescription] = useState<string>("");
    const [eventDate, setEventDate] = useState<Date>(new Date());
    const [durationHours, setDurationHours] = useState<number>(1);
    const [durationMinutes, setDurationMinutes] = useState<number>(0);
    const [eventTypeId, setEventTypeId] = useState<number>(0);
    const [streetAddress, setStreetAddress] = useState<string>();
    const [city, setCity] = useState<string>();
    const [country, setCountry] = useState<string>();
    const [region, setRegion] = useState<string>();
    const [postalCode, setPostalCode] = useState<string>();
    const [latitude, setLatitude] = useState<number>(0);
    const [longitude, setLongitude] = useState<number>(0);
    const [maxNumberOfParticipants, setMaxNumberOfParticipants] = useState<number>(0);
    const [eventTypeList, setEventTypeList] = useState<EventTypeData[]>([]);
    const [center, setCenter] = useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = useState<boolean>(false);
    const [mapOptions, setMapOptions] = useState<IAzureMapOptions>();
    const [userList, setUserList] = useState<UserData[]>([]);
    const [twitterUrl, setTwitterUrl] = useState<string>();
    const [facebookUrl, setFacebookUrl] = useState<string>();
    const [createdById, setCreatedById] = useState<string>("");
    const [copied, setCopied] = useState(false);

    const startDateTime = moment(eventDate);
    const endDateTime = moment(startDateTime).add(durationHours, 'hours').add(durationMinutes, 'minutes');

    const event = {
        name: eventName,
        details: description,
        location: streetAddress + ', ' + city,
        startsAt: moment(eventDate).format(),
        endsAt: moment(endDateTime).format()
    }

    useEffect(() => {
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
            var eventUrl = "https://www.trashmob.eco/eventdetails/" + eventId;

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
                    const shareMessage = "Help clean up Planet Earth! Sign up for this TrashMob.eco event in " + eventData.city + ", " + eventData.region + " on " + (new Date(eventData.eventDate)).toLocaleDateString() + "! via @TrashMobEco " + eventUrl;
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
    }, [eventId]);

    useEffect(() => {
        if (!isUserLoaded || !currentUser) {
            return;
        }

        // If the user is logged in, get the events they are attending
        const accounts = msalClient.getAllAccounts();

        if (accounts !== null && accounts.length > 0) {
            const request = {
                scopes: apiConfig.b2cScopes,
                account: accounts[0]
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/events/eventsuserisattending/' + currentUser.id, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<EventData[]>)
                    .then(data => {
                        handleUpdateAttendanceList(data);
                        handleUpdateIsUserEventDataLoaded(true);

                        const attending = myAttendanceList && (myAttendanceList.findIndex((e) => e.id === eventId) >= 0);
                        const isAttendingStatus = (attending ? 'Yes' : 'No');
                        handleUpdateIsAttending(isAttendingStatus);
                    })
            });
        }
    }, [isUserLoaded, currentUser, eventId]);

    useEffect(() => {
        const headers = getDefaultHeaders('GET');
        fetch('/api/eventattendees/' + eventId, {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<UserData[]>)
            .then(data => {
                setUserList(data);
            });
    }, [eventId])

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
                        let uName = user.userName ? user.userName : user.sourceSystemUserName;
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
        <Container fluid className='bg-grass'>
            <Row className="text-center pt-0">
                <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                    <h1 className='font-weight-bold'>Event Details</h1>
                    <p className="font-weight-bold">Learn, join, and inspire.</p>
                </Col>
                <Col md={5}>
                    <Image src={globes} alt="globes" className="h-100 mt-0" />
                </Col>
            </Row>
        </Container>
        {contents}
    </div>;
}
