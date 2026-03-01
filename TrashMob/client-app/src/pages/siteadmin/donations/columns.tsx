import { ColumnDef } from '@tanstack/react-table';
import { Link } from 'react-router';
import { Edit, Ellipsis, SquareX } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { DonationTypeBadge } from '@/components/contacts/contact-constants';

interface DonationRow {
    id: string;
    contactId: string;
    contactName: string;
    amount: number;
    donationDate: string;
    donationType: number;
    campaign: string;
    receiptSent: boolean;
    thankYouSent: boolean;
}

interface GetColumnsProps {
    onDelete: (id: string) => void;
}

export const getColumns = ({ onDelete }: GetColumnsProps): ColumnDef<DonationRow>[] => [
    {
        accessorKey: 'donationDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Date' />,
        cell: ({ row }) =>
            row.original.donationDate ? new Date(row.original.donationDate).toLocaleDateString() : '—',
    },
    {
        accessorKey: 'contactName',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Contact' />,
        cell: ({ row }) => (
            <Link
                to={`/siteadmin/contacts/${row.original.contactId}`}
                className='font-medium hover:underline'
            >
                {row.original.contactName || '—'}
            </Link>
        ),
    },
    {
        accessorKey: 'donationType',
        header: 'Type',
        cell: ({ row }) => <DonationTypeBadge type={row.original.donationType} />,
    },
    {
        accessorKey: 'campaign',
        header: 'Campaign',
        cell: ({ row }) => row.original.campaign || '—',
    },
    {
        accessorKey: 'amount',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Amount' />,
        cell: ({ row }) => (
            <div className='text-right font-medium'>${row.original.amount.toLocaleString()}</div>
        ),
    },
    {
        accessorKey: 'receiptSent',
        header: 'Receipt',
        cell: ({ row }) =>
            row.original.receiptSent ? (
                <Badge variant='success'>Sent</Badge>
            ) : (
                <Badge variant='secondary'>No</Badge>
            ),
    },
    {
        accessorKey: 'thankYouSent',
        header: 'Thank You',
        cell: ({ row }) =>
            row.original.thankYouSent ? (
                <Badge variant='success'>Sent</Badge>
            ) : (
                <Badge variant='secondary'>No</Badge>
            ),
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
                            <Link to={`/siteadmin/donations/${row.original.id}/edit`}>
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

export type { DonationRow };
