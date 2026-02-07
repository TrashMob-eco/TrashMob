import { useCallback } from 'react';
import { Link, useNavigate } from 'react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useToast } from '@/hooks/use-toast';
import { Loader2 } from 'lucide-react';
import { CreateCommunityProspect, GetCommunityProspects } from '@/services/community-prospects';
import { PIPELINE_STAGES, PROSPECT_TYPES } from '@/components/prospects/pipeline-stage-badge';
import CommunityProspectData from '@/components/Models/CommunityProspectData';

interface FormInputs {
    name: string;
    type: string;
    city: string;
    region: string;
    country: string;
    contactName: string;
    contactEmail: string;
    contactTitle: string;
    website: string;
    population: string;
    fitScore: string;
    pipelineStage: string;
    notes: string;
    latitude: string;
    longitude: string;
}

const formSchema = z.object({
    name: z.string().min(1, 'Name is required'),
    type: z.string(),
    city: z.string(),
    region: z.string(),
    country: z.string(),
    contactName: z.string(),
    contactEmail: z.string().email('Invalid email').or(z.literal('')),
    contactTitle: z.string(),
    website: z.string(),
    population: z.string(),
    fitScore: z.string(),
    pipelineStage: z.string(),
    notes: z.string(),
    latitude: z.string(),
    longitude: z.string(),
});

export const SiteAdminProspectCreate = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const createProspect = useMutation({
        mutationKey: CreateCommunityProspect().key,
        mutationFn: CreateCommunityProspect().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Prospect created!' });
            queryClient.invalidateQueries({ queryKey: ['/communityprospects'], refetchType: 'all' });
            navigate('/siteadmin/prospects');
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: '',
            type: 'Municipality',
            city: '',
            region: '',
            country: 'United States',
            contactName: '',
            contactEmail: '',
            contactTitle: '',
            website: '',
            population: '',
            fitScore: '0',
            pipelineStage: '0',
            notes: '',
            latitude: '',
            longitude: '',
        },
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            const body = new CommunityProspectData();
            body.name = formValues.name;
            body.type = formValues.type;
            body.city = formValues.city;
            body.region = formValues.region;
            body.country = formValues.country;
            body.contactName = formValues.contactName;
            body.contactEmail = formValues.contactEmail;
            body.contactTitle = formValues.contactTitle;
            body.website = formValues.website;
            body.population = formValues.population ? parseInt(formValues.population, 10) : null;
            body.fitScore = parseInt(formValues.fitScore, 10) || 0;
            body.pipelineStage = parseInt(formValues.pipelineStage, 10);
            body.notes = formValues.notes;
            body.latitude = formValues.latitude ? parseFloat(formValues.latitude) : null;
            body.longitude = formValues.longitude ? parseFloat(formValues.longitude) : null;
            createProspect.mutate(body);
        },
        [createProspect],
    );

    const isSubmitting = createProspect.isPending;

    return (
        <Card>
            <CardHeader>
                <CardTitle>Add Prospect</CardTitle>
            </CardHeader>
            <CardContent>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                        <FormField
                            control={form.control}
                            name='name'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-6'>
                                    <FormLabel required>Name</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Community or organization name' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='type'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-3'>
                                    <FormLabel>Type</FormLabel>
                                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            {PROSPECT_TYPES.map((t) => (
                                                <SelectItem key={t} value={t}>
                                                    {t}
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
                            name='pipelineStage'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-3'>
                                    <FormLabel>Pipeline Stage</FormLabel>
                                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            {PIPELINE_STAGES.map((s) => (
                                                <SelectItem key={s.value} value={String(s.value)}>
                                                    {s.label}
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
                            name='city'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>City</FormLabel>
                                    <FormControl>
                                        <Input {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='region'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Region / State</FormLabel>
                                    <FormControl>
                                        <Input {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='country'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Country</FormLabel>
                                    <FormControl>
                                        <Input {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='contactName'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Contact Name</FormLabel>
                                    <FormControl>
                                        <Input {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='contactEmail'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Contact Email</FormLabel>
                                    <FormControl>
                                        <Input {...field} type='email' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='contactTitle'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Contact Title</FormLabel>
                                    <FormControl>
                                        <Input {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='website'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-6'>
                                    <FormLabel>Website</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='https://' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='population'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-3'>
                                    <FormLabel>Population</FormLabel>
                                    <FormControl>
                                        <Input {...field} type='number' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='fitScore'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-3'>
                                    <FormLabel>Fit Score (0-100)</FormLabel>
                                    <FormControl>
                                        <Input {...field} type='number' min='0' max='100' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='notes'
                            render={({ field }) => (
                                <FormItem className='col-span-12'>
                                    <FormLabel>Notes</FormLabel>
                                    <FormControl>
                                        <Textarea {...field} rows={3} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <div className='col-span-12 flex justify-end gap-2'>
                            <Button variant='secondary' asChild>
                                <Link to='/siteadmin/prospects'>Cancel</Link>
                            </Button>
                            <Button type='submit' disabled={isSubmitting}>
                                {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                                Save
                            </Button>
                        </div>
                    </form>
                </Form>
            </CardContent>
        </Card>
    );
};
