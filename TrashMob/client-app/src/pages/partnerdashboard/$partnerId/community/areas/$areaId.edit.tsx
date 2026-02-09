import { useCallback, useEffect, useMemo, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2, ArrowLeft } from 'lucide-react';

import { Form, FormControl, FormField, FormItem, FormMessage, FormDescription } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Switch } from '@/components/ui/switch';
import { useToast } from '@/hooks/use-toast';
import AdoptableAreaData, { AdoptableAreaType, AdoptableAreaStatus } from '@/components/Models/AdoptableAreaData';
import CommunityData from '@/components/Models/CommunityData';
import { GetAdoptableArea, UpdateAdoptableArea, GetAdoptableAreas } from '@/services/adoptable-areas';
import { GetCommunityForAdmin } from '@/services/communities';
import { AreaMapEditor } from '@/components/Map/AreaMapEditor';
import { AreaBoundingBox } from '@/lib/geojson';

const areaTypes: AdoptableAreaType[] = ['Highway', 'Park', 'Trail', 'Waterway', 'Street', 'Spot'];
const areaStatuses: AdoptableAreaStatus[] = ['Available', 'Adopted', 'Unavailable'];

interface FormInputs {
    name: string;
    description: string;
    areaType: AdoptableAreaType;
    status: AdoptableAreaStatus;
    cleanupFrequencyDays: number;
    minEventsPerYear: number;
    safetyRequirements: string;
    allowCoAdoption: boolean;
    isActive: boolean;
    geoJson: string;
}

const formSchema = z.object({
    name: z.string().min(1, 'Name is required').max(200, 'Name must be less than 200 characters'),
    description: z.string().max(2048, 'Description must be less than 2048 characters'),
    areaType: z.enum(['Highway', 'Park', 'Trail', 'Waterway', 'Street', 'Spot']),
    status: z.enum(['Available', 'Adopted', 'Unavailable']),
    cleanupFrequencyDays: z.coerce.number().min(1, 'Must be at least 1 day').max(365, 'Must be less than 365 days'),
    minEventsPerYear: z.coerce.number().min(1, 'Must be at least 1 event').max(52, 'Must be less than 52 events'),
    safetyRequirements: z.string().max(4000, 'Safety requirements must be less than 4000 characters'),
    allowCoAdoption: z.boolean(),
    isActive: z.boolean(),
    geoJson: z.string(),
});

export const PartnerCommunityAreaEdit = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { partnerId, areaId } = useParams<{ partnerId: string; areaId: string }>() as {
        partnerId: string;
        areaId: string;
    };
    const { toast } = useToast();
    const [areaBbox, setAreaBbox] = useState<AreaBoundingBox | null>(null);

    const { data: area, isLoading: areaLoading } = useQuery<
        AxiosResponse<AdoptableAreaData>,
        unknown,
        AdoptableAreaData
    >({
        queryKey: GetAdoptableArea({ partnerId, areaId }).key,
        queryFn: GetAdoptableArea({ partnerId, areaId }).service,
        select: (res) => res.data,
        enabled: !!partnerId && !!areaId,
    });

    const { data: community } = useQuery<AxiosResponse<CommunityData>, unknown, CommunityData>({
        queryKey: GetCommunityForAdmin({ communityId: partnerId }).key,
        queryFn: GetCommunityForAdmin({ communityId: partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const hasDefaults = useMemo(
        () =>
            community?.defaultCleanupFrequencyDays != null ||
            community?.defaultMinEventsPerYear != null ||
            community?.defaultSafetyRequirements != null ||
            community?.defaultAllowCoAdoption != null,
        [community],
    );

    const { mutate, isPending: isSubmitting } = useMutation({
        mutationKey: UpdateAdoptableArea().key,
        mutationFn: (body: AdoptableAreaData) => UpdateAdoptableArea().service({ partnerId, areaId }, body),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetAdoptableAreas({ partnerId }).key,
            });
            queryClient.invalidateQueries({
                queryKey: GetAdoptableArea({ partnerId, areaId }).key,
            });
            toast({
                variant: 'primary',
                title: 'Area updated!',
                description: 'The adoptable area has been updated successfully.',
            });
            navigate(`/partnerdashboard/${partnerId}/community/areas`);
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to update area. Please try again.',
            });
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: '',
            description: '',
            areaType: 'Park',
            status: 'Available',
            cleanupFrequencyDays: 90,
            minEventsPerYear: 4,
            safetyRequirements: '',
            allowCoAdoption: false,
            isActive: true,
            geoJson: '',
        },
    });

    useEffect(() => {
        if (area) {
            form.reset({
                name: area.name || '',
                description: area.description || '',
                areaType: area.areaType,
                status: area.status,
                cleanupFrequencyDays: area.cleanupFrequencyDays,
                minEventsPerYear: area.minEventsPerYear,
                safetyRequirements: area.safetyRequirements || '',
                allowCoAdoption: area.allowCoAdoption,
                isActive: area.isActive,
                geoJson: area.geoJson || '',
            });
        }
    }, [area, form]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!partnerId || !area) return;

            const updatedArea: AdoptableAreaData = {
                ...area,
                name: formValues.name,
                description: formValues.description,
                areaType: formValues.areaType,
                status: formValues.status,
                geoJson: formValues.geoJson,
                startLatitude: areaBbox?.startLatitude ?? area.startLatitude,
                startLongitude: areaBbox?.startLongitude ?? area.startLongitude,
                endLatitude: areaBbox?.endLatitude ?? area.endLatitude,
                endLongitude: areaBbox?.endLongitude ?? area.endLongitude,
                cleanupFrequencyDays: formValues.cleanupFrequencyDays,
                minEventsPerYear: formValues.minEventsPerYear,
                safetyRequirements: formValues.safetyRequirements,
                allowCoAdoption: formValues.allowCoAdoption,
                isActive: formValues.isActive,
            };

            mutate(updatedArea);
        },
        [partnerId, area, mutate],
    );

    if (areaLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    if (!area) {
        return (
            <div className='py-8 text-center'>
                <p>Area not found.</p>
            </div>
        );
    }

    return (
        <div className='py-8'>
            <div className='mb-6'>
                <Button variant='ghost' size='sm' asChild>
                    <Link to={`/partnerdashboard/${partnerId}/community/areas`}>
                        <ArrowLeft className='h-4 w-4 mr-2' />
                        Back to Areas
                    </Link>
                </Button>
            </div>

            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-6'>
                    {/* Basic Information */}
                    <Card>
                        <CardHeader>
                            <CardTitle>Edit Adoptable Area</CardTitle>
                            <CardDescription>Update the details for this adoptable area.</CardDescription>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <FormField
                                control={form.control}
                                name='name'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel required>Area Name</FormLabel>
                                        <FormControl>
                                            <Input {...field} placeholder='e.g., Highway 101 Mile Marker 5-10' />
                                        </FormControl>
                                        <FormDescription>A descriptive name for this adoptable area.</FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name='description'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Description</FormLabel>
                                        <FormControl>
                                            <Textarea
                                                {...field}
                                                placeholder='Describe the area, including any notable landmarks or boundaries...'
                                                className='h-24'
                                            />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                <FormField
                                    control={form.control}
                                    name='areaType'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Area Type</FormLabel>
                                            <Select onValueChange={field.onChange} value={field.value}>
                                                <FormControl>
                                                    <SelectTrigger>
                                                        <SelectValue placeholder='Select area type' />
                                                    </SelectTrigger>
                                                </FormControl>
                                                <SelectContent>
                                                    {areaTypes.map((type) => (
                                                        <SelectItem key={type} value={type}>
                                                            {type}
                                                        </SelectItem>
                                                    ))}
                                                </SelectContent>
                                            </Select>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='status'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Status</FormLabel>
                                            <Select onValueChange={field.onChange} value={field.value}>
                                                <FormControl>
                                                    <SelectTrigger>
                                                        <SelectValue placeholder='Select status' />
                                                    </SelectTrigger>
                                                </FormControl>
                                                <SelectContent>
                                                    {areaStatuses.map((status) => (
                                                        <SelectItem key={status} value={status}>
                                                            {status}
                                                        </SelectItem>
                                                    ))}
                                                </SelectContent>
                                            </Select>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </div>
                        </CardContent>
                    </Card>

                    {/* Requirements */}
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between'>
                            <div>
                                <CardTitle>Requirements</CardTitle>
                                <CardDescription>Set expectations for teams adopting this area.</CardDescription>
                            </div>
                            <Button
                                type='button'
                                variant='outline'
                                size='sm'
                                disabled={!hasDefaults}
                                onClick={() => {
                                    form.setValue('cleanupFrequencyDays', community?.defaultCleanupFrequencyDays ?? 90);
                                    form.setValue('minEventsPerYear', community?.defaultMinEventsPerYear ?? 4);
                                    form.setValue('safetyRequirements', community?.defaultSafetyRequirements ?? '');
                                    form.setValue('allowCoAdoption', community?.defaultAllowCoAdoption ?? false);
                                }}
                            >
                                Use Community Defaults
                            </Button>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                <FormField
                                    control={form.control}
                                    name='cleanupFrequencyDays'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Cleanup Frequency (days)</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='number' min={1} max={365} />
                                            </FormControl>
                                            <FormDescription>
                                                How often the area should be cleaned (in days).
                                            </FormDescription>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='minEventsPerYear'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Minimum Events Per Year</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='number' min={1} max={52} />
                                            </FormControl>
                                            <FormDescription>Minimum cleanup events required annually.</FormDescription>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </div>
                            <FormField
                                control={form.control}
                                name='safetyRequirements'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Safety Requirements</FormLabel>
                                        <FormControl>
                                            <Textarea
                                                {...field}
                                                placeholder='List any safety guidelines, required equipment, or precautions...'
                                                className='h-32'
                                            />
                                        </FormControl>
                                        <FormDescription>
                                            Safety guidelines teams must follow when cleaning this area.
                                        </FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name='allowCoAdoption'
                                render={({ field }) => (
                                    <FormItem className='flex flex-row items-center justify-between rounded-lg border p-4'>
                                        <div className='space-y-0.5'>
                                            <FormLabel>Allow Co-Adoption</FormLabel>
                                            <FormDescription>
                                                Allow multiple teams to adopt this area together.
                                            </FormDescription>
                                        </div>
                                        <FormControl>
                                            <Switch checked={field.value} onCheckedChange={field.onChange} />
                                        </FormControl>
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name='isActive'
                                render={({ field }) => (
                                    <FormItem className='flex flex-row items-center justify-between rounded-lg border p-4'>
                                        <div className='space-y-0.5'>
                                            <FormLabel>Active</FormLabel>
                                            <FormDescription>
                                                Deactivate to hide this area from public view.
                                            </FormDescription>
                                        </div>
                                        <FormControl>
                                            <Switch checked={field.value} onCheckedChange={field.onChange} />
                                        </FormControl>
                                    </FormItem>
                                )}
                            />
                        </CardContent>
                    </Card>

                    {/* Geographic Definition */}
                    <Card>
                        <CardHeader>
                            <CardTitle>Geographic Definition</CardTitle>
                            <CardDescription>
                                Draw the area boundary on the map, or paste raw GeoJSON.
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <FormField
                                control={form.control}
                                name='geoJson'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormControl>
                                            <AreaMapEditor
                                                value={field.value}
                                                onChange={field.onChange}
                                                onBoundsChange={setAreaBbox}
                                                communityBounds={community ? { boundsNorth: community.boundsNorth, boundsSouth: community.boundsSouth, boundsEast: community.boundsEast, boundsWest: community.boundsWest } : undefined}
                                                communityCenter={community?.latitude != null && community?.longitude != null ? { lat: community.latitude, lng: community.longitude } : null}
                                            />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        </CardContent>
                    </Card>

                    {/* Submit Buttons */}
                    <div className='flex justify-end gap-2'>
                        <Button
                            type='button'
                            variant='outline'
                            onClick={() => navigate(`/partnerdashboard/${partnerId}/community/areas`)}
                        >
                            Cancel
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

export default PartnerCommunityAreaEdit;
