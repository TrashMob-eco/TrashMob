import { useRef, useState } from 'react';
import { AdvancedMarker, InfoWindow, MapProps } from '@vis.gl/react-google-maps';
import { GoogleMapWithKey as GoogleMap } from '../Map/GoogleMap';
import { EventDetailInfoWindowHeader, EventDetailInfoWindowContent } from '../Map/EventInfoWindowContent';
import EventData from '../Models/EventData';
import TeamData from '../Models/TeamData';
import LitterReportData from '../Models/LitterReportData';
import { LitterReportStatusEnum } from '../Models/LitterReportStatus';
import { useIsInViewport } from '@/hooks/useIsInViewport';
import { cn } from '@/lib/utils';
import { EventPin } from '../events/event-pin';
import { TeamPin } from '../teams/team-pin';
import { LitterReportPin, litterReportColors } from '../litterreports/litter-report-pin';
import {
    LitterReportInfoWindowHeader,
    LitterReportInfoWindowContent,
} from '../litterreports/litter-report-info-window';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import { Label } from '@/components/ui/label';
import moment from 'moment';

const colors = {
    event: '#1D4ED8', // Blue for events
    team: '#7C3AED', // Purple for teams
};

const getLitterReportColor = (statusId: number): string => {
    switch (statusId) {
        case LitterReportStatusEnum.New:
            return litterReportColors.new;
        case LitterReportStatusEnum.Assigned:
            return litterReportColors.assigned;
        case LitterReportStatusEnum.Cleaned:
            return litterReportColors.cleaned;
        case LitterReportStatusEnum.Cancelled:
            return litterReportColors.cancelled;
        default:
            return litterReportColors.new;
    }
};

interface CommunityDetailMapProps extends MapProps {
    id?: string;
    events: EventData[];
    teams: TeamData[];
    litterReports: LitterReportData[];
    centerLat: number;
    centerLng: number;
}

export const CommunityDetailMap = (props: CommunityDetailMapProps) => {
    const { id, events, teams, litterReports, centerLat, centerLng, gestureHandling, ...rest } = props;

    const [showEvents, setShowEvents] = useState(true);
    const [showTeams, setShowTeams] = useState(true);
    const [showLitterReports, setShowLitterReports] = useState(true);

    const eventMarkersRef = useRef<Record<string, google.maps.marker.AdvancedMarkerElement>>({});
    const teamMarkersRef = useRef<Record<string, google.maps.marker.AdvancedMarkerElement>>({});
    const litterReportMarkersRef = useRef<Record<string, google.maps.marker.AdvancedMarkerElement>>({});

    const [showingEventId, setShowingEventId] = useState<string>('');
    const [showingTeamId, setShowingTeamId] = useState<string>('');
    const [showingLitterReportId, setShowingLitterReportId] = useState<string>('');

    const showingEvent = events.find((event) => event.id === showingEventId);
    const showingTeam = teams.find((team) => team.id === showingTeamId);
    const showingLitterReport = litterReports.find((report) => report.id === showingLitterReportId);

    const { ref, isInViewPort } = useIsInViewport();

    // Get events with coordinates
    const eventsWithLocation = events.filter(
        (event) => event.latitude && event.longitude && event.latitude !== 0 && event.longitude !== 0,
    );

    // Get teams with coordinates
    const teamsWithLocation = teams.filter(
        (team) => team.latitude && team.longitude && team.latitude !== 0 && team.longitude !== 0,
    );

    // Get litter reports with coordinates from their images
    const litterReportsWithLocation = litterReports
        .map((report) => {
            const imageWithLocation = report.litterImages?.find((img) => img.latitude && img.longitude);
            if (!imageWithLocation) return null;
            return {
                ...report,
                latitude: imageWithLocation.latitude!,
                longitude: imageWithLocation.longitude!,
            };
        })
        .filter(Boolean) as (LitterReportData & { latitude: number; longitude: number })[];

    const handleEventMarkerHover = (eventId: string) => {
        setShowingEventId(eventId);
        setShowingTeamId('');
        setShowingLitterReportId('');
    };

    const handleTeamMarkerHover = (teamId: string) => {
        setShowingTeamId(teamId);
        setShowingEventId('');
        setShowingLitterReportId('');
    };

    const handleLitterReportMarkerHover = (reportId: string) => {
        setShowingLitterReportId(reportId);
        setShowingEventId('');
        setShowingTeamId('');
    };

    const closeInfoWindow = () => {
        setShowingEventId('');
        setShowingTeamId('');
        setShowingLitterReportId('');
    };

    return (
        <Card>
            <CardHeader className='pb-3'>
                <CardTitle className='text-lg'>Community Map</CardTitle>
                <div className='flex flex-wrap gap-4 mt-2'>
                    <div className='flex items-center space-x-2'>
                        <Checkbox
                            id='show-events'
                            checked={showEvents}
                            onCheckedChange={(checked) => setShowEvents(checked === true)}
                        />
                        <Label htmlFor='show-events' className='text-sm cursor-pointer'>
                            Events ({eventsWithLocation.length})
                        </Label>
                    </div>
                    <div className='flex items-center space-x-2'>
                        <Checkbox
                            id='show-teams'
                            checked={showTeams}
                            onCheckedChange={(checked) => setShowTeams(checked === true)}
                        />
                        <Label htmlFor='show-teams' className='text-sm cursor-pointer'>
                            Teams ({teamsWithLocation.length})
                        </Label>
                    </div>
                    <div className='flex items-center space-x-2'>
                        <Checkbox
                            id='show-litter-reports'
                            checked={showLitterReports}
                            onCheckedChange={(checked) => setShowLitterReports(checked === true)}
                        />
                        <Label htmlFor='show-litter-reports' className='text-sm cursor-pointer'>
                            Litter Reports ({litterReportsWithLocation.length})
                        </Label>
                    </div>
                </div>
            </CardHeader>
            <CardContent ref={ref}>
                <div className='h-[400px] rounded-lg overflow-hidden'>
                    <GoogleMap
                        id={id}
                        gestureHandling={gestureHandling || 'cooperative'}
                        defaultCenter={{ lat: centerLat, lng: centerLng }}
                        defaultZoom={11}
                        {...rest}
                    >
                        {/* Event Markers */}
                        {showEvents
                            ? eventsWithLocation.map((event) => (
                                  <AdvancedMarker
                                      key={`event-${event.id}`}
                                      ref={(el) => {
                                          eventMarkersRef.current[event.id] = el!;
                                      }}
                                      className={cn({
                                          'animate-[bounce_1s_both_3s]': isInViewPort,
                                      })}
                                      position={{ lat: event.latitude, lng: event.longitude }}
                                      onMouseEnter={() => handleEventMarkerHover(event.id)}
                                      zIndex={10}
                                  >
                                      <EventPin color={colors.event} size={40} />
                                  </AdvancedMarker>
                              ))
                            : null}

                        {/* Team Markers */}
                        {showTeams
                            ? teamsWithLocation.map((team) => (
                                  <AdvancedMarker
                                      key={`team-${team.id}`}
                                      ref={(el) => {
                                          teamMarkersRef.current[team.id] = el!;
                                      }}
                                      className={cn({
                                          'animate-[bounce_1s_both_3s]': isInViewPort,
                                      })}
                                      position={{ lat: team.latitude!, lng: team.longitude! }}
                                      onMouseEnter={() => handleTeamMarkerHover(team.id)}
                                      zIndex={5}
                                  >
                                      <TeamPin color={colors.team} size={36} />
                                  </AdvancedMarker>
                              ))
                            : null}

                        {/* Litter Report Markers */}
                        {showLitterReports
                            ? litterReportsWithLocation.map((report) => (
                                  <AdvancedMarker
                                      key={`litter-${report.id}`}
                                      ref={(el) => {
                                          litterReportMarkersRef.current[report.id] = el!;
                                      }}
                                      className={cn({
                                          'animate-[bounce_1s_both_3s]': isInViewPort,
                                      })}
                                      position={{ lat: report.latitude, lng: report.longitude }}
                                      onMouseEnter={() => handleLitterReportMarkerHover(report.id)}
                                      zIndex={1}
                                  >
                                      <LitterReportPin
                                          color={getLitterReportColor(report.litterReportStatusId)}
                                          size={32}
                                      />
                                  </AdvancedMarker>
                              ))
                            : null}

                        {/* Event Info Window */}
                        {showingEvent ? (
                            <InfoWindow
                                anchor={eventMarkersRef.current[showingEventId]}
                                headerContent={<EventDetailInfoWindowHeader name={showingEvent.name} />}
                                onClose={closeInfoWindow}
                            >
                                <EventDetailInfoWindowContent
                                    event={{
                                        ...showingEvent,
                                        eventDate: moment(showingEvent.eventDate).toDate(),
                                    }}
                                    isAttending={false}
                                />
                            </InfoWindow>
                        ) : null}

                        {/* Team Info Window */}
                        {showingTeam ? (
                            <InfoWindow
                                anchor={teamMarkersRef.current[showingTeamId]}
                                headerContent={<div className='font-semibold text-sm'>{showingTeam.name}</div>}
                                onClose={closeInfoWindow}
                            >
                                <div className='text-xs text-muted-foreground'>
                                    <p>{showingTeam.description?.slice(0, 100)}...</p>
                                    {showingTeam.city ? (
                                        <p className='mt-1'>
                                            {showingTeam.city}, {showingTeam.region}
                                        </p>
                                    ) : null}
                                </div>
                            </InfoWindow>
                        ) : null}

                        {/* Litter Report Info Window */}
                        {showingLitterReport ? (
                            <InfoWindow
                                anchor={litterReportMarkersRef.current[showingLitterReportId]}
                                headerContent={<LitterReportInfoWindowHeader name={showingLitterReport.name} />}
                                onClose={closeInfoWindow}
                            >
                                <LitterReportInfoWindowContent litterReport={showingLitterReport} />
                            </InfoWindow>
                        ) : null}
                    </GoogleMap>
                </div>
            </CardContent>
        </Card>
    );
};
