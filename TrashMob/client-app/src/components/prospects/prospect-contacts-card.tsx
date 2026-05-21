import { useMemo, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Edit, Mail, MoreVertical, Phone, Plus, Star, Trash2, User } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
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
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useToast } from '@/hooks/use-toast';
import { getErrorMessage } from '@/lib/api-errors';
import {
    DeleteProspectContact,
    GetCommunityProspectById,
    GetProspectContacts,
    SetPrimaryProspectContact,
    UpdateProspectContactStatus,
} from '@/services/community-prospects';
import ProspectContactData from '@/components/Models/ProspectContactData';
import { ProspectContactFormDialog } from './prospect-contact-form-dialog';
import { PROSPECT_CONTACT_STATUSES, ProspectContactStatusBadge } from './prospect-contact-status-badge';

interface ProspectContactsCardProps {
    prospectId: string;
}

export function ProspectContactsCard({ prospectId }: ProspectContactsCardProps) {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [formOpen, setFormOpen] = useState(false);
    const [editingContact, setEditingContact] = useState<ProspectContactData | undefined>(undefined);
    const [contactToDelete, setContactToDelete] = useState<ProspectContactData | undefined>(undefined);

    const { data: contacts, isLoading } = useQuery({
        queryKey: GetProspectContacts({ prospectId }).key,
        queryFn: GetProspectContacts({ prospectId }).service,
        select: (res) => res.data,
        enabled: !!prospectId,
    });

    const invalidate = () => {
        queryClient.invalidateQueries({
            queryKey: GetProspectContacts({ prospectId }).key,
            refetchType: 'all',
        });
        queryClient.invalidateQueries({
            queryKey: GetCommunityProspectById({ id: prospectId }).key,
            refetchType: 'all',
        });
    };

    const setPrimary = useMutation({
        mutationKey: SetPrimaryProspectContact().key,
        mutationFn: SetPrimaryProspectContact().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Primary contact updated' });
            invalidate();
        },
        onError: (error: Error) => {
            toast({
                variant: 'destructive',
                title: 'Failed to set primary',
                description: getErrorMessage(error),
            });
        },
    });

    const updateStatus = useMutation({
        mutationKey: UpdateProspectContactStatus().key,
        mutationFn: UpdateProspectContactStatus().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Status updated' });
            invalidate();
        },
        onError: (error: Error) => {
            toast({
                variant: 'destructive',
                title: 'Failed to update status',
                description: getErrorMessage(error),
            });
        },
    });

    const deleteContact = useMutation({
        mutationKey: DeleteProspectContact().key,
        mutationFn: DeleteProspectContact().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Contact deleted' });
            invalidate();
            setContactToDelete(undefined);
        },
        onError: (error: Error) => {
            toast({
                variant: 'destructive',
                title: 'Cannot delete contact',
                description: getErrorMessage(error),
            });
            setContactToDelete(undefined);
        },
    });

    const sortedContacts = useMemo(() => contacts ?? [], [contacts]);

    function handleAdd() {
        setEditingContact(undefined);
        setFormOpen(true);
    }

    function handleEdit(contact: ProspectContactData) {
        setEditingContact(contact);
        setFormOpen(true);
    }

    return (
        <>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <CardTitle>Contacts ({sortedContacts.length})</CardTitle>
                    <Button variant='outline' onClick={handleAdd}>
                        <Plus className='mr-2 h-4 w-4' /> Add Contact
                    </Button>
                </CardHeader>
                <CardContent>
                    {isLoading ? (
                        <p className='text-muted-foreground text-sm'>Loading contacts...</p>
                    ) : sortedContacts.length === 0 ? (
                        <p className='text-muted-foreground text-sm'>
                            No contacts yet. Add a contact to start tracking outreach attempts.
                        </p>
                    ) : (
                        <div className='space-y-3'>
                            {sortedContacts.map((contact) => (
                                <div
                                    key={contact.id}
                                    className='border rounded-md p-3 flex flex-col gap-2 sm:flex-row sm:items-start sm:justify-between'
                                >
                                    <div className='flex-1 space-y-1'>
                                        <div className='flex flex-wrap items-center gap-2'>
                                            <User className='h-4 w-4 text-muted-foreground' />
                                            <span className='font-medium'>{contact.name}</span>
                                            {contact.title ? (
                                                <span className='text-sm text-muted-foreground'>({contact.title})</span>
                                            ) : null}
                                            {contact.isPrimary ? (
                                                <span className='inline-flex items-center gap-1 text-xs bg-amber-100 text-amber-800 px-2 py-0.5 rounded-full'>
                                                    <Star className='h-3 w-3' /> Primary
                                                </span>
                                            ) : null}
                                            <ProspectContactStatusBadge status={contact.contactStatus} />
                                        </div>
                                        <div className='flex flex-wrap items-center gap-4 text-sm text-muted-foreground'>
                                            {contact.email ? (
                                                <a
                                                    href={`mailto:${contact.email}`}
                                                    className='flex items-center gap-1 hover:underline'
                                                >
                                                    <Mail className='h-3 w-3' /> {contact.email}
                                                </a>
                                            ) : null}
                                            {contact.phone ? (
                                                <span className='flex items-center gap-1'>
                                                    <Phone className='h-3 w-3' /> {contact.phone}
                                                </span>
                                            ) : null}
                                            {contact.role ? <span>{contact.role}</span> : null}
                                        </div>
                                        {contact.notes ? (
                                            <p className='text-sm text-muted-foreground whitespace-pre-wrap'>
                                                {contact.notes}
                                            </p>
                                        ) : null}
                                    </div>
                                    <div className='flex items-center gap-2'>
                                        <Select
                                            value={String(contact.contactStatus)}
                                            onValueChange={(value) =>
                                                updateStatus.mutate({
                                                    prospectId,
                                                    contactId: contact.id,
                                                    status: parseInt(value, 10),
                                                })
                                            }
                                        >
                                            <SelectTrigger className='h-8 w-[140px] text-xs'>
                                                <SelectValue />
                                            </SelectTrigger>
                                            <SelectContent>
                                                {PROSPECT_CONTACT_STATUSES.map((s) => (
                                                    <SelectItem key={s.value} value={String(s.value)}>
                                                        {s.label}
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                        <DropdownMenu>
                                            <DropdownMenuTrigger asChild>
                                                <Button variant='ghost' size='icon'>
                                                    <MoreVertical className='h-4 w-4' />
                                                </Button>
                                            </DropdownMenuTrigger>
                                            <DropdownMenuContent align='end'>
                                                {!contact.isPrimary ? (
                                                    <DropdownMenuItem
                                                        onClick={() =>
                                                            setPrimary.mutate({ prospectId, contactId: contact.id })
                                                        }
                                                    >
                                                        <Star className='mr-2 h-4 w-4' /> Set as primary
                                                    </DropdownMenuItem>
                                                ) : null}
                                                <DropdownMenuItem onClick={() => handleEdit(contact)}>
                                                    <Edit className='mr-2 h-4 w-4' /> Edit
                                                </DropdownMenuItem>
                                                <DropdownMenuItem
                                                    className='text-destructive focus:text-destructive'
                                                    onClick={() => setContactToDelete(contact)}
                                                >
                                                    <Trash2 className='mr-2 h-4 w-4' /> Delete
                                                </DropdownMenuItem>
                                            </DropdownMenuContent>
                                        </DropdownMenu>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </CardContent>
            </Card>

            <ProspectContactFormDialog
                prospectId={prospectId}
                open={formOpen}
                onOpenChange={setFormOpen}
                contact={editingContact}
                referralCandidates={sortedContacts}
            />

            <AlertDialog
                open={!!contactToDelete}
                onOpenChange={(open) => {
                    if (!open) setContactToDelete(undefined);
                }}
            >
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>Delete this contact?</AlertDialogTitle>
                        <AlertDialogDescription>
                            {contactToDelete?.name
                                ? `${contactToDelete.name} will be permanently removed.`
                                : 'This contact will be permanently removed.'}{' '}
                            If the contact has any activity or outreach history, deletion is blocked — mark them as
                            "Left Org" or "Wrong Person" instead.
                        </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                        <AlertDialogCancel disabled={deleteContact.isPending}>Cancel</AlertDialogCancel>
                        <AlertDialogAction
                            disabled={deleteContact.isPending}
                            onClick={() => {
                                if (contactToDelete) {
                                    deleteContact.mutate({
                                        prospectId,
                                        contactId: contactToDelete.id,
                                    });
                                }
                            }}
                        >
                            Delete
                        </AlertDialogAction>
                    </AlertDialogFooter>
                </AlertDialogContent>
            </AlertDialog>
        </>
    );
}
