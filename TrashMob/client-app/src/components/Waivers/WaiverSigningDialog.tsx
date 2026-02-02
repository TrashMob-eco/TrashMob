import React, { useState, useRef, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Checkbox } from '@/components/ui/checkbox';
import { Label } from '@/components/ui/label';
import { WaiverVersionData, AcceptWaiverRequest } from '@/components/Models/WaiverVersionData';
import { AcceptWaiver } from '@/services/user-waivers';

interface WaiverSigningDialogProps {
    /** The waiver to sign. */
    waiver: WaiverVersionData;
    /** Whether the dialog is open. */
    open: boolean;
    /** Called when the dialog should close. */
    onClose: () => void;
    /** Called when the waiver is successfully signed. */
    onSigned: () => void;
    /** Optional progress indicator (e.g., "1 of 2"). */
    progress?: string;
}

export const WaiverSigningDialog: React.FC<WaiverSigningDialogProps> = ({
    waiver,
    open,
    onClose,
    onSigned,
    progress,
}) => {
    const [hasScrolledToBottom, setHasScrolledToBottom] = useState(false);
    const [hasAgreed, setHasAgreed] = useState(false);
    const [typedLegalName, setTypedLegalName] = useState('');
    const scrollContainerRef = useRef<HTMLDivElement>(null);

    // Reset state when dialog opens with a new waiver
    useEffect(() => {
        if (open) {
            setHasScrolledToBottom(false);
            setHasAgreed(false);
            setTypedLegalName('');
        }
    }, [open, waiver.id]);

    const acceptMutation = useMutation({
        mutationKey: AcceptWaiver().key,
        mutationFn: AcceptWaiver().service,
        onSuccess: () => {
            onSigned();
        },
    });

    const handleScroll = () => {
        if (scrollContainerRef.current) {
            const { scrollTop, scrollHeight, clientHeight } = scrollContainerRef.current;
            // Consider "scrolled to bottom" if within 20px of the bottom
            if (scrollHeight - scrollTop - clientHeight < 20) {
                setHasScrolledToBottom(true);
            }
        }
    };

    const handleSign = async () => {
        const request: AcceptWaiverRequest = {
            waiverVersionId: waiver.id,
            typedLegalName: typedLegalName.trim(),
        };
        await acceptMutation.mutateAsync(request);
    };

    const canSign = hasScrolledToBottom && hasAgreed && typedLegalName.trim().length >= 2;

    return (
        <Dialog open={open} onOpenChange={(isOpen) => !isOpen && onClose()}>
            <DialogContent className='sm:max-w-[700px] max-h-[90vh] flex flex-col'>
                <DialogHeader>
                    <DialogTitle className='flex items-center justify-between'>
                        <span>{waiver.name}</span>
                        {progress ? <span className='text-sm font-normal text-muted-foreground'>{progress}</span> : null}
                    </DialogTitle>
                    <DialogDescription>
                        Please read the waiver carefully and scroll to the bottom before signing.
                    </DialogDescription>
                </DialogHeader>

                {/* Scrollable waiver text */}
                <div
                    ref={scrollContainerRef}
                    onScroll={handleScroll}
                    className='flex-1 overflow-y-auto border rounded-md p-4 max-h-[40vh] bg-muted/20'
                >
                    <div
                        className='prose prose-sm max-w-none'
                        dangerouslySetInnerHTML={{ __html: waiver.waiverText }}
                    />
                </div>

                {/* Scroll indicator */}
                {!hasScrolledToBottom && (
                    <p className='text-sm text-amber-600 text-center'>
                        Please scroll to the bottom of the waiver to continue.
                    </p>
                )}

                {/* Agreement checkbox */}
                <div className='flex items-center space-x-2'>
                    <Checkbox
                        id='agree-checkbox'
                        checked={hasAgreed}
                        onCheckedChange={(checked) => setHasAgreed(checked === true)}
                        disabled={!hasScrolledToBottom}
                    />
                    <Label
                        htmlFor='agree-checkbox'
                        className={`text-sm ${!hasScrolledToBottom ? 'text-muted-foreground' : ''}`}
                    >
                        I have read and agree to the above waiver
                    </Label>
                </div>

                {/* Typed legal name */}
                <div className='space-y-2'>
                    <Label htmlFor='legal-name'>Type your legal name to sign</Label>
                    <Input
                        id='legal-name'
                        placeholder='Enter your full legal name'
                        value={typedLegalName}
                        onChange={(e) => setTypedLegalName(e.target.value)}
                        disabled={!hasScrolledToBottom || !hasAgreed}
                    />
                </div>

                <DialogFooter>
                    <Button variant='outline' onClick={onClose} disabled={acceptMutation.isPending}>
                        Cancel
                    </Button>
                    <Button
                        onClick={handleSign}
                        disabled={!canSign || acceptMutation.isPending}
                    >
                        {acceptMutation.isPending ? 'Signing...' : 'Sign Waiver'}
                    </Button>
                </DialogFooter>

                {acceptMutation.isError ? <p className='text-sm text-destructive text-center'>
                        Failed to sign waiver. Please try again.
                    </p> : null}
            </DialogContent>
        </Dialog>
    );
};

export default WaiverSigningDialog;
