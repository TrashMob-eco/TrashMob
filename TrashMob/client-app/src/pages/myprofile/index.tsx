import { useMutation, useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router';
import { FormEvent, useCallback, useEffect, useRef, useState } from 'react';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Camera, CircleUserRound, Loader2 } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormDescription, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Separator } from '@/components/ui/separator';
import { GetUserById, UpdateUser, UploadProfilePhoto, VerifyUniqueUserName } from '@/services/users';
import { useLogin } from '@/hooks/useLogin';
import { useToast } from '@/hooks/use-toast';
import UserData from '@/components/Models/UserData';

const profileSchema = z.object({
    userName: z
        .string()
        .min(3, 'Username must be at least 3 characters.')
        .max(64, 'Username must be at most 64 characters.'),
    givenName: z.string().max(64, 'First name must be at most 64 characters.').optional().default(''),
    surname: z.string().max(64, 'Last name must be at most 64 characters.').optional().default(''),
    city: z.string().optional().default(''),
    region: z.string().optional().default(''),
    country: z.string().optional().default(''),
    postalCode: z.string().optional().default(''),
});

export const MyProfile = () => {
    const navigate = useNavigate();
    const { toast } = useToast();
    const { currentUser, handleUserUpdated } = useLogin();
    const [userNameError, setUserNameError] = useState<string | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const uploadPhoto = useMutation({
        mutationKey: UploadProfilePhoto().key,
        mutationFn: UploadProfilePhoto().service,
        onSuccess: () => {
            toast({
                variant: 'primary',
                title: 'Profile photo updated!',
            });
            handleUserUpdated();
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Failed to upload photo. Please try again.',
            });
        },
    });

    const handleFileChange = useCallback(
        (e: React.ChangeEvent<HTMLInputElement>) => {
            const file = e.target.files?.[0];
            if (!file) return;

            const validTypes = ['image/jpeg', 'image/png', 'image/webp'];
            if (!validTypes.includes(file.type)) {
                toast({
                    variant: 'destructive',
                    title: 'Invalid file type. Please upload a JPEG, PNG, or WebP image.',
                });
                return;
            }

            const maxSize = 5 * 1024 * 1024; // 5MB
            if (file.size > maxSize) {
                toast({
                    variant: 'destructive',
                    title: 'File too large. Maximum size is 5MB.',
                });
                return;
            }

            uploadPhoto.mutate(file);

            // Reset input so the same file can be re-selected
            if (fileInputRef.current) {
                fileInputRef.current.value = '';
            }
        },
        [uploadPhoto, toast],
    );

    const { data: user } = useQuery({
        queryKey: GetUserById({ userId: currentUser.id }).key,
        queryFn: GetUserById({ userId: currentUser.id }).service,
        select: (res) => res.data,
    });

    const updateUser = useMutation({
        mutationKey: UpdateUser().key,
        mutationFn: UpdateUser().service,
        onSuccess: () => {
            toast({
                variant: 'primary',
                title: 'Profile updated!',
            });
            handleUserUpdated();
        },
        onError: (error: Error) => {
            toast({
                variant: 'destructive',
                title: `Failed to update profile. Error: ${error.message}`,
            });
        },
    });

    const form = useForm<z.infer<typeof profileSchema>>({
        resolver: zodResolver(profileSchema),
        defaultValues: {},
    });

    useEffect(() => {
        if (!user) return;

        form.reset({
            userName: user.userName,
            givenName: user.givenName || '',
            surname: user.surname || '',
            city: user.city || '',
            region: user.region || '',
            country: user.country || '',
            postalCode: user.postalCode || '',
        });
    }, [user]);

    const onSubmit = useCallback(
        async (values: z.infer<typeof profileSchema>) => {
            if (!currentUser || !user) return;

            // Check username uniqueness before saving
            if (values.userName !== user.userName) {
                try {
                    const res = await VerifyUniqueUserName({
                        userId: currentUser.id,
                        userName: values.userName,
                    }).service();
                    if (res.status === 409) {
                        setUserNameError('This username is already taken.');
                        return;
                    }
                } catch {
                    setUserNameError('This username is already taken.');
                    return;
                }
            }

            setUserNameError(null);

            const body = new UserData();
            body.id = currentUser.id;
            body.email = currentUser.email ?? '';
            body.dateAgreedToTrashMobWaiver = new Date(currentUser.dateAgreedToTrashMobWaiver);
            body.memberSince = new Date(currentUser.memberSince);
            body.trashMobWaiverVersion = currentUser.trashMobWaiverVersion;
            body.latitude = currentUser.latitude;
            body.longitude = currentUser.longitude;
            body.prefersMetric = currentUser.prefersMetric;
            body.travelLimitForLocalEvents = currentUser.travelLimitForLocalEvents;
            body.isSiteAdmin = currentUser.isSiteAdmin;
            body.dateOfBirth = currentUser.dateOfBirth;
            body.profilePhotoUrl = currentUser.profilePhotoUrl;

            body.userName = values.userName;
            body.givenName = values.givenName ?? '';
            body.surname = values.surname ?? '';
            body.city = values.city ?? '';
            body.region = values.region ?? '';
            body.country = values.country ?? '';
            body.postalCode = values.postalCode ?? '';

            await updateUser.mutateAsync(body);
        },
        [currentUser, user],
    );

    const handleCancel = useCallback(
        (event: FormEvent<HTMLElement>) => {
            event.preventDefault();
            navigate('/');
        },
        [navigate],
    );

    if (!user) return null;

    return (
        <div>
            <HeroSection Title='My Profile' Description='Manage your account information' />
            <div className='container mx-auto py-5'>
                <Card>
                    <CardHeader>
                        <CardTitle>Profile Information</CardTitle>
                    </CardHeader>
                    <Form {...form}>
                        <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-2'>
                            <CardContent className='space-y-6'>
                                {/* Profile photo */}
                                <div className='flex items-center gap-4'>
                                    <input
                                        ref={fileInputRef}
                                        type='file'
                                        accept='image/jpeg,image/png,image/webp'
                                        className='hidden'
                                        onChange={handleFileChange}
                                    />
                                    <button
                                        type='button'
                                        className='relative group cursor-pointer'
                                        onClick={() => fileInputRef.current?.click()}
                                        disabled={uploadPhoto.isPending}
                                    >
                                        {user.profilePhotoUrl ? (
                                            <img
                                                src={user.profilePhotoUrl}
                                                alt={user.userName}
                                                className='h-16 w-16 rounded-full object-cover'
                                                referrerPolicy='no-referrer'
                                            />
                                        ) : (
                                            <CircleUserRound className='h-16 w-16 text-muted-foreground' />
                                        )}
                                        <div className='absolute inset-0 flex items-center justify-center rounded-full bg-black/50 opacity-0 group-hover:opacity-100 transition-opacity'>
                                            {uploadPhoto.isPending ? (
                                                <Loader2 className='h-6 w-6 text-white animate-spin' />
                                            ) : (
                                                <Camera className='h-6 w-6 text-white' />
                                            )}
                                        </div>
                                    </button>
                                    <div>
                                        <Button
                                            type='button'
                                            variant='outline'
                                            size='sm'
                                            onClick={() => fileInputRef.current?.click()}
                                            disabled={uploadPhoto.isPending}
                                        >
                                            {uploadPhoto.isPending ? (
                                                <>
                                                    <Loader2 className='mr-2 h-4 w-4 animate-spin' />
                                                    Uploading...
                                                </>
                                            ) : (
                                                'Change Photo'
                                            )}
                                        </Button>
                                        <p className='text-sm text-muted-foreground mt-1'>
                                            JPEG, PNG, or WebP. Max 5MB.
                                        </p>
                                    </div>
                                </div>

                                <Separator />

                                {/* Account info */}
                                <div className='grid grid-cols-12 gap-4'>
                                    <FormField
                                        control={form.control}
                                        name='userName'
                                        render={({ field }) => (
                                            <FormItem className='col-span-12 sm:col-span-6'>
                                                <FormLabel>Username</FormLabel>
                                                <FormControl>
                                                    <Input
                                                        {...field}
                                                        onChange={(e) => {
                                                            field.onChange(e);
                                                            setUserNameError(null);
                                                        }}
                                                    />
                                                </FormControl>
                                                <FormMessage />
                                                {userNameError ? (
                                                    <p className='text-sm font-medium text-destructive'>
                                                        {userNameError}
                                                    </p>
                                                ) : null}
                                            </FormItem>
                                        )}
                                    />

                                    <div className='col-span-12 sm:col-span-6'>
                                        <div className='space-y-2'>
                                            <FormLabel>Email</FormLabel>
                                            <Input value={user.email} disabled />
                                            <FormDescription>
                                                Email is managed by your identity provider and cannot be changed here.
                                            </FormDescription>
                                        </div>
                                    </div>

                                    <FormField
                                        control={form.control}
                                        name='givenName'
                                        render={({ field }) => (
                                            <FormItem className='col-span-12 sm:col-span-6'>
                                                <FormLabel>First Name</FormLabel>
                                                <FormControl>
                                                    <Input {...field} />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />

                                    <FormField
                                        control={form.control}
                                        name='surname'
                                        render={({ field }) => (
                                            <FormItem className='col-span-12 sm:col-span-6'>
                                                <FormLabel>Last Name</FormLabel>
                                                <FormControl>
                                                    <Input {...field} />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />

                                    <div className='col-span-12 sm:col-span-6'>
                                        <div className='space-y-2'>
                                            <FormLabel>Member Since</FormLabel>
                                            <Input value={new Date(user.memberSince).toLocaleDateString()} disabled />
                                        </div>
                                    </div>
                                </div>

                                <Separator />

                                {/* Location */}
                                <div>
                                    <h3 className='text-lg font-medium mb-4'>Location</h3>
                                    <div className='grid grid-cols-12 gap-4'>
                                        <FormField
                                            control={form.control}
                                            name='city'
                                            render={({ field }) => (
                                                <FormItem className='col-span-12 sm:col-span-6'>
                                                    <FormLabel>City</FormLabel>
                                                    <FormControl>
                                                        <Input {...field} />
                                                    </FormControl>
                                                    <FormMessage />
                                                </FormItem>
                                            )}
                                        />

                                        <FormField
                                            control={form.control}
                                            name='region'
                                            render={({ field }) => (
                                                <FormItem className='col-span-12 sm:col-span-6'>
                                                    <FormLabel>State / Region</FormLabel>
                                                    <FormControl>
                                                        <Input {...field} />
                                                    </FormControl>
                                                    <FormMessage />
                                                </FormItem>
                                            )}
                                        />

                                        <FormField
                                            control={form.control}
                                            name='country'
                                            render={({ field }) => (
                                                <FormItem className='col-span-12 sm:col-span-6'>
                                                    <FormLabel>Country</FormLabel>
                                                    <FormControl>
                                                        <Input {...field} />
                                                    </FormControl>
                                                    <FormMessage />
                                                </FormItem>
                                            )}
                                        />

                                        <FormField
                                            control={form.control}
                                            name='postalCode'
                                            render={({ field }) => (
                                                <FormItem className='col-span-12 sm:col-span-6'>
                                                    <FormLabel>Postal Code</FormLabel>
                                                    <FormControl>
                                                        <Input {...field} />
                                                    </FormControl>
                                                    <FormMessage />
                                                </FormItem>
                                            )}
                                        />
                                    </div>
                                </div>
                            </CardContent>
                            <CardFooter className='flex gap-2 justify-end'>
                                <Button variant='outline' onClick={handleCancel}>
                                    Discard
                                </Button>
                                <Button type='submit' disabled={updateUser.isLoading} variant='default'>
                                    {updateUser.isLoading ? <Loader2 className='animate-spin' /> : null}
                                    Save
                                </Button>
                            </CardFooter>
                        </form>
                    </Form>
                </Card>
            </div>
        </div>
    );
};
