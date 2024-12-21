import { FC, useCallback, useEffect, useState } from 'react';
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { APIProvider } from '@vis.gl/react-google-maps';
import { Col, Container, Dropdown, Image, Row } from 'react-bootstrap';
import {
    PersonX,
    Link as LinkIcon,
    FileEarmarkCheck,
    CheckSquare,
    XSquare,
    ArrowRightSquare,
    Share,
} from 'react-bootstrap-icons';
import { Guid } from 'guid-typescript';
import { useMutation, useQuery } from '@tanstack/react-query';
import EventData from '../Models/EventData';
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
import { ShareToSocialsDialog } from '../EventManagement/ShareToSocialsDialog';
import { HeroSection } from '../Customization/HeroSection';
import * as SharingMessages from '../../store/SharingMessages';
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
import { useGetGoogleMapApiKey } from '../../hooks/useGetGoogleMapApiKey';
import { EventsMap } from '../Map';
import { Button } from '@/components/ui/button';

import { Eye, Pencil } from 'lucide-react';
import { EventsTable } from '../EventsTable/EventsTable';
import { useGetUserEvents } from '@/hooks/useGetUserEvents';

const isUpcomingEvent = (event: EventData) => new Date(event.eventDate) >= new Date();
const isPastEvent = (event: EventData) => new Date(event.eventDate) < new Date();

interface MyDashboardProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const MyDashboard: FC<MyDashboardProps> = (props) => {
    const { isUserLoaded, currentUser } = props;
    const [partnerStatusList, setPartnerStatusList] = useState<PartnerStatusData[]>([]);
    const [partnerRequestStatusList, setPartnerRequestStatusList] = useState<PartnerRequestStatusData[]>([]);
    const [myPartnerRequests, setMyPartnerRequests] = useState<DisplayPartnershipData[]>([]);
    const [myPartners, setMyPartners] = useState<DisplayPartnershipData[]>([]);
    const [myPartnerAdminInvitations, setMyPartnerAdminInvitations] = useState<DisplayPartnerAdminInvitationData[]>([]);
    const [myPickupRequests, setMyPickupRequests] = useState<PickupLocationData[]>([]);
    const [isEventDataLoaded, setIsEventDataLoaded] = useState<boolean>(false);
    const [isPartnerAdminInvitationsDataLoaded, setIsPartnerAdminInvitationsDataLoaded] = useState<boolean>(false);
    const [isPickupRequestsDataLoaded, setIsPickupRequestsDataLoaded] = useState<boolean>(false);
    const [upcomingEventsMapView, setUpcomingEventsMapView] = useState<boolean>(false);
    const [pastEventsMapView, setPastEventsMapView] = useState<boolean>(false);
    const [totalBags, setTotalBags] = useState<number>(0);
    const [totalHours, setTotalHours] = useState<number>(0);
    const [totalEvents, setTotalEvents] = useState<number>(0);
    const state = props.history.location.state as { newEventCreated: boolean };
    const [eventToShare, setEventToShare] = useState<EventData>();
    const [showModal, setShowSocialsModal] = useState<boolean>(false);

    const { data: userEvents } = useGetUserEvents(currentUser.id);
    const myEventList = userEvents || [];
    const upcomingEvents = myEventList.filter(isUpcomingEvent);
    const pastEvents = myEventList.filter(isPastEvent);

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

    const pickupLocationMarkAsPickedUp = useMutation({
        mutationKey: PickupLocationMarkAsPickedUp().key,
        mutationFn: PickupLocationMarkAsPickedUp().service,
    });

    useEffect(() => {
        window.scrollTo(0, 0);
    }, []);

    useEffect(() => {
        if (props.isUserLoaded) {
            setIsEventDataLoaded(false);

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
    }, [props.currentUser, props.currentUser.id, props.isUserLoaded]);

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

    const handleMarkAsPickedUp = (id: string) => {
        pickupLocationMarkAsPickedUp.mutateAsync({ locationId: id }).then(() => {
            getEventPickupLocationsByUser.refetch().then((res) => {
                setMyPickupRequests(res.data?.data || []);
                setIsPickupRequestsDataLoaded(true);
            });
        });
    };

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

    function MyPartnersTable() {
        const headerTitles = ['Name', 'Status', 'Actions'];
        if (myPartners) {
            return (
                <div className='bg-white p-3 px-4 overflow-auto'>
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
            <div className='bg-white p-3 px-4 overflow-auto'>
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
                <div className='bg-white p-3 px-4 overflow-auto'>
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
            <div className='bg-white p-3 px-4 overflow-auto'>
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
                <div className='bg-white p-3 px-4 overflow-auto'>
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
            <div className='bg-white p-3 px-4 overflow-auto'>
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
                <div className='bg-white p-3 px-4 overflow-auto'>
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
            <div className='bg-white p-3 px-4 overflow-auto'>
                <Table columnHeaders={headerTitles}>
                    <></>
                </Table>
            </div>
        );
    }

    return (
        <div className='tailwind'>
            <HeroSection Title='Dashboard' Description="See how much you've done!" />
            <div className='container !mt-12 !pb-12'>
                {eventToShare ? (
                    <ShareToSocialsDialog
                        eventToShare={eventToShare}
                        show={showModal}
                        handleShow={handleShowModal}
                        modalTitle='Share Event'
                        message={SharingMessages.getEventShareMessage(eventToShare, props.currentUser.id)}
                    />
                ) : null}
                <div className='!pt-12 flex flex-row flex-wrap gap-8 justify-center'>
                    {[
                        { name: 'Events', value: totalEvents, img: twofigure },
                        { name: 'Hours', value: totalHours, img: calendarclock },
                        { name: 'Bags', value: totalBags, img: bucketplus },
                    ].map((stat) => (
                        <div
                            className='basis-full md:basis-[200px] md:max-w-[255px] md:grow bg-card !px-7 relative'
                            key={stat.name}
                        >
                            <p className='text-[25px] font-medium !mt-6 !mb-3'>{stat.name}</p>
                            <p className='text-primary !mt-0 text-[55px]'>{stat.value}</p>
                            <img
                                src={stat.img}
                                alt={stat.name}
                                className='absolute right-8 bottom-0 w-[95px] h-[95px]'
                            />
                        </div>
                    ))}
                </div>
            </div>
            <div className='container !my-12'>
                <div className='flex justify-between'>
                    <h4 className='font-bold !mr-2 !pb-2 !mt-0 border-b-[3px] border-primary'>
                        My Events ({myEventList.length})
                    </h4>
                    <Button asChild size='lg'>
                        <Link to='/manageeventdashboard'>Create Event</Link>
                    </Button>
                </div>
            </div>
            <Container className='mb-5 pb-5'>
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
                        <EventsMap
                            id='upcomingEventsMap'
                            events={upcomingEvents}
                            isUserLoaded={isUserLoaded}
                            currentUser={currentUser}
                        />
                    ) : (
                        <EventsTable events={upcomingEvents} currentUser={currentUser} />
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
                        <EventsMap
                            id='pastEventsMap'
                            events={pastEvents}
                            isUserLoaded={isUserLoaded}
                            currentUser={currentUser}
                        />
                    ) : (
                        <EventsTable events={pastEvents} currentUser={currentUser} />
                    )}
                </div>
                <div className='d-flex flex-column mt-5 mb-3'>
                    <h4 className='font-weight-bold mr-2 mt-0 active-line pb-2'>
                        My Partnerships ({myPartnerRequests.length + myPartners.length})
                    </h4>
                    <Link
                        className='d-flex align-items-center btn btn-primary banner-button mx-auto mr-sm-auto ml-sm-0 mt-2'
                        to='/inviteapartner'
                    >
                        Send invitation to join TrashMob.eco as a partner
                    </Link>
                    <Link
                        className='d-flex align-items-center btn btn-primary banner-button mx-auto mr-sm-auto ml-sm-0 mt-2'
                        to='/becomeapartner'
                    >
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
        </div>
    );
};

const MyDashboardWrapper = (props: MyDashboardProps) => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();

    if (isLoading) return null;

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <MyDashboard {...props} />
        </APIProvider>
    );
};

export default withRouter(MyDashboardWrapper);
