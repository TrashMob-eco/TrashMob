import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { MapPin, Users, Eye, Crown } from 'lucide-react';

import { DataTable, DataTableColumnHeader } from '@/components/ui/data-table';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import TeamData from '@/components/Models/TeamData';

const getLocation = (team: TeamData) => {
    const parts = [team.city, team.region].filter(Boolean);
    return parts.join(', ') || '-';
};

interface MyTeamsTableProps {
    items: TeamData[];
    teamsILead: TeamData[];
}

export const MyTeamsTable = ({ items, teamsILead }: MyTeamsTableProps) => {
    const leadTeamIds = new Set(teamsILead.map((t) => t.id));

    const columns: ColumnDef<TeamData>[] = [
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
            id: 'role',
            header: 'Role',
            cell: ({ row }) => {
                const isLead = leadTeamIds.has(row.original.id);
                return isLead ? (
                    <Badge variant='outline' className='bg-primary text-white border-0'>
                        <Crown className='h-3 w-3 mr-1' /> Lead
                    </Badge>
                ) : (
                    <Badge variant='outline'>Member</Badge>
                );
            },
        },
        {
            id: 'location',
            header: ({ column }) => <DataTableColumnHeader column={column} title='Location' />,
            cell: ({ row }) => (
                <div className='flex items-center gap-1'>
                    <MapPin className='h-4 w-4 text-muted-foreground' />
                    <span>{getLocation(row.original)}</span>
                </div>
            ),
        },
        {
            id: 'visibility',
            header: 'Visibility',
            cell: ({ row }) => (
                <Badge variant={row.original.isPublic ? 'outline' : 'secondary'}>
                    {row.original.isPublic ? 'Public' : 'Private'}
                </Badge>
            ),
        },
        {
            id: 'actions',
            header: '',
            cell: ({ row }) => (
                <Button variant='ghost' size='sm' asChild>
                    <Link to={`/teams/${row.original.id}`}>
                        <Eye className='h-4 w-4 mr-1' /> View
                    </Link>
                </Button>
            ),
        },
    ];

    if (items.length === 0) {
        return (
            <p className='text-muted-foreground text-center py-4'>
                You are not a member of any teams yet. Browse public teams to join one!
            </p>
        );
    }

    return <DataTable columns={columns} data={items} />;
};
