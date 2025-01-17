import { FC, useEffect, useState } from 'react';
import { RouteComponentProps, withRouter } from 'react-router';
import { APIProvider, Map } from '@vis.gl/react-google-maps';
import { Button, Container } from 'react-bootstrap';
import { Calendar, GeoAlt, Share, Stopwatch } from 'react-bootstrap-icons';
import AddToCalendar from '@culturehq/add-to-calendar';
import moment from 'moment';
import { useQuery } from '@tanstack/react-query';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
import EventTypeData from '../Models/EventTypeData';
import { getEventType } from '../../store/eventTypeHelper';
import * as MapStore from '../../store/MapStore';
import { MarkerWithInfoWindow, EventInfoWindowContent } from '../Map';
import { ShareToSocialsDialog } from '../EventManagement/ShareToSocialsDialog';
import { RegisterBtn } from '../Customization/RegisterBtn';
import { HeroSection } from '../Customization/HeroSection';
import * as SharingMessages from '../../store/SharingMessages';
import { GetAllEventsBeingAttendedByUser, GetEventAttendees, GetEventById, GetEventTypes } from '../../services/events';
import { Services } from '../../config/services.config';
import { useGetGoogleMapApiKey } from '../../hooks/useGetGoogleMapApiKey';

export interface DetailsMatchParams {
    eventId: string;
}

export interface EventDetailsProps extends RouteComponentProps<DetailsMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const EventDetails: FC<EventDetailsProps> = ({ match, currentUser, isUserLoaded, history, location }) => {
    const [isDataLoaded, setIsDataLoaded] = useState<boolean>(false);
    const [eventId, setEventId] = useState<string>(match.params.eventId);
    const [eventName, setEventName] = useState<string>('New Event');
    const [description, setDescription] = useState<string>('');
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
    const [center, setCenter] = useState<google.maps.LatLngLiteral>({
        lat: MapStore.defaultLongitude,
        lng: MapStore.defaultLatitude,
    });

    const [userList, setUserList] = useState<UserData[]>([]);
    const [createdById, setCreatedById] = useState<string>('');
    const [isAttending, setIsAttending] = useState<string>('No');
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
        location: `${streetAddress}, ${city}`,
        startsAt: moment(eventDate).format(),
        endsAt: moment(endDateTime).format(),
    };

    const getEventTypes = useQuery({
        queryKey: GetEventTypes().key,
        queryFn: GetEventTypes().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getEventAttendees = useQuery({
        queryKey: GetEventAttendees({ eventId }).key,
        queryFn: GetEventAttendees({ eventId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getEventById = useQuery({
        queryKey: GetEventById({ eventId }).key,
        queryFn: GetEventById({ eventId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getAllEventsBeingAttendedByUser = useQuery({
        queryKey: GetAllEventsBeingAttendedByUser({ userId: currentUser.id }).key,
        queryFn: GetAllEventsBeingAttendedByUser({ userId: currentUser.id }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    useEffect(() => {
        getEventTypes.refetch().then((res) => {
            setEventTypeList(res.data?.data || []);
        });

        if (eventId !== null) {
            getEventAttendees.refetch().then((res) => {
                setUserList(res.data?.data || []);
            });

            getEventById
                .refetch()
                .then((res) => {
                    if (res.data === undefined) return;
                    setEventId(res.data.data.id);
                    setEventName(res.data.data.name);
                    setDescription(res.data.data.description);
                    setEventDate(new Date(res.data.data.eventDate));
                    setDurationHours(res.data.data.durationHours);
                    setDurationMinutes(res.data.data.durationMinutes);
                    setEventTypeId(res.data.data.eventTypeId);
                    setStreetAddress(res.data.data.streetAddress);
                    setCity(res.data.data.city);
                    setCountry(res.data.data.country);
                    setRegion(res.data.data.region);
                    setPostalCode(res.data.data.postalCode);
                    setLatitude(res.data.data.latitude);
                    setLongitude(res.data.data.longitude);
                    setCreatedById(res.data.data.createdByUserId);
                    setMaxNumberOfParticipants(res.data.data.maxNumberOfParticipants);
                    setCenter({ lat: res.data.data.latitude, lng: res.data.data.longitude });
                    setEventToShare(res.data.data);
                    setIsDataLoaded(true);
                    setIsEventCompleted(new Date(res.data.data.eventDate) < new Date());
                })
                .then(() => {
                    if (!isUserLoaded || !currentUser) return;
                    getAllEventsBeingAttendedByUser.refetch().then((eventsBeingAttendedRes) => {
                        setMyAttendanceList(eventsBeingAttendedRes.data?.data || []);
                        setIsUserEventDataLoaded(true);
                    });
                });
        }
    }, [eventId, currentUser, isUserLoaded]);

    useEffect(() => {
        if (!isUserLoaded || !currentUser) {
            return;
        }

        const attending = myAttendanceList && myAttendanceList.findIndex((e) => e.id === eventId) >= 0;
        setIsAttending(attending ? 'Yes' : 'No');
    }, [isUserLoaded, currentUser, eventId, myAttendanceList, isUserEventDataLoaded]);

    useEffect(() => {
        getEventAttendees.refetch().then((res) => {
            setUserList(res.data?.data || []);
        });
    }, [eventId, myAttendanceList]);

    const handleShowModal = (showModal: boolean) => {
        setShowSocialsModal(showModal);
    };

    function UsersTable() {
        return (
            <div className='overflow-auto'>
                <table className='table table-striped' aria-labelledby='tableLabel'>
                    <thead>
                        <tr className='bg-ice'>
                            <th>User Name</th>
                            <th>City</th>
                            <th>Country</th>
                            <th>Member Since</th>
                        </tr>
                    </thead>
                    <tbody>
                        {userList.map((user) => {
                            let uName = user.userName;
                            if (user.id === createdById) {
                                uName += ' (Lead)';
                            }

                            return (
                                <tr key={user.id.toString()}>
                                    <td>{uName}</td>
                                    <td>{user.city}</td>
                                    <td>{user.country}</td>
                                    <td>{new Date(user.memberSince).toLocaleDateString()}</td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            </div>
        );
    }

    return (
        <div>
            <HeroSection Title='View Events' Description='Learn, join, and inspire.' />
            {!isDataLoaded ? (
                <p>
                    <em>Loding...</em>
                </p>
            ) : (
                <>
                    <Container className='my-5'>
                        <ShareToSocialsDialog
                            eventToShare={eventToShare}
                            show={showModal}
                            handleShow={handleShowModal}
                            modalTitle='Share Event'
                            message={SharingMessages.getEventDetailsMessage(
                                eventDate,
                                city,
                                createdById,
                                currentUser.id,
                            )}
                        />
                        <div className='d-flex justify-content-between align-items-md-end flex-column flex-md-row'>
                            <h2 className='font-weight-bold m-0'>{eventName}</h2>
                            <div className='d-flex my-3 tailwind'>
                                <RegisterBtn
                                    eventId={eventId}
                                    isAttending={isAttending}
                                    isEventCompleted={isEventCompleted!}
                                    currentUser={currentUser}
                                    isUserLoaded={isUserLoaded}
                                />
                                <div id='addToCalendarBtn' className='ml-2 p-18' hidden={isEventCompleted}>
                                    <AddToCalendar event={event} />
                                </div>
                                <Button
                                    variant='outline'
                                    className='p-18'
                                    onClick={() => {
                                        handleShowModal(true);
                                    }}
                                >
                                    <Share className='mr-2' />
                                    Share
                                </Button>
                            </div>
                        </div>
                        <p className='mt-2 color-grey'>{getEventType(eventTypeList, eventTypeId)}</p>
                        <p className='mt-4 color-grey'>{description}</p>
                        <p>
                            <Calendar size={24} className='mr-2' />
                            <span> {moment(startDateTime).local().format('L')}</span>
                        </p>
                        <p>
                            <Stopwatch size={24} className='mr-2' />
                            <span> {moment(startDateTime).local().format('LT')}</span>
                        </p>
                        <p>
                            <GeoAlt size={24} className='mr-2' />
                            <a
                                href={`https://google.com/maps/place/${streetAddress}+${city}+${region}+${postalCode}+${country}`}
                                target='_blank'
                                rel='noopener noreferrer'
                            >
                                {streetAddress},{city},{region} -{postalCode} {country}
                            </a>
                        </p>

                        <Map
                            mapId='6f295631d841c617'
                            gestureHandling='greedy'
                            disableDefaultUI
                            style={{ width: '100%', height: '500px' }}
                            defaultCenter={center}
                            defaultZoom={MapStore.defaultUserLocationZoom}
                        >
                            <MarkerWithInfoWindow
                                position={{ lat: latitude, lng: longitude }}
                                infoWindowTrigger='hover'
                                infoWindowProps={{ headerDisabled: true }}
                                infoWindowContent={
                                    <EventInfoWindowContent
                                        title={eventName}
                                        date={moment(startDateTime).local().format('LL')}
                                        time={moment(startDateTime).local().format('LTS Z')}
                                    />
                                }
                            />
                        </Map>
                    </Container>
                    <Container>
                        <hr />
                        <h2 className='font-weight-bold font-size-xl mr-2 mt-5 mb-4'>
                            <span>Attendees ({userList.length})</span>
                        </h2>
                        <p className='font-weight-bold m-0 mr-2 my-4'>
                            Max Number of Participants:
                            <span className='ml-2 color-grey'>{maxNumberOfParticipants}</span>
                        </p>
                        <UsersTable />
                    </Container>
                </>
            )}
        </div>
    );
};

const EventDetailWrapper = (props: EventDetailsProps) => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();

    if (isLoading) return null;

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <EventDetails {...props} />
        </APIProvider>
    );
};

export default withRouter(EventDetailWrapper);
