import { useCallback, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2, ArrowLeft, ClipboardList } from 'lucide-react';

import { Form, FormControl, FormField, FormItem, FormMessage, FormDescription } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { useToast } from '@/hooks/use-toast';
import SponsoredAdoptionData, { SponsoredAdoptionStatus } from '@/components/Models/SponsoredAdoptionData';
import ProfessionalCleanupLogData from '@/components/Models/ProfessionalCleanupLogData';
import AdoptableAreaData from '@/components/Models/AdoptableAreaData';
import SponsorData from '@/components/Models/SponsorData';
import ProfessionalCompanyData from '@/components/Models/ProfessionalCompanyData';
import { GetAdoptableAreas } from '@/services/adoptable-areas';
import { GetSponsors } from '@/services/sponsors';
import { GetProfessionalCompanies } from '@/services/professional-companies';
import {
    GetSponsoredAdoption,
    GetSponsoredAdoptions,
    UpdateSponsoredAdoption,
    GetSponsoredAdoptionCompliance,
    GetAdoptionCleanupLogs,
} from '@/services/sponsored-adoptions';

const statuses: SponsoredAdoptionStatus[] = ['Active', 'Expired', 'Terminated'];

interface FormInputs {
    adoptableAreaId: string;
    sponsorId: string;
    professionalCompanyId: string;
    startDate: string;
    endDate: string;
    cleanupFrequencyDays: number;
    status: SponsoredAdoptionStatus;
}

const formSchema = z.object({
    adoptableAreaId: z.string().min(1, 'Adoptable area is required'),
    sponsorId: z.string().min(1, 'Sponsor is required'),
    professionalCompanyId: z.string().min(1, 'Professional company is required'),
    startDate: z.string().min(1, 'Start date is required'),
    endDate: z.string(),
    cleanupFrequencyDays: z.coerce.number().min(1, 'Must be at least 1 day').max(365, 'Must be less than 365 days'),
    status: z.enum(['Active', 'Expired', 'Terminated']),
});

export const PartnerCommunitySponsoredAdoptionEdit = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { partnerId, adoptionId } = useParams<{ partnerId: string; adoptionId: string }>() as {
        partnerId: string;
        adoptionId: string;
    };
    const { toast } = useToast();

    const { data: adoption, isLoading } = useQuery<
        AxiosResponse<SponsoredAdoptionData>,
        unknown,
        SponsoredAdoptionData
    >({
        queryKey: GetSponsoredAdoption({ partnerId, id: adoptionId }).key,
        queryFn: GetSponsoredAdoption({ partnerId, id: adoptionId }).service,
        select: (res) => res.data,
        enabled: !!partnerId && !!adoptionId,
    });

    const { data: areas } = useQuery<AxiosResponse<AdoptableAreaData[]>, unknown, AdoptableAreaData[]>({
        queryKey: GetAdoptableAreas({ partnerId }).key,
        queryFn: GetAdoptableAreas({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const { data: sponsors } = useQuery<AxiosResponse<SponsorData[]>, unknown, SponsorData[]>({
        queryKey: GetSponsors({ partnerId }).key,
        queryFn: GetSponsors({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const { data: companies } = useQuery<
        AxiosResponse<ProfessionalCompanyData[]>,
        unknown,
        ProfessionalCompanyData[]
    >({
        queryKey: GetProfessionalCompanies({ partnerId }).key,
        queryFn: GetProfessionalCompanies({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const sponsorId = adoption?.sponsorId;
    const { data: cleanupLogs, isLoading: logsLoading } = useQuery<
        AxiosResponse<ProfessionalCleanupLogData[]>,
        unknown,
        ProfessionalCleanupLogData[]
    >({
        queryKey: GetAdoptionCleanupLogs({ sponsorId: sponsorId ?? '', adoptionId }).key,
        queryFn: GetAdoptionCleanupLogs({ sponsorId: sponsorId ?? '', adoptionId }).service,
        select: (res) => res.data,
        enabled: !!sponsorId && !!adoptionId,
    });

    const { mutate, isPending: isSubmitting } = useMutation({
        mutationKey: UpdateSponsoredAdoption().key,
        mutationFn: (body: SponsoredAdoptionData) =>
            UpdateSponsoredAdoption().service({ partnerId, id: adoptionId }, body),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetSponsoredAdoptions({ partnerId }).key });
            queryClient.invalidateQueries({
                queryKey: GetSponsoredAdoption({ partnerId, id: adoptionId }).key,
            });
            queryClient.invalidateQueries({ queryKey: GetSponsoredAdoptionCompliance({ partnerId }).key });
            toast({
                variant: 'primary',
                title: 'Sponsored adoption updated!',
                description: 'The sponsored adoption has been updated successfully.',
            });
            navigate(`/partnerdashboard/${partnerId}/community/sponsored-adoptions`);
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to update sponsored adoption. Please try again.',
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
            status: 'Active',
        },
    });

    useEffect(() => {
        if (adoption) {
            form.reset({
                adoptableAreaId: adoption.adoptableAreaId,
                sponsorId: adoption.sponsorId,
                professionalCompanyId: adoption.professionalCompanyId,
                startDate: adoption.startDate,
                endDate: adoption.endDate ?? '',
                cleanupFrequencyDays: adoption.cleanupFrequencyDays,
                status: adoption.status,
            });
        }
    }, [adoption, form]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!partnerId || !adoption) return;

            const updated: SponsoredAdoptionData = {
                ...adoption,
                adoptableAreaId: formValues.adoptableAreaId,
                sponsorId: formValues.sponsorId,
                professionalCompanyId: formValues.professionalCompanyId,
                startDate: formValues.startDate,
                endDate: formValues.endDate || null,
                cleanupFrequencyDays: formValues.cleanupFrequencyDays,
                status: formValues.status,
            };

            mutate(updated);
        },
        [partnerId, adoption, mutate],
    );

    const formatDate = (dateStr: string | null | undefined) => {
        if (!dateStr) return '-';
        return new Date(dateStr).toLocaleDateString();
    };

    if (isLoading) {
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
                            <CardTitle>Edit Sponsored Adoption</CardTitle>
                            <CardDescription>Update the sponsored adoption details.</CardDescription>
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
                                                    {sponsors?.map((sponsor) => (
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
                                                    {companies?.map((company) => (
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
                            <div className='grid grid-cols-1 md:grid-cols-4 gap-4'>
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
                                            <FormDescription>Optional.</FormDescription>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='cleanupFrequencyDays'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Frequency (days)</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='number' min={1} max={365} />
                                            </FormControl>
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
                                                        <SelectValue />
                                                    </SelectTrigger>
                                                </FormControl>
                                                <SelectContent>
                                                    {statuses.map((s) => (
                                                        <SelectItem key={s} value={s}>
                                                            {s}
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
                            Save Changes
                        </Button>
                    </div>
                </form>
            </Form>

            <Card className='mt-6'>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <ClipboardList className='h-5 w-5' />
                        Cleanup Logs
                    </CardTitle>
                    <CardDescription>
                        Professional cleanup activity logs for this sponsored adoption.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    {logsLoading ? (
                        <div className='py-4 text-center'>
                            <Loader2 className='h-6 w-6 animate-spin mx-auto' />
                        </div>
                    ) : cleanupLogs && cleanupLogs.length > 0 ? (
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Cleanup Date</TableHead>
                                    <TableHead>Duration (min)</TableHead>
                                    <TableHead>Bags</TableHead>
                                    <TableHead>Weight (lbs)</TableHead>
                                    <TableHead>Notes</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {cleanupLogs.map((log) => (
                                    <TableRow key={log.id}>
                                        <TableCell>{formatDate(log.cleanupDate)}</TableCell>
                                        <TableCell>{log.durationMinutes}</TableCell>
                                        <TableCell>{log.bagsCollected}</TableCell>
                                        <TableCell>
                                            {log.weightInPounds != null ? log.weightInPounds.toFixed(1) : '-'}
                                        </TableCell>
                                        <TableCell className='max-w-xs truncate'>
                                            {log.notes || '-'}
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    ) : (
                        <div className='text-center py-8 text-muted-foreground'>
                            <p>No cleanup logs recorded yet.</p>
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    );
};

export default PartnerCommunitySponsoredAdoptionEdit;
