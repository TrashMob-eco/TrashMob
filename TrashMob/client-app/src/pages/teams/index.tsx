import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { MapPin, Users, Eye, Plus, Crown, Globe, Lock } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent } from '@/components/ui/card';
import { DataTable, DataTableColumnHeader } from '@/components/ui/data-table';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import TeamData from '@/components/Models/TeamData';
import { GetPublicTeams } from '@/services/teams';
import { useLogin } from '@/hooks/useLogin';

const getLocation = (team: TeamData) => {
    const parts = [team.city, team.region, team.country].filter(Boolean);
    return parts.join(', ') || '-';
};

const columns: ColumnDef<TeamData>[] = [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Team Name' />,
        cell: ({ row }) => (
            <Link to={`/teams/${row.original.id}`} className='text-primary hover:underline font-medium'>
                {row.original.name}
            </Link>
        ),
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
        header: 'Type',
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
        id: 'joinType',
        header: 'Join',
        cell: ({ row }) =>
            row.original.requiresApproval ? (
                <span className='text-sm text-muted-foreground'>Requires approval</span>
            ) : (
                <span className='text-sm text-green-600'>Open to join</span>
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

export const TeamsPage = () => {
    const { isUserLoaded } = useLogin();

    const { data: teams, isLoading } = useQuery<AxiosResponse<TeamData[]>, unknown, TeamData[]>({
        queryKey: GetPublicTeams().key,
        queryFn: GetPublicTeams().service,
        select: (res) => res.data,
    });

    return (
        <div>
            <HeroSection
                Title='Teams'
                Description='Join a team to participate in cleanup events together and track your collective impact.'
            />
            <div className='container py-8'>
                <div className='flex justify-between items-center mb-6'>
                    <div>
                        <h2 className='text-2xl font-semibold'>Browse Teams</h2>
                        <p className='text-muted-foreground'>Find a team near you to join</p>
                    </div>
                    {isUserLoaded ? (
                        <Button asChild>
                            <Link to='/teams/create'>
                                <Plus className='h-4 w-4 mr-2' /> Create Team
                            </Link>
                        </Button>
                    ) : null}
                </div>

                <Card>
                    <CardContent className='pt-6'>
                        {isLoading ? (
                            <div className='text-center py-8'>Loading teams...</div>
                        ) : teams && teams.length > 0 ? (
                            <DataTable columns={columns} data={teams} />
                        ) : (
                            <div className='text-center py-8'>
                                <Users className='h-12 w-12 mx-auto text-muted-foreground mb-4' />
                                <h3 className='text-lg font-medium mb-2'>No teams yet</h3>
                                <p className='text-muted-foreground mb-4'>
                                    Be the first to create a team and start organizing cleanups together!
                                </p>
                                {isUserLoaded ? (
                                    <Button asChild>
                                        <Link to='/teams/create'>
                                            <Plus className='h-4 w-4 mr-2' /> Create Team
                                        </Link>
                                    </Button>
                                ) : null}
                            </div>
                        )}
                    </CardContent>
                </Card>
            </div>
        </div>
    );
};

export default TeamsPage;
