import React, { useState } from 'react';
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
 * Handles signing multiple waivers in sequence.
 * Shows waivers one at a time with progress indicator.
 */
export const WaiverSigningFlow: React.FC<WaiverSigningFlowProps> = ({ waivers, open, onComplete }) => {
    const [currentIndex, setCurrentIndex] = useState(0);

    // Reset when flow reopens
    React.useEffect(() => {
        if (open) {
            setCurrentIndex(0);
        }
    }, [open]);

    const handleWaiverSigned = () => {
        if (currentIndex < waivers.length - 1) {
            // Move to next waiver
            setCurrentIndex((prev) => prev + 1);
        } else {
            // All waivers signed
            onComplete(true);
        }
    };

    const handleClose = () => {
        // User cancelled before signing all waivers
        onComplete(false);
    };

    if (!open || waivers.length === 0) {
        return null;
    }

    const currentWaiver = waivers[currentIndex];
    const progress = waivers.length > 1 ? `${currentIndex + 1} of ${waivers.length}` : undefined;

    return (
        <WaiverSigningDialog
            waiver={currentWaiver}
            open={open}
            onClose={handleClose}
            onSigned={handleWaiverSigned}
            progress={progress}
        />
    );
};

export default WaiverSigningFlow;
