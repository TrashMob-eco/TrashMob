import { useRef, useCallback } from 'react';
import { QRCodeCanvas } from 'qrcode.react';
import { Button } from '@/components/ui/button';
import { Download } from 'lucide-react';

interface QRCodeDisplayProps {
    url: string;
    title: string;
    size?: number;
}

export const QRCodeDisplay = ({ url, title, size = 200 }: QRCodeDisplayProps) => {
    const qrRef = useRef<HTMLDivElement>(null);

    const handleDownload = useCallback(() => {
        const canvas = qrRef.current?.querySelector('canvas');
        if (!canvas) return;

        const link = document.createElement('a');
        link.download = `${title.replace(/[^a-z0-9]/gi, '-').toLowerCase()}-qr.png`;
        link.href = canvas.toDataURL('image/png');
        link.click();
    }, [title]);

    return (
        <div className='flex flex-col items-center gap-4'>
            <div ref={qrRef} className='bg-white p-4 rounded-lg'>
                <QRCodeCanvas value={url} size={size} level='M' marginSize={2} title={`QR code for ${title}`} />
            </div>
            <p className='text-sm text-muted-foreground text-center max-w-[250px]'>
                Scan this QR code to share the link
            </p>
            <Button variant='outline' size='sm' onClick={handleDownload}>
                <Download className='h-4 w-4 mr-2' />
                Download QR Code
            </Button>
        </div>
    );
};
