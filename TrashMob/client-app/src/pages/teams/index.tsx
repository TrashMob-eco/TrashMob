import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { MapPin, Users, Eye, Plus, Globe, Lock, Map, List, Loader2 } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent } from '@/components/ui/card';
import { DataTable, DataTableColumnHeader } from '@/components/ui/data-table';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { TeamsMap } from '@/components/teams/teams-map';
import TeamData from '@/components/Models/TeamData';
import { GetPublicTeams } from '@/services/teams';
import { useLogin } from '@/hooks/useLogin';

type ViewMode = 'list' | 'map';

const getLocation = (team: TeamData) => {
    const parts = [team.city, team.region, team.country].filter(Boolean);
    return parts.join(', ') || '-';
};

const columns: ColumnDef<TeamData>[] = [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Team Name' />,
        cell: ({ row }) => (
            <Link to={`/teams/${row.original.id}`} className='flex items-center gap-3 text-primary hover:underline'>
                {row.original.logoUrl ? (
                    <img
                        src={row.original.logoUrl}
                        alt={`${row.original.name} logo`}
                        className='w-8 h-8 rounded object-cover'
                    />
                ) : (
                    <div className='w-8 h-8 rounded bg-muted flex items-center justify-center'>
                        <Users className='h-4 w-4 text-muted-foreground' />
                    </div>
                )}
                <span className='font-medium'>{row.original.name}</span>
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
    const [viewMode, setViewMode] = useState<ViewMode>('list');

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
                <div className='flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6'>
                    <div>
                        <h2 className='text-2xl font-semibold'>Browse Teams</h2>
                        <p className='text-muted-foreground'>Find a team near you to join</p>
                    </div>
                    <div className='flex items-center gap-3'>
                        <div className='flex rounded-lg border p-1'>
                            <Button
                                variant={viewMode === 'list' ? 'secondary' : 'ghost'}
                                size='sm'
                                onClick={() => setViewMode('list')}
                            >
                                <List className='h-4 w-4 mr-1' /> List
                            </Button>
                            <Button
                                variant={viewMode === 'map' ? 'secondary' : 'ghost'}
                                size='sm'
                                onClick={() => setViewMode('map')}
                            >
                                <Map className='h-4 w-4 mr-1' /> Map
                            </Button>
                        </div>
                        {isUserLoaded ? (
                            <Button asChild>
                                <Link to='/teams/create'>
                                    <Plus className='h-4 w-4 mr-2' /> Create Team
                                </Link>
                            </Button>
                        ) : null}
                    </div>
                </div>

                {isLoading ? (
                    <Card>
                        <CardContent className='py-12'>
                            <div className='flex flex-col items-center justify-center'>
                                <Loader2 className='h-8 w-8 animate-spin text-muted-foreground mb-4' />
                                <p className='text-muted-foreground'>Loading teams...</p>
                            </div>
                        </CardContent>
                    </Card>
                ) : teams && teams.length > 0 ? (
                    viewMode === 'list' ? (
                        <Card>
                            <CardContent className='pt-6'>
                                <DataTable columns={columns} data={teams} />
                            </CardContent>
                        </Card>
                    ) : (
                        <Card>
                            <CardContent className='p-0 overflow-hidden rounded-lg'>
                                <TeamsMap
                                    id='teamsDiscoveryMap'
                                    teams={teams}
                                    style={{ width: '100%', height: '500px' }}
                                    gestureHandling='cooperative'
                                />
                            </CardContent>
                        </Card>
                    )
                ) : (
                    <Card>
                        <CardContent className='py-12'>
                            <div className='text-center'>
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
                        </CardContent>
                    </Card>
                )}
            </div>
        </div>
    );
};

export default TeamsPage;
