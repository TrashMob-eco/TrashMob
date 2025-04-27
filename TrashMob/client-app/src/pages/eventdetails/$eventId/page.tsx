import { FC, useState } from 'react';
import { useParams } from 'react-router';
import moment from 'moment';
import { useQuery } from '@tanstack/react-query';
import * as MapStore from '@/store/MapStore';
import { MarkerWithInfoWindow, EventInfoWindowContent } from '@/components/Map';
import { ShareToSocialsDialog } from '@/components/EventManagement/ShareToSocialsDialog';
import { RegisterBtn } from '@/components/Customization/RegisterBtn';
import { HeroSection } from '@/components/Customization/HeroSection';
import * as SharingMessages from '@/store/SharingMessages';
import { GetAllEventsBeingAttendedByUser, GetEventAttendees } from '@/services/events';
import { useGetEvent } from '@/hooks/useGetEvent';
import { useGetEventType } from '@/hooks/useGetEventType';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { EventPlaceAndLocation } from '@/components/events/event-list';
import { EventAttendeeTable } from '@/components/events/event-attendee-table';

import { Calendar, Share2 } from 'lucide-react';
import makeUrls from '@/lib/add-to-calendar';
import { GoogleMapWithKey as GoogleMap } from '@/components/Map/GoogleMap';
import { useLogin } from '@/hooks/useLogin';

export interface EventDetailsProps {}

const useGetEventAttendees = (eventId: string) => {
    return useQuery({
        queryKey: GetEventAttendees({ eventId }).key,
        queryFn: GetEventAttendees({ eventId }).service,
        select: (res) => res.data,
        enabled: !!eventId,
    });
};

const useGetEventsAttendedByUser = (userId: string) => {
    return useQuery({
        queryKey: GetAllEventsBeingAttendedByUser({ userId }).key,
        queryFn: GetAllEventsBeingAttendedByUser({ userId }).service,
        select: (res) => res.data,
    });
};

export const EventDetails: FC<EventDetailsProps> = () => {
    const { eventId } = useParams<{ eventId: string }>() as { eventId: string };
    const { currentUser, isUserLoaded } = useLogin();
    console.log({ currentUser, isUserLoaded });
    const { data: event, isSuccess } = useGetEvent(eventId);
    const { data: eventType } = useGetEventType(event?.eventTypeId || 0);
    const { data: eventAttendees } = useGetEventAttendees(eventId);
    const { data: myAttendanceList } = useGetEventsAttendedByUser(currentUser.id);
    const [showModal, setShowSocialsModal] = useState<boolean>(false);

    const isDataLoaded = isSuccess;

    const {
        name: eventName = 'New Event',
        description,
        eventDate,
        durationHours,
        durationMinutes,
        streetAddress,
        city,
        latitude,
        longitude,
        createdByUserId = '',
        maxNumberOfParticipants,
    } = event || {};

    const isEventCompleted = moment(eventDate).isBefore(new Date());

    const startDateTime = moment(eventDate);
    const endDateTime = moment(startDateTime).add(durationHours, 'hours').add(durationMinutes, 'minutes');

    const isAttending = (myAttendanceList || []).findIndex((e) => e.id === eventId) >= 0 ? 'Yes' : 'No';

    const urls = makeUrls({
        name: eventName,
        details: description ?? '',
        location: `${streetAddress}, ${city}`,
        startsAt: moment(eventDate).format(),
        endsAt: moment(endDateTime).format(),
    });

    return (
        <div className='tailwind'>
            <HeroSection Title='View Events' Description='Learn, join, and inspire.' />
            {!isDataLoaded ? (
                <p>
                    <em>Loading...</em>
                </p>
            ) : (
                <>
                    <div className='container mx-auto my-5'>
                        <ShareToSocialsDialog
                            eventToShare={event}
                            show={showModal}
                            handleShow={setShowSocialsModal}
                            modalTitle='Share Event'
                            message={SharingMessages.getEventDetailsMessage(
                                startDateTime.toDate(),
                                city,
                                createdByUserId,
                                currentUser.id,
                            )}
                        />
                        <div className='flex justify-between items-end flex-col md:flex-row'>
                            <h2 className='font-semibold'>{eventName}</h2>
                            <div className='flex my-3 gap-2'>
                                {currentUser ? (
                                    <RegisterBtn
                                        eventId={eventId}
                                        isAttending={isAttending}
                                        isEventCompleted={isEventCompleted}
                                        currentUser={currentUser}
                                        isUserLoaded={isUserLoaded}
                                    />
                                ) : null}
                                <DropdownMenu>
                                    <DropdownMenuTrigger asChild>
                                        <Button variant='outline'>
                                            <Calendar /> Add to my calendar
                                        </Button>
                                    </DropdownMenuTrigger>
                                    <DropdownMenuContent>
                                        <DropdownMenuItem asChild>
                                            <a href={urls.ics}>Apple</a>
                                        </DropdownMenuItem>
                                        <DropdownMenuItem asChild>
                                            <a href={urls.google}>Google</a>
                                        </DropdownMenuItem>
                                        <DropdownMenuItem asChild>
                                            <a href={urls.ics} download={eventName}>
                                                Outlook
                                            </a>
                                        </DropdownMenuItem>
                                        <DropdownMenuItem asChild>
                                            <a href={urls.outlook}>Outlook Web app</a>
                                        </DropdownMenuItem>
                                        <DropdownMenuItem asChild>
                                            <a href={urls.yahoo}>Yahoo</a>
                                        </DropdownMenuItem>
                                    </DropdownMenuContent>
                                </DropdownMenu>
                                <Button
                                    variant='outline'
                                    onClick={() => {
                                        setShowSocialsModal(true);
                                    }}
                                >
                                    <Share2 />
                                    Share
                                </Button>
                            </div>
                        </div>
                        <Badge>{eventType?.name}</Badge>
                        <p className='mt-4 mb-8 text-foreground'>{description}</p>
                        <div className='!mb-8'>
                            <EventPlaceAndLocation {...event} />
                        </div>
                        {latitude && longitude ? (
                            <GoogleMap
                                gestureHandling='greedy'
                                defaultCenter={{ lat: latitude, lng: longitude }}
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
                            </GoogleMap>
                        ) : null}
                    </div>
                    {currentUser ? (
                        <div className='container mx-auto mb-16'>
                            <hr />
                            <h2 className='font-semibold text-xl mt-5 mb-4'>
                                <span>Attendees ({(eventAttendees || []).length})</span>
                            </h2>
                            <p className='font-semibold m-0 my-4'>
                                Max Number of Participants:
                                <span className='ml-2 text-muted-foreground'>{maxNumberOfParticipants}</span>
                            </p>
                            <EventAttendeeTable users={eventAttendees || []} event={event} />
                        </div>
                    ) : null}
                </>
            )}
        </div>
    );
};
