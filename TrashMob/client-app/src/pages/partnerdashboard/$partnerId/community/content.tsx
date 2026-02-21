import { useCallback, useEffect } from 'react';
import { Link, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2 } from 'lucide-react';

import { Form, FormControl, FormField, FormItem, FormMessage, FormDescription } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/hooks/use-toast';
import { ImageCropUpload } from '@/components/ui/image-crop-upload';
import CommunityData from '@/components/Models/CommunityData';
import {
    GetCommunityForAdmin,
    UpdateCommunityContent,
    UploadCommunityLogo,
    UploadCommunityBanner,
} from '@/services/communities';
import { GetPartnerById } from '@/services/partners';
import { getLocation, getRegionTypeLabel } from '@/lib/community-utils';

interface FormInputs {
    publicNotes: string;
    tagline: string;
    brandingPrimaryColor: string;
    brandingSecondaryColor: string;
    bannerImageUrl: string;
    logoUrl: string;
    contactEmail: string;
    contactPhone: string;
    physicalAddress: string;
    website: string;
}

const hexColorRegex = /^(#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3}))?$/;
const urlRegex = /^(https?:\/\/[^\s]+)?$/;
const emailRegex = /^([a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})?$/;

const formSchema = z.object({
    publicNotes: z.string().max(2048, 'About text must be less than 2048 characters'),
    tagline: z.string().max(100, 'Tagline must be less than 100 characters'),
    brandingPrimaryColor: z.string().regex(hexColorRegex, 'Must be a valid hex color (e.g., #3B82F6)'),
    brandingSecondaryColor: z.string().regex(hexColorRegex, 'Must be a valid hex color (e.g., #1E40AF)'),
    bannerImageUrl: z.string(),
    logoUrl: z.string(),
    contactEmail: z.string().regex(emailRegex, 'Must be a valid email'),
    contactPhone: z.string().max(50, 'Phone must be less than 50 characters'),
    physicalAddress: z.string().max(500, 'Address must be less than 500 characters'),
    website: z.string().regex(urlRegex, 'Must be a valid URL'),
});

export const PartnerCommunityContent = () => {
    const queryClient = useQueryClient();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const { toast } = useToast();

    const { data: currentValues, isLoading } = useQuery<AxiosResponse<CommunityData>, unknown, CommunityData>({
        queryKey: GetCommunityForAdmin({ communityId: partnerId }).key,
        queryFn: GetCommunityForAdmin({ communityId: partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const { mutate, isPending: isSubmitting } = useMutation({
        mutationKey: UpdateCommunityContent().key,
        mutationFn: UpdateCommunityContent().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetCommunityForAdmin({ communityId: partnerId }).key,
                refetchType: 'all',
            });
            queryClient.invalidateQueries({
                queryKey: GetPartnerById({ partnerId }).key,
                refetchType: 'all',
            });

            toast({
                variant: 'primary',
                title: 'Saved!',
                description: 'Community content has been updated.',
            });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to save changes. Please try again.',
            });
        },
    });

    const invalidateCommunity = useCallback(() => {
        queryClient.invalidateQueries({
            queryKey: GetCommunityForAdmin({ communityId: partnerId }).key,
            refetchType: 'all',
        });
    }, [queryClient, partnerId]);

    const uploadLogoMutation = useMutation({
        mutationFn: (file: File) => UploadCommunityLogo().service({ communityId: partnerId }, file),
        onSuccess: (res) => {
            form.setValue('logoUrl', res.data.url);
            invalidateCommunity();
            toast({ variant: 'primary', title: 'Logo uploaded!' });
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Error', description: 'Failed to upload logo.' });
        },
    });

    const uploadBannerMutation = useMutation({
        mutationFn: (file: File) => UploadCommunityBanner().service({ communityId: partnerId }, file),
        onSuccess: (res) => {
            form.setValue('bannerImageUrl', res.data.url);
            invalidateCommunity();
            toast({ variant: 'primary', title: 'Banner uploaded!' });
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Error', description: 'Failed to upload banner.' });
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            publicNotes: '',
            tagline: '',
            brandingPrimaryColor: '#3B82F6',
            brandingSecondaryColor: '#1E40AF',
            bannerImageUrl: '',
            logoUrl: '',
            contactEmail: '',
            contactPhone: '',
            physicalAddress: '',
            website: '',
        },
    });

    useEffect(() => {
        if (currentValues) {
            form.reset({
                publicNotes: currentValues.publicNotes || '',
                tagline: currentValues.tagline || '',
                brandingPrimaryColor: currentValues.brandingPrimaryColor || '#3B82F6',
                brandingSecondaryColor: currentValues.brandingSecondaryColor || '#1E40AF',
                bannerImageUrl: currentValues.bannerImageUrl || '',
                logoUrl: currentValues.logoUrl || '',
                contactEmail: currentValues.contactEmail || '',
                contactPhone: currentValues.contactPhone || '',
                physicalAddress: currentValues.physicalAddress || '',
                website: currentValues.website || '',
            });
        }
    }, [currentValues, form]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!currentValues) return;

            const body: CommunityData = {
                ...currentValues,
                publicNotes: formValues.publicNotes || '',
                tagline: formValues.tagline || '',
                brandingPrimaryColor: formValues.brandingPrimaryColor || '#3B82F6',
                brandingSecondaryColor: formValues.brandingSecondaryColor || '#1E40AF',
                bannerImageUrl: formValues.bannerImageUrl || '',
                logoUrl: formValues.logoUrl || '',
                contactEmail: formValues.contactEmail || '',
                contactPhone: formValues.contactPhone || '',
                physicalAddress: formValues.physicalAddress || '',
                website: formValues.website || '',
            };

            mutate(body);
        },
        [currentValues, mutate],
    );

    const primaryColor = form.watch('brandingPrimaryColor');
    const secondaryColor = form.watch('brandingSecondaryColor');

    if (isLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='py-8'>
            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-6'>
                    {/* Community Coverage (read-only, only for regional communities) */}
                    {currentValues && getRegionTypeLabel(currentValues.regionType) ? (
                        <Card>
                            <CardHeader>
                                <CardTitle>Community Coverage</CardTitle>
                                <CardDescription>
                                    Your community&apos;s geographic scope.{' '}
                                    <Link to='../community/regional-settings' className='text-primary hover:underline'>
                                        Edit regional settings
                                    </Link>
                                </CardDescription>
                            </CardHeader>
                            <CardContent>
                                <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                    <div>
                                        <p className='text-sm font-medium text-muted-foreground'>Type</p>
                                        <p className='text-sm'>
                                            {getRegionTypeLabel(currentValues.regionType)} Community
                                        </p>
                                    </div>
                                    <div>
                                        <p className='text-sm font-medium text-muted-foreground'>Location</p>
                                        <p className='text-sm'>{getLocation(currentValues)}</p>
                                    </div>
                                </div>
                            </CardContent>
                        </Card>
                    ) : null}

                    {/* About Section */}
                    <Card>
                        <CardHeader>
                            <CardTitle>About Your Community</CardTitle>
                            <CardDescription>
                                This information is displayed on your public community page.
                            </CardDescription>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <FormField
                                control={form.control}
                                name='tagline'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Tagline</FormLabel>
                                        <FormControl>
                                            <Input {...field} placeholder='A short description of your community' />
                                        </FormControl>
                                        <FormDescription>
                                            A brief tagline displayed below your community name.
                                        </FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name='publicNotes'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>About</FormLabel>
                                        <FormControl>
                                            <Textarea
                                                {...field}
                                                placeholder='Tell visitors about your community and its environmental initiatives...'
                                                className='h-32'
                                            />
                                        </FormControl>
                                        <FormDescription>
                                            This text appears in the About section of your community page.
                                        </FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        </CardContent>
                    </Card>

                    {/* Branding Section */}
                    <Card>
                        <CardHeader>
                            <CardTitle>Branding</CardTitle>
                            <CardDescription>Customize the look and feel of your community page.</CardDescription>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                <ImageCropUpload
                                    aspectRatio={1}
                                    currentImageUrl={form.watch('logoUrl') || undefined}
                                    onCropComplete={(file) => uploadLogoMutation.mutate(file)}
                                    label='Logo'
                                    recommendedSize='200×200px'
                                    uploading={uploadLogoMutation.isPending}
                                />
                                <ImageCropUpload
                                    aspectRatio={4}
                                    currentImageUrl={form.watch('bannerImageUrl') || undefined}
                                    onCropComplete={(file) => uploadBannerMutation.mutate(file)}
                                    label='Banner Image'
                                    recommendedSize='1200×300px'
                                    uploading={uploadBannerMutation.isPending}
                                />
                            </div>
                            <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                <FormField
                                    control={form.control}
                                    name='brandingPrimaryColor'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Primary Color</FormLabel>
                                            <div className='flex gap-2'>
                                                <FormControl>
                                                    <Input {...field} placeholder='#3B82F6' />
                                                </FormControl>
                                                <div
                                                    className='w-10 h-10 rounded border shrink-0'
                                                    style={{ backgroundColor: primaryColor || '#3B82F6' }}
                                                />
                                            </div>
                                            <FormDescription>Hex color code for primary elements.</FormDescription>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='brandingSecondaryColor'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Secondary Color</FormLabel>
                                            <div className='flex gap-2'>
                                                <FormControl>
                                                    <Input {...field} placeholder='#1E40AF' />
                                                </FormControl>
                                                <div
                                                    className='w-10 h-10 rounded border shrink-0'
                                                    style={{ backgroundColor: secondaryColor || '#1E40AF' }}
                                                />
                                            </div>
                                            <FormDescription>Hex color code for secondary elements.</FormDescription>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </div>
                        </CardContent>
                    </Card>

                    {/* Contact Information */}
                    <Card>
                        <CardHeader>
                            <CardTitle>Contact Information</CardTitle>
                            <CardDescription>How visitors can reach your community.</CardDescription>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                <FormField
                                    control={form.control}
                                    name='contactEmail'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Contact Email</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='email' placeholder='contact@example.com' />
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
                            <FormField
                                control={form.control}
                                name='physicalAddress'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Physical Address</FormLabel>
                                        <FormControl>
                                            <Textarea
                                                {...field}
                                                placeholder='123 Main St, City, State 12345'
                                                className='h-20'
                                            />
                                        </FormControl>
                                        <FormDescription>
                                            Displayed on your community page for visitors.
                                        </FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name='website'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Website</FormLabel>
                                        <FormControl>
                                            <Input {...field} placeholder='https://www.example.com' />
                                        </FormControl>
                                        <FormDescription>Your community&apos;s main website.</FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        </CardContent>
                    </Card>

                    {/* Submit Button */}
                    <div className='flex justify-end gap-2'>
                        <Button type='button' variant='outline' onClick={() => form.reset()}>
                            Reset
                        </Button>
                        <Button type='submit' disabled={isSubmitting}>
                            {isSubmitting ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                            Save Changes
                        </Button>
                    </div>
                </form>
            </Form>
        </div>
    );
};

export default PartnerCommunityContent;
