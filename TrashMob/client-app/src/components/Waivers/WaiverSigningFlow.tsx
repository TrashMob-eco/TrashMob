import React, { useState, useCallback } from 'react';
import { CheckCircle, FileText } from 'lucide-react';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { WaiverVersionData } from '@/components/Models/WaiverVersionData';
import { WaiverSigningDialog } from './WaiverSigningDialog';

interface WaiverSigningFlowProps {
    /** Array of waivers that need to be signed. */
    waivers: WaiverVersionData[];
    /** Whether the flow is active/open. */
    open: boolean;
    /** Called when all waivers are signed or the user cancels. */
    onComplete: (allSigned: boolean) => void;
}

/**
 * Handles signing multiple waivers via a hub/list view.
 *
 * When multiple waivers are required, shows a list with signed/unsigned status.
 * The user picks which waiver to sign, signs it, then returns to the list.
 * When all are signed, a "Continue" button completes the flow.
 *
 * When only one waiver is required, skips the hub and opens the signing dialog directly.
 */
export const WaiverSigningFlow: React.FC<WaiverSigningFlowProps> = ({ waivers, open, onComplete }) => {
    const [signedWaiverIds, setSignedWaiverIds] = useState<Set<string>>(new Set());
    const [selectedWaiver, setSelectedWaiver] = useState<WaiverVersionData | null>(null);

    // Reset state when flow reopens
    React.useEffect(() => {
        if (open) {
            setSignedWaiverIds(new Set());
            setSelectedWaiver(null);
        }
    }, [open]);

    const signedCount = signedWaiverIds.size;
    const allSigned = signedCount === waivers.length;
    const isSingleWaiver = waivers.length === 1;

    const handleSelectWaiver = useCallback((waiver: WaiverVersionData) => {
        setSelectedWaiver(waiver);
    }, []);

    const handleWaiverSigned = useCallback(() => {
        if (selectedWaiver) {
            setSignedWaiverIds((prev) => new Set(prev).add(selectedWaiver.id));
        }
        setSelectedWaiver(null);

        // For single waiver, complete the flow immediately after signing
        if (isSingleWaiver) {
            onComplete(true);
        }
    }, [selectedWaiver, isSingleWaiver, onComplete]);

    const handleSigningCancelled = useCallback(() => {
        setSelectedWaiver(null);

        // For single waiver, cancelling the dialog cancels the entire flow
        if (isSingleWaiver) {
            onComplete(false);
        }
    }, [isSingleWaiver, onComplete]);

    const handleHubClose = useCallback(() => {
        onComplete(false);
    }, [onComplete]);

    if (!open || waivers.length === 0) {
        return null;
    }

    // Single waiver: skip hub, go directly to signing dialog
    if (isSingleWaiver) {
        return (
            <WaiverSigningDialog
                waiver={waivers[0]}
                open={open && selectedWaiver === null ? signedWaiverIds.size === 0 : null}
                onClose={handleSigningCancelled}
                onSigned={handleWaiverSigned}
            />
        );
    }

    // Multiple waivers: show hub/list dialog
    const showHub = selectedWaiver === null;

    return (
        <>
            {/* Hub dialog — list of waivers with status */}
            <Dialog open={showHub} onOpenChange={(isOpen) => !isOpen && handleHubClose()}>
                <DialogContent className='sm:max-w-[500px]'>
                    <DialogHeader>
                        <DialogTitle>Waivers Required</DialogTitle>
                        <DialogDescription>
                            {allSigned
                                ? "All waivers have been signed. You're ready to continue."
                                : 'Please sign the following waivers to proceed.'}
                        </DialogDescription>
                    </DialogHeader>

                    <div className='space-y-2'>
                        {waivers.map((waiver) => {
                            const isSigned = signedWaiverIds.has(waiver.id);
                            return (
                                <button
                                    key={waiver.id}
                                    type='button'
                                    onClick={() => !isSigned && handleSelectWaiver(waiver)}
                                    disabled={isSigned}
                                    className='w-full flex items-center justify-between p-3 rounded-lg border bg-card text-left transition-colors hover:bg-accent disabled:opacity-100 disabled:cursor-default disabled:hover:bg-card'
                                >
                                    <div className='flex items-center gap-3'>
                                        {isSigned ? (
                                            <CheckCircle className='h-5 w-5 text-green-600 shrink-0' />
                                        ) : (
                                            <FileText className='h-5 w-5 text-muted-foreground shrink-0' />
                                        )}
                                        <div>
                                            <p className='font-medium text-sm'>{waiver.name}</p>
                                            <p className='text-xs text-muted-foreground'>Version {waiver.version}</p>
                                        </div>
                                    </div>
                                    <Badge variant={isSigned ? 'default' : 'secondary'}>
                                        {isSigned ? 'Signed' : 'Sign'}
                                    </Badge>
                                </button>
                            );
                        })}
                    </div>

                    <p className='text-sm text-muted-foreground text-center'>
                        {signedCount} of {waivers.length} waivers signed
                    </p>

                    <DialogFooter>
                        {allSigned ? (
                            <Button onClick={() => onComplete(true)}>Continue</Button>
                        ) : (
                            <Button variant='outline' onClick={handleHubClose}>
                                Cancel
                            </Button>
                        )}
                    </DialogFooter>
                </DialogContent>
            </Dialog>

            {/* Signing dialog — opens when a waiver is selected */}
            {selectedWaiver ? (
                <WaiverSigningDialog
                    waiver={selectedWaiver}
                    open
                    onClose={handleSigningCancelled}
                    onSigned={handleWaiverSigned}
                />
            ) : null}
        </>
    );
};

export default WaiverSigningFlow;
