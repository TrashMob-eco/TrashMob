import { FC, useEffect, useState } from 'react';
import { useParams } from 'react-router';
import { APIProvider, Map } from '@vis.gl/react-google-maps';
import moment from 'moment';
import { useQuery } from '@tanstack/react-query';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
import * as MapStore from '../../store/MapStore';
import { MarkerWithInfoWindow, EventInfoWindowContent } from '../Map';
import { ShareToSocialsDialog } from '../EventManagement/ShareToSocialsDialog';
import { RegisterBtn } from '../Customization/RegisterBtn';
import { HeroSection } from '../Customization/HeroSection';
import * as SharingMessages from '../../store/SharingMessages';
import { GetAllEventsBeingAttendedByUser, GetEventAttendees, GetEventById } from '../../services/events';
import { Services } from '../../config/services.config';
import { useGetGoogleMapApiKey } from '../../hooks/useGetGoogleMapApiKey';
import { useGetEvent } from '@/hooks/useGetEvent';
import { useGetEventType } from '@/hooks/useGetEventType';
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
  } from "@/components/ui/dropdown-menu"
import { EventPlaceAndLocation } from '@/components/events/event-card';
import { Calendar, Share2 } from 'lucide-react';
import makeUrls from '@/lib/add-to-calendar';

export interface DetailsMatchParams {
    eventId: string;
}

export interface EventDetailsProps {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const useGetEventAttendees = (eventId: string) => {
    return useQuery({
        queryKey: GetEventAttendees({ eventId }).key,
        queryFn: GetEventAttendees({ eventId }).service,
        select: res => res.data,
        enabled: !!eventId
    });
}

const useGetEventsAttendedByUser = (userId: string) => {
    return useQuery({
        queryKey: GetAllEventsBeingAttendedByUser({ userId }).key,
        queryFn: GetAllEventsBeingAttendedByUser({ userId }).service,
        select: res => res.data,
    });
}

export const EventDetails: FC<EventDetailsProps> = ({ currentUser, isUserLoaded }) => {
    const { eventId } = useParams<DetailsMatchParams>() 
    const { data, isSuccess } = useGetEvent(eventId)
    const { data: eventType } = useGetEventType(data?.eventTypeId || 0)
    const { data: eventAttendees } = useGetEventAttendees(eventId)
    const { data: myAttendanceList } = useGetEventsAttendedByUser(currentUser.id)
    const [showModal, setShowSocialsModal] = useState<boolean>(false);

    const isDataLoaded = isSuccess && isUserLoaded

    const {
        name: eventName = "New Event",
        description,
        eventDate,
        durationHours,
        durationMinutes,
        streetAddress,
        city,
        latitude,
        longitude,
        createdByUserId = '',
        maxNumberOfParticipants
    } = data || {}

    const isEventCompleted = moment(eventDate).isBefore(new Date())

    const startDateTime = moment(eventDate);
    const endDateTime = moment(startDateTime).add(durationHours, 'hours').add(durationMinutes, 'minutes');

    const event = {
        name: eventName,
        details: description ?? '',
        location: `${streetAddress}, ${city}`,
        startsAt: moment(eventDate).format(),
        endsAt: moment(endDateTime).format(),
    };

    const isAttending = (myAttendanceList || []).findIndex((e) => e.id === eventId) >= 0 ? 'Yes' : 'No';

    const urls = makeUrls(event)

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
                        {(eventAttendees || []).map((user) => {
                            let uName = user.userName;
                            if (user.id === createdByUserId) {
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
        <div className="tailwind">
            <HeroSection Title='View Events' Description='Learn, join, and inspire.' />
            {!isDataLoaded ? (
                <p>
                    <em>Loding...</em>
                </p>
            ) : (
                <>
                    <div className='container mx-auto my-5'>
                        <ShareToSocialsDialog
                            eventToShare={data}
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
                                <RegisterBtn
                                    eventId={eventId}
                                    isAttending={isAttending}
                                    isEventCompleted={isEventCompleted!}
                                    currentUser={currentUser}
                                    isUserLoaded={isUserLoaded}
                                />

                                <DropdownMenu>
                                    <DropdownMenuTrigger asChild>
                                        <Button variant='outline'><Calendar /> Add to my calendar</Button>
                                    </DropdownMenuTrigger>
                                    <DropdownMenuContent>
                                        <DropdownMenuItem asChild><a href={urls.ics}>Apple</a></DropdownMenuItem>
                                        <DropdownMenuItem asChild><a href={urls.google}>Google</a></DropdownMenuItem>
                                        <DropdownMenuItem asChild><a href={urls.ics} download={eventName}>Outlook</a></DropdownMenuItem>
                                        <DropdownMenuItem asChild><a href={urls.outlook}>Outlook Web app</a></DropdownMenuItem>
                                        <DropdownMenuItem asChild><a href={urls.yahoo}>Yahoo</a></DropdownMenuItem>
                                    </DropdownMenuContent>
                                </DropdownMenu>
                                <Button
                                    variant='outline'
                                    onClick={() => {
                                        handleShowModal(true);
                                    }}
                                >
                                    <Share2 />Share
                                </Button>
                            </div>
                        </div>
                        <Badge>{eventType?.name}</Badge>
                        <p className='mt-4 text-muted'>{description}</p>
                        <div className="!mb-8">
                            <EventPlaceAndLocation {...data} />
                        </div>
                        {latitude && longitude && (
                            <Map
                                mapId='6f295631d841c617'
                                gestureHandling='greedy'
                                disableDefaultUI
                                style={{ width: '100%', height: '500px' }}
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
                            </Map>
                        )}
                    </div>
                    <div className="container mx-auto">
                        <hr />
                        <h2 className='font-semibold font-size-xl mt-5 mb-4'>
                            <span>Attendees ({(eventAttendees || []).length})</span>
                        </h2>
                        <p className='font-semibold m-0 my-4'>
                            Max Number of Participants:
                            <span className='ml-2 color-grey'>{maxNumberOfParticipants}</span>
                        </p>
                        <UsersTable />
                    </div>
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

export default EventDetailWrapper
