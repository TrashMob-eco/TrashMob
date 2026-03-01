import { ColumnDef } from '@tanstack/react-table';
import { Link } from 'react-router';
import { Edit, Ellipsis, SquareX } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { PledgeStatusBadge, getRecurringFrequencyLabel } from '@/components/contacts/contact-constants';

interface PledgeRow {
    id: string;
    contactId: string;
    contactName: string;
    totalAmount: number;
    startDate: string;
    endDate: string | null;
    frequency: number;
    status: number;
}

interface GetColumnsProps {
    onDelete: (id: string) => void;
}

export const getColumns = ({ onDelete }: GetColumnsProps): ColumnDef<PledgeRow>[] => [
    {
        accessorKey: 'contactName',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Contact' />,
        cell: ({ row }) => (
            <Link to={`/siteadmin/contacts/${row.original.contactId}`} className='font-medium hover:underline'>
                {row.original.contactName || '—'}
            </Link>
        ),
    },
    {
        accessorKey: 'totalAmount',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Total Amount' />,
        cell: ({ row }) => <div className='font-medium'>${row.original.totalAmount.toLocaleString()}</div>,
    },
    {
        accessorKey: 'startDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Start Date' />,
        cell: ({ row }) => (row.original.startDate ? new Date(row.original.startDate).toLocaleDateString() : '—'),
    },
    {
        accessorKey: 'endDate',
        header: 'End Date',
        cell: ({ row }) => (row.original.endDate ? new Date(row.original.endDate).toLocaleDateString() : '—'),
    },
    {
        accessorKey: 'frequency',
        header: 'Frequency',
        cell: ({ row }) => getRecurringFrequencyLabel(row.original.frequency),
    },
    {
        accessorKey: 'status',
        header: 'Status',
        cell: ({ row }) => <PledgeStatusBadge status={row.original.status} />,
    },
    {
        id: 'actions',
        header: () => <div className='text-right'>Actions</div>,
        cell: ({ row }) => (
            <div className='text-right'>
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button variant='ghost' size='icon'>
                            <Ellipsis />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent className='w-56'>
                        <DropdownMenuItem asChild>
                            <Link to={`/siteadmin/pledges/${row.original.id}/edit`}>
                                <Edit /> Edit
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem
                            onClick={() => onDelete(row.original.id)}
                            className='text-destructive focus:text-destructive'
                        >
                            <SquareX /> Delete
                        </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            </div>
        ),
    },
];

export type { PledgeRow };
