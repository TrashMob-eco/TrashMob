import _compact from 'lodash/compact';
import type { ColumnDef } from '@tanstack/react-table';
import type EventData from '@/components/Models/EventData';
import type UserData from '@/components/Models/UserData';
import moment from 'moment';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import { EventActions } from './actions';

interface GetColumnsProps {
    currentUser: UserData;
    onShareEvent: (event: EventData) => void;
    onUnregisterEvent: (id: string, name: string) => void;
}

export const getColumns = ({
    currentUser,
    onShareEvent,
    onUnregisterEvent,
}: GetColumnsProps): ColumnDef<EventData>[] => [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
    },
    {
        accessorKey: 'role',
        header: 'Role',
        cell: ({ row }) => {
            const event = row.original;
            return event.createdByUserId === currentUser.id ? 'Lead' : ' Attendee';
        },
    },
    {
        accessorKey: 'eventDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Date' />,
        cell: ({ row }) => {
            return moment(row.getValue('eventDate')).format('MM/DD/YYYY');
        },
    },
    {
        accessorKey: 'eventStartTime',
        header: 'Time',
        cell: ({ row }) => {
            return moment(row.getValue('eventDate')).format('h:mm A');
        },
    },
    {
        accessorKey: 'location',
        header: 'Location',
        cell: ({ row }) => {
            const event = row.original;
            return _compact([event.streetAddress, event.city]).join(', ');
        },
    },
    {
        accessorKey: 'actions',
        header: 'Actions',
        cell: ({ row }) => (
            <EventActions
                event={row.original}
                currentUser={currentUser}
                onShareEvent={onShareEvent}
                onUnregisterEvent={onUnregisterEvent}
            />
        ),
    },
];
