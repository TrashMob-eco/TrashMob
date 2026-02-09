import { useCallback } from 'react';
import { useParams, useNavigate, Link } from 'react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2, ArrowLeft } from 'lucide-react';

import { Form, FormControl, FormField, FormItem, FormMessage, FormDescription } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Switch } from '@/components/ui/switch';
import { useToast } from '@/hooks/use-toast';
import SponsorData from '@/components/Models/SponsorData';
import { CreateSponsor, GetSponsors } from '@/services/sponsors';

interface FormInputs {
    name: string;
    contactEmail: string;
    contactPhone: string;
    logoUrl: string;
    showOnPublicMap: boolean;
}

const formSchema = z.object({
    name: z.string().min(1, 'Name is required').max(200, 'Name must be less than 200 characters'),
    contactEmail: z.string().max(256).email('Must be a valid email').or(z.literal('')),
    contactPhone: z.string().max(30),
    logoUrl: z.string().max(2048),
    showOnPublicMap: z.boolean(),
});

export const PartnerCommunitySponsorCreate = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const { toast } = useToast();

    const { mutate, isPending: isSubmitting } = useMutation({
        mutationKey: CreateSponsor().key,
        mutationFn: (body: SponsorData) => CreateSponsor().service({ partnerId }, body),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetSponsors({ partnerId }).key,
            });
            toast({
                variant: 'primary',
                title: 'Sponsor created!',
                description: 'The sponsor has been created successfully.',
            });
            navigate(`/partnerdashboard/${partnerId}/community/sponsors`);
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to create sponsor. Please try again.',
            });
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: '',
            contactEmail: '',
            contactPhone: '',
            logoUrl: '',
            showOnPublicMap: true,
        },
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!partnerId) return;

            const sponsor: SponsorData = {
                ...new SponsorData(),
                partnerId,
                name: formValues.name,
                contactEmail: formValues.contactEmail,
                contactPhone: formValues.contactPhone,
                logoUrl: formValues.logoUrl,
                showOnPublicMap: formValues.showOnPublicMap,
            };

            mutate(sponsor);
        },
        [partnerId, mutate],
    );

    return (
        <div className='py-8'>
            <div className='mb-6'>
                <Button variant='ghost' size='sm' asChild>
                    <Link to={`/partnerdashboard/${partnerId}/community/sponsors`}>
                        <ArrowLeft className='h-4 w-4 mr-2' />
                        Back to Sponsors
                    </Link>
                </Button>
            </div>

            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-6'>
                    <Card>
                        <CardHeader>
                            <CardTitle>Create Sponsor</CardTitle>
                            <CardDescription>
                                Add a new sponsor who funds professional cleanup of adopted areas.
                            </CardDescription>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <FormField
                                control={form.control}
                                name='name'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel required>Sponsor Name</FormLabel>
                                        <FormControl>
                                            <Input {...field} placeholder='e.g., Acme Corporation' />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                <FormField
                                    control={form.control}
                                    name='contactEmail'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Contact Email</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='email' placeholder='sponsor@example.com' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='contactPhone'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Contact Phone</FormLabel>
                                            <FormControl>
                                                <Input {...field} placeholder='(555) 123-4567' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </div>
                            <FormField
                                control={form.control}
                                name='logoUrl'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Logo URL</FormLabel>
                                        <FormControl>
                                            <Input {...field} placeholder='https://example.com/logo.png' />
                                        </FormControl>
                                        <FormDescription>URL to the sponsor&apos;s logo image.</FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name='showOnPublicMap'
                                render={({ field }) => (
                                    <FormItem className='flex flex-row items-center justify-between rounded-lg border p-4'>
                                        <div className='space-y-0.5'>
                                            <FormLabel>Show on Public Map</FormLabel>
                                            <FormDescription>
                                                Display this sponsor&apos;s name on the community&apos;s public adoption
                                                map.
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
                        <Button
                            type='button'
                            variant='outline'
                            onClick={() => navigate(`/partnerdashboard/${partnerId}/community/sponsors`)}
                        >
                            Cancel
                        </Button>
                        <Button type='submit' disabled={isSubmitting}>
                            {isSubmitting ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                            Create Sponsor
                        </Button>
                    </div>
                </form>
            </Form>
        </div>
    );
};

export default PartnerCommunitySponsorCreate;
