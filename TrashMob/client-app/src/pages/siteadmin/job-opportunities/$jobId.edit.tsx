import { useCallback, useEffect } from 'react';
import { Link, useNavigate, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { Loader2 } from 'lucide-react';
import { Input } from '@/components/ui/input';
import * as ToolTips from '@/store/ToolTips';
import { useLogin } from '@/hooks/useLogin';
import { GetJobOpportunityById, UpdateJobOpportunity } from '@/services/opportunities';
import { Checkbox } from '@/components/ui/checkbox';
import { GetAllJobOpportunities } from '@/services/opportunities';
import JobOpportunityData from '@/components/Models/JobOpportunityData';
import { MarkdownEditor } from '@/components/ui/custom/markdown-editor';

interface FormInputs {
    title: string;
    tagLine: string;
    fullDescription: string;
    isActive: boolean;
}

const formSchema = z.object({
    title: z.string({ required_error: 'Title cannot be blank.' }),
    tagLine: z.string(),
    fullDescription: z.string(),
    isActive: z.boolean(),
});

export const SiteAdminJobOpportunityEdit = () => {
    const { currentUser } = useLogin();
    const { jobId } = useParams<{ jobId: string }>() as { jobId: string };

    const { data: jobOpportunity } = useQuery({
        queryKey: GetJobOpportunityById(jobId).key,
        queryFn: GetJobOpportunityById(jobId).service,
        select: (res) => res.data,
    });

    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const onUpdateSuccess = (_data: unknown, variables: JobOpportunityData) => {
        toast({
            variant: 'primary',
            title: 'Job saved!',
            description: '',
        });
        queryClient.invalidateQueries({
            queryKey: GetAllJobOpportunities().key,
            refetchType: 'all',
        });

        navigate(`/siteadmin/job-opportunities`);
    };

    const updateJobOpportunity = useMutation({
        mutationKey: UpdateJobOpportunity().key,
        mutationFn: UpdateJobOpportunity().service,
        onSuccess: onUpdateSuccess,
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            title: jobOpportunity?.title,
            tagLine: jobOpportunity?.tagLine,
            fullDescription: jobOpportunity?.fullDescription,
            isActive: jobOpportunity?.isActive,
        },
    });

    useEffect(() => {
        if (jobOpportunity) {
            form.reset({
                title: jobOpportunity.title,
                tagLine: jobOpportunity.tagLine,
                fullDescription: jobOpportunity.fullDescription,
                isActive: jobOpportunity.isActive,
            });
        }
    }, [jobOpportunity]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback((formValues) => {
        const body = new JobOpportunityData();
        body.id = jobId;
        body.title = formValues.title;
        body.tagLine = formValues.tagLine;
        body.fullDescription = formValues.fullDescription;
        body.isActive = formValues.isActive;
        body.lastUpdatedByUserId = currentUser.id;
        updateJobOpportunity.mutate(body);
    }, []);

    const isSubmitting = updateJobOpportunity.isLoading;

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                <FormField
                    control={form.control}
                    name='title'
                    render={({ field }) => (
                        <FormItem className='col-span-12'>
                            <FormLabel tooltip={ToolTips.JobOpportunityTitle} required>
                                Title
                            </FormLabel>
                            <FormControl>
                                <Input {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='tagLine'
                    render={({ field }) => (
                        <FormItem className='col-span-12'>
                            <FormLabel tooltip={ToolTips.JobOpportunityTagLine} required>
                                Tag Line
                            </FormLabel>
                            <FormControl>
                                <Input {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='fullDescription'
                    render={({ field }) => (
                        <FormItem className='col-span-12'>
                            <FormLabel tooltip={ToolTips.JobOpportunityFullDescription} required>
                                Full Description
                            </FormLabel>
                            <FormControl>
                                <MarkdownEditor {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='isActive'
                    render={({ field }) => (
                        <FormItem className='col-span-3'>
                            <FormLabel> </FormLabel>
                            <FormControl>
                                <div className='flex items-center space-x-2 h-9'>
                                    <Checkbox id='isActive' checked={field.value} onCheckedChange={field.onChange} />
                                    <label
                                        htmlFor='isActive'
                                        className='text-sm mb-0 font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70'
                                    >
                                        Is Active
                                    </label>
                                </div>
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <div className='col-span-12 flex justify-end gap-2'>
                    <Button variant='secondary' data-test='cancel' asChild>
                        <Link to='/siteadmin/job-opportunities'>Cancel</Link>
                    </Button>
                    <Button type='submit' disabled={isSubmitting}>
                        {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                        Save
                    </Button>
                </div>
            </form>
        </Form>
    );
};
