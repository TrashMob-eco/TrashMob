import { FC, useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { ArrowLeft, Clock, Mail, UserPlus } from 'lucide-react';
import { Link } from 'react-router';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { InitiateChildInitiatedConsent } from '@/services/privo-consent';
import { DatePicker } from '@/components/ui/datepicker';
import { isUnder13 } from '@/lib/age-utils';

const childSignupSchema = z.object({
    parentEmail: z.string().email('Please enter a valid email address.'),
    childFirstName: z.string().min(1, 'First name is required.').max(100),
    childEmail: z.string().email('Please enter a valid email address.'),
    childBirthDate: z.date({ required_error: 'Date of birth is required.' }),
});

type FormValues = z.infer<typeof childSignupSchema>;

type Step = 'parent-email' | 'child-info' | 'pending' | 'no-parent';

export const ChildSignup: FC = () => {
    const { toast } = useToast();
    const [step, setStep] = useState<Step>('parent-email');
    const [parentEmail, setParentEmail] = useState('');

    const form = useForm<FormValues>({
        resolver: zodResolver(childSignupSchema),
        defaultValues: { parentEmail: '', childFirstName: '', childEmail: '', childBirthDate: undefined },
    });

    const consentMutation = useMutation({
        mutationKey: InitiateChildInitiatedConsent().key,
        mutationFn: InitiateChildInitiatedConsent().service,
        onSuccess: (res) => {
            if (res.status === 204) {
                // Parent account doesn't exist
                setStep('no-parent');
            } else {
                setStep('pending');
            }
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Unable to proceed', description: error.message });
        },
    });

    const handleParentEmailSubmit = () => {
        const email = form.getValues('parentEmail');
        if (!email || !email.includes('@')) {
            form.setError('parentEmail', { message: 'Please enter a valid email address.' });
            return;
        }
        setParentEmail(email);
        setStep('child-info');
    };

    const handleChildInfoSubmit = (values: FormValues) => {
        if (values.childBirthDate && isUnder13(values.childBirthDate)) {
            toast({
                variant: 'destructive',
                title: 'Age requirement',
                description: 'You must be 13 or older to create an account.',
            });
            return;
        }

        consentMutation.mutate({
            parentEmail: values.parentEmail,
            childFirstName: values.childFirstName,
            childEmail: values.childEmail,
            childBirthDate: values.childBirthDate.toISOString().split('T')[0],
        });
    };

    return (
        <div>
            <HeroSection Title='Create Account (Under 18)' Description='Parental consent is required for minors' />
            <div className='container mx-auto py-5 max-w-lg'>
                <Button variant='ghost' size='sm' asChild className='mb-4'>
                    <Link to='/'>
                        <ArrowLeft className='mr-2 h-4 w-4' /> Back to Home
                    </Link>
                </Button>

                {step === 'parent-email' && (
                    <Card>
                        <CardHeader>
                            <div className='flex items-center gap-2'>
                                <Mail className='h-5 w-5' />
                                <CardTitle>Parent Email Required</CardTitle>
                            </div>
                            <CardDescription>
                                Since you are under 18, we need your parent or guardian&apos;s email to request their
                                consent.
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <Form {...form}>
                                <div className='space-y-4'>
                                    <FormField
                                        control={form.control}
                                        name='parentEmail'
                                        render={({ field }) => (
                                            <FormItem>
                                                <FormLabel>Parent/Guardian Email *</FormLabel>
                                                <FormControl>
                                                    <Input type='email' placeholder='parent@example.com' {...field} />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <Button onClick={handleParentEmailSubmit}>Continue</Button>
                                </div>
                            </Form>
                        </CardContent>
                    </Card>
                )}

                {step === 'child-info' && (
                    <Card>
                        <CardHeader>
                            <div className='flex items-center gap-2'>
                                <UserPlus className='h-5 w-5' />
                                <CardTitle>Your Information</CardTitle>
                            </div>
                            <CardDescription>
                                We&apos;ll send a consent request to {parentEmail}. Once approved, you&apos;ll receive
                                an email to create your account.
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <Form {...form}>
                                <form onSubmit={form.handleSubmit(handleChildInfoSubmit)} className='space-y-4'>
                                    <FormField
                                        control={form.control}
                                        name='childFirstName'
                                        render={({ field }) => (
                                            <FormItem>
                                                <FormLabel>First Name *</FormLabel>
                                                <FormControl>
                                                    <Input {...field} />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='childEmail'
                                        render={({ field }) => (
                                            <FormItem>
                                                <FormLabel>Your Email *</FormLabel>
                                                <FormControl>
                                                    <Input type='email' {...field} />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='childBirthDate'
                                        render={({ field }) => (
                                            <FormItem>
                                                <FormLabel>Date of Birth *</FormLabel>
                                                <FormControl>
                                                    <DatePicker
                                                        value={field.value}
                                                        onChange={field.onChange}
                                                        placeholder='Select date of birth'
                                                        calendarProps={{
                                                            captionLayout: 'dropdown',
                                                            startMonth: new Date(new Date().getFullYear() - 18, 0),
                                                            endMonth: new Date(new Date().getFullYear(), 11),
                                                            defaultMonth: new Date(2010, 0),
                                                        }}
                                                    />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <div className='flex gap-2'>
                                        <Button variant='outline' type='button' onClick={() => setStep('parent-email')}>
                                            Back
                                        </Button>
                                        <Button type='submit' disabled={consentMutation.isPending}>
                                            {consentMutation.isPending ? 'Sending...' : 'Request Consent'}
                                        </Button>
                                    </div>
                                </form>
                            </Form>
                        </CardContent>
                    </Card>
                )}

                {step === 'pending' && (
                    <Card>
                        <CardHeader>
                            <div className='flex items-center gap-2'>
                                <Clock className='h-5 w-5 text-amber-500' />
                                <CardTitle>Consent Requested</CardTitle>
                            </div>
                        </CardHeader>
                        <CardContent className='space-y-3'>
                            <p>
                                We&apos;ve sent a consent request to your parent/guardian at{' '}
                                <strong>{parentEmail}</strong>.
                            </p>
                            <p className='text-sm text-muted-foreground'>
                                Once they approve, you&apos;ll receive an email with a link to create your TrashMob
                                account. This usually takes a few minutes.
                            </p>
                        </CardContent>
                    </Card>
                )}

                {step === 'no-parent' && (
                    <Card>
                        <CardHeader>
                            <div className='flex items-center gap-2'>
                                <Mail className='h-5 w-5 text-amber-500' />
                                <CardTitle>Parent Account Needed</CardTitle>
                            </div>
                        </CardHeader>
                        <CardContent className='space-y-3'>
                            <p>
                                We couldn&apos;t find a TrashMob account for <strong>{parentEmail}</strong>.
                            </p>
                            <p className='text-sm text-muted-foreground'>
                                Your parent/guardian needs to create a TrashMob account first, then they can add you as
                                a dependent from their dashboard.
                            </p>
                            <Button variant='outline' onClick={() => setStep('parent-email')}>
                                Try a Different Email
                            </Button>
                        </CardContent>
                    </Card>
                )}
            </div>
        </div>
    );
};

export default ChildSignup;
