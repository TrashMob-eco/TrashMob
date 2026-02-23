import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { getMsalClientInstance } from '@/store/AuthStore';
import { useLogin } from '@/hooks/useLogin';
import { useToast } from '@/hooks/use-toast';
import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
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
    AlertDialogTrigger,
} from '@/components/ui/alert-dialog';
import { DeleteUserById, ExportUserData } from '@/services/users';
import { ChevronDown, Download, Loader2, TriangleAlert } from 'lucide-react';
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from '@/components/ui/collapsible';

export const DeleteMyData = () => {
    const { currentUser, isUserLoaded } = useLogin();
    const { toast } = useToast();
    const [confirmText, setConfirmText] = useState('');
    const [dialogOpen, setDialogOpen] = useState(false);

    const [isExporting, setIsExporting] = useState(false);
    const [detailsOpen, setDetailsOpen] = useState(false);

    const handleExport = async () => {
        if (!currentUser?.id) return;
        setIsExporting(true);
        try {
            const response = await ExportUserData({ userId: currentUser.id }).service();
            const url = URL.createObjectURL(response.data);
            const a = document.createElement('a');
            a.href = url;
            a.download = `trashmob-data-export.json`;
            document.body.appendChild(a);
            a.click();
            URL.revokeObjectURL(url);
            document.body.removeChild(a);
            toast({ variant: 'primary', title: 'Your data export has been downloaded.' });
        } catch {
            toast({ variant: 'destructive', title: 'Failed to export your data. Please try again.' });
        } finally {
            setIsExporting(false);
        }
    };

    const { mutate: deleteAccount, isPending } = useMutation({
        mutationKey: DeleteUserById().key,
        mutationFn: DeleteUserById().service,
        onSuccess: () => {
            const logoutRequest = {
                account: getMsalClientInstance().getActiveAccount(),
            };
            getMsalClientInstance().logoutRedirect(logoutRequest);
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to delete your account. Please try again or contact support.',
            });
        },
    });

    const handleConfirmDelete = () => {
        if (!currentUser?.id) return;
        deleteAccount({ id: currentUser.id });
    };

    const isConfirmed = confirmText === 'DELETE';

    return (
        <div>
            <HeroSection
                Title='Delete your account?'
                Description='TrashMob members are making the world a better place!'
            />
            <div className='container mx-auto py-5'>
                <Card>
                    <CardHeader>
                        <CardTitle>Delete my data</CardTitle>
                        <CardDescription>
                            <p>
                                If you no longer wish to be a member of the TrashMob.eco community, clicking the delete
                                button below will delete your account and anonymize any event-related data for events
                                you may have participated in. Warning: Deleting an account cannot be undone.
                            </p>
                            <p>We are sorry to see you go!</p>
                            <p>The Team at TrashMob.eco</p>
                        </CardDescription>
                    </CardHeader>
                    <CardContent className='space-y-6'>
                        {isUserLoaded ? (
                            <>
                                <div className='rounded-lg border p-4'>
                                    <h3 className='font-medium mb-2'>Download your data first</h3>
                                    <p className='text-sm text-muted-foreground mb-3'>
                                        Before deleting your account, you may want to download a copy of all your
                                        personal data. This includes your profile, event history, routes, and more.
                                    </p>
                                    <Button variant='outline' onClick={handleExport} disabled={isExporting}>
                                        {isExporting ? (
                                            <>
                                                <Loader2 className='mr-2 h-4 w-4 animate-spin' />
                                                Exporting...
                                            </>
                                        ) : (
                                            <>
                                                <Download className='mr-2 h-4 w-4' />
                                                Download My Data
                                            </>
                                        )}
                                    </Button>
                                </div>
                                <Collapsible open={detailsOpen} onOpenChange={setDetailsOpen}>
                                    <CollapsibleTrigger asChild>
                                        <Button variant='ghost' className='flex items-center gap-2 px-0'>
                                            <ChevronDown
                                                className={`h-4 w-4 transition-transform ${detailsOpen ? 'rotate-180' : ''}`}
                                            />
                                            What happens to my data?
                                        </Button>
                                    </CollapsibleTrigger>
                                    <CollapsibleContent>
                                        <div className='rounded-lg border p-4 mt-2 space-y-3 text-sm'>
                                            <div>
                                                <h4 className='font-medium mb-1'>Permanently deleted</h4>
                                                <p className='text-muted-foreground'>
                                                    Your account profile (name, email, photo, location), event
                                                    registrations, notification history and preferences, team
                                                    memberships, achievement records, and partner admin roles.
                                                </p>
                                            </div>
                                            <div>
                                                <h4 className='font-medium mb-1'>
                                                    Anonymized (your name removed, data preserved)
                                                </h4>
                                                <p className='text-muted-foreground'>
                                                    Cleanup routes (GPS paths preserved for community heatmaps), cleanup
                                                    metrics (bags, weight, duration preserved for aggregate totals),
                                                    event summaries you submitted, events you created (remain visible,
                                                    your name removed), and feedback you submitted.
                                                </p>
                                            </div>
                                            <div>
                                                <h4 className='font-medium mb-1'>Retained for legal compliance</h4>
                                                <p className='text-muted-foreground'>
                                                    Signed waiver records are kept for up to 7 years from the event date
                                                    per liability requirements. Your name is anonymized immediately, but
                                                    the waiver record is preserved.
                                                </p>
                                            </div>
                                            <div>
                                                <p className='text-muted-foreground'>
                                                    Photos you uploaded to events are preserved for community
                                                    documentation, but your name is removed from the upload record.
                                                </p>
                                            </div>
                                        </div>
                                    </CollapsibleContent>
                                </Collapsible>

                                <AlertDialog
                                    open={dialogOpen}
                                    onOpenChange={(open) => {
                                        setDialogOpen(open);
                                        if (!open) setConfirmText('');
                                    }}
                                >
                                    <AlertDialogTrigger asChild>
                                        <Button variant='destructive'>Delete Account</Button>
                                    </AlertDialogTrigger>
                                    <AlertDialogContent>
                                        <AlertDialogHeader>
                                            <AlertDialogTitle className='flex items-center gap-2'>
                                                <TriangleAlert className='h-5 w-5 text-destructive' />
                                                Are you absolutely sure?
                                            </AlertDialogTitle>
                                            <AlertDialogDescription>
                                                This action cannot be undone. Your account will be permanently deleted
                                                and all event-related data will be anonymized.
                                            </AlertDialogDescription>
                                        </AlertDialogHeader>
                                        <div className='py-2'>
                                            <Label htmlFor='confirm-delete'>
                                                Type <span className='font-mono font-bold'>DELETE</span> to confirm
                                            </Label>
                                            <Input
                                                id='confirm-delete'
                                                value={confirmText}
                                                onChange={(e) => setConfirmText(e.target.value)}
                                                placeholder='DELETE'
                                                className='mt-2'
                                                autoComplete='off'
                                            />
                                        </div>
                                        <AlertDialogFooter>
                                            <AlertDialogCancel disabled={isPending}>Cancel</AlertDialogCancel>
                                            <AlertDialogAction
                                                disabled={!isConfirmed || isPending}
                                                onClick={(e) => {
                                                    e.preventDefault();
                                                    handleConfirmDelete();
                                                }}
                                                className='bg-destructive text-destructive-foreground hover:bg-destructive/90'
                                            >
                                                {isPending ? (
                                                    <>
                                                        <Loader2 className='mr-2 h-4 w-4 animate-spin' />
                                                        Deleting...
                                                    </>
                                                ) : (
                                                    'Delete my account'
                                                )}
                                            </AlertDialogAction>
                                        </AlertDialogFooter>
                                    </AlertDialogContent>
                                </AlertDialog>
                            </>
                        ) : (
                            <p className='text-muted-foreground'>Please sign in to manage your account.</p>
                        )}
                    </CardContent>
                </Card>
            </div>
        </div>
    );
};
