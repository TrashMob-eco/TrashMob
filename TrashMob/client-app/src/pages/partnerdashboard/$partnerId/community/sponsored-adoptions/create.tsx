import { useCallback } from 'react';
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
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useToast } from '@/hooks/use-toast';
import SponsoredAdoptionData from '@/components/Models/SponsoredAdoptionData';
import AdoptableAreaData from '@/components/Models/AdoptableAreaData';
import SponsorData from '@/components/Models/SponsorData';
import ProfessionalCompanyData from '@/components/Models/ProfessionalCompanyData';
import { GetAdoptableAreas } from '@/services/adoptable-areas';
import { GetSponsors } from '@/services/sponsors';
import { GetProfessionalCompanies } from '@/services/professional-companies';
import {
    CreateSponsoredAdoption,
    GetSponsoredAdoptions,
    GetSponsoredAdoptionCompliance,
} from '@/services/sponsored-adoptions';

interface FormInputs {
    adoptableAreaId: string;
    sponsorId: string;
    professionalCompanyId: string;
    startDate: string;
    endDate: string;
    cleanupFrequencyDays: number;
}

const formSchema = z.object({
    adoptableAreaId: z.string().min(1, 'Adoptable area is required'),
    sponsorId: z.string().min(1, 'Sponsor is required'),
    professionalCompanyId: z.string().min(1, 'Professional company is required'),
    startDate: z.string().min(1, 'Start date is required'),
    endDate: z.string(),
    cleanupFrequencyDays: z.coerce.number().min(1, 'Must be at least 1 day').max(365, 'Must be less than 365 days'),
});

export const PartnerCommunitySponsoredAdoptionCreate = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const { toast } = useToast();

    const { data: areas, isLoading: areasLoading } = useQuery<
        AxiosResponse<AdoptableAreaData[]>,
        unknown,
        AdoptableAreaData[]
    >({
        queryKey: GetAdoptableAreas({ partnerId }).key,
        queryFn: GetAdoptableAreas({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const { data: sponsors, isLoading: sponsorsLoading } = useQuery<
        AxiosResponse<SponsorData[]>,
        unknown,
        SponsorData[]
    >({
        queryKey: GetSponsors({ partnerId }).key,
        queryFn: GetSponsors({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const { data: companies, isLoading: companiesLoading } = useQuery<
        AxiosResponse<ProfessionalCompanyData[]>,
        unknown,
        ProfessionalCompanyData[]
    >({
        queryKey: GetProfessionalCompanies({ partnerId }).key,
        queryFn: GetProfessionalCompanies({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const { mutate, isPending: isSubmitting } = useMutation({
        mutationKey: CreateSponsoredAdoption().key,
        mutationFn: (body: SponsoredAdoptionData) => CreateSponsoredAdoption().service({ partnerId }, body),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetSponsoredAdoptions({ partnerId }).key });
            queryClient.invalidateQueries({ queryKey: GetSponsoredAdoptionCompliance({ partnerId }).key });
            toast({
                variant: 'primary',
                title: 'Sponsored adoption created!',
                description: 'The sponsored adoption has been created successfully.',
            });
            navigate(`/partnerdashboard/${partnerId}/community/sponsored-adoptions`);
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to create sponsored adoption. Please try again.',
            });
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            adoptableAreaId: '',
            sponsorId: '',
            professionalCompanyId: '',
            startDate: '',
            endDate: '',
            cleanupFrequencyDays: 14,
        },
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!partnerId) return;

            const adoption: SponsoredAdoptionData = {
                ...new SponsoredAdoptionData(),
                adoptableAreaId: formValues.adoptableAreaId,
                sponsorId: formValues.sponsorId,
                professionalCompanyId: formValues.professionalCompanyId,
                startDate: formValues.startDate,
                endDate: formValues.endDate || null,
                cleanupFrequencyDays: formValues.cleanupFrequencyDays,
                status: 'Active',
            };

            mutate(adoption);
        },
        [partnerId, mutate],
    );

    if (areasLoading || sponsorsLoading || companiesLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='py-8'>
            <div className='mb-6'>
                <Button variant='ghost' size='sm' asChild>
                    <Link to={`/partnerdashboard/${partnerId}/community/sponsored-adoptions`}>
                        <ArrowLeft className='h-4 w-4 mr-2' />
                        Back to Sponsored Adoptions
                    </Link>
                </Button>
            </div>

            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-6'>
                    <Card>
                        <CardHeader>
                            <CardTitle>Create Sponsored Adoption</CardTitle>
                            <CardDescription>
                                Set up a new sponsor-funded professional cleanup program for an adopted area.
                            </CardDescription>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <FormField
                                control={form.control}
                                name='adoptableAreaId'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel required>Adoptable Area</FormLabel>
                                        <Select onValueChange={field.onChange} value={field.value}>
                                            <FormControl>
                                                <SelectTrigger>
                                                    <SelectValue placeholder='Select an area' />
                                                </SelectTrigger>
                                            </FormControl>
                                            <SelectContent>
                                                {areas?.map((area) => (
                                                    <SelectItem key={area.id} value={area.id}>
                                                        {area.name} ({area.areaType})
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                <FormField
                                    control={form.control}
                                    name='sponsorId'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Sponsor</FormLabel>
                                            <Select onValueChange={field.onChange} value={field.value}>
                                                <FormControl>
                                                    <SelectTrigger>
                                                        <SelectValue placeholder='Select a sponsor' />
                                                    </SelectTrigger>
                                                </FormControl>
                                                <SelectContent>
                                                    {sponsors
                                                        ?.filter((s) => s.isActive)
                                                        .map((sponsor) => (
                                                            <SelectItem
                                                                key={sponsor.id}
                                                                value={sponsor.id}
                                                            >
                                                                {sponsor.name}
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
                                    name='professionalCompanyId'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Professional Company</FormLabel>
                                            <Select onValueChange={field.onChange} value={field.value}>
                                                <FormControl>
                                                    <SelectTrigger>
                                                        <SelectValue placeholder='Select a company' />
                                                    </SelectTrigger>
                                                </FormControl>
                                                <SelectContent>
                                                    {companies
                                                        ?.filter((c) => c.isActive)
                                                        .map((company) => (
                                                            <SelectItem
                                                                key={company.id}
                                                                value={company.id}
                                                            >
                                                                {company.name}
                                                            </SelectItem>
                                                        ))}
                                                </SelectContent>
                                            </Select>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </div>
                            <div className='grid grid-cols-1 md:grid-cols-3 gap-4'>
                                <FormField
                                    control={form.control}
                                    name='startDate'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Start Date</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='date' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='endDate'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>End Date</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='date' />
                                            </FormControl>
                                            <FormDescription>Optional. Leave blank for ongoing.</FormDescription>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='cleanupFrequencyDays'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Cleanup Frequency (days)</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='number' min={1} max={365} />
                                            </FormControl>
                                            <FormDescription>How often cleanups should occur.</FormDescription>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </div>
                        </CardContent>
                    </Card>

                    <div className='flex justify-end gap-2'>
                        <Button
                            type='button'
                            variant='outline'
                            onClick={() =>
                                navigate(`/partnerdashboard/${partnerId}/community/sponsored-adoptions`)
                            }
                        >
                            Cancel
                        </Button>
                        <Button type='submit' disabled={isSubmitting}>
                            {isSubmitting ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                            Create Sponsored Adoption
                        </Button>
                    </div>
                </form>
            </Form>
        </div>
    );
};

export default PartnerCommunitySponsoredAdoptionCreate;
