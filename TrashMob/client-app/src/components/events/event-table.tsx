import { useCallback, useState } from 'react';
import moment from 'moment';
import _compact from 'lodash/compact';
import { Link } from 'react-router';
import { FileCheck, Ellipsis, Eye, Link2, Pencil, Share2, SquareX, UserRoundX } from 'lucide-react';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Button } from '../ui/button';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
import { ShareToSocialsDialog } from '../EventManagement/ShareToSocialsDialog';
import * as SharingMessages from '@/store/SharingMessages';
import { useDeleteEventAttendee } from '@/hooks/useDeleteEventAttendee';

interface EventTableProps {
    events: EventData[];
    currentUser: UserData;
}

export const EventsTable = (props: EventTableProps) => {
    const { events, currentUser } = props;
    const headerTitles = ['Name', 'Role', 'Date', 'Time', 'Location', 'Actions'];

    const [copied, setCopied] = useState<boolean>(false);
    const [eventToShare, setEventToShare] = useState<EventData | null>();

    const deleteEventAttendee = useDeleteEventAttendee();

    const handleCopyLink = (eventId: string) => {
        navigator.clipboard.writeText(`${window.location.origin}/eventdetails/${eventId}`);
        setCopied(true);
        setTimeout(() => {
            setCopied(false);
        }, 2000);
    };

    const setSharingEvent = useCallback((newEventToShare: EventData) => {
        setEventToShare(newEventToShare);
    }, []);

    const handleUnregisterEvent = (id: string, name: string) => {
        if (!window.confirm(`Do you want to remove yourself from this event: ${name}?`)) return;
        deleteEventAttendee.mutateAsync({ eventId: id, userId: currentUser.id });
    };

    return (
        <div className='overflow-auto'>
            {eventToShare ? (
                <ShareToSocialsDialog
                    eventToShare={eventToShare}
                    show={!!eventToShare}
                    handleShow={() => setEventToShare(null)}
                    modalTitle='Share Event'
                    message={SharingMessages.getEventShareMessage(eventToShare, props.currentUser.id)}
                />
            ) : null}
            <Table>
                <TableHeader>
                    <TableRow>
                        {headerTitles.map((header) => (
                            <TableHead key={header}>{header}</TableHead>
                        ))}
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {events.map((event) => {
                        const isEventOwner = event.createdByUserId === props.currentUser.id;
                        const isCompleted = new Date(event.eventDate) < new Date();
                        return (
                            <TableRow key={event.id.toString()}>
                                <TableCell>{event.name}</TableCell>
                                <TableCell>
                                    {event.createdByUserId === props.currentUser.id ? 'Lead' : ' Attendee'}
                                </TableCell>
                                <TableCell>{moment(event.eventDate).format('MM/DD/YYYY')}</TableCell>
                                <TableCell>{moment(event.eventDate).format('hh:mm A')}</TableCell>
                                <TableCell>{_compact([event.streetAddress, event.city]).join(', ')}</TableCell>
                                <TableCell className='btn py-0'>
                                    <DropdownMenu>
                                        <DropdownMenuTrigger asChild>
                                            <Button variant='ghost' size='icon'>
                                                <Ellipsis />
                                            </Button>
                                        </DropdownMenuTrigger>
                                        <DropdownMenuContent className='w-56'>
                                            {isEventOwner && isCompleted ? (
                                                <DropdownMenuItem asChild>
                                                    <Link to={`/eventsummary/${event.id}`}>
                                                        <FileCheck />
                                                        Event Summary
                                                    </Link>
                                                </DropdownMenuItem>
                                            ) : null}
                                            {isEventOwner ? (
                                                <DropdownMenuItem asChild>
                                                    <Link to={`/events/${event.id}/edit`}>
                                                        <Pencil /> Manage event
                                                    </Link>
                                                </DropdownMenuItem>
                                            ) : null}
                                            <DropdownMenuItem asChild>
                                                <Link to={`/eventdetails/${event.id}`}>
                                                    <Eye /> View event
                                                </Link>
                                            </DropdownMenuItem>
                                            {!isEventOwner && !isCompleted ? (
                                                <DropdownMenuItem
                                                    onClick={() =>
                                                        handleUnregisterEvent(event.id, props.currentUser.userName)
                                                    }
                                                >
                                                    <UserRoundX /> Unregister for event
                                                </DropdownMenuItem>
                                            ) : null}
                                            <DropdownMenuItem onClick={() => handleCopyLink(event.id)}>
                                                <Link2 /> {copied ? 'Copied!' : 'Copy event link'}
                                            </DropdownMenuItem>
                                            {!isCompleted ? (
                                                <DropdownMenuItem onClick={() => setSharingEvent(event)}>
                                                    <Share2 /> Share Event
                                                </DropdownMenuItem>
                                            ) : null}
                                            {!isCompleted && isEventOwner ? (
                                                <DropdownMenuItem asChild>
                                                    <Link to={`/cancelevent/${event.id}`}>
                                                        <SquareX /> Cancel event
                                                    </Link>
                                                </DropdownMenuItem>
                                            ) : null}
                                        </DropdownMenuContent>
                                    </DropdownMenu>
                                </TableCell>
                            </TableRow>
                        );
                    })}
                </TableBody>
            </Table>
        </div>
    );
};
