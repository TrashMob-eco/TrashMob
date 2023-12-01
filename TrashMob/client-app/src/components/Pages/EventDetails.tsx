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
import { Button, Container } from 'react-bootstrap';
import { Calendar, GeoAlt, Share, Stopwatch } from 'react-bootstrap-icons';
import { SocialsModal } from '../EventManagement/ShareToSocialsModal';
import MapControllerSinglePoint from '../MapControllerSinglePoint';
import AddToCalendar from '@culturehq/add-to-calendar';
import moment from 'moment';
import { RegisterBtn } from '../Customization/RegisterBtn';
import { HeroSection } from '../Customization/HeroSection'
import * as SharingMessages from '../../store/SharingMessages';

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
            <div className="overflow-auto">
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
            </div>
        );
    };

    const renderEvent = () => {
        return (
            <>
                <Container className="my-5">
                    {isDataLoaded &&
                        <SocialsModal eventToShare={eventToShare} show={showModal} handleShow={handleShowModal} modalTitle='Share Event' message={SharingMessages.getEventDetailsMessage(eventDate, city, createdById, currentUser.id)} />
                    }
                    <div className="d-flex justify-content-between align-items-md-end flex-column flex-md-row">
                        <h2 className="font-weight-bold m-0">{eventName}</h2>
                        <div className="d-flex my-3">
                            <Button variant="outline" className="p-18" onClick={() => { handleShowModal(true) }}>
                                <Share className="mr-2" />
                                Share
                            </Button>
                            <RegisterBtn eventId={eventId} isAttending={isAttending} isEventCompleted={isEventCompleted!} currentUser={currentUser} onAttendanceChanged={handleAttendanceChanged} isUserLoaded={isUserLoaded} history={history} location={location} match={match}></RegisterBtn>
                            <div id="addToCalendarBtn" className='ml-2 p-18' hidden={isEventCompleted}><AddToCalendar event={event} /></div>
                        </div>
                    </div>
                    <p className='mt-2 color-grey'>{getEventType(eventTypeList, eventTypeId)}</p>
                    <p className='mt-4 color-grey'>{description}</p>
                    <p><Calendar size={24} className="mr-2" /><span> {moment(startDateTime).local().format('L')}</span></p>
                    <p><Stopwatch size={24} className="mr-2" /><span> {moment(startDateTime).local().format('LT')}</span></p>
                    <p><GeoAlt size={24} className="mr-2" /><a href={`https://google.com/maps/place/${streetAddress}+${city}+${region}+${postalCode}+${country}`} target="_blank" rel="noopener noreferrer">{streetAddress}, {city}, {region} - {postalCode} {country}</a></p>

                    <AzureMapsProvider>
                        <MapControllerSinglePoint center={center} isEventDataLoaded={isDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={eventName} eventDate={eventDate} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} isDraggable={false} />
                    </AzureMapsProvider>

                </Container>
                <Container>
                    <hr></hr>
                    <h2 className="font-weight-bold font-size-xl mr-2 mt-5 mb-4"><span>Attendees ({userList.length})</span></h2>
                    <p className="font-weight-bold m-0 mr-2 my-4">Max Number of Participants:<span className="ml-2 color-grey">{maxNumberOfParticipants}</span></p>
                    <UsersTable />
                </Container>
            </>
        )
    }

    const contents = isDataLoaded
        ? renderEvent()
        : <p><em>Loading...</em></p>;

    return <div>
        <HeroSection Title='View Events' Description='Learn, join, and inspire.'></HeroSection>
        {contents}
    </div>;
}
