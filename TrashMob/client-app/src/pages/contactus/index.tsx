import { GoogleReCaptchaProvider, useGoogleReCaptcha } from 'react-google-recaptcha-v3';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { Loader2 } from 'lucide-react';
import { useNavigate } from 'react-router';
import { useMutation } from '@tanstack/react-query';

import { Recaptcha } from '@/config/recaptcha.config';
import { CreateContactRequest } from '@/services/contact';
import ContactRequestData from '@/components/Models/ContactRequestData';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/hooks/use-toast';
import { useCallback } from 'react';

const contactUsSchema = z.object({
    name: z.string().min(1, 'Name cannot be empty').max(1000, 'Name cannot be more than 64 characters long.'),
    email: z.string().email('Please enter valid email address.'),
    message: z
        .string()
        .min(1, 'Message cannot be empty')
        .max(1000, 'Message cannot be more than 1000 characters long.'),
    humanDontSeeThisField: z.string(),
});

export const ContactUs = () => {
    const navigate = useNavigate();
    const { toast } = useToast();
    const { executeRecaptcha } = useGoogleReCaptcha();

    const createContactRequest = useMutation<unknown, unknown, { body: ContactRequestData; captchaToken: string }>({
        mutationKey: CreateContactRequest().key,
        mutationFn: (variable) => CreateContactRequest().service(variable.body, variable.captchaToken),
        onSuccess: () => {
            navigate('/');

            toast({
                variant: 'primary',
                title: 'Contact submitted',
            });
        },
    });

    const form = useForm<z.infer<typeof contactUsSchema>>({
        resolver: zodResolver(contactUsSchema),
    });

    const onSubmit = useCallback(
        async (values: z.infer<typeof contactUsSchema>) => {
            /** Bot & Spam Protection */
            if (!executeRecaptcha) {
                return;
            }

            // If honeypot "humanDontSeeThisField" field is filled, silently reject the submission
            if (values.humanDontSeeThisField) {
                return;
            }

            // Execute reCAPTCHA and get token
            const captchaToken = await executeRecaptcha('form_submit');

            const { name, email, message } = values;
            const body = new ContactRequestData();
            body.name = name ?? '';
            body.email = email ?? '';
            body.message = message ?? '';

            return createContactRequest.mutateAsync({ body, captchaToken });
        },
        [executeRecaptcha],
    );

    return (
        <div className='tailwind'>
            <div className='w-full max-w-xl mx-auto py-16'>
                <Card>
                    <CardHeader>
                        <CardTitle>Contact us</CardTitle>
                        <CardDescription>
                            Have a question for the TrashMob team, want to submit a suggestion for improving the
                            website, or just want to tell us you love us? Drop us a note here and we'll be sure to read
                            it. Can't wait to hear from you!
                        </CardDescription>
                    </CardHeader>
                    <Form {...form}>
                        <form onSubmit={form.handleSubmit(onSubmit)}>
                            <CardContent className='space-y-4'>
                                <FormField
                                    control={form.control}
                                    name='name'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Name</FormLabel>
                                            <FormControl>
                                                <Input {...field} placeholder='Enter Name' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='email'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Email</FormLabel>
                                            <FormControl>
                                                <Input {...field} placeholder='Enter email' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='message'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Message</FormLabel>
                                            <FormControl>
                                                <Textarea {...field} rows={5} />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                {/* Honeypot field - hidden from humans but visible to bots */}
                                <div style={{ opacity: 0, position: 'absolute', top: '-999px' }}>
                                    <input
                                        {...form.register('humanDontSeeThisField')}
                                        tabIndex={-1}
                                        autoComplete='off'
                                    />
                                </div>
                            </CardContent>
                            <CardFooter className='flex gap-2 justify-end'>
                                <Button variant='outline' onClick={() => navigate('/')}>
                                    Back
                                </Button>
                                <Button type='submit' disabled={createContactRequest.isLoading} variant='default'>
                                    {createContactRequest.isLoading ? <Loader2 className='animate-spin' /> : null}
                                    Submit
                                </Button>
                            </CardFooter>
                        </form>
                    </Form>
                </Card>
            </div>
        </div>
    );
};

export const ContactUsWrapper = () => {
    return (
        <GoogleReCaptchaProvider reCaptchaKey={Recaptcha.KEY}>
            <ContactUs />
        </GoogleReCaptchaProvider>
    );
};
