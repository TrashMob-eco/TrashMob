import { ColumnDef } from '@tanstack/react-table';
import { Link } from 'react-router';
import { Edit, Ellipsis, Eye, SquareX } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { GrantStatusBadge } from '@/components/contacts/contact-constants';

export interface GrantRow {
    id: string;
    funderName: string;
    programName: string;
    status: number;
    submissionDeadline: string | null;
    amountMin: number | null;
    amountMax: number | null;
    amountAwarded: number | null;
}

interface GetColumnsProps {
    onDelete: (id: string) => void;
}

function formatAmount(min: number | null, max: number | null, awarded: number | null): string {
    if (awarded != null) return `$${awarded.toLocaleString()} (awarded)`;
    if (min != null && max != null) return `$${min.toLocaleString()} – $${max.toLocaleString()}`;
    if (min != null) return `$${min.toLocaleString()}+`;
    if (max != null) return `Up to $${max.toLocaleString()}`;
    return '—';
}

export const getColumns = ({ onDelete }: GetColumnsProps): ColumnDef<GrantRow>[] => [
    {
        accessorKey: 'funderName',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Funder' />,
        cell: ({ row }) => (
            <Link to={`/siteadmin/grants/${row.original.id}`} className='font-medium hover:underline'>
                {row.original.funderName}
            </Link>
        ),
    },
    {
        accessorKey: 'programName',
        header: 'Program',
        cell: ({ row }) => row.original.programName || '—',
    },
    {
        accessorKey: 'status',
        header: 'Status',
        cell: ({ row }) => <GrantStatusBadge status={row.original.status} />,
    },
    {
        accessorKey: 'submissionDeadline',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Deadline' />,
        cell: ({ row }) =>
            row.original.submissionDeadline ? new Date(row.original.submissionDeadline).toLocaleDateString() : '—',
    },
    {
        id: 'amount',
        header: 'Amount',
        cell: ({ row }) => (
            <div className='text-right'>
                {formatAmount(row.original.amountMin, row.original.amountMax, row.original.amountAwarded)}
            </div>
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
                            <Link to={`/siteadmin/grants/${row.original.id}`}>
                                <Eye /> View
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem asChild>
                            <Link to={`/siteadmin/grants/${row.original.id}/edit`}>
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
