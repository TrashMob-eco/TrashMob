import { FC, useCallback, useEffect, useState } from 'react';
import orderBy from 'lodash/orderBy';
import { Link, useNavigate, useLocation } from 'react-router';
import { APIProvider } from '@vis.gl/react-google-maps';
import { useQuery } from '@tanstack/react-query';
import { Plus } from 'lucide-react';
import { AxiosResponse } from 'axios';

import * as SharingMessages from '@/store/SharingMessages';

import EventData from '@/components/Models/EventData';
import PickupLocationData from '@/components/Models/PickupLocationData';
import StatsData from '@/components/Models/StatsData';
import DisplayPartnershipData from '@/components/Models/DisplayPartnershipData';
import DisplayPartnerAdminInvitationData from '@/components/Models/DisplayPartnerAdminInvitationData';
import DisplayPartnerLocationEventData from '@/components/Models/DisplayPartnerLocationEventServiceData';

import twofigure from '@/components/assets/card/twofigure.svg';
import calendarclock from '@/components/assets/card/calendarclock.svg';
import bucketplus from '@/components/assets/card/bucketplus.svg';
import { ShareToSocialsDialog } from '@/components/EventManagement/ShareToSocialsDialog';
import { HeroSection } from '@/components/Customization/HeroSection';
import { EventsMap } from '@/components/events/event-map';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import { EventsTable } from './events-table/table';

import { PartnerEventRequestTable } from '@/components/Partners/partner-event-request-table';

import { GetStatsForUser } from '@/services/stats';
import { GetPartnerRequestByUserId } from '@/services/partners';
import { GetPartnerAdminsForUser } from '@/services/admin';
import { GetPartnerAdminInvitationsByUser } from '@/services/invitations';
import { GetEventPickupLocationsByUser, GetPartnerLocationEventServicesByUserId } from '@/services/locations';

import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import { useGetUserEvents } from '@/hooks/useGetUserEvents';
import { useLogin } from '@/hooks/useLogin';

import { MyPartnersTable } from '@/pages/mydashboard/MyPartnersTable';
import { MyPickupRequestsTable } from '@/pages/mydashboard/MyPickupRequestsTable';
import { MyPartnersRequestTable } from '@/pages/mydashboard/MyPartnersRequestTable';
import { PartnerAdminInvitationsTable } from '@/pages/mydashboard/PartnerAdminInvitationsTable';

const isUpcomingEvent = (event: EventData) => new Date(event.eventDate) >= new Date();
const isPastEvent = (event: EventData) => new Date(event.eventDate) < new Date();

interface MyDashboardProps {}

const MyDashboard: FC<MyDashboardProps> = () => {
    const { isUserLoaded, currentUser } = useLogin();
    const location = useLocation();
    const navigate = useNavigate();
    const userId = currentUser.id;
    const userPreferredLocation = { lat: currentUser.latitude, lng: currentUser.longitude };

    const [upcomingEventsMapView, setUpcomingEventsMapView] = useState<boolean>(false);
    const [pastEventsMapView, setPastEventsMapView] = useState<boolean>(false);
    const state = location.state as { newEventCreated: boolean };
    const [eventToShare, setEventToShare] = useState<EventData>();
    const [showModal, setShowSocialsModal] = useState<boolean>(false);

    const { data: userEvents } = useGetUserEvents(userId);
    const myEventList = userEvents || [];
    const upcomingEvents = myEventList.filter(isUpcomingEvent);
    const pastEvents = orderBy(myEventList.filter(isPastEvent), ['eventDate'], ['desc']);

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

    // Event Request
    const { data: partnerEventServices, isLoading: isPartnerEventServicesLoading } = useQuery<
        AxiosResponse<DisplayPartnerLocationEventData[]>,
        unknown,
        DisplayPartnerLocationEventData[]
    >({
        queryKey: GetPartnerLocationEventServicesByUserId({ userId }).key,
        queryFn: GetPartnerLocationEventServicesByUserId({ userId }).service,
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
                .filter((event) => event.createdByUserId === currentUser.id)
                .sort((a, b) => (a.createdDate < b.createdDate ? 1 : -1));

            setSharingEvent(myFilteredList[0], true);

            // replace state
            state.newEventCreated = false;
            navigate(location.pathname, { replace: true, state });
        }
    }, [state, currentUser.id, navigate, myEventList, setSharingEvent]);

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
            <HeroSection Description="See how much you've done!" Title='Dashboard' />
            <div className='container !mt-12 !pb-12'>
                {eventToShare ? (
                    <ShareToSocialsDialog
                        eventToShare={eventToShare}
                        handleShow={setShowSocialsModal}
                        message={SharingMessages.getEventShareMessage(eventToShare, currentUser.id)}
                        modalTitle='Share Event'
                        show={showModal}
                    />
                ) : null}
                <div className='!pt-12 flex flex-row flex-wrap gap-8 justify-center'>
                    {[
                        { name: 'Events', value: totalEvents, img: twofigure },
                        { name: 'Hours', value: totalHours, img: calendarclock },
                        { name: 'Bags', value: totalBags, img: bucketplus },
                    ].map((stat) => (
                        <div
                            className='basis-full md:basis-[200px] md:max-w-[255px] md:grow bg-card !px-7 relative rounded-lg'
                            key={stat.name}
                        >
                            <p className='text-[25px] font-medium !mt-6 !mb-3'>{stat.name}</p>
                            <p className='text-primary !mt-0 text-[46px]'>{stat.value}</p>
                            <img
                                alt={stat.name}
                                className='absolute right-8 bottom-0 w-[95px] h-[95px]'
                                src={stat.img}
                            />
                        </div>
                    ))}
                </div>
            </div>
            <div className='container !my-12'>
                <div className='flex justify-between'>
                    <h4 className='font-bold text-3xl mr-2 pb-2 mt-0 border-b-[3px] border-primary flex items-center w-full'>
                        <div className='grow'>My Events ({myEventList.length})</div>
                        <Button asChild>
                            <Link to='/events/create'>
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
                                Upcoming events ({upcomingEvents.length})
                            </CardTitle>
                            <RadioGroup
                                className='grid-cols-2'
                                onValueChange={(value) => handleEventView(value, 'Upcoming events')}
                                orientation='horizontal'
                                value={upcomingEventsMapView ? 'map' : 'list'}
                            >
                                <div className='flex items-center space-x-2'>
                                    <RadioGroupItem id='upcomingevents-list' value='list' />
                                    <Label htmlFor='upcomingevents-list'>List view</Label>
                                </div>
                                <div className='flex items-center space-x-2'>
                                    <RadioGroupItem id='upcomingevents-map' value='map' />
                                    <Label htmlFor='upcomingevents-map'>Map view</Label>
                                </div>
                            </RadioGroup>
                        </div>
                    </CardHeader>
                    <CardContent>
                        {upcomingEventsMapView ? (
                            <EventsMap
                                currentUser={currentUser}
                                defaultCenter={userPreferredLocation}
                                events={upcomingEvents}
                                isUserLoaded={isUserLoaded}
                            />
                        ) : (
                            <EventsTable currentUser={currentUser} events={upcomingEvents} />
                        )}
                    </CardContent>
                </Card>

                <Card className='mb-4'>
                    <CardHeader>
                        <div className='flex flex-row'>
                            <CardTitle className='grow text-primary'>Completed events ({pastEvents.length})</CardTitle>
                            <RadioGroup
                                className='grid-cols-2'
                                onValueChange={(value) => handleEventView(value, 'Completed events')}
                                orientation='horizontal'
                                value={pastEventsMapView ? 'map' : 'list'}
                            >
                                <div className='flex items-center space-x-2'>
                                    <RadioGroupItem id='pastevents-list' value='list' />
                                    <Label htmlFor='pastevents-list'>List view</Label>
                                </div>
                                <div className='flex items-center space-x-2'>
                                    <RadioGroupItem id='pastevents-map' value='map' />
                                    <Label htmlFor='pastevents-map'>Map view</Label>
                                </div>
                            </RadioGroup>
                        </div>
                    </CardHeader>
                    <CardContent>
                        {pastEventsMapView ? (
                            <EventsMap
                                currentUser={currentUser}
                                defaultCenter={userPreferredLocation}
                                events={pastEvents}
                                isUserLoaded={isUserLoaded}
                            />
                        ) : (
                            <EventsTable currentUser={currentUser} events={pastEvents} />
                        )}
                    </CardContent>
                </Card>
                <div className='flex flex-col mt-10 mb-3'>
                    <h4 className='font-semibold text-3xl mr-2 mt-0 pb-2 border-b-[3px] border-primary'>
                        My Partnerships ({(myPartnerRequests || []).length + (myPartners || []).length})
                    </h4>
                    <div className='flex flex-row flex-wrap gap-4 !my-4'>
                        <Button asChild variant='outline'>
                            <Link to='/inviteapartner'>Send invitation to join TrashMob.eco as a partner</Link>
                        </Button>
                        <Button asChild variant='outline'>
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
                        <PartnerEventRequestTable
                            data={partnerEventServices}
                            isLoading={isPartnerEventServicesLoading}
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
                            <MyPickupRequestsTable items={myPickupRequests || []} userId={currentUser.id} />
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
                                items={myPartnerAdminInvitations || []}
                                userId={currentUser.id}
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

export default MyDashboardWrapper;
