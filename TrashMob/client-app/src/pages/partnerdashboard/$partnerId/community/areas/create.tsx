import { useCallback, useMemo } from 'react';
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
import AdoptableAreaData, { AdoptableAreaType } from '@/components/Models/AdoptableAreaData';
import CommunityData from '@/components/Models/CommunityData';
import { CreateAdoptableArea, GetAdoptableAreas } from '@/services/adoptable-areas';
import { GetCommunityForAdmin } from '@/services/communities';

const areaTypes: AdoptableAreaType[] = ['Highway', 'Park', 'Trail', 'Waterway', 'Street', 'Spot'];

interface FormInputs {
    name: string;
    description: string;
    areaType: AdoptableAreaType;
    cleanupFrequencyDays: number;
    minEventsPerYear: number;
    safetyRequirements: string;
    allowCoAdoption: boolean;
    geoJson: string;
}

const formSchema = z.object({
    name: z.string().min(1, 'Name is required').max(200, 'Name must be less than 200 characters'),
    description: z.string().max(2048, 'Description must be less than 2048 characters'),
    areaType: z.enum(['Highway', 'Park', 'Trail', 'Waterway', 'Street', 'Spot']),
    cleanupFrequencyDays: z.coerce.number().min(1, 'Must be at least 1 day').max(365, 'Must be less than 365 days'),
    minEventsPerYear: z.coerce.number().min(1, 'Must be at least 1 event').max(52, 'Must be less than 52 events'),
    safetyRequirements: z.string().max(4000, 'Safety requirements must be less than 4000 characters'),
    allowCoAdoption: z.boolean(),
    geoJson: z.string(),
});

export const PartnerCommunityAreaCreate = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const { toast } = useToast();

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
        mutationKey: CreateAdoptableArea().key,
        mutationFn: (body: AdoptableAreaData) => CreateAdoptableArea().service({ partnerId }, body),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetAdoptableAreas({ partnerId }).key,
            });
            toast({
                variant: 'primary',
                title: 'Area created!',
                description: 'The adoptable area has been created successfully.',
            });
            navigate(`/partnerdashboard/${partnerId}/community/areas`);
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to create area. Please try again.',
            });
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: '',
            description: '',
            areaType: 'Park',
            cleanupFrequencyDays: 90,
            minEventsPerYear: 4,
            safetyRequirements: '',
            allowCoAdoption: false,
            geoJson: '',
        },
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!partnerId) return;

            const area: AdoptableAreaData = {
                id: '',
                partnerId,
                name: formValues.name,
                description: formValues.description,
                areaType: formValues.areaType,
                status: 'Available',
                geoJson: formValues.geoJson,
                startLatitude: null,
                startLongitude: null,
                endLatitude: null,
                endLongitude: null,
                cleanupFrequencyDays: formValues.cleanupFrequencyDays,
                minEventsPerYear: formValues.minEventsPerYear,
                safetyRequirements: formValues.safetyRequirements,
                allowCoAdoption: formValues.allowCoAdoption,
                isActive: true,
                createdByUserId: '',
                createdDate: new Date(),
                lastUpdatedByUserId: '',
                lastUpdatedDate: new Date(),
            };

            mutate(area);
        },
        [partnerId, mutate],
    );

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
                            <CardTitle>Create Adoptable Area</CardTitle>
                            <CardDescription>
                                Define a geographic area that teams can adopt for regular cleanup.
                            </CardDescription>
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
                            <FormField
                                control={form.control}
                                name='areaType'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel required>Area Type</FormLabel>
                                        <Select onValueChange={field.onChange} defaultValue={field.value}>
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
                                        <FormDescription>
                                            The type of geographic area (Highway, Park, Trail, etc.).
                                        </FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
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
                                    form.setValue(
                                        'safetyRequirements',
                                        community?.defaultSafetyRequirements ?? '',
                                    );
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
                        </CardContent>
                    </Card>

                    {/* Geographic Definition */}
                    <Card>
                        <CardHeader>
                            <CardTitle>Geographic Definition</CardTitle>
                            <CardDescription>
                                Define the boundaries of this area using GeoJSON (advanced).
                            </CardDescription>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <FormField
                                control={form.control}
                                name='geoJson'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>GeoJSON (Optional)</FormLabel>
                                        <FormControl>
                                            <Textarea
                                                {...field}
                                                placeholder='{"type": "Polygon", "coordinates": [...]}'
                                                className='h-32 font-mono text-sm'
                                            />
                                        </FormControl>
                                        <FormDescription>
                                            Paste GeoJSON polygon or linestring to define the area boundaries. Map-based
                                            editing will be available in a future update.
                                        </FormDescription>
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
                            Create Area
                        </Button>
                    </div>
                </form>
            </Form>
        </div>
    );
};

export default PartnerCommunityAreaCreate;
