import { useCallback, useState } from 'react';
import type EventData from '@/components/Models/EventData';
import type UserData from '@/components/Models/UserData';
import { useDeleteEventAttendee } from '@/hooks/useDeleteEventAttendee';
import { getColumns } from './columns';
import { DataTable } from '@/components/ui/data-table';
import { ShareDialog } from '@/components/sharing';
import { getEventShareableContent, getEventShareMessage } from '@/lib/sharing-messages';

interface EventTableProps {
    currentUser: UserData;
    events: EventData[];
}
export const EventsTable = (props: EventTableProps) => {
    const { events, currentUser } = props;
    const [eventToShare, setEventToShare] = useState<EventData | null>();

    const deleteEventAttendee = useDeleteEventAttendee();

    const setSharingEvent = useCallback((newEventToShare: EventData) => {
        setEventToShare(newEventToShare);
    }, []);

    const handleUnregisterEvent = (id: string, name: string) => {
        if (!window.confirm(`Do you want to remove yourself from this event: ${name}?`)) return;
        deleteEventAttendee.mutateAsync({ eventId: id, userId: currentUser.id });
    };

    const columns = getColumns({
        currentUser,
        onShareEvent: setSharingEvent,
        onUnregisterEvent: handleUnregisterEvent,
    });

    return (
        <div className='overflow-auto'>
            {eventToShare ? (
                <ShareDialog
                    content={getEventShareableContent(eventToShare, currentUser?.id)}
                    open={!!eventToShare}
                    onOpenChange={() => setEventToShare(null)}
                    message={getEventShareMessage(eventToShare, currentUser?.id)}
                />
            ) : null}
            <DataTable columns={columns} data={events || []} />
        </div>
    );
};
