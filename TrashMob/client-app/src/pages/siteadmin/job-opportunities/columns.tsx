import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, Pencil, SquareX } from 'lucide-react';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import JobOpportunityData from '@/components/Models/JobOpportunityData';
import { Badge } from '@/components/ui/badge';
import { Link } from 'react-router';

interface GetColumnsProps {
    onDelete: (id: string, name: string) => void;
}

export const getColumns = ({ onDelete }: GetColumnsProps): ColumnDef<JobOpportunityData>[] => [
    {
        accessorKey: 'title',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Title' />,
    },
    {
        accessorKey: 'tagLine',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Tag Line' />,
    },
    {
        accessorKey: 'fullDescription',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Full Description' />,
    },
    {
        accessorKey: 'isActive',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Is Active' />,
        cell: ({ row }) =>
            row.getValue('isActive') ? (
                <Badge variant='success'>Active</Badge>
            ) : (
                <Badge variant='secondary'>Inactive</Badge>
            ),
    },
    {
        accessorKey: 'actions',
        header: 'Actions',
        cell: ({ row }) => {
            const jobOpportunity = row.original;
            return (
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button variant='ghost' size='icon'>
                            <Ellipsis />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent className='w-56'>
                        <DropdownMenuItem asChild>
                            <Link to={`${jobOpportunity.id}/edit`}>
                                <Pencil />
                                Edit Job
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem onClick={() => onDelete(jobOpportunity.id, jobOpportunity.title)}>
                            <SquareX />
                            Delete Job
                        </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            );
        },
    },
];
