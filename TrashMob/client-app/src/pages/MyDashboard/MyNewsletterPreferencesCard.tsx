import { FC, useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Mail, Bell, BellOff } from 'lucide-react';
import { AxiosResponse } from 'axios';

import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Switch } from '@/components/ui/switch';
import { Label } from '@/components/ui/label';
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { useToast } from '@/hooks/use-toast';
import {
    GetMyNewsletterPreferences,
    UpdateNewsletterPreference,
    UnsubscribeAllNewsletters,
    UserNewsletterPreference,
} from '@/services/newsletters';

export const MyNewsletterPreferencesCard: FC = () => {
    const queryClient = useQueryClient();
    const { toast } = useToast();
    const [showUnsubscribeAll, setShowUnsubscribeAll] = useState(false);

    const { data: preferences, isLoading } = useQuery<
        AxiosResponse<UserNewsletterPreference[]>,
        unknown,
        UserNewsletterPreference[]
    >({
        queryKey: GetMyNewsletterPreferences().key,
        queryFn: GetMyNewsletterPreferences().service,
        select: (res) => res.data,
    });

    const updateMutation = useMutation({
        mutationKey: UpdateNewsletterPreference().key,
        mutationFn: UpdateNewsletterPreference().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetMyNewsletterPreferences().key });
        },
        onError: () => {
            toast({
                title: 'Error',
                description: 'Failed to update preference. Please try again.',
                variant: 'destructive',
            });
        },
    });

    const unsubscribeAllMutation = useMutation({
        mutationKey: UnsubscribeAllNewsletters().key,
        mutationFn: UnsubscribeAllNewsletters().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetMyNewsletterPreferences().key });
            toast({
                title: 'Unsubscribed',
                description: 'You have been unsubscribed from all newsletters.',
            });
            setShowUnsubscribeAll(false);
        },
        onError: () => {
            toast({
                title: 'Error',
                description: 'Failed to unsubscribe. Please try again.',
                variant: 'destructive',
            });
        },
    });

    const handleToggle = (categoryId: number, currentValue: boolean) => {
        updateMutation.mutate({ categoryId, isSubscribed: !currentValue });
    };

    const handleUnsubscribeAll = () => {
        unsubscribeAllMutation.mutate();
    };

    const subscribedCount = (preferences || []).filter((p) => p.isSubscribed).length;
    const totalCount = (preferences || []).length;

    if (isLoading) {
        return (
            <Card className='mb-4'>
                <CardHeader>
                    <div className='flex flex-row items-center'>
                        <Mail className='inline-block h-5 w-5 mr-2 text-primary' />
                        <CardTitle className='grow text-primary'>Newsletter Preferences</CardTitle>
                    </div>
                </CardHeader>
                <CardContent>
                    <p className='text-muted-foreground text-center py-4'>Loading preferences...</p>
                </CardContent>
            </Card>
        );
    }

    return (
        <>
            <Card className='mb-4'>
                <CardHeader>
                    <div className='flex flex-row items-center'>
                        <Mail className='inline-block h-5 w-5 mr-2 text-primary' />
                        <CardTitle className='grow text-primary'>
                            Newsletter Preferences ({subscribedCount}/{totalCount})
                        </CardTitle>
                        {subscribedCount > 0 ? (
                            <Button variant='outline' size='sm' onClick={() => setShowUnsubscribeAll(true)}>
                                <BellOff className='h-4 w-4 mr-1' />
                                Unsubscribe All
                            </Button>
                        ) : null}
                    </div>
                    <CardDescription>
                        Manage which newsletters you receive from TrashMob.eco. You can unsubscribe at any time.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    {totalCount === 0 ? (
                        <p className='text-muted-foreground text-center py-4'>No newsletter categories available.</p>
                    ) : (
                        <div className='space-y-4'>
                            {(preferences || []).map((pref) => (
                                <div
                                    key={pref.categoryId}
                                    className='flex items-center justify-between py-3 px-4 rounded-lg border bg-card'
                                >
                                    <div className='flex items-start gap-3'>
                                        {pref.isSubscribed ? (
                                            <Bell className='h-5 w-5 text-primary mt-0.5' />
                                        ) : (
                                            <BellOff className='h-5 w-5 text-muted-foreground mt-0.5' />
                                        )}
                                        <div>
                                            <Label htmlFor={`newsletter-${pref.categoryId}`} className='font-medium'>
                                                {pref.categoryName}
                                            </Label>
                                            {pref.categoryDescription ? (
                                                <p className='text-sm text-muted-foreground mt-1'>
                                                    {pref.categoryDescription}
                                                </p>
                                            ) : null}
                                        </div>
                                    </div>
                                    <Switch
                                        id={`newsletter-${pref.categoryId}`}
                                        checked={pref.isSubscribed}
                                        onCheckedChange={() => handleToggle(pref.categoryId, pref.isSubscribed)}
                                        disabled={updateMutation.isPending}
                                    />
                                </div>
                            ))}
                        </div>
                    )}
                </CardContent>
            </Card>

            <AlertDialog open={showUnsubscribeAll} onOpenChange={setShowUnsubscribeAll}>
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>Unsubscribe from all newsletters?</AlertDialogTitle>
                        <AlertDialogDescription>
                            You will stop receiving all newsletter emails from TrashMob.eco. You can re-subscribe at any
                            time from your dashboard.
                        </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                        <AlertDialogCancel>Cancel</AlertDialogCancel>
                        <AlertDialogAction
                            onClick={handleUnsubscribeAll}
                            disabled={unsubscribeAllMutation.isPending}
                            className='bg-destructive text-white hover:bg-destructive/90'
                        >
                            {unsubscribeAllMutation.isPending ? 'Unsubscribing...' : 'Unsubscribe All'}
                        </AlertDialogAction>
                    </AlertDialogFooter>
                </AlertDialogContent>
            </AlertDialog>
        </>
    );
};
