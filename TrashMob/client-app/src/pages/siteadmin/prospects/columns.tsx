import moment from 'moment';
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
        header: 'Primary Contact',
        cell: ({ row }) => {
            // contactName / contactEmail are the legacy passthrough fields populated from
            // the primary ProspectContact (Project 60 backend mapper). Once the frontend
            // adopts the contacts API everywhere, these can come from row.original.contacts
            // directly.
            const name = row.original.contactName;
            const email = row.original.contactEmail;
            const contactCount = row.original.contacts?.length ?? 0;
            if (!name && !email) {
                return contactCount > 0 ? `${contactCount} contact${contactCount === 1 ? '' : 's'}` : '-';
            }
            return (
                <div className='flex flex-col text-sm'>
                    <span>{name || email}</span>
                    {name && email ? <span className='text-xs text-muted-foreground'>{email}</span> : null}
                    {contactCount > 1 ? (
                        <span className='text-xs text-muted-foreground'>+{contactCount - 1} more</span>
                    ) : null}
                </div>
            );
        },
    },
    {
        accessorKey: 'createdDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Days in Pipeline' />,
        cell: ({ row }) => {
            const createdDate = row.original.createdDate;
            if (!createdDate) return '-';
            const days = Math.max(0, moment().diff(moment(createdDate), 'days'));
            return <span className='text-sm'>{days}</span>;
        },
        sortingFn: (a, b) => {
            const aDate = a.original.createdDate ? moment(a.original.createdDate).valueOf() : 0;
            const bDate = b.original.createdDate ? moment(b.original.createdDate).valueOf() : 0;
            // Newer prospects have HIGHER createdDate but LOWER days-in-pipeline,
            // so flip the comparison to sort by days ascending when the user clicks the header.
            return bDate - aDate;
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
