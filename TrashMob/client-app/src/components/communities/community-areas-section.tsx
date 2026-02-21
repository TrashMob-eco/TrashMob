import { useState } from 'react';
import { Calendar, ClipboardCheck, List, Map as MapIcon, MapPin, Search, X } from 'lucide-react';
import AdoptableAreaData from '@/components/Models/AdoptableAreaData';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { GoogleMapWithKey } from '@/components/Map/GoogleMap';
import { ExistingAreasOverlay } from '@/components/Map/AreaMapEditor/ExistingAreasOverlay';
import { CommunityBoundsOverlay } from '@/components/Map/CommunityBoundsOverlay';
import { AreaStatusLegend } from '@/components/Map/AreaMapEditor/AreaStatusLegend';
import { AdoptAreaDialog } from './adopt-area-dialog';
import { useAreaFilters, AREA_TYPES, AREA_STATUSES } from '@/hooks/useAreaFilters';

const COMMUNITY_AREAS_MAP_ID = 'communityAreasMap';

const statusVariant: Record<string, 'success' | 'default' | 'secondary'> = {
    Available: 'success',
    Adopted: 'default',
    Unavailable: 'secondary',
};

interface CommunityAreasSectionProps {
    areas: AdoptableAreaData[];
    isLoading?: boolean;
    communityId: string;
    boundaryGeoJson?: string;
}

export const CommunityAreasSection = ({
    areas,
    isLoading,
    communityId,
    boundaryGeoJson,
}: CommunityAreasSectionProps) => {
    const [selectedArea, setSelectedArea] = useState<AdoptableAreaData | null>(null);
    const [viewMode, setViewMode] = useState<'list' | 'map'>('list');
    const { search, setSearch, areaType, setAreaType, status, setStatus, filteredAreas, totalCount, hasActiveFilters } =
        useAreaFilters(areas);

    if (isLoading) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle className='text-lg'>Adoptable Areas</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className='space-y-3'>
                        {[1, 2, 3].map((i) => (
                            <div key={i} className='animate-pulse'>
                                <div className='h-4 bg-muted rounded w-3/4 mb-2' />
                                <div className='h-3 bg-muted rounded w-1/2' />
                            </div>
                        ))}
                    </div>
                </CardContent>
            </Card>
        );
    }

    if (areas.length === 0) return null;

    const canAdopt = (area: AdoptableAreaData) =>
        area.status === 'Available' || (area.status === 'Adopted' && area.allowCoAdoption);

    return (
        <>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <CardTitle className='text-lg'>Adoptable Areas</CardTitle>
                    <div className='flex border rounded-md'>
                        <Button
                            variant={viewMode === 'list' ? 'default' : 'ghost'}
                            size='sm'
                            className='rounded-r-none'
                            onClick={() => setViewMode('list')}
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
                </CardHeader>
                <CardContent>
                    <div className='flex flex-wrap items-center gap-2 pb-4'>
                        <div className='relative flex-1 min-w-[180px] max-w-xs'>
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
                            <SelectTrigger className='w-[150px]'>
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
                            <SelectTrigger className='w-[150px]'>
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
                    {filteredAreas.length === 0 && hasActiveFilters ? (
                        <div className='text-center py-8 text-muted-foreground'>
                            <p>No areas match your filters.</p>
                        </div>
                    ) : viewMode === 'map' ? (
                        <div className='space-y-2'>
                            <div className='rounded-md overflow-hidden border'>
                                <GoogleMapWithKey
                                    id={COMMUNITY_AREAS_MAP_ID}
                                    style={{ width: '100%', height: '400px' }}
                                >
                                    {boundaryGeoJson ? (
                                        <CommunityBoundsOverlay
                                            mapId={COMMUNITY_AREAS_MAP_ID}
                                            geoJson={boundaryGeoJson}
                                        />
                                    ) : null}
                                    <ExistingAreasOverlay
                                        mapId={COMMUNITY_AREAS_MAP_ID}
                                        areas={filteredAreas}
                                        fitBounds
                                    />
                                </GoogleMapWithKey>
                            </div>
                            <AreaStatusLegend />
                        </div>
                    ) : (
                        <div className='space-y-4'>
                            {filteredAreas.map((area) => (
                                <div key={area.id} className='p-3 rounded-lg border space-y-2'>
                                    <div className='flex items-center justify-between gap-2'>
                                        <div className='flex items-center gap-2 min-w-0'>
                                            <h4 className='font-medium text-sm truncate'>{area.name}</h4>
                                            <Badge variant='outline'>{area.areaType}</Badge>
                                            <Badge variant={statusVariant[area.status] ?? 'secondary'}>
                                                {area.status}
                                            </Badge>
                                        </div>
                                        {canAdopt(area) ? (
                                            <Button size='sm' variant='outline' onClick={() => setSelectedArea(area)}>
                                                <MapPin className='h-3 w-3 mr-1' /> Adopt
                                            </Button>
                                        ) : null}
                                    </div>
                                    {area.description ? (
                                        <p className='text-xs text-muted-foreground line-clamp-2'>{area.description}</p>
                                    ) : null}
                                    <div className='flex gap-4 text-xs text-muted-foreground'>
                                        <div className='flex items-center gap-1'>
                                            <Calendar className='h-3 w-3' />
                                            <span>Every {area.cleanupFrequencyDays} days</span>
                                        </div>
                                        <div className='flex items-center gap-1'>
                                            <ClipboardCheck className='h-3 w-3' />
                                            <span>Min {area.minEventsPerYear} events/year</span>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </CardContent>
            </Card>

            {selectedArea ? (
                <AdoptAreaDialog
                    area={selectedArea}
                    communityId={communityId}
                    open={!!selectedArea}
                    onOpenChange={(open) => {
                        if (!open) setSelectedArea(null);
                    }}
                />
            ) : null}
        </>
    );
};
