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
import { useToast } from '@/hooks/use-toast';
import { Loader2 } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { MarkdownEditor } from '@/components/ui/custom/markdown-editor';
import { Checkbox } from '@/components/ui/checkbox';
import { useLogin } from '@/hooks/useLogin';
import { CreateWaiverVersion, GetAllWaiverVersions } from '@/services/waiver-admin';
import { WaiverScope, WaiverVersionRequest } from '@/components/Models/WaiverVersionData';

interface FormInputs {
    name: string;
    version: string;
    scope: WaiverScope;
    effectiveDate: string;
    waiverText: string;
    isActive: boolean;
}

const formSchema = z.object({
    name: z.string().min(1, 'Name is required'),
    version: z.string().min(1, 'Version is required'),
    scope: z.nativeEnum(WaiverScope),
    effectiveDate: z.string().min(1, 'Effective date is required'),
    waiverText: z.string().min(1, 'Waiver text is required'),
    isActive: z.boolean(),
});

export const SiteAdminWaiverCreate = () => {
    const { currentUser } = useLogin();
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const onCreateSuccess = () => {
        toast({
            variant: 'primary',
            title: 'Waiver created!',
            description: '',
        });
        queryClient.invalidateQueries({
            queryKey: GetAllWaiverVersions().key,
            refetchType: 'all',
        });
        navigate(`/siteadmin/waivers`);
    };

    const createWaiver = useMutation({
        mutationKey: CreateWaiverVersion().key,
        mutationFn: CreateWaiverVersion().service,
        onSuccess: onCreateSuccess,
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: '',
            version: '1.0',
            scope: WaiverScope.Global,
            effectiveDate: new Date().toISOString().split('T')[0],
            waiverText: '',
            isActive: true,
        },
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            const body: WaiverVersionRequest = {
                name: formValues.name,
                version: formValues.version,
                scope: formValues.scope,
                effectiveDate: new Date(formValues.effectiveDate).toISOString(),
                waiverText: formValues.waiverText,
                isActive: formValues.isActive,
            };
            createWaiver.mutate(body);
        },
        [createWaiver],
    );

    const isSubmitting = createWaiver.isPending;

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                <FormField
                    control={form.control}
                    name='name'
                    render={({ field }) => (
                        <FormItem className='col-span-8'>
                            <FormLabel required>Name</FormLabel>
                            <FormControl>
                                <Input {...field} placeholder='e.g., TrashMob Liability Waiver' />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='version'
                    render={({ field }) => (
                        <FormItem className='col-span-4'>
                            <FormLabel required>Version</FormLabel>
                            <FormControl>
                                <Input {...field} placeholder='e.g., 2.0' />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='scope'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormLabel required>Scope</FormLabel>
                            <Select onValueChange={(val) => field.onChange(parseInt(val))} value={String(field.value)}>
                                <FormControl>
                                    <SelectTrigger>
                                        <SelectValue placeholder='Select scope' />
                                    </SelectTrigger>
                                </FormControl>
                                <SelectContent>
                                    <SelectItem value={String(WaiverScope.Global)}>Global</SelectItem>
                                    <SelectItem value={String(WaiverScope.Community)}>Community</SelectItem>
                                </SelectContent>
                            </Select>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='effectiveDate'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormLabel required>Effective Date</FormLabel>
                            <FormControl>
                                <Input type='date' {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='waiverText'
                    render={({ field }) => (
                        <FormItem className='col-span-12'>
                            <FormLabel required>Waiver Text (Markdown supported)</FormLabel>
                            <FormControl>
                                <MarkdownEditor
                                    value={field.value}
                                    onChange={field.onChange}
                                    placeholder='Enter the full waiver text here. You can use Markdown formatting.'
                                />
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
                        <Link to='/siteadmin/waivers'>Cancel</Link>
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
