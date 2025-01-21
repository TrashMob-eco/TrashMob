import { FC, useCallback, useEffect, useState } from 'react';
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { APIProvider } from '@vis.gl/react-google-maps';
import { Guid } from 'guid-typescript';
import { useQuery } from '@tanstack/react-query';
import EventData from '@/components/Models/EventData';
import UserData from '@/components/Models/UserData';
import twofigure from '@/components/assets/card/twofigure.svg';
import calendarclock from '@/components/assets/card/calendarclock.svg';
import bucketplus from '@/components/assets/card/bucketplus.svg';
import StatsData from '@/components/Models/StatsData';
import DisplayPartnershipData from '@/components/Models/DisplayPartnershipData';
import DisplayPartnerAdminInvitationData from '@/components/Models/DisplayPartnerAdminInvitationData';
import { PartnerLocationEventRequests } from '@/components/Partners/PartnerLocationEventRequests';
import { ShareToSocialsDialog } from '@/components/EventManagement/ShareToSocialsDialog';
import { HeroSection } from '@/components/Customization/HeroSection';
import * as SharingMessages from '@/store/SharingMessages';
import { GetStatsForUser } from '@/services/stats';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import { EventsMap } from '@/components/Map';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';

import { Plus } from 'lucide-react';
import { EventsTable } from '@/components/events/event-table';
import { useGetUserEvents } from '@/hooks/useGetUserEvents';
import { MyPartnersTable } from '@/pages/mydashboard/MyPartnersTable';
import { MyPickupRequestsTable } from '@/pages/mydashboard/MyPickupRequestsTable';
import { MyPartnersRequestTable } from '@/pages/mydashboard/MyPartnersRequestTable';
import { PartnerAdminInvitationsTable } from '@/pages/mydashboard/PartnerAdminInvitationsTable';
import { AxiosResponse } from 'axios';
import { GetPartnerRequestByUserId } from '@/services/partners';
import { GetPartnerAdminsForUser } from '@/services/admin';
import { GetPartnerAdminInvitationsByUser } from '@/services/invitations';
import { GetEventPickupLocationsByUser } from '@/services/locations';
import PickupLocationData from '@/components/Models/PickupLocationData';

const isUpcomingEvent = (event: EventData) => new Date(event.eventDate) >= new Date();
const isPastEvent = (event: EventData) => new Date(event.eventDate) < new Date();

interface MyDashboardProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const MyDashboard: FC<MyDashboardProps> = (props) => {
    const { isUserLoaded, currentUser } = props;
    const userId = currentUser.id;

    const [upcomingEventsMapView, setUpcomingEventsMapView] = useState<boolean>(false);
    const [pastEventsMapView, setPastEventsMapView] = useState<boolean>(false);
    const state = props.history.location.state as { newEventCreated: boolean };
    const [eventToShare, setEventToShare] = useState<EventData>();
    const [showModal, setShowSocialsModal] = useState<boolean>(false);

    const { data: userEvents } = useGetUserEvents(userId);
    const myEventList = userEvents || [];
    const upcomingEvents = myEventList.filter(isUpcomingEvent);
    const pastEvents = myEventList.filter(isPastEvent);

    const { data: myPartnerRequests } = useQuery<
        AxiosResponse<DisplayPartnershipData[]>,
        unknown,
        DisplayPartnershipData[]
    >({
        queryKey: GetPartnerRequestByUserId({ userId }).key,
        queryFn: GetPartnerRequestByUserId({ userId }).service,
        select: (res) => res.data,
    });

    const { data: myPartners } = useQuery<AxiosResponse<DisplayPartnershipData[]>, unknown, DisplayPartnershipData[]>({
        queryKey: GetPartnerAdminsForUser({ userId }).key,
        queryFn: GetPartnerAdminsForUser({ userId }).service,
        select: (res) => res.data,
    });

    const { data: myPartnerAdminInvitations } = useQuery<
        AxiosResponse<DisplayPartnerAdminInvitationData[]>,
        unknown,
        DisplayPartnerAdminInvitationData[]
    >({
        queryKey: GetPartnerAdminInvitationsByUser({ userId }).key,
        queryFn: GetPartnerAdminInvitationsByUser({ userId }).service,
        select: (res) => res.data,
    });

    const { data: myPickupRequests } = useQuery<AxiosResponse<PickupLocationData[]>, unknown, PickupLocationData[]>({
        queryKey: GetEventPickupLocationsByUser({ userId }).key,
        queryFn: GetEventPickupLocationsByUser({ userId }).service,
        select: (res) => res.data,
    });

    // getStatsForUser
    const { data: stats } = useQuery<AxiosResponse<StatsData>, unknown, StatsData>({
        queryKey: GetStatsForUser({ userId: currentUser.id }).key,
        queryFn: GetStatsForUser({ userId: currentUser.id }).service,
        select: (res) => res.data,
    });

    const totalBags = stats?.totalBags || 0;
    const totalHours = stats?.totalHours || 0;
    const totalEvents = stats?.totalEvents || 0;

    const setSharingEvent = useCallback((newEventToShare: EventData, updateShowModal: boolean) => {
        setEventToShare(newEventToShare);
        setShowSocialsModal(updateShowModal);
    }, []);

    useEffect(() => {
        if (state?.newEventCreated) {
            const myFilteredList = myEventList
                .filter((event) => event.createdByUserId === props.currentUser.id)
                .sort((a, b) => (a.createdDate < b.createdDate ? 1 : -1));

            setSharingEvent(myFilteredList[0], true);

            // replace state
            state.newEventCreated = false;
            props.history.replace({ ...props.history.location, state });
        }
    }, [state, props.currentUser.id, props.history, myEventList, setSharingEvent]);

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

    return (
        <div className='tailwind'>
            <HeroSection Title='Dashboard' Description="See how much you've done!" />
            <div className='container !mt-12 !pb-12'>
                {eventToShare ? (
                    <ShareToSocialsDialog
                        eventToShare={eventToShare}
                        show={showModal}
                        handleShow={setShowSocialsModal}
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
                        My Partnerships ({(myPartnerRequests || []).length + (myPartners || []).length})
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
                        <CardTitle className='text-primary'>My Partners ({(myPartners || []).length})</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className='overflow-auto'>
                            <MyPartnersTable items={myPartners || []} />
                        </div>
                    </CardContent>
                </Card>

                <Card className='mb-4'>
                    <CardHeader>
                        <CardTitle className='text-primary'>
                            Partner Requests and Invitations Sent ({(myPartnerRequests || []).length})
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className='overflow-auto'>
                            <MyPartnersRequestTable items={myPartnerRequests || []} />
                        </div>
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
                            Pickup Requests Pending ({(myPickupRequests || []).length})
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className='overflow-auto'>
                            <MyPickupRequestsTable userId={currentUser.id} items={myPickupRequests || []} />
                        </div>
                    </CardContent>
                </Card>

                <Card className='mb-4'>
                    <CardHeader>
                        <CardTitle className='text-primary'>
                            Partner Admin Invitations Pending ({(myPartnerAdminInvitations || []).length})
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className='overflow-auto'>
                            <PartnerAdminInvitationsTable
                                userId={currentUser.id}
                                items={myPartnerAdminInvitations || []}
                            />
                        </div>
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
