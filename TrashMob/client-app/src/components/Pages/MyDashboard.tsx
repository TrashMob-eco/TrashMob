import { FC, useCallback, useEffect, useState } from 'react';
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { APIProvider } from '@vis.gl/react-google-maps';
import { Guid } from 'guid-typescript';
import { useMutation, useQuery } from '@tanstack/react-query';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
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
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';

import { Ellipsis, Eye, Pencil, SquareCheck, SquareX, Plus, SquareArrowRight, FileCheck } from 'lucide-react';
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

    function MyPartnersTable() {
        const headerTitles = ['Name', 'Status', 'Actions'];
        return (
            <div className='overflow-auto'>
                <Table>
                    <TableHeader>
                        <TableRow>
                            {headerTitles.map((header) => (
                                <TableHead key={header}>{header}</TableHead>
                            ))}
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {(myPartners || [])
                            .sort((a, b) => (a.name < b.name ? 1 : -1))
                            .map((displayPartner) => (
                                <TableRow key={displayPartner.id.toString()}>
                                    <TableCell>{displayPartner.name}</TableCell>
                                    <TableCell>
                                        {getDisplayPartnershipStatus(
                                            partnerStatusList,
                                            partnerRequestStatusList,
                                            displayPartner.partnerStatusId,
                                            displayPartner.partnerRequestStatusId,
                                        )}
                                    </TableCell>
                                    <TableCell className='py-0'>
                                        <DropdownMenu>
                                            <DropdownMenuTrigger asChild>
                                                <Button variant='ghost' size='icon'>
                                                    <Ellipsis />
                                                </Button>
                                            </DropdownMenuTrigger>
                                            <DropdownMenuContent className='w-56'>
                                                {displayPartner.partnerStatusId === PartnerStatusActive ? (
                                                    <DropdownMenuItem asChild>
                                                        <Link to={`/partnerdashboard/${displayPartner.id}`}>
                                                            <Pencil />
                                                            <span>Manage partnership</span>
                                                        </Link>
                                                    </DropdownMenuItem>
                                                ) : (
                                                    <DropdownMenuItem asChild>
                                                        <Link to={`/partnerdashboard/${displayPartner.id}`}>
                                                            <SquareArrowRight />
                                                            <span>Activate Partnership</span>
                                                        </Link>
                                                    </DropdownMenuItem>
                                                )}
                                            </DropdownMenuContent>
                                        </DropdownMenu>
                                    </TableCell>
                                </TableRow>
                            ))}
                    </TableBody>
                </Table>
            </div>
        );
    }

    function MyPickupRequestsTable() {
        const headerTitles = ['Street Address', 'City', 'Notes', 'Actions'];
        return (
            <div className='overflow-auto'>
                <Table>
                    <TableHeader>
                        <TableRow>
                            {headerTitles.map((header) => (
                                <TableHead key={header}>{header}</TableHead>
                            ))}
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {(myPickupRequests || []).map((displayPickup) => (
                            <TableRow key={displayPickup.id.toString()}>
                                <TableCell>{displayPickup.name}</TableCell>
                                <TableCell>{displayPickup.streetAddress}</TableCell>
                                <TableCell>{displayPickup.city}</TableCell>
                                <TableCell>{displayPickup.notes}</TableCell>
                                <TableCell className='py-0'>
                                    <DropdownMenu>
                                        <DropdownMenuTrigger asChild>
                                            <Button variant='ghost' size='icon'>
                                                <Ellipsis />
                                            </Button>
                                        </DropdownMenuTrigger>
                                        <DropdownMenuContent className='w-56'>
                                            <DropdownMenuItem asChild>
                                                <Link to={`/eventsummary/${displayPickup.eventId}`}>
                                                    <FileCheck />
                                                    <span>Event Summary</span>
                                                </Link>
                                            </DropdownMenuItem>
                                            <DropdownMenuItem onClick={() => handleMarkAsPickedUp(displayPickup.id)}>
                                                <SquareCheck />
                                                <span>Marked picked up</span>
                                            </DropdownMenuItem>
                                        </DropdownMenuContent>
                                    </DropdownMenu>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </div>
        );
    }

    function MyPartnerRequestsTable() {
        const headerTitles = ['Name', 'Status', 'Actions'];
        return (
            <div className='overflow-auto'>
                <Table>
                    <TableHeader>
                        <TableRow>
                            {headerTitles.map((header) => (
                                <TableHead key={header}>{header}</TableHead>
                            ))}
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {(myPartnerRequests || [])
                            .sort((a, b) => (a.name < b.name ? 1 : -1))
                            .map((displayPartner) => (
                                <TableRow key={displayPartner.id.toString()}>
                                    <TableCell>{displayPartner.name}</TableCell>
                                    <TableCell>
                                        {getDisplayPartnershipStatus(
                                            partnerStatusList,
                                            partnerRequestStatusList,
                                            displayPartner.partnerStatusId,
                                            displayPartner.partnerRequestStatusId,
                                        )}
                                    </TableCell>
                                    <TableCell className='py-0'>
                                        <DropdownMenu>
                                            <DropdownMenuTrigger asChild>
                                                <Button variant='ghost' size='icon'>
                                                    <Ellipsis />
                                                </Button>
                                            </DropdownMenuTrigger>
                                            <DropdownMenuContent className='w-56'>
                                                <DropdownMenuItem asChild>
                                                    <Link to={`/partnerRequestDetails/${displayPartner.id}`}>
                                                        <Eye />
                                                        View request form
                                                    </Link>
                                                </DropdownMenuItem>
                                            </DropdownMenuContent>
                                        </DropdownMenu>
                                    </TableCell>
                                </TableRow>
                            ))}
                    </TableBody>
                </Table>
            </div>
        );
    }

    function PartnerAdminInvitationsTable() {
        const headerTitles = ['Partner Name', 'Actions'];
        return (
            <div className='overflow-auto'>
                <Table>
                    <TableHeader>
                        <TableRow>
                            {headerTitles.map((header) => (
                                <TableHead key={header}>{header}</TableHead>
                            ))}
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {(myPartnerAdminInvitations || []).map((displayInvitation) => (
                            <TableRow key={displayInvitation.id.toString()}>
                                <TableCell>{displayInvitation.partnerName}</TableCell>
                                <TableCell className='py-0'>
                                    <DropdownMenu>
                                        <DropdownMenuTrigger asChild>
                                            <Button variant='ghost' size='icon'>
                                                <Ellipsis />
                                            </Button>
                                        </DropdownMenuTrigger>
                                        <DropdownMenuContent className='w-56'>
                                            <DropdownMenuItem
                                                onClick={() => handleAcceptInvitation(displayInvitation.id)}
                                            >
                                                <SquareCheck />
                                                Accept Invitation
                                            </DropdownMenuItem>
                                            <DropdownMenuItem
                                                onClick={() => handleDeclineInvitation(displayInvitation.id)}
                                            >
                                                <SquareX />
                                                Decline Invitation
                                            </DropdownMenuItem>
                                        </DropdownMenuContent>
                                    </DropdownMenu>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
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
                    <h4 className='font-bold !mr-2 !pb-2 !mt-0 border-b-[3px] border-primary flex items-center w-full'>
                        <div className='grow'>My Events ({myEventList.length})</div>
                        <Button asChild>
                            <Link to='/manageeventdashboard'>
                                <Plus /> Create Event
                            </Link>
                        </Button>
                    </h4>
                </div>
            </div>
            <div className='container mb-5 pb-5'>
                <Card className='mb-4'>
                    <CardHeader>
                        <div className='flex flex-row'>
                            <CardTitle className='grow text-primary'>
                                Upcoming events (
                                {myEventList.filter((event) => new Date(event.eventDate) >= new Date()).length})
                            </CardTitle>
                            <RadioGroup
                                value={upcomingEventsMapView ? 'map' : 'list'}
                                onValueChange={(value) => handleEventView(value, 'Upcoming events')}
                                className='grid-cols-2'
                                orientation='horizontal'
                            >
                                <div className='flex items-center space-x-2'>
                                    <RadioGroupItem value='list' id='upcomingevents-list' />
                                    <Label htmlFor='upcomingevents-list'>List view</Label>
                                </div>
                                <div className='flex items-center space-x-2'>
                                    <RadioGroupItem value='map' id='upcomingevents-map' />
                                    <Label htmlFor='upcomingevents-map'>Map view</Label>
                                </div>
                            </RadioGroup>
                        </div>
                    </CardHeader>
                    <CardContent>
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
                    </CardContent>
                </Card>

                <Card className='mb-4'>
                    <CardHeader>
                        <div className='flex flex-row'>
                            <CardTitle className='grow text-primary'>
                                Past events (
                                {myEventList.filter((event) => new Date(event.eventDate) < new Date()).length})
                            </CardTitle>
                            <RadioGroup
                                value={pastEventsMapView ? 'map' : 'list'}
                                onValueChange={(value) => handleEventView(value, 'Past events')}
                                className='grid-cols-2'
                                orientation='horizontal'
                            >
                                <div className='flex items-center space-x-2'>
                                    <RadioGroupItem value='list' id='pastevents-list' />
                                    <Label htmlFor='pastevents-list'>List view</Label>
                                </div>
                                <div className='flex items-center space-x-2'>
                                    <RadioGroupItem value='map' id='pastevents-map' />
                                    <Label htmlFor='pastevents-map'>Map view</Label>
                                </div>
                            </RadioGroup>
                        </div>
                    </CardHeader>
                    <CardContent>
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
                    </CardContent>
                </Card>
                <div className='flex flex-column mt-5 mb-3'>
                    <h4 className='font-semibold mr-2 mt-0 pb-2 border-b-[3px] border-primary'>
                        My Partnerships ({myPartnerRequests.length + myPartners.length})
                    </h4>
                    <div className='flex flex-row flex-wrap gap-4 !my-4'>
                        <Button variant='outline' asChild>
                            <Link to='/inviteapartner'>Send invitation to join TrashMob.eco as a partner</Link>
                        </Button>
                        <Button variant='outline' asChild>
                            <Link to='/becomeapartner'>Apply to become a partner</Link>
                        </Button>
                    </div>
                </div>
                <Card className='mb-4'>
                    <CardHeader>
                        <CardTitle className='text-primary'>My Partners ({myPartners.length})</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <MyPartnersTable />
                    </CardContent>
                </Card>

                <Card className='mb-4'>
                    <CardHeader>
                        <CardTitle className='text-primary'>
                            Partner Requests and Invitations Sent ({myPartnerRequests.length})
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <MyPartnerRequestsTable />
                    </CardContent>
                </Card>

                <Card className='mb-4'>
                    <CardHeader>
                        <CardTitle className='text-primary'>Partner Event Requests</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <PartnerLocationEventRequests
                            partnerLocationId={Guid.EMPTY}
                            currentUser={props.currentUser}
                            isUserLoaded={props.isUserLoaded}
                        />
                    </CardContent>
                </Card>

                <Card className='mb-4'>
                    <CardHeader>
                        <CardTitle className='text-primary'>
                            Pickup Requests Pending ({myPickupRequests.length})
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <MyPickupRequestsTable />
                    </CardContent>
                </Card>

                <Card className='mb-4'>
                    <CardHeader>
                        <CardTitle className='text-primary'>
                            Partner Admin Invitations Pending ({myPartnerAdminInvitations.length})
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <PartnerAdminInvitationsTable />
                    </CardContent>
                </Card>
            </div>
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
