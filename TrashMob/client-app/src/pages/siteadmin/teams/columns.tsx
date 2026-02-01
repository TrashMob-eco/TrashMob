import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, Eye, Globe, Lock, RotateCcw, Trash2, Users } from 'lucide-react';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import TeamData from '@/components/Models/TeamData';

interface GetColumnsProps {
    onDelete: (id: string, name: string) => void;
    onReactivate: (id: string, name: string) => void;
}

const getLocation = (team: TeamData) => {
    const parts = [team.city, team.region, team.country].filter(Boolean);
    return parts.join(', ') || '-';
};

export const getColumns = ({ onDelete, onReactivate }: GetColumnsProps): ColumnDef<TeamData>[] => [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
        cell: ({ row }) => (
            <Link to={`/teams/${row.original.id}`} className='text-primary hover:underline font-medium'>
                {row.original.name}
            </Link>
        ),
    },
    {
        id: 'location',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Location' />,
        cell: ({ row }) => getLocation(row.original),
    },
    {
        id: 'visibility',
        header: 'Visibility',
        cell: ({ row }) =>
            row.original.isPublic ? (
                <Badge variant='outline' className='bg-green-100 text-green-800 border-green-300'>
                    <Globe className='h-3 w-3 mr-1' /> Public
                </Badge>
            ) : (
                <Badge variant='outline' className='bg-gray-100 text-gray-800 border-gray-300'>
                    <Lock className='h-3 w-3 mr-1' /> Private
                </Badge>
            ),
    },
    {
        id: 'status',
        header: 'Status',
        cell: ({ row }) =>
            row.original.isActive ? (
                <Badge variant='outline' className='bg-green-100 text-green-800 border-green-300'>
                    Active
                </Badge>
            ) : (
                <Badge variant='outline' className='bg-red-100 text-red-800 border-red-300'>
                    Inactive
                </Badge>
            ),
    },
    {
        accessorKey: 'actions',
        header: () => <div className='text-right'>Actions</div>,
        cell: ({ row }) => {
            const team = row.original;
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
                                <Link to={`/teams/${team.id}`}>
                                    <Eye className='h-4 w-4 mr-2' />
                                    View Team
                                </Link>
                            </DropdownMenuItem>
                            <DropdownMenuItem asChild>
                                <Link to={`/teams/${team.id}/edit`}>
                                    <Users className='h-4 w-4 mr-2' />
                                    Manage Team
                                </Link>
                            </DropdownMenuItem>
                            <DropdownMenuSeparator />
                            {!team.isActive ? (
                                <DropdownMenuItem onClick={() => onReactivate(team.id, team.name)}>
                                    <RotateCcw className='h-4 w-4 mr-2' />
                                    Reactivate Team
                                </DropdownMenuItem>
                            ) : null}
                            <DropdownMenuItem className='text-destructive' onClick={() => onDelete(team.id, team.name)}>
                                <Trash2 className='h-4 w-4 mr-2' />
                                Delete Team
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                </div>
            );
        },
    },
];
