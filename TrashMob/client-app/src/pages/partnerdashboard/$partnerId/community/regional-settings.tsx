import { useCallback, useEffect, useState } from 'react';
import { useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2, MapPin } from 'lucide-react';

import { Form, FormControl, FormField, FormItem, FormMessage, FormDescription } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useToast } from '@/hooks/use-toast';
import CommunityData from '@/components/Models/CommunityData';
import { GetCommunityForAdmin, UpdateCommunityContent, SuggestCommunityBounds } from '@/services/communities';
import { GetPartnerById } from '@/services/partners';
import { RegionType } from '@/lib/community-utils';
import { BoundsPreviewMap } from '@/components/communities/bounds-preview-map';

interface FormInputs {
    regionType: string;
    countyName: string;
}

const formSchema = z.object({
    regionType: z.string(),
    countyName: z.string().max(256, 'County name must be less than 256 characters'),
});

const regionTypeOptions = [
    { value: String(RegionType.City), label: 'City' },
    { value: String(RegionType.County), label: 'County' },
    { value: String(RegionType.State), label: 'State' },
    { value: String(RegionType.Province), label: 'Province' },
    { value: String(RegionType.Region), label: 'Region' },
    { value: String(RegionType.Country), label: 'Country' },
];

const toStr = (v: number | null | undefined): string => (v != null ? String(v) : '');

export const CommunityRegionalSettings = () => {
    const queryClient = useQueryClient();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const { toast } = useToast();

    // Derived geographic values stored in state (not in form — auto-detected from Nominatim)
    const [boundaryGeoJson, setBoundaryGeoJson] = useState<string>('');
    const [derivedBounds, setDerivedBounds] = useState<{
        north: number | null;
        south: number | null;
        east: number | null;
        west: number | null;
        centerLat: number | null;
        centerLng: number | null;
    }>({ north: null, south: null, east: null, west: null, centerLat: null, centerLng: null });

    const { data: currentValues, isLoading } = useQuery<AxiosResponse<CommunityData>, unknown, CommunityData>({
        queryKey: GetCommunityForAdmin({ communityId: partnerId }).key,
        queryFn: GetCommunityForAdmin({ communityId: partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const { mutate, isPending: isSubmitting } = useMutation({
        mutationKey: UpdateCommunityContent().key,
        mutationFn: UpdateCommunityContent().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetCommunityForAdmin({ communityId: partnerId }).key,
                refetchType: 'all',
            });
            queryClient.invalidateQueries({
                queryKey: GetPartnerById({ partnerId }).key,
                refetchType: 'all',
            });
            toast({
                variant: 'primary',
                title: 'Saved!',
                description: 'Regional settings have been updated.',
            });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to save changes. Please try again.',
            });
        },
    });

    const { mutate: detectBoundary, isPending: isDetecting } = useMutation({
        mutationFn: async () => {
            const result = await SuggestCommunityBounds({ communityId: partnerId }).service();
            return result.data;
        },
        onSuccess: (data) => {
            setDerivedBounds({
                north: data.north,
                south: data.south,
                east: data.east,
                west: data.west,
                centerLat: data.centerLatitude,
                centerLng: data.centerLongitude,
            });
            if (data.boundaryGeoJson) {
                setBoundaryGeoJson(data.boundaryGeoJson);
            }
            toast({
                variant: 'primary',
                title: 'Boundary detected',
                description: `Geographic boundary derived from "${data.query}". Review the map and save.`,
            });
        },
        onError: (error: any) => {
            const message = error?.response?.data || 'Could not detect boundary. Check the community location fields.';
            toast({
                variant: 'destructive',
                title: 'Detection failed',
                description: String(message),
            });
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            regionType: String(RegionType.City),
            countyName: '',
        },
    });

    useEffect(() => {
        if (currentValues) {
            form.reset({
                regionType: toStr(currentValues.regionType) || String(RegionType.City),
                countyName: currentValues.countyName || '',
            });
            setBoundaryGeoJson(currentValues.boundaryGeoJson || '');
            setDerivedBounds({
                north: currentValues.boundsNorth ?? null,
                south: currentValues.boundsSouth ?? null,
                east: currentValues.boundsEast ?? null,
                west: currentValues.boundsWest ?? null,
                centerLat: currentValues.latitude ?? null,
                centerLng: currentValues.longitude ?? null,
            });
        }
    }, [currentValues, form]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!currentValues) return;

            const body: CommunityData = {
                ...currentValues,
                regionType: formValues.regionType === '' ? null : Number(formValues.regionType),
                countyName: formValues.countyName || '',
                latitude: derivedBounds.centerLat,
                longitude: derivedBounds.centerLng,
                boundsNorth: derivedBounds.north,
                boundsSouth: derivedBounds.south,
                boundsEast: derivedBounds.east,
                boundsWest: derivedBounds.west,
                boundaryGeoJson: boundaryGeoJson,
            };

            mutate(body);
        },
        [currentValues, mutate, boundaryGeoJson, derivedBounds],
    );

    const watchedRegionType = form.watch('regionType');
    const isCounty = watchedRegionType === String(RegionType.County);

    if (isLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    const hasGeoJson = !!boundaryGeoJson;
    const hasBounds =
        derivedBounds.north != null &&
        derivedBounds.south != null &&
        derivedBounds.east != null &&
        derivedBounds.west != null;
    const hasCenterPoint = derivedBounds.centerLat != null && derivedBounds.centerLng != null;

    return (
        <div className='py-8'>
            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-6'>
                    {/* Region Type & Boundary Detection */}
                    <Card>
                        <CardHeader>
                            <CardTitle>Community Type</CardTitle>
                            <CardDescription>
                                Define the geographic scope of your community. Use &ldquo;Detect Boundary&rdquo; to
                                automatically derive the boundary from your community&rsquo;s location.
                            </CardDescription>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <FormField
                                control={form.control}
                                name='regionType'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel required>Region Type</FormLabel>
                                        <Select onValueChange={field.onChange} value={field.value}>
                                            <FormControl>
                                                <SelectTrigger>
                                                    <SelectValue placeholder='Select a region type' />
                                                </SelectTrigger>
                                            </FormControl>
                                            <SelectContent>
                                                {regionTypeOptions.map((opt) => (
                                                    <SelectItem key={opt.value} value={opt.value}>
                                                        {opt.label}
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                        <FormDescription>
                                            Defines the geographic scope of your community.
                                        </FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />

                            {isCounty ? (
                                <FormField
                                    control={form.control}
                                    name='countyName'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>County Name</FormLabel>
                                            <FormControl>
                                                <Input {...field} placeholder='e.g., King County' />
                                            </FormControl>
                                            <FormDescription>
                                                The name of the county this community covers.
                                            </FormDescription>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            ) : null}

                            {/* Current location info (read-only) */}
                            {currentValues ? (
                                <div className='rounded-lg border bg-muted/50 p-4'>
                                    <p className='text-sm font-medium text-muted-foreground mb-2'>Current Location</p>
                                    <div className='grid grid-cols-3 gap-2 text-sm'>
                                        <div>
                                            <span className='text-muted-foreground'>City:</span>{' '}
                                            {currentValues.city || '—'}
                                        </div>
                                        <div>
                                            <span className='text-muted-foreground'>Region:</span>{' '}
                                            {currentValues.region || '—'}
                                        </div>
                                        <div>
                                            <span className='text-muted-foreground'>Country:</span>{' '}
                                            {currentValues.country || '—'}
                                        </div>
                                    </div>
                                </div>
                            ) : null}

                            <Button
                                type='button'
                                variant='outline'
                                disabled={isDetecting}
                                onClick={() => detectBoundary()}
                            >
                                {isDetecting ? (
                                    <Loader2 className='h-4 w-4 animate-spin mr-2' />
                                ) : (
                                    <MapPin className='h-4 w-4 mr-2' />
                                )}
                                Detect Boundary
                            </Button>
                        </CardContent>
                    </Card>

                    {/* Map Preview */}
                    {hasGeoJson || hasBounds || hasCenterPoint ? (
                        <BoundsPreviewMap
                            centerLat={derivedBounds.centerLat ?? 0}
                            centerLng={derivedBounds.centerLng ?? 0}
                            boundsNorth={derivedBounds.north}
                            boundsSouth={derivedBounds.south}
                            boundsEast={derivedBounds.east}
                            boundsWest={derivedBounds.west}
                            boundaryGeoJson={boundaryGeoJson}
                        />
                    ) : null}

                    {/* Submit */}
                    <div className='flex justify-end gap-2'>
                        <Button type='button' variant='outline' onClick={() => form.reset()}>
                            Reset
                        </Button>
                        <Button type='submit' disabled={isSubmitting}>
                            {isSubmitting ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                            Save Changes
                        </Button>
                    </div>
                </form>
            </Form>
        </div>
    );
};

export default CommunityRegionalSettings;
