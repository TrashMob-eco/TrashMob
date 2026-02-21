import { FC, useCallback, useEffect, useMemo, useState } from 'react';
import uniqBy from 'lodash/uniqBy';
import orderBy from 'lodash/orderBy';
import { Link, useNavigate, useLocation } from 'react-router';
import { APIProvider } from '@vis.gl/react-google-maps';
import { useQuery } from '@tanstack/react-query';
import {
    Plus,
    Users,
    FileText,
    Mail,
    Trophy,
    UserPlus,
    CalendarCheck,
    CalendarX,
    UsersRound,
    Route,
    Trash2,
    MapPin,
    Building2,
    Handshake,
    ClipboardList,
    Truck,
    Briefcase,
    Heart,
    List,
    Map as MapIcon,
} from 'lucide-react';
import { AxiosResponse } from 'axios';

import { getEventShareableContent, getEventShareMessage } from '@/lib/sharing-messages';
import { isCompletedEvent } from '@/lib/event-helpers';

import EventData from '@/components/Models/EventData';
import PickupLocationData from '@/components/Models/PickupLocationData';
import StatsData from '@/components/Models/StatsData';
import DisplayPartnershipData from '@/components/Models/DisplayPartnershipData';
import DisplayPartnerAdminInvitationData from '@/components/Models/DisplayPartnerAdminInvitationData';
import DisplayPartnerLocationEventData from '@/components/Models/DisplayPartnerLocationEventServiceData';
import LitterReportData from '@/components/Models/LitterReportData';
import { LitterReportStatusEnum } from '@/components/Models/LitterReportStatus';
import TeamData from '@/components/Models/TeamData';
import ProfessionalCompanyData from '@/components/Models/ProfessionalCompanyData';
import SponsorData from '@/components/Models/SponsorData';

import twofigure from '@/components/assets/card/twofigure.svg';
import calendarclock from '@/components/assets/card/calendarclock.svg';
import bucketplus from '@/components/assets/card/bucketplus.svg';
import Weight from '@/components/assets/home/Weight.svg';
import LitterReportIcon from '@/components/assets/home/LitterReport.svg';
import { ShareDialog } from '@/components/sharing';
import { HeroSection } from '@/components/Customization/HeroSection';
import { EventsMap } from '@/components/events/event-map';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { EventsTable } from './events-table/table';

import { PartnerEventRequestTable } from '@/components/Partners/partner-event-request-table';

import { GetStatsForUser } from '@/services/stats';
import { GetPartnerRequestByUserId } from '@/services/partners';
import { GetPartnerAdminsForUser } from '@/services/admin';
import { GetPartnerAdminInvitationsByUser } from '@/services/invitations';
import { GetEventPickupLocationsByUser, GetPartnerLocationEventServicesByUserId } from '@/services/locations';
import { GetUserLitterReports } from '@/services/litter-report';
import { GetMyTeams, GetTeamsILead } from '@/services/teams';
import { GetMyCompanies } from '@/services/professional-company-portal';
import { GetMySponsors } from '@/services/sponsor-portal';

import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import { useGetUserEvents } from '@/hooks/useGetUserEvents';
import { useLogin } from '@/hooks/useLogin';

import { MyPartnersTable } from '@/pages/MyDashboard/MyPartnersTable';
import { MyPickupRequestsTable } from '@/pages/MyDashboard/MyPickupRequestsTable';
import { MyPartnersRequestTable } from '@/pages/MyDashboard/MyPartnersRequestTable';
import { PartnerAdminInvitationsTable } from '@/pages/MyDashboard/PartnerAdminInvitationsTable';
import { MyLitterReportsTable } from '@/pages/MyDashboard/MyLitterReportsTable';
import { NearbyLitterReportsWidget } from '@/pages/MyDashboard/NearbyLitterReportsWidget';
import { MyTeamsTable } from '@/pages/MyDashboard/MyTeamsTable';
import { MyRoutesCard } from '@/pages/MyDashboard/MyRoutesCard';
import { MyWaiversCard } from '@/pages/MyDashboard/MyWaiversCard';
import { MyImpactCard } from '@/pages/MyDashboard/MyImpactCard';
import { MyCompaniesTable } from '@/pages/MyDashboard/MyCompaniesTable';
import { MySponsorsTable } from '@/pages/MyDashboard/MySponsorsTable';
import { InviteFriendsCard } from '@/pages/MyDashboard/InviteFriendsCard';
import { MyNewsletterPreferencesCard } from '@/pages/MyDashboard/MyNewsletterPreferencesCard';
import { LitterReportsMapView } from './LitterReportsMapView';
import { MyTeamsMapView } from './MyTeamsMapView';
import { PickupRequestsMapView } from './PickupRequestsMapView';
import { DashboardScrollNav, DashboardNavGroup } from './DashboardScrollNav';
import { DashboardMobileNav } from './DashboardMobileNav';

// An event is upcoming if it hasn't finished yet (start time + duration > now)
const isUpcomingEvent = (event: EventData) => !isCompletedEvent(event);
// An event is past/completed when start time + duration has passed
const isPastEvent = (event: EventData) => isCompletedEvent(event);

type EventFilterType = 'all' | 'created' | 'attending';
type LitterReportFilterType = 'all' | 'new' | 'assigned' | 'cleaned' | 'cancelled';

const ViewModeToggle = ({
    viewMode,
    onChange,
}: {
    viewMode: 'table' | 'map';
    onChange: (mode: 'table' | 'map') => void;
}) => (
    <div className='flex border rounded-md'>
        <Button
            variant={viewMode === 'table' ? 'default' : 'ghost'}
            size='sm'
            className='rounded-r-none'
            onClick={() => onChange('table')}
        >
            <List className='h-4 w-4' />
        </Button>
        <Button
            variant={viewMode === 'map' ? 'default' : 'ghost'}
            size='sm'
            className='rounded-l-none'
            onClick={() => onChange('map')}
        >
            <MapIcon className='h-4 w-4' />
        </Button>
    </div>
);

const EVENTS_SECTIONS = new Set(['upcoming-events', 'completed-events']);
const PARTNERSHIP_SECTIONS = new Set([
    'my-partners',
    'partner-requests',
    'partner-event-requests',
    'pickup-requests',
    'partner-admin-invitations',
]);

const navGroups: DashboardNavGroup[] = [
    {
        title: 'Account',
        items: [
            { id: 'waivers', label: 'Waivers', icon: FileText },
            { id: 'newsletters', label: 'Newsletters', icon: Mail },
            { id: 'impact', label: 'Impact', icon: Trophy },
            { id: 'invite-friends', label: 'Invite Friends', icon: Users },
        ],
    },
    {
        title: 'Events',
        items: [
            { id: 'upcoming-events', label: 'Upcoming', icon: CalendarCheck },
            { id: 'completed-events', label: 'Completed', icon: CalendarX },
        ],
    },
    {
        title: 'Teams',
        items: [{ id: 'teams', label: 'My Teams', icon: UsersRound }],
    },
    {
        title: 'Routes',
        items: [{ id: 'routes', label: 'My Routes', icon: Route }],
    },
    {
        title: 'Litter Reports',
        items: [
            { id: 'litter-reports', label: 'My Reports', icon: Trash2 },
            { id: 'nearby-litter-reports', label: 'Nearby', icon: MapPin },
        ],
    },
    {
        title: 'Partnerships',
        items: [
            { id: 'my-partners', label: 'My Partners', icon: Building2 },
            { id: 'partner-requests', label: 'Requests Sent', icon: Handshake },
            { id: 'partner-event-requests', label: 'Event Requests', icon: ClipboardList },
            { id: 'pickup-requests', label: 'Pickup Requests', icon: Truck },
            { id: 'partner-admin-invitations', label: 'Admin Invitations', icon: UserPlus },
        ],
    },
    {
        title: 'Professional Companies',
        items: [{ id: 'my-companies', label: 'My Companies', icon: Briefcase }],
    },
    {
        title: 'Sponsors',
        items: [{ id: 'my-sponsors', label: 'My Sponsors', icon: Heart }],
    },
];

const flatNavItems = navGroups.flatMap((g) => g.items);

interface MyDashboardProps {}

const MyDashboard: FC<MyDashboardProps> = () => {
    const { isUserLoaded, currentUser } = useLogin();
    const location = useLocation();
    const navigate = useNavigate();
    const userId = currentUser.id;
    const userPreferredLocation = { lat: currentUser.latitude, lng: currentUser.longitude };

    const [upcomingEventsView, setUpcomingEventsView] = useState<'table' | 'map'>('table');
    const [pastEventsView, setPastEventsView] = useState<'table' | 'map'>('table');
    const [eventFilter, setEventFilter] = useState<EventFilterType>('all');
    const [litterReportFilter, setLitterReportFilter] = useState<LitterReportFilterType>('all');
    const [litterReportsView, setLitterReportsView] = useState<'table' | 'map'>('table');
    const [nearbyLitterView, setNearbyLitterView] = useState<'table' | 'map'>('table');
    const [teamsView, setTeamsView] = useState<'table' | 'map'>('table');
    const [pickupRequestsView, setPickupRequestsView] = useState<'table' | 'map'>('table');
    const state = location.state as { newEventCreated: boolean };
    const [eventToShare, setEventToShare] = useState<EventData>();
    const [showModal, setShowSocialsModal] = useState<boolean>(false);
    const [activeSection, setActiveSection] = useState<string | null>(null);

    const isSectionVisible = useCallback(
        (sectionId: string) => activeSection === null || activeSection === sectionId,
        [activeSection],
    );
    const isEventsVisible = activeSection === null || EVENTS_SECTIONS.has(activeSection);
    const isPartnershipsVisible = activeSection === null || PARTNERSHIP_SECTIONS.has(activeSection);

    const { data: userEvents } = useGetUserEvents(userId);
    const myEventList = userEvents || [];

    // Filter events based on user selection
    const filteredEvents = useMemo(() => {
        if (eventFilter === 'all') return myEventList;
        if (eventFilter === 'created') return myEventList.filter((e) => e.createdByUserId === userId);
        // 'attending' - events where user is attending but didn't create
        return myEventList.filter((e) => e.createdByUserId !== userId);
    }, [myEventList, eventFilter, userId]);

    const upcomingEvents = filteredEvents.filter(isUpcomingEvent);
    const pastEvents = orderBy(filteredEvents.filter(isPastEvent), ['eventDate'], ['desc']);

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
        select: (res) => uniqBy(res.data, 'id'),
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

    // User's Litter Reports
    const { data: myLitterReports } = useQuery<AxiosResponse<LitterReportData[]>, unknown, LitterReportData[]>({
        queryKey: GetUserLitterReports({ userId }).key,
        queryFn: GetUserLitterReports({ userId }).service,
        select: (res) => res.data,
    });

    // User's Teams
    const { data: myTeams } = useQuery<AxiosResponse<TeamData[]>, unknown, TeamData[]>({
        queryKey: GetMyTeams().key,
        queryFn: GetMyTeams().service,
        select: (res) => res.data,
    });

    // Teams user leads
    const { data: teamsILead } = useQuery<AxiosResponse<TeamData[]>, unknown, TeamData[]>({
        queryKey: GetTeamsILead().key,
        queryFn: GetTeamsILead().service,
        select: (res) => res.data,
    });

    // Professional companies user belongs to
    const { data: myCompanies } = useQuery<
        AxiosResponse<ProfessionalCompanyData[]>,
        unknown,
        ProfessionalCompanyData[]
    >({
        queryKey: GetMyCompanies().key,
        queryFn: GetMyCompanies().service,
        select: (res) => res.data,
    });

    // Sponsors user has access to (via partner admin memberships)
    const { data: mySponsors } = useQuery<AxiosResponse<SponsorData[]>, unknown, SponsorData[]>({
        queryKey: GetMySponsors().key,
        queryFn: GetMySponsors().service,
        select: (res) => res.data,
    });

    // Filter litter reports based on user selection
    const filteredLitterReports = useMemo(() => {
        const reports = myLitterReports || [];
        if (litterReportFilter === 'all') return reports;
        const statusMap: Record<LitterReportFilterType, number> = {
            all: 0,
            new: LitterReportStatusEnum.New,
            assigned: LitterReportStatusEnum.Assigned,
            cleaned: LitterReportStatusEnum.Cleaned,
            cancelled: LitterReportStatusEnum.Cancelled,
        };
        return reports.filter((r) => r.litterReportStatusId === statusMap[litterReportFilter]);
    }, [myLitterReports, litterReportFilter]);

    // getStatsForUser
    const { data: stats } = useQuery<AxiosResponse<StatsData>, unknown, StatsData>({
        queryKey: GetStatsForUser({ userId: currentUser.id }).key,
        queryFn: GetStatsForUser({ userId: currentUser.id }).service,
        select: (res) => res.data,
    });

    const totalBags = stats?.totalBags || 0;
    const totalHours = stats?.totalHours || 0;
    const totalEvents = stats?.totalEvents || 0;
    const totalLitterReports = stats?.totalLitterReportsSubmitted || 0;

    // Use user's weight preference (prefersMetric: true = kg, false = lbs)
    const prefersMetric = currentUser?.prefersMetric ?? false;
    const totalWeight = Math.round(
        prefersMetric ? stats?.totalWeightInKilograms || 0 : stats?.totalWeightInPounds || 0,
    );
    const weightLabel = prefersMetric ? 'Weight (kg)' : 'Weight (lbs)';

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

    return (
        <div>
            <HeroSection Title='Dashboard' Description="See how much you've done!" />

            {/* Mobile nav - visible below lg */}
            <div className='lg:hidden'>
                <DashboardMobileNav
                    items={flatNavItems}
                    activeSection={activeSection}
                    onSectionChange={setActiveSection}
                />
            </div>

            <div className='container mt-6! pb-12!'>
                {eventToShare ? (
                    <ShareDialog
                        content={getEventShareableContent(eventToShare, currentUser?.id)}
                        open={showModal}
                        onOpenChange={setShowSocialsModal}
                        message={getEventShareMessage(eventToShare, currentUser?.id)}
                    />
                ) : null}

                <div className='flex flex-col gap-6 lg:flex-row'>
                    {/* Desktop sidebar - hidden below lg */}
                    <aside className='hidden shrink-0 lg:block lg:w-56'>
                        <div className='sticky top-4 rounded-lg border bg-card p-4'>
                            <DashboardScrollNav
                                groups={navGroups}
                                activeSection={activeSection}
                                onSectionChange={setActiveSection}
                            />
                        </div>
                    </aside>

                    {/* Main content */}
                    <main className='min-w-0 flex-1 space-y-8'>
                        {/* Stats */}
                        <section id='stats' className='scroll-mt-20'>
                            <div className='grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4'>
                                {[
                                    { name: 'Events', value: totalEvents, img: twofigure },
                                    { name: 'Hours', value: totalHours, img: calendarclock },
                                    { name: 'Bags', value: totalBags, img: bucketplus },
                                    { name: weightLabel, value: totalWeight, img: Weight },
                                    {
                                        name: 'Litter Reports',
                                        value: totalLitterReports,
                                        img: LitterReportIcon,
                                    },
                                ].map((stat) => (
                                    <div
                                        className='bg-card rounded-lg overflow-hidden min-w-0 flex items-end justify-between p-4 lg:p-2.5 xl:p-3'
                                        key={stat.name}
                                    >
                                        <div className='min-w-0'>
                                            <p className='text-lg lg:text-sm font-medium mb-2 lg:mb-1'>{stat.name}</p>
                                            <p className='text-primary text-4xl lg:text-2xl'>{stat.value}</p>
                                        </div>
                                        <img
                                            src={stat.img}
                                            alt={stat.name}
                                            className='w-[70px] h-[70px] lg:w-10 lg:h-10 xl:w-12 xl:h-12 shrink-0'
                                        />
                                    </div>
                                ))}
                            </div>
                        </section>

                        {/* Account sections */}
                        {isSectionVisible('waivers') ? (
                            <section id='waivers'>
                                <MyWaiversCard userId={userId} />
                            </section>
                        ) : null}

                        {isSectionVisible('newsletters') ? (
                            <section id='newsletters'>
                                <MyNewsletterPreferencesCard />
                            </section>
                        ) : null}

                        {isSectionVisible('impact') ? (
                            <section id='impact'>
                                <MyImpactCard userId={userId} prefersMetric={currentUser?.prefersMetric ?? false} />
                            </section>
                        ) : null}

                        {isSectionVisible('invite-friends') ? (
                            <section id='invite-friends'>
                                <InviteFriendsCard />
                            </section>
                        ) : null}

                        {/* Events */}
                        {isEventsVisible ? (
                            <div>
                                <div className='flex justify-between'>
                                    <h4 className='font-bold text-3xl mr-2 pb-2 mt-0 border-b-[3px] border-primary flex items-center w-full'>
                                        <div className='grow'>My Events ({filteredEvents.length})</div>
                                        <Select
                                            value={eventFilter}
                                            onValueChange={(v) => setEventFilter(v as EventFilterType)}
                                        >
                                            <SelectTrigger className='w-[180px] mr-4'>
                                                <SelectValue placeholder='Filter events' />
                                            </SelectTrigger>
                                            <SelectContent>
                                                <SelectItem value='all'>All events</SelectItem>
                                                <SelectItem value='created'>Events I created</SelectItem>
                                                <SelectItem value='attending'>Events I joined</SelectItem>
                                            </SelectContent>
                                        </Select>
                                        <Button asChild>
                                            <Link to='/events/create'>
                                                <Plus /> Create Event
                                            </Link>
                                        </Button>
                                    </h4>
                                </div>
                            </div>
                        ) : null}

                        {isSectionVisible('upcoming-events') ? (
                            <section id='upcoming-events'>
                                <Card>
                                    <CardHeader>
                                        <div className='flex flex-row items-center'>
                                            <CardTitle className='grow text-primary'>
                                                Upcoming events ({upcomingEvents.length})
                                            </CardTitle>
                                            <ViewModeToggle
                                                viewMode={upcomingEventsView}
                                                onChange={setUpcomingEventsView}
                                            />
                                        </div>
                                    </CardHeader>
                                    <CardContent>
                                        {upcomingEventsView === 'map' ? (
                                            <EventsMap
                                                events={upcomingEvents}
                                                isUserLoaded={isUserLoaded}
                                                currentUser={currentUser}
                                                defaultCenter={userPreferredLocation}
                                            />
                                        ) : (
                                            <EventsTable events={upcomingEvents} currentUser={currentUser} />
                                        )}
                                    </CardContent>
                                </Card>
                            </section>
                        ) : null}

                        {isSectionVisible('completed-events') ? (
                            <section id='completed-events'>
                                <Card>
                                    <CardHeader>
                                        <div className='flex flex-row items-center'>
                                            <CardTitle className='grow text-primary'>
                                                Completed events ({pastEvents.length})
                                            </CardTitle>
                                            <ViewModeToggle viewMode={pastEventsView} onChange={setPastEventsView} />
                                        </div>
                                    </CardHeader>
                                    <CardContent>
                                        {pastEventsView === 'map' ? (
                                            <EventsMap
                                                events={pastEvents}
                                                isUserLoaded={isUserLoaded}
                                                currentUser={currentUser}
                                                defaultCenter={userPreferredLocation}
                                            />
                                        ) : (
                                            <EventsTable events={pastEvents} currentUser={currentUser} />
                                        )}
                                    </CardContent>
                                </Card>
                            </section>
                        ) : null}

                        {/* Teams */}
                        {isSectionVisible('teams') ? (
                            <section id='teams'>
                                <Card>
                                    <CardHeader>
                                        <div className='flex flex-row items-center gap-2'>
                                            <CardTitle className='grow text-primary'>
                                                <Users className='inline-block h-5 w-5 mr-2' />
                                                My Teams ({(myTeams || []).length})
                                            </CardTitle>
                                            {(myTeams || []).length > 0 ? (
                                                <ViewModeToggle viewMode={teamsView} onChange={setTeamsView} />
                                            ) : null}
                                            <Button variant='outline' size='sm' asChild>
                                                <Link to='/teams'>Browse Teams</Link>
                                            </Button>
                                        </div>
                                    </CardHeader>
                                    <CardContent>
                                        {teamsView === 'map' && (myTeams || []).length > 0 ? (
                                            <MyTeamsMapView teams={myTeams || []} teamsILead={teamsILead || []} />
                                        ) : (
                                            <div className='overflow-auto'>
                                                <MyTeamsTable items={myTeams || []} teamsILead={teamsILead || []} />
                                            </div>
                                        )}
                                    </CardContent>
                                </Card>
                            </section>
                        ) : null}

                        {/* Routes */}
                        {isSectionVisible('routes') ? (
                            <section id='routes'>
                                <MyRoutesCard />
                            </section>
                        ) : null}

                        {/* Litter Reports */}
                        {isSectionVisible('litter-reports') ? (
                            <section id='litter-reports'>
                                <Card>
                                    <CardHeader>
                                        <div className='flex flex-row flex-wrap items-center gap-2'>
                                            <CardTitle className='grow text-primary'>
                                                My Litter Reports ({filteredLitterReports.length})
                                            </CardTitle>
                                            {filteredLitterReports.length > 0 ? (
                                                <ViewModeToggle
                                                    viewMode={litterReportsView}
                                                    onChange={setLitterReportsView}
                                                />
                                            ) : null}
                                            <Select
                                                value={litterReportFilter}
                                                onValueChange={(v) =>
                                                    setLitterReportFilter(v as LitterReportFilterType)
                                                }
                                            >
                                                <SelectTrigger className='w-[140px]'>
                                                    <SelectValue placeholder='Filter' />
                                                </SelectTrigger>
                                                <SelectContent>
                                                    <SelectItem value='all'>All</SelectItem>
                                                    <SelectItem value='new'>New</SelectItem>
                                                    <SelectItem value='assigned'>Assigned</SelectItem>
                                                    <SelectItem value='cleaned'>Cleaned</SelectItem>
                                                    <SelectItem value='cancelled'>Cancelled</SelectItem>
                                                </SelectContent>
                                            </Select>
                                            <Button variant='outline' size='sm' asChild>
                                                <Link to='/litterreports'>View All Reports</Link>
                                            </Button>
                                        </div>
                                    </CardHeader>
                                    <CardContent>
                                        {litterReportsView === 'map' && filteredLitterReports.length > 0 ? (
                                            <LitterReportsMapView reports={filteredLitterReports} />
                                        ) : (
                                            <div className='overflow-auto'>
                                                <MyLitterReportsTable items={filteredLitterReports} />
                                            </div>
                                        )}
                                    </CardContent>
                                </Card>
                            </section>
                        ) : null}

                        {isSectionVisible('nearby-litter-reports') && currentUser.latitude && currentUser.longitude ? (
                            <section id='nearby-litter-reports'>
                                <Card>
                                    <CardHeader>
                                        <div className='flex flex-row items-center'>
                                            <CardTitle className='grow text-primary'>Nearby Litter Reports</CardTitle>
                                            <ViewModeToggle
                                                viewMode={nearbyLitterView}
                                                onChange={setNearbyLitterView}
                                            />
                                        </div>
                                    </CardHeader>
                                    <CardContent>
                                        <NearbyLitterReportsWidget
                                            viewMode={nearbyLitterView}
                                            userLocation={{
                                                lat: currentUser.latitude,
                                                lng: currentUser.longitude,
                                            }}
                                            radiusMiles={currentUser.travelLimitForLocalEvents || 10}
                                        />
                                    </CardContent>
                                </Card>
                            </section>
                        ) : null}

                        {/* Partnerships */}
                        {isPartnershipsVisible ? (
                            <div>
                                <div className='flex flex-col'>
                                    <h4 className='font-semibold text-3xl mr-2 mt-0 pb-2 border-b-[3px] border-primary'>
                                        My Partnerships ({(myPartnerRequests || []).length + (myPartners || []).length})
                                    </h4>
                                    <div className='flex flex-row flex-wrap gap-4 my-4!'>
                                        <Button variant='outline' asChild>
                                            <Link to='/inviteapartner'>
                                                Send invitation to join TrashMob.eco as a partner
                                            </Link>
                                        </Button>
                                        <Button variant='outline' asChild>
                                            <Link to='/becomeapartner'>Apply to become a partner</Link>
                                        </Button>
                                    </div>
                                </div>
                            </div>
                        ) : null}

                        {isSectionVisible('my-partners') ? (
                            <section id='my-partners'>
                                <Card>
                                    <CardHeader>
                                        <CardTitle className='text-primary'>
                                            My Partners ({(myPartners || []).length})
                                        </CardTitle>
                                    </CardHeader>
                                    <CardContent>
                                        <div className='overflow-auto'>
                                            <MyPartnersTable items={myPartners || []} />
                                        </div>
                                    </CardContent>
                                </Card>
                            </section>
                        ) : null}

                        {isSectionVisible('partner-requests') ? (
                            <section id='partner-requests'>
                                <Card>
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
                            </section>
                        ) : null}

                        {isSectionVisible('partner-event-requests') ? (
                            <section id='partner-event-requests'>
                                <Card>
                                    <CardHeader>
                                        <CardTitle className='text-primary'>Partner Event Requests</CardTitle>
                                    </CardHeader>
                                    <CardContent>
                                        <PartnerEventRequestTable
                                            isLoading={isPartnerEventServicesLoading}
                                            data={partnerEventServices}
                                        />
                                    </CardContent>
                                </Card>
                            </section>
                        ) : null}

                        {isSectionVisible('pickup-requests') ? (
                            <section id='pickup-requests'>
                                <Card>
                                    <CardHeader>
                                        <div className='flex flex-row items-center'>
                                            <CardTitle className='grow text-primary'>
                                                Pickup Requests Pending ({(myPickupRequests || []).length})
                                            </CardTitle>
                                            {(myPickupRequests || []).length > 0 ? (
                                                <ViewModeToggle
                                                    viewMode={pickupRequestsView}
                                                    onChange={setPickupRequestsView}
                                                />
                                            ) : null}
                                        </div>
                                    </CardHeader>
                                    <CardContent>
                                        {pickupRequestsView === 'map' && (myPickupRequests || []).length > 0 ? (
                                            <PickupRequestsMapView pickups={myPickupRequests || []} />
                                        ) : (
                                            <div className='overflow-auto'>
                                                <MyPickupRequestsTable
                                                    userId={currentUser.id}
                                                    items={myPickupRequests || []}
                                                />
                                            </div>
                                        )}
                                    </CardContent>
                                </Card>
                            </section>
                        ) : null}

                        {isSectionVisible('partner-admin-invitations') ? (
                            <section id='partner-admin-invitations'>
                                <Card>
                                    <CardHeader>
                                        <CardTitle className='text-primary'>
                                            Partner Admin Invitations Pending (
                                            {(myPartnerAdminInvitations || []).length})
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
                            </section>
                        ) : null}

                        {/* Professional Companies */}
                        {isSectionVisible('my-companies') ? (
                            <section id='my-companies'>
                                <Card>
                                    <CardHeader>
                                        <CardTitle className='text-primary'>
                                            <Briefcase className='inline-block h-5 w-5 mr-2' />
                                            My Professional Companies ({(myCompanies || []).length})
                                        </CardTitle>
                                    </CardHeader>
                                    <CardContent>
                                        {myCompanies && myCompanies.length > 0 ? (
                                            <div className='overflow-auto'>
                                                <MyCompaniesTable items={myCompanies} />
                                            </div>
                                        ) : (
                                            <p className='text-muted-foreground text-center py-4'>
                                                You don't have any professional companies yet.
                                            </p>
                                        )}
                                    </CardContent>
                                </Card>
                            </section>
                        ) : null}

                        {/* Sponsors */}
                        {isSectionVisible('my-sponsors') && mySponsors && mySponsors.length > 0 ? (
                            <section id='my-sponsors'>
                                <Card>
                                    <CardHeader>
                                        <CardTitle className='text-primary'>
                                            <Heart className='inline-block h-5 w-5 mr-2' />
                                            My Sponsors ({mySponsors.length})
                                        </CardTitle>
                                    </CardHeader>
                                    <CardContent>
                                        <div className='overflow-auto'>
                                            <MySponsorsTable items={mySponsors} />
                                        </div>
                                    </CardContent>
                                </Card>
                            </section>
                        ) : null}
                    </main>
                </div>
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
