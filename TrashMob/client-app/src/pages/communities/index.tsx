import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { MapPin, Building2, Eye, Map, List, Loader2 } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent } from '@/components/ui/card';
import { DataTable, DataTableColumnHeader } from '@/components/ui/data-table';
import { Button } from '@/components/ui/button';
import { CommunitiesMap } from '@/components/communities/communities-map';
import CommunityData from '@/components/Models/CommunityData';
import { GetCommunities } from '@/services/communities';

type ViewMode = 'list' | 'map';

const getLocation = (community: CommunityData) => {
    const parts = [community.city, community.region, community.country].filter(Boolean);
    return parts.join(', ') || '-';
};

const columns: ColumnDef<CommunityData>[] = [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Community Name' />,
        cell: ({ row }) => (
            <Link
                to={`/communities/${row.original.slug}`}
                className='flex items-center gap-3 text-primary hover:underline'
            >
                {row.original.bannerImageUrl ? (
                    <img
                        src={row.original.bannerImageUrl}
                        alt={`${row.original.name} banner`}
                        className='w-8 h-8 rounded object-cover'
                    />
                ) : (
                    <div className='w-8 h-8 rounded bg-muted flex items-center justify-center'>
                        <Building2 className='h-4 w-4 text-muted-foreground' />
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
        accessorKey: 'tagline',
        header: 'Tagline',
        cell: ({ row }) => (
            <span className='text-sm text-muted-foreground line-clamp-1'>{row.original.tagline || '-'}</span>
        ),
    },
    {
        id: 'actions',
        header: '',
        cell: ({ row }) => (
            <Button variant='ghost' size='sm' asChild>
                <Link to={`/communities/${row.original.slug}`}>
                    <Eye className='h-4 w-4 mr-1' /> View
                </Link>
            </Button>
        ),
    },
];

export const CommunitiesPage = () => {
    const [viewMode, setViewMode] = useState<ViewMode>('list');

    const { data: communities, isLoading } = useQuery<AxiosResponse<CommunityData[]>, unknown, CommunityData[]>({
        queryKey: GetCommunities().key,
        queryFn: GetCommunities().service,
        select: (res) => res.data,
    });

    return (
        <div>
            <HeroSection
                Title='Communities'
                Description='Discover partner communities with dedicated cleanup programs in your area.'
            />
            <div className='container py-8'>
                <div className='flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6'>
                    <div>
                        <h2 className='text-2xl font-semibold'>Browse Communities</h2>
                        <p className='text-muted-foreground'>Find a community near you</p>
                    </div>
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
                </div>

                {isLoading ? (
                    <Card>
                        <CardContent className='py-12'>
                            <div className='flex flex-col items-center justify-center'>
                                <Loader2 className='h-8 w-8 animate-spin text-muted-foreground mb-4' />
                                <p className='text-muted-foreground'>Loading communities...</p>
                            </div>
                        </CardContent>
                    </Card>
                ) : communities && communities.length > 0 ? (
                    viewMode === 'list' ? (
                        <Card>
                            <CardContent className='pt-6'>
                                <DataTable columns={columns} data={communities} />
                            </CardContent>
                        </Card>
                    ) : (
                        <Card>
                            <CardContent className='p-0 overflow-hidden rounded-lg'>
                                <CommunitiesMap
                                    id='communitiesDiscoveryMap'
                                    communities={communities}
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
                                <Building2 className='h-12 w-12 mx-auto text-muted-foreground mb-4' />
                                <h3 className='text-lg font-medium mb-2'>No communities yet</h3>
                                <p className='text-muted-foreground'>
                                    Partner communities will appear here when available.
                                </p>
                            </div>
                        </CardContent>
                    </Card>
                )}
            </div>
        </div>
    );
};

export default CommunitiesPage;
