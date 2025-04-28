import { useCallback, useState } from 'react';
import type EventData from '@/components/Models/EventData';
import type UserData from '@/components/Models/UserData';
import { useDeleteEventAttendee } from '@/hooks/useDeleteEventAttendee';
import { getColumns } from './columns';
import { DataTable } from '@/components/ui/data-table';
import { ShareToSocialsDialog } from '@/components/EventManagement/ShareToSocialsDialog';
import * as SharingMessages from '@/store/SharingMessages';

interface EventTableProps {
    readonly currentUser: UserData;
    readonly events: EventData[];
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
                <ShareToSocialsDialog
                    eventToShare={eventToShare}
                    handleShow={() => setEventToShare(null)}
                    message={SharingMessages.getEventShareMessage(eventToShare, props.currentUser.id)}
                    modalTitle='Share Event'
                    show={!!eventToShare}
                />
            ) : null}
            <DataTable columns={columns} data={events || []} />
        </div>
    );
};
