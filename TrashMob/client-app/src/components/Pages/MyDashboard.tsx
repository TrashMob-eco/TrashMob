import { FC, useCallback, useEffect, useState } from 'react';
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import { Col, Container, Dropdown, Image, Row } from 'react-bootstrap';
import { data } from 'azure-maps-control';
import {
    Eye,
    PersonX,
    Link as LinkIcon,
    Pencil,
    FileEarmarkCheck,
    CheckSquare,
    XSquare,
    ArrowRightSquare,
    Share,
} from 'react-bootstrap-icons';
import { Guid } from 'guid-typescript';
import { useMutation, useQuery } from '@tanstack/react-query';
import EventData from '../Models/EventData';
import * as MapStore from '../../store/MapStore';
import MapControllerPointCollection from '../MapControllerPointCollection';
import UserData from '../Models/UserData';
import { Table } from '../Customization/Table';
import twofigure from '../assets/card/twofigure.svg';
import calendarclock from '../assets/card/calendarclock.svg';
import bucketplus from '../assets/card/bucketplus.svg';
import { PartnerStatusActive } from '../Models/Constants';
import DisplayPartnershipData from '../Models/DisplayPartnershipData';
import { getDisplayPartnershipStatus } from '../../store/displayPartnershipStatusHelper';
import PartnerRequestStatusData from '../Models/PartnerRequestStatusData';
import PartnerStatusData from '../Models/PartnerStatusData';
import DisplayPartnerAdminInvitationData from '../Models/DisplayPartnerAdminInvitationData';
import { PartnerLocationEventRequests } from '../Partners/PartnerLocationEventRequests';
import PickupLocationData from '../Models/PickupLocationData';
import { SocialsModal } from '../EventManagement/ShareToSocialsModal';
import { HeroSection } from '../Customization/HeroSection';
import * as SharingMessages from '../../store/SharingMessages';
import { DeleteEventAttendee, GetUserEvents } from '../../services/events';
import { Services } from '../../config/services.config';
import {
    AcceptPartnerAdminInvitation,
    DeclinePartnerAdminInvitation,
    GetPartnerAdminInvitationsByUser,
} from '../../services/invitations';
import { GetEventPickupLocationsByUser, PickupLocationMarkAsPickedUp } from '../../services/locations';
import { GetPartnerRequestByUserId, GetPartnerRequestStatuses, GetPartnerStatuses } from '../../services/partners';
import { GetPartnerAdminsForUser } from '../../services/admin';
import { GetStatsForUser } from '../../services/stats';

interface MyDashboardProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const MyDashboard: FC<MyDashboardProps> = (props) => {
    const [myEventList, setMyEventList] = useState<EventData[]>([]);
    const [partnerStatusList, setPartnerStatusList] = useState<PartnerStatusData[]>([]);
    const [partnerRequestStatusList, setPartnerRequestStatusList] = useState<PartnerRequestStatusData[]>([]);
    const [myPartnerRequests, setMyPartnerRequests] = useState<DisplayPartnershipData[]>([]);
    const [myPartners, setMyPartners] = useState<DisplayPartnershipData[]>([]);
    const [myPartnerAdminInvitations, setMyPartnerAdminInvitations] = useState<DisplayPartnerAdminInvitationData[]>([]);
    const [myPickupRequests, setMyPickupRequests] = useState<PickupLocationData[]>([]);
    const [isEventDataLoaded, setIsEventDataLoaded] = useState<boolean>(false);
    const [isPartnerAdminInvitationsDataLoaded, setIsPartnerAdminInvitationsDataLoaded] = useState<boolean>(false);
    const [isPickupRequestsDataLoaded, setIsPickupRequestsDataLoaded] = useState<boolean>(false);
    const [center, setCenter] = useState<data.Position>(
        new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude),
    );
    const [isMapKeyLoaded, setIsMapKeyLoaded] = useState<boolean>(false);
    const [mapOptions, setMapOptions] = useState<IAzureMapOptions>();
    const [currentUser, setCurrentUser] = useState<UserData>(props.currentUser);
    const [isUserLoaded, setIsUserLoaded] = useState<boolean>(props.isUserLoaded);
    const [reloadEvents, setReloadEvents] = useState<number>(0);
    const [upcomingEventsMapView, setUpcomingEventsMapView] = useState<boolean>(false);
    const [pastEventsMapView, setPastEventsMapView] = useState<boolean>(false);
    const [copied, setCopied] = useState(false);
    const [totalBags, setTotalBags] = useState<number>(0);
    const [totalHours, setTotalHours] = useState<number>(0);
    const [totalEvents, setTotalEvents] = useState<number>(0);
    const state = props.history.location.state as { newEventCreated: boolean };
    const [eventToShare, setEventToShare] = useState<EventData>();
    const [showModal, setShowSocialsModal] = useState<boolean>(false);

    const getUserEvents = useQuery({
        queryKey: GetUserEvents({ userId: props.currentUser.id }).key,
        queryFn: GetUserEvents({ userId: props.currentUser.id }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getPartnerAdminInvitationsByUser = useQuery({
        queryKey: GetPartnerAdminInvitationsByUser({
            userId: props.currentUser.id,
        }).key,
        queryFn: GetPartnerAdminInvitationsByUser({
            userId: props.currentUser.id,
        }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getEventPickupLocationsByUser = useQuery({
        queryKey: GetEventPickupLocationsByUser({
            userId: props.currentUser.id,
        }).key,
        queryFn: GetEventPickupLocationsByUser({ userId: props.currentUser.id }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getPartnerRequestByUserId = useQuery({
        queryKey: GetPartnerRequestByUserId({ userId: props.currentUser.id }).key,
        queryFn: GetPartnerRequestByUserId({ userId: props.currentUser.id }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getPartnerAdminsForUser = useQuery({
        queryKey: GetPartnerAdminsForUser({ userId: props.currentUser.id }).key,
        queryFn: GetPartnerAdminsForUser({ userId: props.currentUser.id }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getStatsForUser = useQuery({
        queryKey: GetStatsForUser({ userId: props.currentUser.id }).key,
        queryFn: GetStatsForUser({ userId: props.currentUser.id }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getPartnerRequestStatuses = useQuery({
        queryKey: GetPartnerRequestStatuses().key,
        queryFn: GetPartnerRequestStatuses().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getPartnerStatuses = useQuery({
        queryKey: GetPartnerStatuses().key,
        queryFn: GetPartnerStatuses().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const acceptPartnerAdminInvitation = useMutation({
        mutationKey: AcceptPartnerAdminInvitation().key,
        mutationFn: AcceptPartnerAdminInvitation().service,
    });

    const declinePartnerAdminInvitation = useMutation({
        mutationKey: DeclinePartnerAdminInvitation().key,
        mutationFn: DeclinePartnerAdminInvitation().service,
    });

    const deleteEventAttendee = useMutation({
        mutationKey: DeleteEventAttendee().key,
        mutationFn: DeleteEventAttendee().service,
    });

    const pickupLocationMarkAsPickedUp = useMutation({
        mutationKey: PickupLocationMarkAsPickedUp().key,
        mutationFn: PickupLocationMarkAsPickedUp().service,
    });

    useEffect(() => {
        window.scrollTo(0, 0);

        MapStore.getOption().then((opts) => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        });

        if ('geolocation' in navigator) {
            navigator.geolocation.getCurrentPosition((position) => {
                const point = new data.Position(position.coords.longitude, position.coords.latitude);
                setCenter(point);
            });
        } else {
            console.log('Not Available');
        }
    }, []);

    useEffect(() => {
        if (props.isUserLoaded) {
            setCurrentUser(props.currentUser);
            setIsUserLoaded(props.isUserLoaded);
            setIsEventDataLoaded(false);

            getUserEvents.refetch().then((res) => {
                setMyEventList(res.data?.data || []);
                setIsEventDataLoaded(true);
            });

            getPartnerAdminInvitationsByUser.refetch().then((res) => {
                setMyPartnerAdminInvitations(res.data?.data || []);
                setIsPartnerAdminInvitationsDataLoaded(true);
            });

            getEventPickupLocationsByUser.refetch().then((res) => {
                setMyPickupRequests(res.data?.data || []);
                setIsPickupRequestsDataLoaded(true);
            });

            getPartnerRequestStatuses.refetch().then((partnerRequestStatusesRes) => {
                setPartnerRequestStatusList(partnerRequestStatusesRes.data?.data || []);
                getPartnerRequestByUserId
                    .refetch()
                    .then((res) => {
                        if (res.data?.status !== 200) {
                            throw new Error('No Partner Requests found for this user');
                        }
                        setMyPartnerRequests(res.data.data || []);
                    })
                    .catch((err) => setMyPartnerRequests([]));
            });

            getPartnerStatuses.refetch().then((partnerStatusesRes) => {
                setPartnerStatusList(partnerStatusesRes.data?.data || []);
                getPartnerAdminsForUser
                    .refetch()
                    .then((res) => {
                        if (res.data?.status !== 200) throw new Error('No Partners found for this user');
                        setMyPartners(res.data.data || []);
                    })
                    .catch((err) => setMyPartners([]));
            });

            getStatsForUser.refetch().then((res) => {
                setTotalBags(res.data?.data.totalBags || 0);
                setTotalHours(res.data?.data.totalHours || 0);
                setTotalEvents(res.data?.data.totalEvents || 0);
            });
        }
    }, [reloadEvents, props.currentUser, props.currentUser.id, props.isUserLoaded]);

    const setSharingEvent = useCallback((newEventToShare: EventData, updateShowModal: boolean) => {
        setEventToShare(newEventToShare);

        handleShowModal(updateShowModal);
    }, []);

    useEffect(() => {
        if (state?.newEventCreated && isEventDataLoaded) {
            const myFilteredList = myEventList
                .filter((event) => event.createdByUserId === props.currentUser.id)
                .sort((a, b) => (a.createdDate < b.createdDate ? 1 : -1));

            setSharingEvent(myFilteredList[0], true);

            // replace state
            state.newEventCreated = false;
            props.history.replace({ ...props.history.location, state });
        }
    }, [state, isEventDataLoaded, props.currentUser.id, props.history, myEventList, setSharingEvent]);

    const handleLocationChange = (point: data.Position) => {
        // do nothing
    };

    const handleAttendanceChanged = (point: data.Position) => {
        // do nothing
    };

    const handleDetailsSelected = (eventId: string) => {
        props.history.push(`eventdetails/${eventId}`);
    };

    const handleReloadEvents = () => {
        // A trick to force the reload as needed.
        setReloadEvents(reloadEvents + 1);
    };

    const handleEventView = (view: string, table: string) => {
        if (table === 'Upcoming events') {
            if (view === 'list') {
                return setUpcomingEventsMapView(false);
            }
            return setUpcomingEventsMapView(true);
        }
        if (view === 'list') {
            return setPastEventsMapView(false);
        }
        return setPastEventsMapView(true);
    };

    const handleShowModal = (showModal: boolean) => {
        setShowSocialsModal(showModal);
    };

    const handleCopyLink = (eventId: string) => {
        navigator.clipboard.writeText(`${window.location.origin}/eventdetails/${eventId}`);
        setCopied(true);
        setTimeout(() => {
            setCopied(false);
        }, 2000);
    };

    const handleAcceptInvitation = (partnerAdminInvitationId: string) => {
        acceptPartnerAdminInvitation.mutateAsync({ invitationId: partnerAdminInvitationId }).then(() => {
            setIsPartnerAdminInvitationsDataLoaded(false);
            getPartnerAdminInvitationsByUser.refetch().then((partnerAdminInvitationRes) => {
                setMyPartnerAdminInvitations(partnerAdminInvitationRes.data?.data || []);
                setIsPartnerAdminInvitationsDataLoaded(true);
                getPartnerAdminsForUser
                    .refetch()
                    .then((partnerAdminRes) => {
                        if (partnerAdminRes.data?.status !== 200) {
                            throw new Error('No Partners found for this user');
                        }
                        setMyPartners(partnerAdminRes.data.data || []);
                    })
                    .catch((err) => setMyPartners([]));
            });
        });
    };

    const handleDeclineInvitation = (partnerAdminInvitationId: string) => {
        declinePartnerAdminInvitation.mutateAsync({ invitationId: partnerAdminInvitationId }).then(() => {
            setIsPartnerAdminInvitationsDataLoaded(false);
            getPartnerAdminInvitationsByUser.refetch().then((res) => {
                setMyPartnerAdminInvitations(res.data?.data || []);
                setIsPartnerAdminInvitationsDataLoaded(true);
            });
        });
    };

    const handleUnregisterEvent = (id: string, name: string) => {
        if (!window.confirm(`Do you want to remove yourself from this event: ${name}?`)) return;

        deleteEventAttendee.mutateAsync({ eventId: id, userId: props.currentUser.id }).then(() => {
            handleReloadEvents();
        });
    };

    const handleMarkAsPickedUp = (id: string) => {
        pickupLocationMarkAsPickedUp.mutateAsync({ locationId: id }).then(() => {
            getEventPickupLocationsByUser.refetch().then((res) => {
                setMyPickupRequests(res.data?.data || []);
                setIsPickupRequestsDataLoaded(true);
            });
        });
    };

    const attendeeActionDropdownList = (event: EventData) => (
        <>
            <Dropdown.Item href={`/eventdetails/${event.id}`}>
                <Eye />
                View event
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handleUnregisterEvent(event.id, props.currentUser.userName)}>
                <PersonX />
                Unregister for event
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handleCopyLink(event.id)}>
                <LinkIcon />
                {copied ? 'Copied!' : 'Copy event link'}
            </Dropdown.Item>
            <Dropdown.Item onClick={() => setSharingEvent(event, true)}>
                <Share />
                Share Event
            </Dropdown.Item>
        </>
    );

    const eventOwnerActionDropdownList = (event: EventData) => (
        <>
            <Dropdown.Item href={`/manageeventdashboard/${event.id}`}>
                <Pencil />
                Manage event
            </Dropdown.Item>
            <Dropdown.Item href={`/eventdetails/${event.id}`}>
                <Eye />
                View event
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handleCopyLink(event.id)}>
                <LinkIcon />
                {copied ? 'Copied!' : 'Copy event link'}
            </Dropdown.Item>
            <Dropdown.Item onClick={() => setSharingEvent(event, true)}>
                <Share />
                Share Event
            </Dropdown.Item>
            <Dropdown.Item href={`/cancelevent/${event.id}`}>
                <XSquare />
                Cancel event
            </Dropdown.Item>
        </>
    );

    const completedAttendeeActionDropdownList = (eventId: string) => (
        <>
            <Dropdown.Item href={`/eventdetails/${eventId}`}>
                <Eye />
                View event
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handleCopyLink(eventId)}>
                <LinkIcon />
                {copied ? 'Copied!' : 'Copy event link'}
            </Dropdown.Item>
        </>
    );

    const completedEventOwnerActionDropdownList = (eventId: string) => (
        <>
            <Dropdown.Item href={`/eventsummary/${eventId}`}>
                <FileEarmarkCheck />
                Event Summary
            </Dropdown.Item>
            <Dropdown.Item href={`/manageeventdashboard/${eventId}`}>
                <Pencil />
                Manage event
            </Dropdown.Item>
            <Dropdown.Item href={`/eventdetails/${eventId}`}>
                <Eye />
                View event
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handleCopyLink(eventId)}>
                <LinkIcon />
                {copied ? 'Copied!' : 'Copy event link'}
            </Dropdown.Item>
        </>
    );

    const partnerAdminInvitationsActionDropdownList = (partnerAdminInvitationId: string) => (
        <>
            <Dropdown.Item onClick={() => handleAcceptInvitation(partnerAdminInvitationId)}>
                <CheckSquare />
                Accept Invitation
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handleDeclineInvitation(partnerAdminInvitationId)}>
                <XSquare />
                Decline Invitation
            </Dropdown.Item>
        </>
    );

    const partnerRequestActionDropdownList = (partnerRequestId: string) => (
        <Dropdown.Item href={`/partnerRequestDetails/${partnerRequestId}`}>
            <Eye />
            View request form
        </Dropdown.Item>
    );

    const pickupRequestActionDropdownList = (pickupRequestId: string, eventId: string) => (
        <>
            <Dropdown.Item href={`/eventsummary/${eventId}`}>
                <FileEarmarkCheck />
                Event Summary
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handleMarkAsPickedUp(pickupRequestId)}>
                <CheckSquare />
                Marked picked up
            </Dropdown.Item>
        </>
    );

    const activePartnerActionDropdownList = (partnerId: string) => (
        <Dropdown.Item href={`/partnerdashboard/${partnerId}`}>
            <Pencil />
            Manage partnership
        </Dropdown.Item>
    );

    const inactivePartnerActionDropdownList = (partnerId: string) => (
        <Dropdown.Item href={`/partnerDashboard/${partnerId}`}>
            <ArrowRightSquare />
            Activate partnership
        </Dropdown.Item>
    );

    function UpcomingEventsTable() {
        const headerTitles = ['Name', 'Role', 'Date', 'Time', 'Location', 'Actions'];
        return (
            <div className='bg-white p-3 px-4'>
                <Table columnHeaders={headerTitles}>
                    {myEventList
                        .sort((a, b) => (a.eventDate < b.eventDate ? 1 : -1))
                        .filter(({ eventDate }) => new Date(eventDate) >= new Date())
                        .map((event) => (
                            <tr key={event.id.toString()}>
                                <td>{event.name}</td>
                                <td>{event.createdByUserId === props.currentUser.id ? 'Lead' : ' Attendee'}</td>
                                <td>
                                    {new Date(event.eventDate).toLocaleDateString('en-us', {
                                        year: 'numeric',
                                        month: '2-digit',
                                        day: '2-digit',
                                    })}
                                </td>
                                <td>
                                    {new Date(event.eventDate).toLocaleTimeString('en-us', {
                                        hour12: true,
                                        hour: 'numeric',
                                        minute: '2-digit',
                                    })}
                                </td>
                                <td>
                                    {event.streetAddress},{event.city}
                                </td>
                                <td className='btn py-0'>
                                    <Dropdown role='menuitem'>
                                        <Dropdown.Toggle id='share-toggle' variant='outline' className='h-100 border-0'>
                                            ...
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu id='share-menu'>
                                            {event.createdByUserId === props.currentUser.id
                                                ? eventOwnerActionDropdownList(event)
                                                : attendeeActionDropdownList(event)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        ))}
                </Table>
            </div>
        );
    }

    function PastEventsTable() {
        const headerTitles = ['Name', 'Role', 'Date', 'Time', 'Location', 'Actions'];
        return (
            <div className='bg-white p-3 px-4'>
                <Table columnHeaders={headerTitles}>
                    {myEventList
                        .sort((a, b) => (a.eventDate < b.eventDate ? 1 : -1))
                        .filter(({ eventDate }) => new Date(eventDate) < new Date())
                        .map(({ id, name, createdByUserId, eventDate, streetAddress, city }) => (
                            <tr key={id.toString()}>
                                <td>{name}</td>
                                <td>{createdByUserId === props.currentUser.id ? 'Lead' : ' Attendee'}</td>
                                <td>
                                    {new Date(eventDate).toLocaleDateString('en-us', {
                                        year: 'numeric',
                                        month: '2-digit',
                                        day: '2-digit',
                                    })}
                                </td>
                                <td>
                                    {new Date(eventDate).toLocaleTimeString('en-us', {
                                        hour12: true,
                                        hour: 'numeric',
                                        minute: '2-digit',
                                    })}
                                </td>
                                <td>
                                    {streetAddress},{city}
                                </td>
                                <td className='btn py-0'>
                                    <Dropdown role='menuitem'>
                                        <Dropdown.Toggle id='share-toggle' variant='outline' className='h-100 border-0'>
                                            ...
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu id='share-menu'>
                                            {createdByUserId === props.currentUser.id
                                                ? completedEventOwnerActionDropdownList(id)
                                                : completedAttendeeActionDropdownList(id)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        ))}
                </Table>
            </div>
        );
    }

    function MyPartnersTable() {
        const headerTitles = ['Name', 'Status', 'Actions'];
        if (myPartners) {
            return (
                <div className='bg-white p-3 px-4'>
                    <Table columnHeaders={headerTitles}>
                        {myPartners
                            .sort((a, b) => (a.name < b.name ? 1 : -1))
                            .map((displayPartner) => (
                                <tr key={displayPartner.id.toString()}>
                                    <td>{displayPartner.name}</td>
                                    <td>
                                        {getDisplayPartnershipStatus(
                                            partnerStatusList,
                                            partnerRequestStatusList,
                                            displayPartner.partnerStatusId,
                                            displayPartner.partnerRequestStatusId,
                                        )}
                                    </td>
                                    <td className='btn py-0'>
                                        <Dropdown role='menuitem'>
                                            <Dropdown.Toggle
                                                id='share-toggle'
                                                variant='outline'
                                                className='h-100 border-0'
                                            >
                                                ...
                                            </Dropdown.Toggle>
                                            <Dropdown.Menu id='share-menu'>
                                                {displayPartner.partnerStatusId === PartnerStatusActive
                                                    ? activePartnerActionDropdownList(displayPartner.id)
                                                    : inactivePartnerActionDropdownList(displayPartner.id)}
                                            </Dropdown.Menu>
                                        </Dropdown>
                                    </td>
                                </tr>
                            ))}
                    </Table>
                </div>
            );
        }
        return (
            <div className='bg-white p-3 px-4'>
                <Table columnHeaders={headerTitles}>
                    <></>
                </Table>
            </div>
        );
    }

    function MyPickupRequestsTable() {
        const headerTitles = ['Street Address', 'City', 'Notes', 'Actions'];
        if (isPickupRequestsDataLoaded && myPickupRequests) {
            return (
                <div className='bg-white p-3 px-4'>
                    <Table columnHeaders={headerTitles}>
                        {myPickupRequests.map((displayPickup) => (
                            <tr key={displayPickup.id.toString()}>
                                <td>{displayPickup.name}</td>
                                <td>{displayPickup.streetAddress}</td>
                                <td>{displayPickup.city}</td>
                                <td>{displayPickup.notes}</td>
                                <td className='btn py-0'>
                                    <Dropdown role='menuitem'>
                                        <Dropdown.Toggle id='share-toggle' variant='outline' className='h-100 border-0'>
                                            ...
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu id='share-menu'>
                                            {pickupRequestActionDropdownList(displayPickup.id, displayPickup.eventId)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        ))}
                    </Table>
                </div>
            );
        }
        return (
            <div className='bg-white p-3 px-4'>
                <Table columnHeaders={headerTitles}>
                    <></>
                </Table>
            </div>
        );
    }

    function MyPartnerRequestsTable() {
        const headerTitles = ['Name', 'Status', 'Actions'];
        if (myPartnerRequests) {
            return (
                <div className='bg-white p-3 px-4'>
                    <Table columnHeaders={headerTitles}>
                        {myPartnerRequests
                            .sort((a, b) => (a.name < b.name ? 1 : -1))
                            .map((displayPartner) => (
                                <tr key={displayPartner.id.toString()}>
                                    <td>{displayPartner.name}</td>
                                    <td>
                                        {getDisplayPartnershipStatus(
                                            partnerStatusList,
                                            partnerRequestStatusList,
                                            displayPartner.partnerStatusId,
                                            displayPartner.partnerRequestStatusId,
                                        )}
                                    </td>
                                    <td className='btn py-0'>
                                        <Dropdown role='menuitem'>
                                            <Dropdown.Toggle
                                                id='share-toggle'
                                                variant='outline'
                                                className='h-100 border-0'
                                            >
                                                ...
                                            </Dropdown.Toggle>
                                            <Dropdown.Menu id='share-menu'>
                                                {partnerRequestActionDropdownList(displayPartner.id)}
                                            </Dropdown.Menu>
                                        </Dropdown>
                                    </td>
                                </tr>
                            ))}
                    </Table>
                </div>
            );
        }
        return (
            <div className='bg-white p-3 px-4'>
                <Table columnHeaders={headerTitles}>
                    <></>
                </Table>
            </div>
        );
    }

    function PartnerAdminInvitationsTable() {
        const headerTitles = ['Partner Name', 'Actions'];
        if (isPartnerAdminInvitationsDataLoaded && myPartnerAdminInvitations) {
            return (
                <div className='bg-white p-3 px-4'>
                    <Table columnHeaders={headerTitles}>
                        {myPartnerAdminInvitations.map((displayInvitation) => (
                            <tr key={displayInvitation.id.toString()}>
                                <td>{displayInvitation.partnerName}</td>
                                <td className='btn py-0'>
                                    <Dropdown role='menuitem'>
                                        <Dropdown.Toggle id='share-toggle' variant='outline' className='h-100 border-0'>
                                            ...
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu id='share-menu'>
                                            {partnerAdminInvitationsActionDropdownList(displayInvitation.id)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        ))}
                    </Table>
                </div>
            );
        }
        return (
            <div className='bg-white p-3 px-4'>
                <Table columnHeaders={headerTitles}>
                    <></>
                </Table>
            </div>
        );
    }

    return (
        <>
            <HeroSection Title='Dashboard' Description="See how much you've done!" />
            <Container className='mt-5 pb-5'>
                {eventToShare ? (
                    <SocialsModal
                        eventToShare={eventToShare}
                        show={showModal}
                        handleShow={handleShowModal}
                        modalTitle='Share Event'
                        message={SharingMessages.getEventShareMessage(eventToShare, props.currentUser.id)}
                    />
                ) : null}
                <Row className='pt-5'>
                    <Col>
                        <div className='d-flex bg-white'>
                            <Col className='ml-3'>
                                <p className='card-title'>Events</p>
                                <p className='card-statistic color-primary mt-0'>{totalEvents}</p>
                            </Col>
                            <Col className='d-flex justify-content-end'>
                                <Image
                                    src={twofigure}
                                    alt='person silouhette icons'
                                    className='card-icon align-self-end mr-3'
                                />
                            </Col>
                        </div>
                    </Col>
                    <Col>
                        <div className='d-flex bg-white'>
                            <Col className='ml-3'>
                                <p className='card-title'>Hours</p>
                                <p className='card-statistic color-primary mt-0'>{totalHours}</p>
                            </Col>
                            <Col className='d-flex justify-content-end'>
                                <Image
                                    src={calendarclock}
                                    alt='calendar clock icons'
                                    className='card-icon align-self-end mr-3'
                                />
                            </Col>
                        </div>
                    </Col>
                    <Col>
                        <div className='d-flex bg-white'>
                            <Col className='ml-3'>
                                <p className='card-title'>Bags</p>
                                <p className='card-statistic color-primary mt-0'>{totalBags}</p>
                            </Col>
                            <Col className='d-flex justify-content-end'>
                                <Image
                                    src={bucketplus}
                                    alt='add bucket icons'
                                    className='card-icon align-self-end mr-3'
                                />
                            </Col>
                        </div>
                    </Col>
                </Row>
            </Container>
            <Container className='mb-5 pb-5'>
                <div className='d-flex my-5 mb-4 justify-content-between'>
                    <h4 className='font-weight-bold mr-2 pb-2 mt-0 active-line'>My Events ({myEventList.length})</h4>
                    <Link
                        className='d-flex align-items-center btn btn-primary banner-button'
                        to='/manageeventdashboard'
                    >
                        Create Event
                    </Link>
                </div>
                <div className='mb-4 bg-white'>
                    <div className='d-flex justify-content-between px-4'>
                        <p className='color-primary font-weight-bold pt-3'>
                            Upcoming events (
                            {myEventList.filter((event) => new Date(event.eventDate) >= new Date()).length})
                        </p>
                        <div className='d-flex align-items-center mt-4'>
                            <label className='mr-2'>
                                <input
                                    type='radio'
                                    className='mb-0 radio'
                                    name='Event view'
                                    value='list'
                                    onChange={(e) => handleEventView(e.target.value, 'Upcoming events')}
                                    checked={!upcomingEventsMapView}
                                />
                                <span className='px-2'>List view</span>
                            </label>
                            <label className='pr-3'>
                                <input
                                    type='radio'
                                    className='mb-0 radio'
                                    name='Event view'
                                    value='map'
                                    onChange={(e) => handleEventView(e.target.value, 'Upcoming events')}
                                    checked={upcomingEventsMapView}
                                />
                                <span className='px-2'>Map view</span>
                            </label>
                        </div>
                    </div>
                    {upcomingEventsMapView ? (
                        <AzureMapsProvider>
                            <MapControllerPointCollection
                                forceReload={false}
                                center={center}
                                multipleEvents={myEventList.filter((event) => new Date(event.eventDate) >= new Date())}
                                isEventDataLoaded={isEventDataLoaded}
                                mapOptions={mapOptions}
                                isMapKeyLoaded={isMapKeyLoaded}
                                eventName=''
                                latitude={0}
                                longitude={0}
                                onLocationChange={handleLocationChange}
                                currentUser={currentUser}
                                isUserLoaded={isUserLoaded}
                                onAttendanceChanged={handleAttendanceChanged}
                                myAttendanceList={myEventList}
                                isUserEventDataLoaded={isEventDataLoaded}
                                onDetailsSelected={handleDetailsSelected}
                                history={props.history}
                                location={props.location}
                                match={props.match}
                            />
                        </AzureMapsProvider>
                    ) : (
                        <UpcomingEventsTable />
                    )}
                </div>
                <div className='mb-4 bg-white'>
                    <div className='d-flex justify-content-between px-4'>
                        <p className='color-primary font-weight-bold pt-3'>
                            Past events ({myEventList.filter((event) => new Date(event.eventDate) < new Date()).length})
                        </p>
                        <div className='d-flex align-items-center mt-4'>
                            <label className='mr-2'>
                                <input
                                    type='radio'
                                    className='mb-0 radio'
                                    name='Past event view'
                                    value='list'
                                    onChange={(e) => handleEventView(e.target.value, 'Past events')}
                                    checked={!pastEventsMapView}
                                />
                                <span className='px-2'>List view</span>
                            </label>
                            <label className='pr-3'>
                                <input
                                    type='radio'
                                    className='mb-0 radio'
                                    name='Past event view'
                                    value='map'
                                    onChange={(e) => handleEventView(e.target.value, 'Past events')}
                                    checked={pastEventsMapView}
                                />
                                <span className='px-2'>Map view</span>
                            </label>
                        </div>
                    </div>
                    {pastEventsMapView ? (
                        <AzureMapsProvider>
                            <MapControllerPointCollection
                                forceReload={false}
                                center={center}
                                multipleEvents={myEventList.filter((event) => new Date(event.eventDate) < new Date())}
                                isEventDataLoaded={isEventDataLoaded}
                                mapOptions={mapOptions}
                                isMapKeyLoaded={isMapKeyLoaded}
                                eventName=''
                                latitude={0}
                                longitude={0}
                                onLocationChange={handleLocationChange}
                                currentUser={currentUser}
                                isUserLoaded={isUserLoaded}
                                onAttendanceChanged={handleAttendanceChanged}
                                myAttendanceList={myEventList}
                                isUserEventDataLoaded={isEventDataLoaded}
                                onDetailsSelected={handleDetailsSelected}
                                history={props.history}
                                location={props.location}
                                match={props.match}
                            />
                        </AzureMapsProvider>
                    ) : (
                        <PastEventsTable />
                    )}
                </div>
                <div className='d-flex my-5 mb-4 justify-content-between'>
                    <h4 className='font-weight-bold mr-2 mt-0 active-line pb-2'>
                        My Partnerships ({myPartnerRequests.length + myPartners.length})
                    </h4>
                    <Link className='d-flex align-items-center btn btn-primary banner-button' to='/inviteapartner'>
                        Send invitation to join TrashMob.eco as a partner
                    </Link>
                    <Link className='d-flex align-items-center btn btn-primary banner-button' to='/becomeapartner'>
                        Apply to become a partner
                    </Link>
                </div>
                <div className='mb-4 bg-white'>
                    <div className='d-flex justify-content-between px-4'>
                        <p className='color-primary font-weight-bold pt-3'>My Partners ({myPartners.length})</p>
                    </div>
                    <MyPartnersTable />
                </div>
                <div className='mb-4 bg-white'>
                    <div className='d-flex justify-content-between px-4'>
                        <p className='color-primary font-weight-bold pt-3'>
                            Partner Requests and Invitations Sent ({myPartnerRequests.length})
                        </p>
                    </div>
                    <MyPartnerRequestsTable />
                </div>
                <div className='mb-4 bg-white'>
                    <div className='d-flex justify-content-between px-4'>
                        <p className='color-primary font-weight-bold pt-3'>Partner Event Requests</p>
                    </div>
                    <PartnerLocationEventRequests
                        partnerLocationId={Guid.EMPTY}
                        currentUser={props.currentUser}
                        isUserLoaded={props.isUserLoaded}
                    />
                </div>
                <div className='mb-4 bg-white'>
                    <div className='d-flex justify-content-between px-4'>
                        <p className='color-primary font-weight-bold pt-3'>
                            Pickup Requests Pending ({myPickupRequests.length})
                        </p>
                    </div>
                    <MyPickupRequestsTable />
                </div>
                <div className='mb-4 bg-white'>
                    <div className='d-flex justify-content-between px-4'>
                        <p className='color-primary font-weight-bold pt-3'>
                            Partner Admin Invitations Pending ({myPartnerAdminInvitations.length})
                        </p>
                    </div>
                    <PartnerAdminInvitationsTable />
                </div>
            </Container>
        </>
    );
};

export default withRouter(MyDashboard);
