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
import { Textarea } from '@/components/ui/textarea';
import { Switch } from '@/components/ui/switch';
import { useToast } from '@/hooks/use-toast';
import CommunityData from '@/components/Models/CommunityData';
import { GetCommunityForAdmin, UpdateCommunityContent } from '@/services/communities';
import { GetPartnerById } from '@/services/partners';

interface FormInputs {
    defaultCleanupFrequencyDays: number;
    defaultMinEventsPerYear: number;
    defaultSafetyRequirements: string;
    defaultAllowCoAdoption: boolean;
}

const formSchema = z.object({
    defaultCleanupFrequencyDays: z.coerce
        .number()
        .min(1, 'Must be at least 1 day')
        .max(365, 'Must be less than 365 days'),
    defaultMinEventsPerYear: z.coerce
        .number()
        .min(1, 'Must be at least 1 event')
        .max(52, 'Must be less than 52 events'),
    defaultSafetyRequirements: z.string().max(4000, 'Safety requirements must be less than 4000 characters'),
    defaultAllowCoAdoption: z.boolean(),
});

export const PartnerCommunityAreaDefaults = () => {
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
                description: 'Area defaults have been updated.',
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
            defaultCleanupFrequencyDays: 90,
            defaultMinEventsPerYear: 4,
            defaultSafetyRequirements: '',
            defaultAllowCoAdoption: false,
        },
    });

    useEffect(() => {
        if (currentValues) {
            form.reset({
                defaultCleanupFrequencyDays: currentValues.defaultCleanupFrequencyDays ?? 90,
                defaultMinEventsPerYear: currentValues.defaultMinEventsPerYear ?? 4,
                defaultSafetyRequirements: currentValues.defaultSafetyRequirements || '',
                defaultAllowCoAdoption: currentValues.defaultAllowCoAdoption ?? false,
            });
        }
    }, [currentValues, form]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!currentValues) return;

            const body: CommunityData = {
                ...currentValues,
                defaultCleanupFrequencyDays: formValues.defaultCleanupFrequencyDays,
                defaultMinEventsPerYear: formValues.defaultMinEventsPerYear,
                defaultSafetyRequirements: formValues.defaultSafetyRequirements || '',
                defaultAllowCoAdoption: formValues.defaultAllowCoAdoption,
            };

            mutate(body);
        },
        [currentValues, mutate],
    );

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
                    <Card>
                        <CardHeader>
                            <CardTitle>Area Defaults</CardTitle>
                            <CardDescription>
                                Set default requirement values for new adoptable areas. When creating or editing an
                                area, you can apply these defaults with a single click.
                            </CardDescription>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                <FormField
                                    control={form.control}
                                    name='defaultCleanupFrequencyDays'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Cleanup Frequency (days)</FormLabel>
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
                                    name='defaultMinEventsPerYear'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Minimum Events Per Year</FormLabel>
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
                                name='defaultSafetyRequirements'
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
                                            Default safety guidelines teams must follow when cleaning areas.
                                        </FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name='defaultAllowCoAdoption'
                                render={({ field }) => (
                                    <FormItem className='flex flex-row items-center justify-between rounded-lg border p-4'>
                                        <div className='space-y-0.5'>
                                            <FormLabel>Allow Co-Adoption</FormLabel>
                                            <FormDescription>
                                                Allow multiple teams to adopt areas together by default.
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

                    <div className='flex justify-end gap-2'>
                        <Button type='button' variant='outline' onClick={() => form.reset()}>
                            Reset
                        </Button>
                        <Button type='submit' disabled={isSubmitting}>
                            {isSubmitting ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                            Save Defaults
                        </Button>
                    </div>
                </form>
            </Form>
        </div>
    );
};

export default PartnerCommunityAreaDefaults;
