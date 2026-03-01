import { useState } from 'react';
import { Link, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Edit, FileText, Heart, Loader2, Megaphone, Plus, Trash2 } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { useToast } from '@/hooks/use-toast';
import {
    ContactTypeBadge,
    DonorLifecycleBadge,
    EngagementScoreBadge,
    NoteTypeBadge,
    PledgeStatusBadge,
    getDonationTypeLabel,
    getRecurringFrequencyLabel,
} from '@/components/contacts/contact-constants';
import ContactNoteData from '@/components/Models/ContactNoteData';
import DonationData from '@/components/Models/DonationData';
import {
    DeleteContactNote,
    DeleteDonation,
    DeletePledge,
    GetContactById,
    GetContactNotes,
    GetContactTagIds,
    GetContactTags,
    GetDonationsByContact,
    GetPledgesByContact,
    SendDonationReceipt,
    SendDonationThankYou,
    UpdateContactTags,
    GetContactEngagementScore,
} from '@/services/contacts';
import { ThankYouDialog } from '@/components/contacts/thank-you-dialog';
import { ReceiptDialog } from '@/components/contacts/receipt-dialog';
import { AppealDialog } from '@/components/contacts/appeal-dialog';
import { NoteDialog } from './note-dialog';

export const SiteAdminContactDetail = () => {
    const { contactId } = useParams<{ contactId: string }>() as { contactId: string };
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [noteDialogOpen, setNoteDialogOpen] = useState(false);
    const [editingNote, setEditingNote] = useState<ContactNoteData | null>(null);
    const [thankYouDonation, setThankYouDonation] = useState<DonationData | null>(null);
    const [receiptDonation, setReceiptDonation] = useState<DonationData | null>(null);
    const [appealDialogOpen, setAppealDialogOpen] = useState(false);

    const { data: contact } = useQuery({
        queryKey: GetContactById({ id: contactId }).key,
        queryFn: GetContactById({ id: contactId }).service,
        select: (res) => res.data,
        enabled: !!contactId,
    });

    const { data: notes } = useQuery({
        queryKey: GetContactNotes({ contactId }).key,
        queryFn: GetContactNotes({ contactId }).service,
        select: (res) => res.data,
        enabled: !!contactId,
    });

    const { data: allTags } = useQuery({
        queryKey: GetContactTags().key,
        queryFn: GetContactTags().service,
        select: (res) => res.data,
    });

    const { data: assignedTagIds } = useQuery({
        queryKey: GetContactTagIds({ contactId }).key,
        queryFn: GetContactTagIds({ contactId }).service,
        select: (res) => res.data,
        enabled: !!contactId,
    });

    const { data: donations } = useQuery({
        queryKey: GetDonationsByContact({ contactId }).key,
        queryFn: GetDonationsByContact({ contactId }).service,
        select: (res) => res.data,
        enabled: !!contactId,
    });

    const { data: pledges } = useQuery({
        queryKey: GetPledgesByContact({ contactId }).key,
        queryFn: GetPledgesByContact({ contactId }).service,
        select: (res) => res.data,
        enabled: !!contactId,
    });

    const { data: engagementScore } = useQuery({
        queryKey: GetContactEngagementScore({ contactId }).key,
        queryFn: GetContactEngagementScore({ contactId }).service,
        select: (res) => res.data,
        enabled: !!contactId,
    });

    const updateTags = useMutation({
        mutationKey: UpdateContactTags({ contactId }).key,
        mutationFn: UpdateContactTags({ contactId }).service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/contacts', contactId, 'tags'], refetchType: 'all' });
            toast({ variant: 'primary', title: 'Tags updated' });
        },
    });

    const deleteNote = useMutation({
        mutationKey: DeleteContactNote().key,
        mutationFn: DeleteContactNote().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/contactnotes', contactId], refetchType: 'all' });
            toast({ variant: 'default', title: 'Note deleted' });
        },
    });

    const deleteDonation = useMutation({
        mutationKey: DeleteDonation().key,
        mutationFn: DeleteDonation().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: ['/donations', 'bycontact', contactId],
                refetchType: 'all',
            });
            toast({ variant: 'default', title: 'Donation deleted' });
        },
    });

    const deletePledge = useMutation({
        mutationKey: DeletePledge().key,
        mutationFn: DeletePledge().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: ['/pledges', 'bycontact', contactId],
                refetchType: 'all',
            });
            toast({ variant: 'default', title: 'Pledge deleted' });
        },
    });

    const sendThankYou = useMutation({
        mutationKey: SendDonationThankYou().key,
        mutationFn: SendDonationThankYou().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: ['/donations', 'bycontact', contactId],
                refetchType: 'all',
            });
            queryClient.invalidateQueries({ queryKey: ['/contactnotes', contactId], refetchType: 'all' });
            toast({ variant: 'primary', title: 'Thank you email sent' });
            setThankYouDonation(null);
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to send thank you email' });
        },
    });

    const sendReceipt = useMutation({
        mutationKey: SendDonationReceipt().key,
        mutationFn: SendDonationReceipt().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: ['/donations', 'bycontact', contactId],
                refetchType: 'all',
            });
            queryClient.invalidateQueries({ queryKey: ['/contactnotes', contactId], refetchType: 'all' });
            toast({ variant: 'primary', title: 'Tax receipt email sent' });
            setReceiptDonation(null);
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to send receipt email' });
        },
    });

    const toggleTag = (tagId: string) => {
        const current = assignedTagIds || [];
        const updated = current.includes(tagId) ? current.filter((id) => id !== tagId) : [...current, tagId];
        updateTags.mutate(updated);
    };

    const handleDeleteNote = (noteId: string) => {
        if (!window.confirm('Delete this note?')) return;
        deleteNote.mutate({ id: noteId });
    };

    const handleDeleteDonation = (donationId: string) => {
        if (!window.confirm('Delete this donation?')) return;
        deleteDonation.mutate({ id: donationId });
    };

    const handleDeletePledge = (pledgeId: string) => {
        if (!window.confirm('Delete this pledge?')) return;
        deletePledge.mutate({ id: pledgeId });
    };

    const openEditNote = (note: ContactNoteData) => {
        setEditingNote(note);
        setNoteDialogOpen(true);
    };

    const openCreateNote = () => {
        setEditingNote(null);
        setNoteDialogOpen(true);
    };

    if (!contact) {
        return (
            <div className='flex justify-center items-center py-16'>
                <Loader2 className='animate-spin mr-2' /> Loading...
            </div>
        );
    }

    const name = [contact.firstName, contact.lastName].filter(Boolean).join(' ');
    const location = [contact.city, contact.region, contact.country].filter(Boolean).join(', ');
    const totalDonations = donations?.reduce((sum, d) => sum + d.amount, 0) ?? 0;

    return (
        <div className='space-y-6'>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <div>
                        <CardTitle>{name || 'Unnamed Contact'}</CardTitle>
                        <div className='flex items-center gap-2 mt-1'>
                            <ContactTypeBadge type={contact.contactType} />
                            {contact.isActive ? (
                                <Badge variant='success'>Active</Badge>
                            ) : (
                                <Badge variant='secondary'>Inactive</Badge>
                            )}
                        </div>
                    </div>
                    <div className='flex gap-2'>
                        {contact.email ? (
                            <Button variant='outline' onClick={() => setAppealDialogOpen(true)}>
                                <Megaphone /> Send Appeal
                            </Button>
                        ) : null}
                        <Button asChild>
                            <Link to={`/siteadmin/contacts/${contactId}/edit`}>
                                <Edit /> Edit
                            </Link>
                        </Button>
                    </div>
                </CardHeader>
            </Card>

            <Tabs defaultValue='overview'>
                <TabsList>
                    <TabsTrigger value='overview'>Overview</TabsTrigger>
                    <TabsTrigger value='notes'>Notes ({(notes || []).length})</TabsTrigger>
                    <TabsTrigger value='donations'>Donations ({(donations || []).length})</TabsTrigger>
                    <TabsTrigger value='pledges'>Pledges ({(pledges || []).length})</TabsTrigger>
                </TabsList>

                <TabsContent value='overview' className='space-y-6'>
                    <Card>
                        <CardHeader>
                            <CardTitle className='text-lg'>Contact Information</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className='grid grid-cols-12 gap-4'>
                                <div className='col-span-12 md:col-span-6 space-y-3'>
                                    <InfoField label='Email' value={contact.email} />
                                    <InfoField label='Phone' value={contact.phone} />
                                    <InfoField label='Organization' value={contact.organizationName} />
                                    <InfoField label='Title' value={contact.title} />
                                </div>
                                <div className='col-span-12 md:col-span-6 space-y-3'>
                                    <InfoField label='Address' value={contact.address} />
                                    <InfoField label='Location' value={location} />
                                    <InfoField label='Postal Code' value={contact.postalCode} />
                                    <InfoField label='Source' value={contact.source} />
                                </div>
                                {contact.notes ? (
                                    <div className='col-span-12'>
                                        <InfoField label='Notes' value={contact.notes} />
                                    </div>
                                ) : null}
                            </div>
                        </CardContent>
                    </Card>

                    <Card>
                        <CardHeader>
                            <CardTitle className='text-lg'>Tags</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className='flex flex-wrap gap-2'>
                                {(allTags || []).map((tag) => {
                                    const isAssigned = (assignedTagIds || []).includes(tag.id);
                                    return (
                                        <button
                                            key={tag.id}
                                            type='button'
                                            onClick={() => toggleTag(tag.id)}
                                            className={`inline-flex items-center gap-1.5 rounded-full border px-3 py-1 text-sm transition-colors ${
                                                isAssigned
                                                    ? 'bg-primary text-primary-foreground border-primary'
                                                    : 'bg-background hover:bg-accent border-input'
                                            }`}
                                        >
                                            {tag.color ? (
                                                <span
                                                    className='inline-block h-2.5 w-2.5 rounded-full'
                                                    style={{ backgroundColor: tag.color }}
                                                />
                                            ) : null}
                                            {tag.name}
                                        </button>
                                    );
                                })}
                                {(allTags || []).length === 0 ? (
                                    <p className='text-sm text-muted-foreground'>
                                        No tags defined yet.{' '}
                                        <Link to='/siteadmin/contact-tags' className='underline'>
                                            Create tags
                                        </Link>
                                    </p>
                                ) : null}
                            </div>
                        </CardContent>
                    </Card>

                    {engagementScore ? (
                        <Card>
                            <CardHeader>
                                <CardTitle className='text-lg'>Engagement Score</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <div className='flex items-center gap-4 mb-4'>
                                    <div className='text-3xl font-bold'>{engagementScore.engagementScore}</div>
                                    <EngagementScoreBadge score={engagementScore.engagementScore} />
                                    <DonorLifecycleBadge stage={engagementScore.donorLifecycleStage} />
                                    {engagementScore.isLybunt ? <Badge variant='destructive'>LYBUNT</Badge> : null}
                                </div>
                                <div className='grid grid-cols-3 gap-4 text-sm'>
                                    <div>
                                        <p className='text-muted-foreground'>Donation</p>
                                        <p className='font-semibold'>
                                            {engagementScore.donationScoreComponent}/
                                            {engagementScore.hasUserId ? '40' : '57'}
                                        </p>
                                    </div>
                                    <div>
                                        <p className='text-muted-foreground'>Volunteer</p>
                                        <p className='font-semibold'>
                                            {engagementScore.volunteerScoreComponent}/
                                            {engagementScore.hasUserId ? '40' : '0'}
                                        </p>
                                    </div>
                                    <div>
                                        <p className='text-muted-foreground'>Interaction</p>
                                        <p className='font-semibold'>
                                            {engagementScore.interactionScoreComponent}/
                                            {engagementScore.hasUserId ? '20' : '43'}
                                        </p>
                                    </div>
                                </div>
                            </CardContent>
                        </Card>
                    ) : null}

                    {totalDonations > 0 ? (
                        <Card>
                            <CardHeader>
                                <CardTitle className='text-lg'>Donation Summary</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <div className='grid grid-cols-2 gap-4'>
                                    <div>
                                        <p className='text-sm text-muted-foreground'>Total Donations</p>
                                        <p className='text-2xl font-semibold'>${totalDonations.toLocaleString()}</p>
                                    </div>
                                    <div>
                                        <p className='text-sm text-muted-foreground'>Number of Donations</p>
                                        <p className='text-2xl font-semibold'>{(donations || []).length}</p>
                                    </div>
                                </div>
                            </CardContent>
                        </Card>
                    ) : null}
                </TabsContent>

                <TabsContent value='notes' className='space-y-4'>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between'>
                            <CardTitle className='text-lg'>Notes</CardTitle>
                            <Button onClick={openCreateNote} size='sm'>
                                <Plus /> Add Note
                            </Button>
                        </CardHeader>
                        <CardContent>
                            {(notes || []).length === 0 ? (
                                <p className='text-sm text-muted-foreground'>No notes yet.</p>
                            ) : (
                                <div className='space-y-4'>
                                    {(notes || []).map((note) => (
                                        <div key={note.id} className='rounded-lg border p-4'>
                                            <div className='flex items-center justify-between mb-2'>
                                                <div className='flex items-center gap-2'>
                                                    <NoteTypeBadge type={note.noteType} />
                                                    {note.subject ? (
                                                        <span className='font-medium'>{note.subject}</span>
                                                    ) : null}
                                                </div>
                                                <div className='flex items-center gap-1'>
                                                    <Button
                                                        variant='ghost'
                                                        size='icon'
                                                        onClick={() => openEditNote(note)}
                                                    >
                                                        <Edit className='h-4 w-4' />
                                                    </Button>
                                                    <Button
                                                        variant='ghost'
                                                        size='icon'
                                                        onClick={() => handleDeleteNote(note.id)}
                                                    >
                                                        <Trash2 className='h-4 w-4 text-destructive' />
                                                    </Button>
                                                </div>
                                            </div>
                                            <p className='text-sm whitespace-pre-wrap'>{note.body}</p>
                                            <p className='text-xs text-muted-foreground mt-2'>
                                                {note.createdDate
                                                    ? new Date(note.createdDate).toLocaleDateString()
                                                    : ''}
                                            </p>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </CardContent>
                    </Card>
                </TabsContent>

                <TabsContent value='donations'>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between'>
                            <CardTitle className='text-lg'>Donations</CardTitle>
                            <Button asChild size='sm'>
                                <Link to={`/siteadmin/donations/create?contactId=${contactId}`}>
                                    <Plus /> Add Donation
                                </Link>
                            </Button>
                        </CardHeader>
                        <CardContent>
                            {(donations || []).length === 0 ? (
                                <p className='text-sm text-muted-foreground'>No donations recorded yet.</p>
                            ) : (
                                <Table>
                                    <TableHeader>
                                        <TableRow>
                                            <TableHead>Date</TableHead>
                                            <TableHead>Type</TableHead>
                                            <TableHead>Campaign</TableHead>
                                            <TableHead className='text-right'>Amount</TableHead>
                                            <TableHead>Receipt</TableHead>
                                            <TableHead>Thank You</TableHead>
                                            <TableHead className='text-right'>Actions</TableHead>
                                        </TableRow>
                                    </TableHeader>
                                    <TableBody>
                                        {(donations || []).map((d) => (
                                            <TableRow key={d.id}>
                                                <TableCell>
                                                    {d.donationDate
                                                        ? new Date(d.donationDate).toLocaleDateString()
                                                        : '—'}
                                                </TableCell>
                                                <TableCell>{getDonationTypeLabel(d.donationType)}</TableCell>
                                                <TableCell>{d.campaign || '—'}</TableCell>
                                                <TableCell className='text-right'>
                                                    ${d.amount.toLocaleString()}
                                                </TableCell>
                                                <TableCell>
                                                    {d.receiptSent ? (
                                                        <Badge variant='success'>Sent</Badge>
                                                    ) : (
                                                        <Badge variant='secondary'>No</Badge>
                                                    )}
                                                </TableCell>
                                                <TableCell>
                                                    {d.thankYouSent ? (
                                                        <Badge variant='success'>Sent</Badge>
                                                    ) : (
                                                        <Badge variant='secondary'>No</Badge>
                                                    )}
                                                </TableCell>
                                                <TableCell className='text-right'>
                                                    <TooltipProvider>
                                                        <div className='flex justify-end gap-1'>
                                                            {contact.email ? (
                                                                <>
                                                                    <Tooltip>
                                                                        <TooltipTrigger asChild>
                                                                            <Button
                                                                                variant='ghost'
                                                                                size='icon'
                                                                                disabled={d.thankYouSent}
                                                                                onClick={() => setThankYouDonation(d)}
                                                                            >
                                                                                <Heart
                                                                                    className={`h-4 w-4 ${d.thankYouSent ? 'text-green-500' : ''}`}
                                                                                />
                                                                            </Button>
                                                                        </TooltipTrigger>
                                                                        <TooltipContent>
                                                                            {d.thankYouSent
                                                                                ? 'Thank you already sent'
                                                                                : 'Send thank you'}
                                                                        </TooltipContent>
                                                                    </Tooltip>
                                                                    <Tooltip>
                                                                        <TooltipTrigger asChild>
                                                                            <Button
                                                                                variant='ghost'
                                                                                size='icon'
                                                                                disabled={d.receiptSent}
                                                                                onClick={() => setReceiptDonation(d)}
                                                                            >
                                                                                <FileText
                                                                                    className={`h-4 w-4 ${d.receiptSent ? 'text-green-500' : ''}`}
                                                                                />
                                                                            </Button>
                                                                        </TooltipTrigger>
                                                                        <TooltipContent>
                                                                            {d.receiptSent
                                                                                ? 'Receipt already sent'
                                                                                : 'Send tax receipt'}
                                                                        </TooltipContent>
                                                                    </Tooltip>
                                                                </>
                                                            ) : null}
                                                            <Button variant='ghost' size='icon' asChild>
                                                                <Link to={`/siteadmin/donations/${d.id}/edit`}>
                                                                    <Edit className='h-4 w-4' />
                                                                </Link>
                                                            </Button>
                                                            <Button
                                                                variant='ghost'
                                                                size='icon'
                                                                onClick={() => handleDeleteDonation(d.id)}
                                                            >
                                                                <Trash2 className='h-4 w-4 text-destructive' />
                                                            </Button>
                                                        </div>
                                                    </TooltipProvider>
                                                </TableCell>
                                            </TableRow>
                                        ))}
                                    </TableBody>
                                </Table>
                            )}
                        </CardContent>
                    </Card>
                </TabsContent>

                <TabsContent value='pledges'>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between'>
                            <CardTitle className='text-lg'>Pledges</CardTitle>
                            <Button asChild size='sm'>
                                <Link to={`/siteadmin/pledges/create?contactId=${contactId}`}>
                                    <Plus /> Add Pledge
                                </Link>
                            </Button>
                        </CardHeader>
                        <CardContent>
                            {(pledges || []).length === 0 ? (
                                <p className='text-sm text-muted-foreground'>No pledges recorded yet.</p>
                            ) : (
                                <Table>
                                    <TableHeader>
                                        <TableRow>
                                            <TableHead>Total Amount</TableHead>
                                            <TableHead>Start Date</TableHead>
                                            <TableHead>End Date</TableHead>
                                            <TableHead>Frequency</TableHead>
                                            <TableHead>Status</TableHead>
                                            <TableHead className='text-right'>Actions</TableHead>
                                        </TableRow>
                                    </TableHeader>
                                    <TableBody>
                                        {(pledges || []).map((p) => (
                                            <TableRow key={p.id}>
                                                <TableCell className='font-medium'>
                                                    ${p.totalAmount.toLocaleString()}
                                                </TableCell>
                                                <TableCell>
                                                    {p.startDate ? new Date(p.startDate).toLocaleDateString() : '—'}
                                                </TableCell>
                                                <TableCell>
                                                    {p.endDate ? new Date(p.endDate).toLocaleDateString() : '—'}
                                                </TableCell>
                                                <TableCell>{getRecurringFrequencyLabel(p.frequency)}</TableCell>
                                                <TableCell>
                                                    <PledgeStatusBadge status={p.status} />
                                                </TableCell>
                                                <TableCell className='text-right'>
                                                    <div className='flex justify-end gap-1'>
                                                        <Button variant='ghost' size='icon' asChild>
                                                            <Link to={`/siteadmin/pledges/${p.id}/edit`}>
                                                                <Edit className='h-4 w-4' />
                                                            </Link>
                                                        </Button>
                                                        <Button
                                                            variant='ghost'
                                                            size='icon'
                                                            onClick={() => handleDeletePledge(p.id)}
                                                        >
                                                            <Trash2 className='h-4 w-4 text-destructive' />
                                                        </Button>
                                                    </div>
                                                </TableCell>
                                            </TableRow>
                                        ))}
                                    </TableBody>
                                </Table>
                            )}
                        </CardContent>
                    </Card>
                </TabsContent>
            </Tabs>

            <NoteDialog
                open={noteDialogOpen}
                onOpenChange={setNoteDialogOpen}
                contactId={contactId}
                editingNote={editingNote}
            />

            <ThankYouDialog
                open={!!thankYouDonation}
                onOpenChange={(open) => !open && setThankYouDonation(null)}
                donation={thankYouDonation}
                contactName={name}
                contactEmail={contact.email || ''}
                isPending={sendThankYou.isPending}
                onConfirm={() => thankYouDonation && sendThankYou.mutate({ donationId: thankYouDonation.id })}
            />

            <ReceiptDialog
                open={!!receiptDonation}
                onOpenChange={(open) => !open && setReceiptDonation(null)}
                donation={receiptDonation}
                contactName={name}
                contactEmail={contact.email || ''}
                isPending={sendReceipt.isPending}
                onConfirm={() => receiptDonation && sendReceipt.mutate({ donationId: receiptDonation.id })}
            />

            <AppealDialog
                open={appealDialogOpen}
                onOpenChange={setAppealDialogOpen}
                contactId={contactId}
                contactName={name}
                contactEmail={contact.email || ''}
            />
        </div>
    );
};

const InfoField = ({ label, value }: { label: string; value: string | null | undefined }) => (
    <div>
        <h4 className='text-sm font-medium text-muted-foreground'>{label}</h4>
        <p>{value || '—'}</p>
    </div>
);
