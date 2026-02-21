import { useCallback, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import {
    Loader2,
    Plus,
    Pencil,
    Trash2,
    MapPin,
    Upload,
    Download,
    List,
    Map as MapIcon,
    Sparkles,
    MoreHorizontal,
    Search,
    X,
} from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
    AlertDialogTrigger,
} from '@/components/ui/alert-dialog';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { useToast } from '@/hooks/use-toast';
import { useAreaFilters, AREA_TYPES, AREA_STATUSES } from '@/hooks/useAreaFilters';
import AdoptableAreaData, { AdoptableAreaStatus } from '@/components/Models/AdoptableAreaData';
import CommunityData from '@/components/Models/CommunityData';
import { GetAdoptableAreas, DeleteAdoptableArea, ClearAllAreas, ExportAreas } from '@/services/adoptable-areas';
import { GetCommunityForAdmin } from '@/services/communities';
import { GoogleMapWithKey } from '@/components/Map/GoogleMap';
import { ExistingAreasOverlay } from '@/components/Map/AreaMapEditor/ExistingAreasOverlay';
import { CommunityBoundsOverlay } from '@/components/Map/CommunityBoundsOverlay';
import { AreaStatusLegend } from '@/components/Map/AreaMapEditor/AreaStatusLegend';

const statusColors: Record<AdoptableAreaStatus, string> = {
    Available: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    Adopted: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    Unavailable: 'bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-200',
};

const OVERVIEW_MAP_ID = 'areasOverviewMap';

export const PartnerCommunityAreas = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const { toast } = useToast();
    const [viewMode, setViewMode] = useState<'table' | 'map'>('table');
    const [clearAllDialogOpen, setClearAllDialogOpen] = useState(false);

    const { data: community } = useQuery<AxiosResponse<CommunityData>, unknown, CommunityData>({
        queryKey: GetCommunityForAdmin({ communityId: partnerId }).key,
        queryFn: GetCommunityForAdmin({ communityId: partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const { data: areas, isLoading } = useQuery<AxiosResponse<AdoptableAreaData[]>, unknown, AdoptableAreaData[]>({
        queryKey: GetAdoptableAreas({ partnerId }).key,
        queryFn: GetAdoptableAreas({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const { search, setSearch, areaType, setAreaType, status, setStatus, filteredAreas, totalCount, hasActiveFilters } =
        useAreaFilters(areas ?? []);

    const { mutate: deleteArea, isPending: isDeleting } = useMutation({
        mutationKey: DeleteAdoptableArea().key,
        mutationFn: DeleteAdoptableArea().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetAdoptableAreas({ partnerId }).key,
            });
            toast({
                variant: 'primary',
                title: 'Area deleted',
                description: 'The adoptable area has been removed.',
            });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to delete area. Please try again.',
            });
        },
    });

    const handleDelete = useCallback(
        (areaId: string) => {
            if (!partnerId) return;
            deleteArea({ partnerId, areaId });
        },
        [partnerId, deleteArea],
    );

    const { mutate: clearAll, isPending: isClearing } = useMutation({
        mutationKey: ClearAllAreas().key,
        mutationFn: ClearAllAreas().service,
        onSuccess: (response) => {
            queryClient.invalidateQueries({
                queryKey: GetAdoptableAreas({ partnerId }).key,
            });
            const data = response.data;
            toast({
                variant: 'primary',
                title: 'All areas cleared',
                description: `Removed ${data.areasRemoved} areas, ${data.batchesDeleted} generation batches, and ${data.stagedAreasDeleted} staged areas.`,
            });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to clear areas. Please try again.',
            });
        },
    });

    const handleClearAll = useCallback(() => {
        if (!partnerId) return;
        clearAll({ partnerId });
    }, [partnerId, clearAll]);

    const handleExport = useCallback(
        async (format: 'geojson' | 'kml') => {
            if (!partnerId) return;
            try {
                const response = await ExportAreas({ partnerId, format }).service();
                const blob = response.data;
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = url;
                link.download = `Areas_${new Date().toISOString().split('T')[0]}.${format}`;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                window.URL.revokeObjectURL(url);
                toast({ variant: 'primary', title: 'Export successful' });
            } catch {
                toast({ variant: 'destructive', title: 'Export failed', description: 'Please try again.' });
            }
        },
        [partnerId, toast],
    );

    if (isLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    const hasAreas = areas && areas.length > 0;

    return (
        <div className='py-8'>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <div>
                        <CardTitle className='flex items-center gap-2'>
                            <MapPin className='h-5 w-5' />
                            Adoptable Areas
                        </CardTitle>
                        <CardDescription>
                            Manage geographic areas available for team adoption within your community.
                        </CardDescription>
                    </div>
                    <div className='flex gap-2'>
                        {hasAreas ? (
                            <div className='flex border rounded-md'>
                                <Button
                                    variant={viewMode === 'table' ? 'default' : 'ghost'}
                                    size='sm'
                                    className='rounded-r-none'
                                    onClick={() => setViewMode('table')}
                                >
                                    <List className='h-4 w-4' />
                                </Button>
                                <Button
                                    variant={viewMode === 'map' ? 'default' : 'ghost'}
                                    size='sm'
                                    className='rounded-l-none'
                                    onClick={() => setViewMode('map')}
                                >
                                    <MapIcon className='h-4 w-4' />
                                </Button>
                            </div>
                        ) : null}
                        <Button
                            variant='outline'
                            onClick={() => navigate(`/partnerdashboard/${partnerId}/community/areas/generate`)}
                        >
                            <Sparkles className='h-4 w-4 mr-2' />
                            Generate
                        </Button>
                        <DropdownMenu>
                            <DropdownMenuTrigger asChild>
                                <Button variant='outline'>
                                    <MoreHorizontal className='h-4 w-4' />
                                </Button>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent align='end'>
                                <DropdownMenuItem
                                    onClick={() => navigate(`/partnerdashboard/${partnerId}/community/areas/import`)}
                                >
                                    <Upload className='h-4 w-4 mr-2' />
                                    Import
                                </DropdownMenuItem>
                                {hasAreas ? (
                                    <>
                                        <DropdownMenuItem onClick={() => handleExport('geojson')}>
                                            <Download className='h-4 w-4 mr-2' />
                                            Export as GeoJSON
                                        </DropdownMenuItem>
                                        <DropdownMenuItem onClick={() => handleExport('kml')}>
                                            <Download className='h-4 w-4 mr-2' />
                                            Export as KML
                                        </DropdownMenuItem>
                                        <DropdownMenuSeparator />
                                        <DropdownMenuItem
                                            className='text-destructive focus:text-destructive'
                                            disabled={isClearing}
                                            onClick={() => setClearAllDialogOpen(true)}
                                        >
                                            <Trash2 className='h-4 w-4 mr-2' />
                                            Clear All Areas
                                        </DropdownMenuItem>
                                    </>
                                ) : null}
                            </DropdownMenuContent>
                        </DropdownMenu>
                        <Button onClick={() => navigate(`/partnerdashboard/${partnerId}/community/areas/create`)}>
                            <Plus className='h-4 w-4 mr-2' />
                            Add Area
                        </Button>
                    </div>
                    <AlertDialog open={clearAllDialogOpen} onOpenChange={setClearAllDialogOpen}>
                        <AlertDialogContent>
                            <AlertDialogHeader>
                                <AlertDialogTitle>Clear All Areas?</AlertDialogTitle>
                                <AlertDialogDescription>
                                    This will permanently remove ALL {areas?.length ?? 0} adoptable areas and delete all
                                    AI generation history (batches and staged areas) for this community. This action
                                    cannot be undone. Are you REALLY sure?
                                </AlertDialogDescription>
                            </AlertDialogHeader>
                            <AlertDialogFooter>
                                <AlertDialogCancel>Cancel</AlertDialogCancel>
                                <AlertDialogAction
                                    className='bg-destructive text-destructive-foreground hover:bg-destructive/90'
                                    onClick={handleClearAll}
                                >
                                    Yes, Clear Everything
                                </AlertDialogAction>
                            </AlertDialogFooter>
                        </AlertDialogContent>
                    </AlertDialog>
                </CardHeader>
                <CardContent>
                    {hasAreas ? (
                        <>
                            <div className='flex flex-wrap items-center gap-2 pb-4'>
                                <div className='relative flex-1 min-w-[200px] max-w-sm'>
                                    <Search className='absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground' />
                                    <Input
                                        placeholder='Search areas...'
                                        value={search}
                                        onChange={(e) => setSearch(e.target.value)}
                                        className='pl-8 pr-8'
                                    />
                                    {search ? (
                                        <Button
                                            variant='ghost'
                                            size='sm'
                                            className='absolute right-0 top-0 h-full px-2 hover:bg-transparent'
                                            onClick={() => setSearch('')}
                                        >
                                            <X className='h-4 w-4' />
                                            <span className='sr-only'>Clear search</span>
                                        </Button>
                                    ) : null}
                                </div>
                                <Select value={areaType} onValueChange={setAreaType}>
                                    <SelectTrigger className='w-[160px]'>
                                        <SelectValue placeholder='Area type' />
                                    </SelectTrigger>
                                    <SelectContent>
                                        <SelectItem value='all'>All Types</SelectItem>
                                        {AREA_TYPES.map((t) => (
                                            <SelectItem key={t} value={t}>
                                                {t}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                                <Select value={status} onValueChange={setStatus}>
                                    <SelectTrigger className='w-[160px]'>
                                        <SelectValue placeholder='Status' />
                                    </SelectTrigger>
                                    <SelectContent>
                                        <SelectItem value='all'>All Statuses</SelectItem>
                                        {AREA_STATUSES.map((s) => (
                                            <SelectItem key={s} value={s}>
                                                {s}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                                {hasActiveFilters ? (
                                    <span className='text-sm text-muted-foreground'>
                                        {filteredAreas.length} of {totalCount} areas
                                    </span>
                                ) : null}
                            </div>
                            {filteredAreas.length === 0 ? (
                                <div className='text-center py-8 text-muted-foreground'>
                                    <p>No areas match your filters.</p>
                                </div>
                            ) : viewMode === 'map' ? (
                                <div className='space-y-2'>
                                    <div className='h-[500px] rounded-md overflow-hidden border'>
                                        <GoogleMapWithKey
                                            id={OVERVIEW_MAP_ID}
                                            style={{ width: '100%', height: '500px' }}
                                        >
                                            {community?.boundaryGeoJson ? (
                                                <CommunityBoundsOverlay
                                                    mapId={OVERVIEW_MAP_ID}
                                                    geoJson={community.boundaryGeoJson}
                                                />
                                            ) : null}
                                            <ExistingAreasOverlay
                                                mapId={OVERVIEW_MAP_ID}
                                                areas={filteredAreas}
                                                fitBounds
                                            />
                                        </GoogleMapWithKey>
                                    </div>
                                    <AreaStatusLegend />
                                </div>
                            ) : (
                                <Table>
                                    <TableHeader>
                                        <TableRow>
                                            <TableHead>Name</TableHead>
                                            <TableHead>Type</TableHead>
                                            <TableHead>Status</TableHead>
                                            <TableHead>Frequency</TableHead>
                                            <TableHead>Min Events/Year</TableHead>
                                            <TableHead className='text-right'>Actions</TableHead>
                                        </TableRow>
                                    </TableHeader>
                                    <TableBody>
                                        {filteredAreas.map((area) => (
                                            <TableRow key={area.id}>
                                                <TableCell className='font-medium'>{area.name}</TableCell>
                                                <TableCell>{area.areaType}</TableCell>
                                                <TableCell>
                                                    <Badge className={statusColors[area.status]}>{area.status}</Badge>
                                                </TableCell>
                                                <TableCell>{area.cleanupFrequencyDays} days</TableCell>
                                                <TableCell>{area.minEventsPerYear}</TableCell>
                                                <TableCell className='text-right'>
                                                    <div className='flex justify-end gap-2'>
                                                        <Button variant='outline' size='sm' asChild>
                                                            <Link
                                                                to={`/partnerdashboard/${partnerId}/community/areas/${area.id}/edit`}
                                                            >
                                                                <Pencil className='h-4 w-4' />
                                                            </Link>
                                                        </Button>
                                                        <AlertDialog>
                                                            <AlertDialogTrigger asChild>
                                                                <Button
                                                                    variant='outline'
                                                                    size='sm'
                                                                    disabled={isDeleting}
                                                                >
                                                                    <Trash2 className='h-4 w-4' />
                                                                </Button>
                                                            </AlertDialogTrigger>
                                                            <AlertDialogContent>
                                                                <AlertDialogHeader>
                                                                    <AlertDialogTitle>Delete Area</AlertDialogTitle>
                                                                    <AlertDialogDescription>
                                                                        Are you sure you want to delete "{area.name}"?
                                                                        This action cannot be undone.
                                                                    </AlertDialogDescription>
                                                                </AlertDialogHeader>
                                                                <AlertDialogFooter>
                                                                    <AlertDialogCancel>Cancel</AlertDialogCancel>
                                                                    <AlertDialogAction
                                                                        onClick={() => handleDelete(area.id)}
                                                                    >
                                                                        Delete
                                                                    </AlertDialogAction>
                                                                </AlertDialogFooter>
                                                            </AlertDialogContent>
                                                        </AlertDialog>
                                                    </div>
                                                </TableCell>
                                            </TableRow>
                                        ))}
                                    </TableBody>
                                </Table>
                            )}
                        </>
                    ) : (
                        <div className='text-center py-12'>
                            <MapPin className='h-12 w-12 mx-auto text-muted-foreground mb-4' />
                            <h3 className='text-lg font-medium mb-2'>No adoptable areas yet</h3>
                            <p className='text-muted-foreground mb-4'>
                                Create your first adoptable area to allow teams to adopt and care for locations in your
                                community.
                            </p>
                            <Button onClick={() => navigate(`/partnerdashboard/${partnerId}/community/areas/create`)}>
                                <Plus className='h-4 w-4 mr-2' />
                                Create First Area
                            </Button>
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    );
};

export default PartnerCommunityAreas;
