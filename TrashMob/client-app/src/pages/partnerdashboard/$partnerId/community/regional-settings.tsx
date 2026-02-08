import { useCallback, useEffect } from 'react';
import { useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2 } from 'lucide-react';

import { Form, FormControl, FormField, FormItem, FormMessage, FormDescription } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useToast } from '@/hooks/use-toast';
import CommunityData from '@/components/Models/CommunityData';
import { GetCommunityForAdmin, UpdateCommunityContent } from '@/services/communities';
import { GetPartnerById } from '@/services/partners';
import { RegionType } from '@/lib/community-utils';
import { BoundsPreviewMap } from '@/components/communities/bounds-preview-map';

interface FormInputs {
    regionType: string;
    countyName: string;
    latitude: string;
    longitude: string;
    boundsNorth: string;
    boundsSouth: string;
    boundsEast: string;
    boundsWest: string;
}

const formSchema = z.object({
    regionType: z.string(),
    countyName: z.string().max(256, 'County name must be less than 256 characters'),
    latitude: z.string().refine((v) => v === '' || !isNaN(Number(v)), 'Must be a valid number'),
    longitude: z.string().refine((v) => v === '' || !isNaN(Number(v)), 'Must be a valid number'),
    boundsNorth: z.string().refine((v) => v === '' || !isNaN(Number(v)), 'Must be a valid number'),
    boundsSouth: z.string().refine((v) => v === '' || !isNaN(Number(v)), 'Must be a valid number'),
    boundsEast: z.string().refine((v) => v === '' || !isNaN(Number(v)), 'Must be a valid number'),
    boundsWest: z.string().refine((v) => v === '' || !isNaN(Number(v)), 'Must be a valid number'),
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
const toNumOrNull = (v: string): number | null => (v === '' ? null : Number(v));

export const CommunityRegionalSettings = () => {
    const queryClient = useQueryClient();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const { toast } = useToast();

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

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            regionType: String(RegionType.City),
            countyName: '',
            latitude: '',
            longitude: '',
            boundsNorth: '',
            boundsSouth: '',
            boundsEast: '',
            boundsWest: '',
        },
    });

    useEffect(() => {
        if (currentValues) {
            form.reset({
                regionType: toStr(currentValues.regionType) || String(RegionType.City),
                countyName: currentValues.countyName || '',
                latitude: toStr(currentValues.latitude),
                longitude: toStr(currentValues.longitude),
                boundsNorth: toStr(currentValues.boundsNorth),
                boundsSouth: toStr(currentValues.boundsSouth),
                boundsEast: toStr(currentValues.boundsEast),
                boundsWest: toStr(currentValues.boundsWest),
            });
        }
    }, [currentValues, form]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!currentValues) return;

            const body: CommunityData = {
                ...currentValues,
                regionType: toNumOrNull(formValues.regionType),
                countyName: formValues.countyName || '',
                latitude: toNumOrNull(formValues.latitude),
                longitude: toNumOrNull(formValues.longitude),
                boundsNorth: toNumOrNull(formValues.boundsNorth),
                boundsSouth: toNumOrNull(formValues.boundsSouth),
                boundsEast: toNumOrNull(formValues.boundsEast),
                boundsWest: toNumOrNull(formValues.boundsWest),
            };

            mutate(body);
        },
        [currentValues, mutate],
    );

    const watchedRegionType = form.watch('regionType');
    const isNonCity = watchedRegionType !== String(RegionType.City);
    const isCounty = watchedRegionType === String(RegionType.County);

    const watchedNorth = form.watch('boundsNorth');
    const watchedSouth = form.watch('boundsSouth');
    const watchedEast = form.watch('boundsEast');
    const watchedWest = form.watch('boundsWest');
    const watchedLat = form.watch('latitude');
    const watchedLng = form.watch('longitude');

    if (isLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='py-8'>
            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-6'>
                    {/* Region Type */}
                    <Card>
                        <CardHeader>
                            <CardTitle>Community Type</CardTitle>
                            <CardDescription>
                                Define the geographic scope of your community. City communities match events by
                                city/region. County, state, and other regional types use a bounding box to find events
                                within their geographic area.
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
                                            City communities use exact city/region matching. All other types use
                                            geographic bounds.
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
                        </CardContent>
                    </Card>

                    {/* Center Point */}
                    {isNonCity ? (
                        <Card>
                            <CardHeader>
                                <CardTitle>Center Point</CardTitle>
                                <CardDescription>
                                    The center coordinates for the community map. This is used when no bounding box is
                                    configured.
                                </CardDescription>
                            </CardHeader>
                            <CardContent>
                                <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                    <FormField
                                        control={form.control}
                                        name='latitude'
                                        render={({ field }) => (
                                            <FormItem>
                                                <FormLabel>Latitude</FormLabel>
                                                <FormControl>
                                                    <Input {...field} placeholder='e.g., 47.6062' />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='longitude'
                                        render={({ field }) => (
                                            <FormItem>
                                                <FormLabel>Longitude</FormLabel>
                                                <FormControl>
                                                    <Input {...field} placeholder='e.g., -122.3321' />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                </div>
                            </CardContent>
                        </Card>
                    ) : null}

                    {/* Geographic Bounds */}
                    {isNonCity ? (
                        <Card>
                            <CardHeader>
                                <CardTitle>Geographic Bounds</CardTitle>
                                <CardDescription>
                                    Define the bounding box for your community. Events, stats, and litter reports within
                                    these bounds will appear on your community page.
                                </CardDescription>
                            </CardHeader>
                            <CardContent className='space-y-4'>
                                <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                    <FormField
                                        control={form.control}
                                        name='boundsNorth'
                                        render={({ field }) => (
                                            <FormItem>
                                                <FormLabel>North Latitude</FormLabel>
                                                <FormControl>
                                                    <Input {...field} placeholder='e.g., 47.78' />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='boundsSouth'
                                        render={({ field }) => (
                                            <FormItem>
                                                <FormLabel>South Latitude</FormLabel>
                                                <FormControl>
                                                    <Input {...field} placeholder='e.g., 47.34' />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='boundsEast'
                                        render={({ field }) => (
                                            <FormItem>
                                                <FormLabel>East Longitude</FormLabel>
                                                <FormControl>
                                                    <Input {...field} placeholder='e.g., -121.83' />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='boundsWest'
                                        render={({ field }) => (
                                            <FormItem>
                                                <FormLabel>West Longitude</FormLabel>
                                                <FormControl>
                                                    <Input {...field} placeholder='e.g., -122.54' />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                </div>
                            </CardContent>
                        </Card>
                    ) : null}

                    {/* Map Preview */}
                    {isNonCity && watchedLat && watchedLng ? (
                        <BoundsPreviewMap
                            centerLat={Number(watchedLat) || 0}
                            centerLng={Number(watchedLng) || 0}
                            boundsNorth={toNumOrNull(watchedNorth)}
                            boundsSouth={toNumOrNull(watchedSouth)}
                            boundsEast={toNumOrNull(watchedEast)}
                            boundsWest={toNumOrNull(watchedWest)}
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
