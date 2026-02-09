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
import { Textarea } from '@/components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useToast } from '@/hooks/use-toast';
import ProfessionalCleanupLogData from '@/components/Models/ProfessionalCleanupLogData';
import SponsoredAdoptionData from '@/components/Models/SponsoredAdoptionData';
import { GetCompanyAssignments, GetCompanyCleanupLogs, LogCleanup } from '@/services/professional-company-portal';

interface FormInputs {
    sponsoredAdoptionId: string;
    cleanupDate: string;
    durationMinutes: number;
    bagsCollected: number;
    weightInPounds: string;
    weightInKilograms: string;
    notes: string;
}

const formSchema = z.object({
    sponsoredAdoptionId: z.string().min(1, 'Please select an assignment'),
    cleanupDate: z.string().min(1, 'Cleanup date is required'),
    durationMinutes: z.coerce.number().min(1, 'Duration must be at least 1 minute'),
    bagsCollected: z.coerce.number().min(0, 'Cannot be negative'),
    weightInPounds: z.string(),
    weightInKilograms: z.string(),
    notes: z.string(),
});

const today = () => new Date().toISOString().split('T')[0];

export const CompanyLogCleanup = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { companyId } = useParams<{ companyId: string }>() as { companyId: string };
    const { toast } = useToast();

    const { data: assignments, isLoading } = useQuery<
        AxiosResponse<SponsoredAdoptionData[]>,
        unknown,
        SponsoredAdoptionData[]
    >({
        queryKey: GetCompanyAssignments({ companyId }).key,
        queryFn: GetCompanyAssignments({ companyId }).service,
        select: (res) => res.data,
        enabled: !!companyId,
    });

    const { mutate, isPending: isSubmitting } = useMutation({
        mutationKey: LogCleanup().key,
        mutationFn: (body: ProfessionalCleanupLogData) => LogCleanup().service({ companyId }, body),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetCompanyCleanupLogs({ companyId }).key });
            queryClient.invalidateQueries({ queryKey: GetCompanyAssignments({ companyId }).key });
            toast({
                variant: 'primary',
                title: 'Cleanup logged!',
                description: 'Your cleanup has been recorded successfully.',
            });
            navigate(`/companydashboard/${companyId}`);
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to log cleanup. Please try again.',
            });
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            sponsoredAdoptionId: '',
            cleanupDate: today(),
            durationMinutes: 60,
            bagsCollected: 0,
            weightInPounds: '',
            weightInKilograms: '',
            notes: '',
        },
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            const log = new ProfessionalCleanupLogData();
            log.sponsoredAdoptionId = formValues.sponsoredAdoptionId;
            log.professionalCompanyId = companyId;
            log.cleanupDate = formValues.cleanupDate;
            log.durationMinutes = formValues.durationMinutes;
            log.bagsCollected = formValues.bagsCollected;
            log.weightInPounds = formValues.weightInPounds ? parseFloat(formValues.weightInPounds) : null;
            log.weightInKilograms = formValues.weightInKilograms ? parseFloat(formValues.weightInKilograms) : null;
            log.notes = formValues.notes;

            mutate(log);
        },
        [companyId, mutate],
    );

    if (isLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='py-4'>
            <div className='mb-6'>
                <Button variant='ghost' size='sm' asChild>
                    <Link to={`/companydashboard/${companyId}`}>
                        <ArrowLeft className='h-4 w-4 mr-2' />
                        Back to Dashboard
                    </Link>
                </Button>
            </div>

            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-6'>
                    <Card>
                        <CardHeader>
                            <CardTitle>Log a Cleanup</CardTitle>
                            <CardDescription>
                                Record a professional cleanup for one of your assigned segments.
                            </CardDescription>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <FormField
                                control={form.control}
                                name='sponsoredAdoptionId'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel required>Assignment</FormLabel>
                                        <Select onValueChange={field.onChange} value={field.value}>
                                            <FormControl>
                                                <SelectTrigger className='h-12'>
                                                    <SelectValue placeholder='Select an assignment' />
                                                </SelectTrigger>
                                            </FormControl>
                                            <SelectContent>
                                                {assignments?.map((adoption) => (
                                                    <SelectItem key={adoption.id} value={adoption.id}>
                                                        {adoption.adoptableArea?.name || 'Unknown Area'}
                                                        {adoption.sponsor?.name ? ` — ${adoption.sponsor.name}` : ''}
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />

                            <div className='grid grid-cols-1 md:grid-cols-3 gap-4'>
                                <FormField
                                    control={form.control}
                                    name='cleanupDate'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Cleanup Date</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='date' className='h-12' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='durationMinutes'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Duration (minutes)</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='number' min={1} className='h-12' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='bagsCollected'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Bags Collected</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='number' min={0} className='h-12' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </div>

                            <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                <FormField
                                    control={form.control}
                                    name='weightInPounds'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Weight (lbs)</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='number' min={0} step='0.1' className='h-12' />
                                            </FormControl>
                                            <FormDescription>Optional.</FormDescription>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='weightInKilograms'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Weight (kg)</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='number' min={0} step='0.1' className='h-12' />
                                            </FormControl>
                                            <FormDescription>Optional.</FormDescription>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </div>

                            <FormField
                                control={form.control}
                                name='notes'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Notes</FormLabel>
                                        <FormControl>
                                            <Textarea
                                                {...field}
                                                className='h-24'
                                                placeholder='Any notes about this cleanup...'
                                            />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        </CardContent>
                    </Card>

                    <div className='flex justify-end gap-2'>
                        <Button
                            type='button'
                            variant='outline'
                            onClick={() => navigate(`/companydashboard/${companyId}`)}
                        >
                            Cancel
                        </Button>
                        <Button type='submit' disabled={isSubmitting} size='lg' className='h-12'>
                            {isSubmitting ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                            Submit Cleanup Log
                        </Button>
                    </div>
                </form>
            </Form>
        </div>
    );
};

export default CompanyLogCleanup;
