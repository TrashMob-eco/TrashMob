import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, Eye, Edit, SquareX } from 'lucide-react';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import { PipelineStageBadge } from '@/components/prospects/pipeline-stage-badge';
import CommunityProspectData from '@/components/Models/CommunityProspectData';

interface GetColumnsProps {
    onDelete: (id: string, name: string) => void;
}

export const getColumns = ({ onDelete }: GetColumnsProps): ColumnDef<CommunityProspectData>[] => [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
        cell: ({ row }) => (
            <Link to={`/siteadmin/prospects/${row.original.id}`} className='font-medium hover:underline'>
                {row.getValue('name')}
            </Link>
        ),
    },
    {
        accessorKey: 'type',
        header: 'Type',
    },
    {
        accessorKey: 'city',
        header: 'City / Region',
        cell: ({ row }) => {
            const city = row.original.city;
            const region = row.original.region;
            return [city, region].filter(Boolean).join(', ') || '-';
        },
    },
    {
        accessorKey: 'pipelineStage',
        header: 'Stage',
        cell: ({ row }) => <PipelineStageBadge stage={row.getValue('pipelineStage')} />,
    },
    {
        accessorKey: 'fitScore',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Fit Score' />,
    },
    {
        accessorKey: 'contactEmail',
        header: 'Contact',
        cell: ({ row }) => {
            const name = row.original.contactName;
            const email = row.original.contactEmail;
            if (name && email) return `${name} (${email})`;
            return name || email || '-';
        },
    },
    {
        accessorKey: 'actions',
        header: () => <div className='text-right'>Actions</div>,
        cell: ({ row }) => {
            const prospect = row.original;
            return (
                <div className='text-right'>
                    <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                            <Button variant='ghost' size='icon'>
                                <Ellipsis />
                            </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent className='w-56'>
                            <DropdownMenuItem asChild>
                                <Link to={`/siteadmin/prospects/${prospect.id}`}>
                                    <Eye /> View details
                                </Link>
                            </DropdownMenuItem>
                            <DropdownMenuItem asChild>
                                <Link to={`/siteadmin/prospects/${prospect.id}/edit`}>
                                    <Edit /> Edit
                                </Link>
                            </DropdownMenuItem>
                            <DropdownMenuSeparator />
                            <DropdownMenuItem
                                onClick={() => onDelete(prospect.id, prospect.name)}
                                className='text-destructive focus:text-destructive'
                            >
                                <SquareX /> Delete
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                </div>
            );
        },
    },
];
