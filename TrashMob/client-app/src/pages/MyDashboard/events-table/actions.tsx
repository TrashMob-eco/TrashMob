import { FileCheck, Ellipsis, Eye, Link2, Pencil, Share2, SquareX, UserRoundX } from 'lucide-react';
import { Link } from 'react-router';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';

import type EventData from '@/components/Models/EventData';
import type UserData from '@/components/Models/UserData';
import { useToast } from '@/hooks/use-toast';

interface EventActionsProps {
    readonly event: EventData;
    readonly currentUser: UserData;
    readonly onShareEvent: (event: EventData) => void;
    readonly onUnregisterEvent: (id: string, name: string) => void;
}

export const EventActions = (props: EventActionsProps) => {
    const { event, currentUser, onShareEvent, onUnregisterEvent } = props;
    const { toast } = useToast();

    const handleCopyLink = (event: EventData) => {
        navigator.clipboard.writeText(`${window.location.origin}/eventdetails/${event.id}`);
        toast({
            variant: 'primary',
            title: `"${event.name}" event link Copied!`,
        });
    };

    const isEventOwner = event.createdByUserId === currentUser.id;
    const isCompleted = new Date(event.eventDate) < new Date();

    return (
        <DropdownMenu>
            <DropdownMenuTrigger asChild>
                <Button size='icon' variant='ghost'>
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
                    <DropdownMenuItem onClick={() => onUnregisterEvent(event.id, currentUser.userName)}>
                        <UserRoundX /> Unregister for event
                    </DropdownMenuItem>
                ) : null}
                <DropdownMenuItem onClick={() => handleCopyLink(event)}>
                    <Link2 /> Copy event link
                </DropdownMenuItem>
                {!isCompleted ? (
                    <DropdownMenuItem onClick={() => onShareEvent(event)}>
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
    );
};
