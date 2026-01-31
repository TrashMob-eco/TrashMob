import { useCallback, useEffect } from 'react';
import { Link, useNavigate, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { AxiosResponse } from 'axios';
import { ArrowLeft, Loader2 } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import LitterReportData from '@/components/Models/LitterReportData';
import { LitterReportStatusEnum, LitterReportStatusLabels } from '@/components/Models/LitterReportStatus';
import { GetLitterReport, UpdateLitterReport, GetUserLitterReports } from '@/services/litter-report';
import { useLogin } from '@/hooks/useLogin';
import { useToast } from '@/hooks/use-toast';

interface FormInputs {
    name: string;
    description: string;
    litterReportStatusId: string;
}

const formSchema = z.object({
    name: z.string().min(1, 'Name is required'),
    description: z.string(),
    litterReportStatusId: z.string(),
});

export const LitterReportEditPage = () => {
    const { litterReportId } = useParams<{ litterReportId: string }>() as { litterReportId: string };
    const { currentUser, isUserLoaded } = useLogin();
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const { data: litterReport, isLoading } = useQuery<AxiosResponse<LitterReportData>, unknown, LitterReportData>({
        queryKey: GetLitterReport({ litterReportId }).key,
        queryFn: GetLitterReport({ litterReportId }).service,
        select: (res) => res.data,
        enabled: !!litterReportId,
    });

    const canEdit =
        isUserLoaded && litterReport && (litterReport.createdByUserId === currentUser.id || currentUser.isSiteAdmin);

    const updateMutation = useMutation({
        mutationKey: UpdateLitterReport({ litterReport: litterReport! }).key,
        mutationFn: UpdateLitterReport({ litterReport: litterReport! }).service,
        onSuccess: async () => {
            toast({
                title: 'Litter report updated',
                description: 'The litter report has been successfully updated.',
            });
            await queryClient.invalidateQueries({
                queryKey: GetLitterReport({ litterReportId }).key,
            });
            await queryClient.invalidateQueries({
                queryKey: GetUserLitterReports({ userId: currentUser.id }).key,
            });
            navigate(`/litterreports/${litterReportId}`);
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to update the litter report. Please try again.',
            });
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: '',
            description: '',
            litterReportStatusId: '1',
        },
    });

    useEffect(() => {
        if (litterReport) {
            form.reset({
                name: litterReport.name || '',
                description: litterReport.description || '',
                litterReportStatusId: String(litterReport.litterReportStatusId),
            });
        }
    }, [litterReport, form]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!litterReport) return;

            const updatedReport: LitterReportData = {
                ...litterReport,
                name: formValues.name,
                description: formValues.description,
                litterReportStatusId: parseInt(formValues.litterReportStatusId, 10),
                lastUpdatedByUserId: currentUser.id,
            };

            updateMutation.mutate({ litterReport: updatedReport } as any);
        },
        [litterReport, currentUser.id, updateMutation],
    );

    if (isLoading) {
        return (
            <div>
                <HeroSection Title='Edit Litter Report' Description='Loading...' />
                <div className='container py-8 text-center'>Loading litter report...</div>
            </div>
        );
    }

    if (!litterReport) {
        return (
            <div>
                <HeroSection Title='Edit Litter Report' Description='Not Found' />
                <div className='container py-8 text-center'>
                    <p className='mb-4'>This litter report could not be found.</p>
                    <Button asChild>
                        <Link to='/litterreports'>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Litter Reports
                        </Link>
                    </Button>
                </div>
            </div>
        );
    }

    if (!canEdit) {
        return (
            <div>
                <HeroSection Title='Edit Litter Report' Description='Access Denied' />
                <div className='container py-8 text-center'>
                    <p className='mb-4'>You do not have permission to edit this litter report.</p>
                    <Button asChild>
                        <Link to={`/litterreports/${litterReportId}`}>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Report
                        </Link>
                    </Button>
                </div>
            </div>
        );
    }

    const statusOptions = [
        { value: String(LitterReportStatusEnum.New), label: LitterReportStatusLabels[LitterReportStatusEnum.New] },
        {
            value: String(LitterReportStatusEnum.Assigned),
            label: LitterReportStatusLabels[LitterReportStatusEnum.Assigned],
        },
        {
            value: String(LitterReportStatusEnum.Cleaned),
            label: LitterReportStatusLabels[LitterReportStatusEnum.Cleaned],
        },
        {
            value: String(LitterReportStatusEnum.Cancelled),
            label: LitterReportStatusLabels[LitterReportStatusEnum.Cancelled],
        },
    ];

    return (
        <div>
            <HeroSection Title='Edit Litter Report' Description={litterReport.name || 'Update litter report details'} />
            <div className='container py-8'>
                <div className='mb-4'>
                    <Button variant='outline' asChild>
                        <Link to={`/litterreports/${litterReportId}`}>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Report
                        </Link>
                    </Button>
                </div>

                <Card className='max-w-2xl'>
                    <Form {...form}>
                        <form onSubmit={form.handleSubmit(onSubmit)}>
                            <CardHeader>
                                <CardTitle>Edit Litter Report</CardTitle>
                            </CardHeader>
                            <CardContent className='space-y-4'>
                                <FormField
                                    control={form.control}
                                    name='name'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Name</FormLabel>
                                            <FormControl>
                                                <Input {...field} placeholder='Enter a name for this report' />
                                            </FormControl>
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
                                                    placeholder='Describe the litter location and details'
                                                    rows={4}
                                                />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='litterReportStatusId'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Status</FormLabel>
                                            <Select onValueChange={field.onChange} value={field.value}>
                                                <FormControl>
                                                    <SelectTrigger>
                                                        <SelectValue placeholder='Select status' />
                                                    </SelectTrigger>
                                                </FormControl>
                                                <SelectContent>
                                                    {statusOptions.map((option) => (
                                                        <SelectItem key={option.value} value={option.value}>
                                                            {option.label}
                                                        </SelectItem>
                                                    ))}
                                                </SelectContent>
                                            </Select>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </CardContent>
                            <CardFooter className='flex justify-end gap-2'>
                                <Button variant='outline' asChild>
                                    <Link to={`/litterreports/${litterReportId}`}>Cancel</Link>
                                </Button>
                                <Button type='submit' disabled={updateMutation.isPending}>
                                    {updateMutation.isPending ? (
                                        <Loader2 className='h-4 w-4 mr-1 animate-spin' />
                                    ) : null}
                                    Save Changes
                                </Button>
                            </CardFooter>
                        </form>
                    </Form>
                </Card>
            </div>
        </div>
    );
};

export default LitterReportEditPage;
