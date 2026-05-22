import { useEffect, useMemo, useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Send } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useToast } from '@/hooks/use-toast';
import {
    PreviewOutreach,
    SendOutreach,
    GetOutreachHistory,
    GetProspectActivities,
    GetCommunityProspectById,
} from '@/services/community-prospects';
import OutreachPreviewData from '@/components/Models/OutreachPreviewData';
import ProspectContactData from '@/components/Models/ProspectContactData';
import { ProspectContactStatusBadge } from '@/components/prospects/prospect-contact-status-badge';

const CADENCE_STEP_LABELS: Record<number, string> = {
    1: 'Initial Outreach',
    2: 'Follow-up',
    3: 'Value Add',
    4: 'Final Follow-up',
};

const PRIMARY_CONTACT_VALUE = '__primary__';

interface OutreachPreviewDialogProps {
    prospectId: string;
    open: boolean;
    onOpenChange: (open: boolean) => void;
    /**
     * The prospect's contacts. The dialog pre-selects the primary active contact and
     * lets the user pick a different one before sending. Pass [] to fall back to the
     * backend's automatic primary-contact resolution.
     */
    contacts?: ProspectContactData[];
}

export function OutreachPreviewDialog({ prospectId, open, onOpenChange, contacts = [] }: OutreachPreviewDialogProps) {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [preview, setPreview] = useState<OutreachPreviewData | null>(null);
    const [loading, setLoading] = useState(false);
    const [editedSubject, setEditedSubject] = useState('');
    const [editedBody, setEditedBody] = useState('');
    const [selectedContactId, setSelectedContactId] = useState<string>(PRIMARY_CONTACT_VALUE);

    const sendableContacts = useMemo(
        () => contacts.filter((c) => c.contactStatus === 0 /* Active */ || c.contactStatus === 4 /* RightPerson */),
        [contacts],
    );

    // When the dialog opens (or the contacts list changes), default to the primary
    // contact's id so the dropdown reflects who the email will actually go to.
    useEffect(() => {
        if (!open) {
            return;
        }
        const primary = sendableContacts.find((c) => c.isPrimary);
        if (primary) {
            setSelectedContactId(primary.id);
        } else if (sendableContacts.length === 1) {
            setSelectedContactId(sendableContacts[0].id);
        } else {
            setSelectedContactId(PRIMARY_CONTACT_VALUE);
        }
    }, [open, sendableContacts]);

    const generatePreview = async () => {
        setLoading(true);
        try {
            const res = await PreviewOutreach({ id: prospectId }).service();
            setPreview(res.data);
            setEditedSubject(res.data.subject);
            setEditedBody(res.data.htmlBody);
        } catch {
            toast({ variant: 'destructive', title: 'Failed to generate preview' });
        } finally {
            setLoading(false);
        }
    };

    const sendOutreach = useMutation({
        mutationKey: SendOutreach().key,
        mutationFn: SendOutreach().service,
        onSuccess: (res) => {
            if (res.data.success) {
                toast({ variant: 'primary', title: 'Outreach email sent' });
                queryClient.invalidateQueries({
                    queryKey: GetOutreachHistory({ id: prospectId }).key,
                    refetchType: 'all',
                });
                queryClient.invalidateQueries({
                    queryKey: GetProspectActivities({ id: prospectId }).key,
                    refetchType: 'all',
                });
                queryClient.invalidateQueries({
                    queryKey: GetCommunityProspectById({ id: prospectId }).key,
                    refetchType: 'all',
                });
                onOpenChange(false);
                setPreview(null);
            } else {
                toast({ variant: 'destructive', title: res.data.errorMessage || 'Failed to send' });
            }
        },
    });

    const handleOpen = (isOpen: boolean) => {
        onOpenChange(isOpen);
        if (isOpen && !preview) {
            generatePreview();
        }
        if (!isOpen) {
            setPreview(null);
        }
    };

    const isCadenceComplete = preview?.subject?.includes('complete') ?? false;
    const canSend = preview && preview.cadenceStep <= 4 && !isCadenceComplete;

    const selectedContact =
        selectedContactId === PRIMARY_CONTACT_VALUE ? null : (contacts.find((c) => c.id === selectedContactId) ?? null);

    return (
        <Dialog open={open} onOpenChange={handleOpen}>
            <DialogContent className='max-w-2xl max-h-[80vh] overflow-y-auto'>
                <DialogHeader>
                    <DialogTitle>Outreach Email Preview</DialogTitle>
                </DialogHeader>

                {loading ? (
                    <div className='py-8 text-center text-muted-foreground'>Generating AI-personalized content...</div>
                ) : preview ? (
                    <div className='space-y-4'>
                        <div className='flex items-center gap-2'>
                            <Badge variant='outline'>
                                Step {preview.cadenceStep}: {CADENCE_STEP_LABELS[preview.cadenceStep] || 'Outreach'}
                            </Badge>
                            {preview.tokensUsed > 0 && (
                                <span className='text-xs text-muted-foreground'>{preview.tokensUsed} tokens</span>
                            )}
                        </div>

                        {!isCadenceComplete ? (
                            <>
                                {contacts.length > 0 ? (
                                    <div>
                                        <label htmlFor='outreach-contact' className='text-sm font-medium'>
                                            Send to
                                        </label>
                                        <Select value={selectedContactId} onValueChange={setSelectedContactId}>
                                            <SelectTrigger id='outreach-contact'>
                                                <SelectValue />
                                            </SelectTrigger>
                                            <SelectContent>
                                                <SelectItem value={PRIMARY_CONTACT_VALUE}>
                                                    Auto (primary active contact)
                                                </SelectItem>
                                                {sendableContacts.map((c) => (
                                                    <SelectItem key={c.id} value={c.id}>
                                                        {c.name}
                                                        {c.email ? ` <${c.email}>` : ''}
                                                        {c.isPrimary ? ' — primary' : ''}
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                        {selectedContact && !selectedContact.email ? (
                                            <p className='text-xs text-destructive mt-1'>
                                                This contact has no email address — the send will fail.
                                            </p>
                                        ) : null}
                                        {selectedContact ? (
                                            <div className='mt-1'>
                                                <ProspectContactStatusBadge status={selectedContact.contactStatus} />
                                            </div>
                                        ) : null}
                                    </div>
                                ) : null}
                                <div>
                                    <label htmlFor='outreach-subject' className='text-sm font-medium'>
                                        Subject
                                    </label>
                                    <Input
                                        id='outreach-subject'
                                        value={editedSubject}
                                        onChange={(e) => setEditedSubject(e.target.value)}
                                    />
                                </div>
                                <div>
                                    <label htmlFor='outreach-body' className='text-sm font-medium'>
                                        Email Body
                                    </label>
                                    <div
                                        id='outreach-body'
                                        contentEditable
                                        suppressContentEditableWarning
                                        className='border rounded-md p-4 bg-white text-sm prose prose-sm max-w-none min-h-[300px] focus:outline-none focus:ring-2 focus:ring-ring'
                                        dangerouslySetInnerHTML={{ __html: editedBody }}
                                        onBlur={(e) => setEditedBody(e.currentTarget.innerHTML)}
                                    />
                                </div>
                            </>
                        ) : (
                            <p className='text-muted-foreground'>{preview.subject}</p>
                        )}
                    </div>
                ) : null}

                <DialogFooter>
                    <Button variant='outline' onClick={() => onOpenChange(false)}>
                        Cancel
                    </Button>
                    <Button variant='outline' onClick={generatePreview} disabled={loading}>
                        Regenerate
                    </Button>
                    {canSend ? (
                        <Button
                            onClick={() =>
                                sendOutreach.mutate({
                                    id: prospectId,
                                    subject: editedSubject,
                                    htmlBody: editedBody,
                                    prospectContactId:
                                        selectedContactId === PRIMARY_CONTACT_VALUE ? undefined : selectedContactId,
                                })
                            }
                            disabled={sendOutreach.isPending || !editedSubject.trim() || !editedBody.trim()}
                        >
                            <Send className='mr-2 h-4 w-4' /> Send
                        </Button>
                    ) : null}
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}
