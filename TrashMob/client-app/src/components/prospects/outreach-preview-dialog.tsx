import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Send } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/hooks/use-toast';
import {
    PreviewOutreach,
    SendOutreach,
    GetOutreachHistory,
    GetProspectActivities,
    GetCommunityProspectById,
} from '@/services/community-prospects';
import OutreachPreviewData from '@/components/Models/OutreachPreviewData';

const CADENCE_STEP_LABELS: Record<number, string> = {
    1: 'Initial Outreach',
    2: 'Follow-up',
    3: 'Value Add',
    4: 'Final Follow-up',
};

interface OutreachPreviewDialogProps {
    prospectId: string;
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

export function OutreachPreviewDialog({ prospectId, open, onOpenChange }: OutreachPreviewDialogProps) {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [preview, setPreview] = useState<OutreachPreviewData | null>(null);
    const [loading, setLoading] = useState(false);
    const [editedSubject, setEditedSubject] = useState('');
    const [editedBody, setEditedBody] = useState('');

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
                                    <Textarea
                                        id='outreach-body'
                                        value={editedBody}
                                        onChange={(e) => setEditedBody(e.target.value)}
                                        className='min-h-[300px] font-mono text-sm'
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
