import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { getConsent, setConsent, loadClarity, loadAppInsights } from '@/lib/analytics';

export function CookieConsent() {
    const [visible, setVisible] = useState(() => getConsent() === null);

    if (!visible) return null;

    function handleAccept() {
        setConsent(true);
        loadClarity();
        loadAppInsights();
        setVisible(false);
    }

    function handleReject() {
        setConsent(false);
        setVisible(false);
    }

    return (
        <div className='fixed bottom-0 left-0 right-0 z-50 border-t bg-white p-4 shadow-lg'>
            <div className='mx-auto flex max-w-4xl flex-col items-center gap-3 sm:flex-row sm:justify-between'>
                <p className='text-sm text-muted-foreground'>
                    We use cookies for analytics and to improve your experience.{' '}
                    <a href='/privacypolicy' className='underline hover:text-foreground'>
                        Privacy Policy
                    </a>
                </p>
                <div className='flex shrink-0 gap-2'>
                    <Button variant='outline' size='sm' onClick={handleReject}>
                        Reject Non-Essential
                    </Button>
                    <Button size='sm' onClick={handleAccept}>
                        Accept All
                    </Button>
                </div>
            </div>
        </div>
    );
}
