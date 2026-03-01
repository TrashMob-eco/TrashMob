import { useState } from 'react';
import { useMutation, useQuery } from '@tanstack/react-query';
import { Loader2, Send } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/hooks/use-toast';
import ContactData from '@/components/Models/ContactData';
import { BulkAppealResult, GetContacts, GetContactTags, SendBulkAppeal } from '@/services/contacts';

export const SiteAdminAppeals = () => {
    const { toast } = useToast();
    const [selectedTagIds, setSelectedTagIds] = useState<string[]>([]);
    const [selectedContactIds, setSelectedContactIds] = useState<Set<string>>(new Set());
    const [subject, setSubject] = useState('');
    const [body, setBody] = useState('');

    const { data: allTags } = useQuery({
        queryKey: GetContactTags().key,
        queryFn: GetContactTags().service,
        select: (res) => res.data,
    });

    const activeTagId = selectedTagIds.length === 1 ? selectedTagIds[0] : undefined;
    const { data: contacts, isLoading: contactsLoading } = useQuery({
        queryKey: GetContacts({ tagId: activeTagId }).key,
        queryFn: GetContacts({ tagId: activeTagId }).service,
        select: (res) => res.data,
    });

    // Filter to contacts with email addresses
    const emailableContacts = (contacts || []).filter((c) => c.email);

    const toggleTag = (tagId: string) => {
        setSelectedTagIds((prev) => (prev.includes(tagId) ? prev.filter((id) => id !== tagId) : [tagId]));
        setSelectedContactIds(new Set());
    };

    const toggleContact = (contactId: string) => {
        setSelectedContactIds((prev) => {
            const next = new Set(prev);
            if (next.has(contactId)) {
                next.delete(contactId);
            } else {
                next.add(contactId);
            }
            return next;
        });
    };

    const toggleAll = () => {
        if (selectedContactIds.size === emailableContacts.length) {
            setSelectedContactIds(new Set());
        } else {
            setSelectedContactIds(new Set(emailableContacts.map((c) => c.id)));
        }
    };

    const sendBulk = useMutation({
        mutationKey: SendBulkAppeal().key,
        mutationFn: SendBulkAppeal().service,
        onSuccess: (res) => {
            const result = res.data as BulkAppealResult;
            toast({
                variant: 'primary',
                title: 'Bulk appeal complete',
                description: `Sent: ${result.sentCount}, Skipped: ${result.skippedCount}, Failed: ${result.failedCount}`,
            });
            setSelectedContactIds(new Set());
            setSubject('');
            setBody('');
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to send bulk appeal' });
        },
    });

    const handleSend = () => {
        if (selectedContactIds.size === 0) {
            toast({ variant: 'destructive', title: 'No contacts selected' });
            return;
        }
        if (!subject.trim()) {
            toast({ variant: 'destructive', title: 'Subject is required' });
            return;
        }
        if (!body.trim()) {
            toast({ variant: 'destructive', title: 'Message body is required' });
            return;
        }
        sendBulk.mutate({
            contactIds: Array.from(selectedContactIds),
            subject: subject.trim(),
            body: body.trim(),
        });
    };

    const contactName = (c: ContactData) => [c.firstName, c.lastName].filter(Boolean).join(' ') || 'Unnamed';

    return (
        <div className='space-y-6'>
            <Card>
                <CardHeader>
                    <CardTitle>Bulk Fundraising Appeal</CardTitle>
                </CardHeader>
                <CardContent>
                    <p className='text-sm text-muted-foreground'>
                        Select contacts by tag, compose your appeal, and send to all selected contacts. Each email
                        will be personalized with the contact&apos;s name and logged as a contact note.
                    </p>
                </CardContent>
            </Card>

            <div className='grid grid-cols-12 gap-6'>
                {/* Tag filter */}
                <div className='col-span-12 lg:col-span-3'>
                    <Card>
                        <CardHeader>
                            <CardTitle className='text-lg'>Filter by Tag</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className='space-y-2'>
                                {(allTags || []).map((tag) => (
                                    <button
                                        key={tag.id}
                                        type='button'
                                        onClick={() => toggleTag(tag.id)}
                                        className={`flex w-full items-center gap-2 rounded-md border px-3 py-2 text-sm transition-colors ${
                                            selectedTagIds.includes(tag.id)
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
                                ))}
                                {(allTags || []).length === 0 ? (
                                    <p className='text-sm text-muted-foreground'>No tags defined.</p>
                                ) : null}
                            </div>
                        </CardContent>
                    </Card>
                </div>

                {/* Contact list + compose */}
                <div className='col-span-12 lg:col-span-9 space-y-6'>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between'>
                            <CardTitle className='text-lg'>
                                Contacts {emailableContacts.length > 0 ? `(${emailableContacts.length})` : ''}
                            </CardTitle>
                            {emailableContacts.length > 0 ? (
                                <Badge variant='outline'>
                                    {selectedContactIds.size} selected
                                </Badge>
                            ) : null}
                        </CardHeader>
                        <CardContent>
                            {contactsLoading ? (
                                <div className='flex justify-center py-8'>
                                    <Loader2 className='animate-spin' />
                                </div>
                            ) : emailableContacts.length === 0 ? (
                                <p className='text-sm text-muted-foreground'>
                                    {selectedTagIds.length > 0
                                        ? 'No contacts with email addresses found for this tag.'
                                        : 'Select a tag to filter contacts, or all contacts will be shown.'}
                                </p>
                            ) : (
                                <Table>
                                    <TableHeader>
                                        <TableRow>
                                            <TableHead className='w-12'>
                                                <Checkbox
                                                    checked={
                                                        emailableContacts.length > 0 &&
                                                        selectedContactIds.size === emailableContacts.length
                                                    }
                                                    onCheckedChange={toggleAll}
                                                />
                                            </TableHead>
                                            <TableHead>Name</TableHead>
                                            <TableHead>Email</TableHead>
                                            <TableHead>Organization</TableHead>
                                        </TableRow>
                                    </TableHeader>
                                    <TableBody>
                                        {emailableContacts.map((c) => (
                                            <TableRow key={c.id}>
                                                <TableCell>
                                                    <Checkbox
                                                        checked={selectedContactIds.has(c.id)}
                                                        onCheckedChange={() => toggleContact(c.id)}
                                                    />
                                                </TableCell>
                                                <TableCell className='font-medium'>{contactName(c)}</TableCell>
                                                <TableCell>{c.email}</TableCell>
                                                <TableCell>{c.organizationName || '—'}</TableCell>
                                            </TableRow>
                                        ))}
                                    </TableBody>
                                </Table>
                            )}
                        </CardContent>
                    </Card>

                    <Card>
                        <CardHeader>
                            <CardTitle className='text-lg'>Compose Appeal</CardTitle>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <div className='space-y-2'>
                                <Label htmlFor='appeal-subject'>Subject</Label>
                                <Input
                                    id='appeal-subject'
                                    value={subject}
                                    onChange={(e) => setSubject(e.target.value)}
                                    placeholder='Email subject line'
                                />
                            </div>
                            <div className='space-y-2'>
                                <Label htmlFor='appeal-body'>Message</Label>
                                <Textarea
                                    id='appeal-body'
                                    value={body}
                                    onChange={(e) => setBody(e.target.value)}
                                    placeholder='Write your appeal message...'
                                    rows={10}
                                />
                                <p className='text-xs text-muted-foreground'>
                                    Each recipient&apos;s name will be automatically inserted in the greeting.
                                </p>
                            </div>
                            <div className='flex justify-end'>
                                <Button
                                    onClick={handleSend}
                                    disabled={sendBulk.isPending || selectedContactIds.size === 0}
                                >
                                    {sendBulk.isPending ? <Loader2 className='animate-spin' /> : <Send />}
                                    Send to {selectedContactIds.size} Contact
                                    {selectedContactIds.size !== 1 ? 's' : ''}
                                </Button>
                            </div>
                        </CardContent>
                    </Card>
                </div>
            </div>
        </div>
    );
};
