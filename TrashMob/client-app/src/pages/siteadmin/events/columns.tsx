import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, Eye, Pencil, SquareX } from 'lucide-react';
import { EventStatusBadge } from '@/components/events/event-status-badge';
import EventData from '@/components/Models/EventData';
import moment from 'moment';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/data-table';

export const columns: ColumnDef<EventData>[] = [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
    },
    {
        accessorKey: 'eventStatusId',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Status' />,
        cell: ({ row }) => {
            return <EventStatusBadge statusId={row.getValue('eventStatusId')} />;
        },
    },
    {
        accessorKey: 'eventDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Date' />,
        cell: ({ row }) => {
            return moment(row.getValue('eventDate')).format('lll');
        },
    },
    {
        accessorKey: 'city',
        header: 'City',
    },
    {
        accessorKey: 'region',
        header: 'Region',
    },
    {
        accessorKey: 'country',
        header: 'Country',
    },
    {
        accessorKey: 'postalCode',
        header: 'Postal Code',
    },
    {
        accessorKey: 'actions',
        header: 'Actions',
        cell: ({ row }) => {
            const event = row.original;

            return (
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button size='icon' variant='ghost'>
                            <Ellipsis />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent className='w-56'>
                        <DropdownMenuItem asChild>
                            <Link to={`/events/${event.id}/edit`}>
                                <Pencil />
                                Manage Event
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem asChild>
                            <Link to={`/cancelevent/${event.id}`}>
                                <SquareX />
                                Delete Event
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem asChild>
                            <Link to={`/eventdetails/${event.id}`}>
                                <Eye />
                                View Event
                            </Link>
                        </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            );
        },
    },
];
