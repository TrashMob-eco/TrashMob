import { useState } from 'react';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { DatePicker } from '@/components/ui/datepicker';
import { isUnder13 } from '@/lib/age-utils';

interface AgeGateDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    onConfirm: (dob: Date) => void;
}

export function AgeGateDialog({ open, onOpenChange, onConfirm }: AgeGateDialogProps) {
    const [dob, setDob] = useState<Date | undefined>();
    const [blocked, setBlocked] = useState(false);

    function handleContinue() {
        if (!dob) return;

        if (isUnder13(dob)) {
            setBlocked(true);
            return;
        }

        onConfirm(dob);
    }

    function handleOpenChange(nextOpen: boolean) {
        if (!nextOpen) {
            setDob(undefined);
            setBlocked(false);
        }
        onOpenChange(nextOpen);
    }

    return (
        <Dialog open={open} onOpenChange={handleOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>{blocked ? 'Unable to Create Account' : 'Create Account'}</DialogTitle>
                    <DialogDescription>
                        {blocked
                            ? 'You must be 13 or older to join TrashMob. Thank you for your interest in keeping our communities clean!'
                            : 'Please enter your date of birth to continue. This is required for age verification.'}
                    </DialogDescription>
                </DialogHeader>

                {blocked ? (
                    <DialogFooter>
                        <Button variant='outline' onClick={() => handleOpenChange(false)}>
                            Go Back
                        </Button>
                    </DialogFooter>
                ) : (
                    <>
                        <div className='py-2'>
                            <DatePicker
                                value={dob as Date}
                                onChange={(value) => setDob(value)}
                                placeholder='Select your date of birth'
                                calendarProps={{
                                    captionLayout: 'dropdown-buttons',
                                    fromYear: 1920,
                                    toYear: new Date().getFullYear(),
                                    defaultMonth: new Date(2000, 0),
                                }}
                            />
                        </div>
                        <DialogFooter>
                            <Button variant='outline' onClick={() => handleOpenChange(false)}>
                                Cancel
                            </Button>
                            <Button onClick={handleContinue} disabled={!dob}>
                                Continue
                            </Button>
                        </DialogFooter>
                    </>
                )}
            </DialogContent>
        </Dialog>
    );
}
