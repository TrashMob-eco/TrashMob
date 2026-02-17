import { useState } from 'react';
import { Calendar, ClipboardCheck, MapPin } from 'lucide-react';
import AdoptableAreaData from '@/components/Models/AdoptableAreaData';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { AdoptAreaDialog } from './adopt-area-dialog';

const statusVariant: Record<string, 'success' | 'default' | 'secondary'> = {
    Available: 'success',
    Adopted: 'default',
    Unavailable: 'secondary',
};

interface CommunityAreasSectionProps {
    areas: AdoptableAreaData[];
    isLoading?: boolean;
    communityId: string;
}

export const CommunityAreasSection = ({ areas, isLoading, communityId }: CommunityAreasSectionProps) => {
    const [selectedArea, setSelectedArea] = useState<AdoptableAreaData | null>(null);

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
                <CardHeader>
                    <CardTitle className='text-lg'>Adoptable Areas</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className='space-y-4'>
                        {areas.slice(0, 6).map((area) => (
                            <div key={area.id} className='p-3 rounded-lg border space-y-2'>
                                <div className='flex items-center justify-between gap-2'>
                                    <div className='flex items-center gap-2 min-w-0'>
                                        <h4 className='font-medium text-sm truncate'>{area.name}</h4>
                                        <Badge variant='outline'>{area.areaType}</Badge>
                                        <Badge variant={statusVariant[area.status] ?? 'secondary'}>{area.status}</Badge>
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
                        {areas.length > 6 ? (
                            <p className='text-xs text-muted-foreground text-center'>+ {areas.length - 6} more areas</p>
                        ) : null}
                    </div>
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
