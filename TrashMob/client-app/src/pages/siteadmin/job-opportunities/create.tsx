import { useCallback, useEffect } from 'react';
import { Link, useNavigate, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { Loader2 } from 'lucide-react';
import { Input } from '@/components/ui/input';
import * as ToolTips from '@/store/ToolTips';
import { useLogin } from '@/hooks/useLogin';
import { CreateJobOpportunity } from '@/services/opportunities';
import { Checkbox } from '@/components/ui/checkbox';
import { GetAllJobOpportunities } from '@/services/opportunities';
import JobOpportunityData from '@/components/Models/JobOpportunityData';
import { Textarea } from '@/components/ui/textarea';

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

export const SiteAdminJobOpportunityCreate = () => {
    const { currentUser } = useLogin();

    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const onCreateSuccess = (_data: unknown, variables: JobOpportunityData) => {
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

    const createJobOpportunity = useMutation({
        mutationKey: CreateJobOpportunity().key,
        mutationFn: CreateJobOpportunity().service,
        onSuccess: onCreateSuccess,
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            title: '',
            tagLine: '',
            fullDescription: '',
            isActive: true,
        },
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback((formValues) => {
        const body = new JobOpportunityData();
        body.title = formValues.title;
        body.tagLine = formValues.tagLine;
        body.fullDescription = formValues.fullDescription;
        body.isActive = formValues.isActive;
        body.createdByUserId = currentUser.id;
        body.lastUpdatedByUserId = currentUser.id;
        createJobOpportunity.mutate(body);
    }, []);

    const isSubmitting = createJobOpportunity.isLoading;

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
                                <Textarea {...field} />
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
