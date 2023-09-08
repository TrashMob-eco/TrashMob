import { FC, useEffect, useState } from 'react'
import { RouteComponentProps } from 'react-router';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
import EventTypeData from '../Models/EventTypeData';
import { getApiConfig, getDefaultHeaders, msalClient, validateToken } from '../../store/AuthStore';
import { getEventType } from '../../store/eventTypeHelper';
import { data } from 'azure-maps-control';
import * as MapStore from '../../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import { Button, Col, Container, Image, Row } from 'react-bootstrap';
import MapControllerSinglePoint from '../MapControllerSinglePoint';
import AddToCalendar from '@culturehq/add-to-calendar';
import moment from 'moment';
import { Calendar, GeoAlt, Share, Stopwatch } from 'react-bootstrap-icons';
import { RegisterBtn } from '../RegisterBtn';
import globes from '../assets/gettingStarted/globes.png';
import { SocialsModal } from '../EventManagement/ShareToSocialsModal';

export interface DetailsMatchParams {
    eventId: string;
}

export interface EventDetailsProps extends RouteComponentProps<DetailsMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const EventDetails: FC<EventDetailsProps> = ({ match, currentUser, isUserLoaded, history, location }) => {
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
    const [createdById, setCreatedById] = useState<string>("");
    const [isAttending, setIsAttending] = useState<string>("No");
    const [myAttendanceList, setMyAttendanceList] = useState<EventData[]>([]);
    const [isUserEventDataLoaded, setIsUserEventDataLoaded] = useState(false);
    const [isEventCompleted, setIsEventCompleted] = useState<boolean>();
    const [showModal, setShowSocialsModal] = useState<boolean>(false);
    const [eventToShare, setEventToShare] = useState<EventData>();

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
        window.scrollTo(0, 0);

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
                    setMaxNumberOfParticipants(eventData.maxNumberOfParticipants);
                    setCenter(new data.Position(eventData.longitude, eventData.latitude));
                    setEventToShare(eventData)
                    setIsDataLoaded(true);
                    setIsEventCompleted(new Date(eventData.eventDate) < new Date());
                })
                .then(() => {
                    if (!isUserLoaded || !currentUser) {
                        return;
                    }

                    // If the user is logged in, get the events they are attending
                    const accounts = msalClient.getAllAccounts();
                    var apiConfig = getApiConfig();

                    if (accounts !== null && accounts.length > 0) {
                        const request = {
                            scopes: apiConfig.b2cScopes,
                            account: accounts[0]
                        };

                        msalClient.acquireTokenSilent(request).then(tokenResponse => {
                            if (!validateToken(tokenResponse.idTokenClaims)) {
                                return;
                            }

                            const headers = getDefaultHeaders('GET');
                            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                            fetch('/api/events/eventsuserisattending/' + currentUser.id, {
                                method: 'GET',
                                headers: headers
                            })
                                .then(response => response.json() as Promise<EventData[]>)
                                .then(data => {
                                    setMyAttendanceList(data);
                                    setIsUserEventDataLoaded(true);
                                })
                        });
                    }
                })

        }

        MapStore.getOption().then(opts => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        })
    }, [eventId, currentUser, isUserLoaded]);

    useEffect(() => {
        if (!isUserLoaded || !currentUser) {
            return;
        }

        const attending = myAttendanceList && (myAttendanceList.findIndex((e) => e.id === eventId) >= 0);
        setIsAttending(attending ? 'Yes' : 'No');
    }, [isUserLoaded, currentUser, eventId, myAttendanceList, isUserEventDataLoaded]);

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
    }, [eventId, myAttendanceList])

    const handleAttendanceChanged = () => {
        if (!isUserLoaded || !currentUser) {
            return;
        }

        // If the user is logged in, get the events they are attending
        const accounts = msalClient.getAllAccounts();
        var apiConfig = getApiConfig();

        if (accounts !== null && accounts.length > 0) {
            const request = {
                scopes: apiConfig.b2cScopes,
                account: accounts[0]
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                if (!validateToken(tokenResponse.idTokenClaims)) {
                    return;
                }

                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/events/eventsuserisattending/' + currentUser.id, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<EventData[]>)
                    .then(data => {
                        setMyAttendanceList(data);
                        setIsUserEventDataLoaded(true);
                    })
            });
        }
    }

    const handleLocationChange = (point: data.Position) => {
        // do nothing
    }

    const handleShowModal = (showModal: boolean) => {
        setShowSocialsModal(showModal)
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
                        let uName = user.userName;
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
                { isDataLoaded &&
                    <SocialsModal eventToShare={eventToShare} show={showModal} handleShow={handleShowModal} currentUserID={currentUser.id}/>
                }
                <Container className="my-5">
                    <div className="d-flex justify-content-between align-items-end">
                        <h1 className="font-weight-bold m-0">{eventName}</h1>
                        <div className="d-flex">
                            <div id="addToCalendarBtn" className='p-18' hidden={isEventCompleted}><AddToCalendar event={event} /></div>
                            <Button variant="outline" className="h-100 p-18" onClick={() => {handleShowModal(true)}}>
                                <Share className="mr-2" />
                                Share 
                            </Button>
                            <RegisterBtn eventId={eventId} isAttending={isAttending} isEventCompleted={isEventCompleted!} currentUser={currentUser} onAttendanceChanged={handleAttendanceChanged} isUserLoaded={isUserLoaded} history={history} location={location} match={match}></RegisterBtn>
                        </div>
                    </div>
                    <span className="my-2 event-list-event-type p-2 rounded d-block p-15">{getEventType(eventTypeList, eventTypeId)}</span>
                    <p className='mt-4 color-grey'>{description}</p>
                    <p><Calendar size={24} className="mr-2" /><span className='color-grey'> {moment(startDateTime).local().format('L')}</span></p>
                    <p><Stopwatch size={24} className="mr-2" /><span className='color-grey'> {moment(startDateTime).local().format('LT')}</span></p>
                    <p><GeoAlt size={24} className="mr-2" /><a href={`https://google.com/maps/place/${streetAddress}+${city}+${region}+${postalCode}+${country}`} target="_blank" rel="noopener noreferrer">{streetAddress}, {city}, {region} - {postalCode} {country}</a></p>
                    <div className="d-flex align-items-center">
                        <h5 className="font-weight-bold m-0 mr-2">Latitude</h5>
                        <span className="mr-5 color-grey p-18">{latitude}</span>
                        <h5 className="font-weight-bold m-0 mr-2">Longitude</h5>
                        <span className='color-grey p-18'>{longitude}</span>
                    </div>

                    <div hidden={maxNumberOfParticipants === 0} className="my-4">
                        <h5 className="font-weight-bold m-0 mr-2">Max Number of Participants</h5>
                        <span className="mr-5 color-grey p-18">{maxNumberOfParticipants}</span>
                    </div>
                </Container>
                <Container>
                    <h5 className="font-weight-bold font-size-xl mr-2 mt-5 mb-4"><span className='active-line'>Attendees ({userList.length})</span></h5>
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
                    <h1 className='font-weight-bold'>View Events</h1>
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
