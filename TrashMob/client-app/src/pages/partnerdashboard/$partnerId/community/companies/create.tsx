import { useCallback } from 'react';
import { useParams, useNavigate, Link } from 'react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2, ArrowLeft } from 'lucide-react';

import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { useToast } from '@/hooks/use-toast';
import ProfessionalCompanyData from '@/components/Models/ProfessionalCompanyData';
import { CreateProfessionalCompany, GetProfessionalCompanies } from '@/services/professional-companies';

interface FormInputs {
    name: string;
    contactEmail: string;
    contactPhone: string;
}

const formSchema = z.object({
    name: z.string().min(1, 'Name is required').max(200, 'Name must be less than 200 characters'),
    contactEmail: z.string().max(256).email('Must be a valid email').or(z.literal('')),
    contactPhone: z.string().max(30),
});

export const PartnerCommunityCompanyCreate = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const { toast } = useToast();

    const { mutate, isPending: isSubmitting } = useMutation({
        mutationKey: CreateProfessionalCompany().key,
        mutationFn: (body: ProfessionalCompanyData) => CreateProfessionalCompany().service({ partnerId }, body),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetProfessionalCompanies({ partnerId }).key,
            });
            toast({
                variant: 'primary',
                title: 'Company created!',
                description: 'The professional company has been created successfully.',
            });
            navigate(`/partnerdashboard/${partnerId}/community/companies`);
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to create company. Please try again.',
            });
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: '',
            contactEmail: '',
            contactPhone: '',
        },
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!partnerId) return;

            const company: ProfessionalCompanyData = {
                ...new ProfessionalCompanyData(),
                partnerId,
                name: formValues.name,
                contactEmail: formValues.contactEmail,
                contactPhone: formValues.contactPhone,
            };

            mutate(company);
        },
        [partnerId, mutate],
    );

    return (
        <div className='py-8'>
            <div className='mb-6'>
                <Button variant='ghost' size='sm' asChild>
                    <Link to={`/partnerdashboard/${partnerId}/community/companies`}>
                        <ArrowLeft className='h-4 w-4 mr-2' />
                        Back to Companies
                    </Link>
                </Button>
            </div>

            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-6'>
                    <Card>
                        <CardHeader>
                            <CardTitle>Create Professional Company</CardTitle>
                            <CardDescription>
                                Add a new company that performs professional cleanup services.
                            </CardDescription>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <FormField
                                control={form.control}
                                name='name'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel required>Company Name</FormLabel>
                                        <FormControl>
                                            <Input {...field} placeholder='e.g., CleanPro Services' />
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
                                                <Input
                                                    {...field}
                                                    type='email'
                                                    placeholder='contact@company.com'
                                                />
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
                        </CardContent>
                    </Card>

                    <div className='flex justify-end gap-2'>
                        <Button
                            type='button'
                            variant='outline'
                            onClick={() => navigate(`/partnerdashboard/${partnerId}/community/companies`)}
                        >
                            Cancel
                        </Button>
                        <Button type='submit' disabled={isSubmitting}>
                            {isSubmitting ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                            Create Company
                        </Button>
                    </div>
                </form>
            </Form>
        </div>
    );
};

export default PartnerCommunityCompanyCreate;
