import { useState, useCallback, useRef } from 'react';
import Cropper, { Area } from 'react-easy-crop';
import { Upload, Loader2, X, ImageIcon } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
    DialogDescription,
} from '@/components/ui/dialog';

const MAX_FILE_SIZE_MB = 10;
const ACCEPTED_TYPES = ['image/jpeg', 'image/png', 'image/webp'];

interface ImageCropUploadProps {
    aspectRatio: number;
    currentImageUrl?: string;
    onCropComplete: (file: File) => void;
    label: string;
    recommendedSize: string;
    uploading?: boolean;
}

async function getCroppedImg(imageSrc: string, pixelCrop: Area): Promise<File> {
    const image = new Image();
    image.crossOrigin = 'anonymous';
    await new Promise<void>((resolve, reject) => {
        image.onload = () => resolve();
        image.onerror = reject;
        image.src = imageSrc;
    });

    const canvas = document.createElement('canvas');
    canvas.width = pixelCrop.width;
    canvas.height = pixelCrop.height;
    const ctx = canvas.getContext('2d')!;

    ctx.drawImage(
        image,
        pixelCrop.x,
        pixelCrop.y,
        pixelCrop.width,
        pixelCrop.height,
        0,
        0,
        pixelCrop.width,
        pixelCrop.height,
    );

    return new Promise((resolve, reject) => {
        canvas.toBlob(
            (blob) => {
                if (!blob) {
                    reject(new Error('Canvas export failed'));
                    return;
                }
                resolve(new File([blob], 'cropped.jpg', { type: 'image/jpeg' }));
            },
            'image/jpeg',
            0.9,
        );
    });
}

export function ImageCropUpload({
    aspectRatio,
    currentImageUrl,
    onCropComplete,
    label,
    recommendedSize,
    uploading,
}: ImageCropUploadProps) {
    const [imageSrc, setImageSrc] = useState<string | null>(null);
    const [crop, setCrop] = useState({ x: 0, y: 0 });
    const [zoom, setZoom] = useState(1);
    const [croppedAreaPixels, setCroppedAreaPixels] = useState<Area | null>(null);
    const [dialogOpen, setDialogOpen] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const inputRef = useRef<HTMLInputElement>(null);

    const onFileSelect = useCallback(
        (e: React.ChangeEvent<HTMLInputElement>) => {
            setError(null);
            const file = e.target.files?.[0];
            if (!file) return;

            if (!ACCEPTED_TYPES.includes(file.type)) {
                setError('Please select a JPEG, PNG, or WebP image.');
                return;
            }

            if (file.size > MAX_FILE_SIZE_MB * 1024 * 1024) {
                setError(`File must be under ${MAX_FILE_SIZE_MB}MB.`);
                return;
            }

            const reader = new FileReader();
            reader.onload = () => {
                setImageSrc(reader.result as string);
                setCrop({ x: 0, y: 0 });
                setZoom(1);
                setDialogOpen(true);
            };
            reader.readAsDataURL(file);

            // Reset input so the same file can be re-selected
            e.target.value = '';
        },
        [],
    );

    const handleCropConfirm = useCallback(async () => {
        if (!imageSrc || !croppedAreaPixels) return;

        try {
            const croppedFile = await getCroppedImg(imageSrc, croppedAreaPixels);
            setDialogOpen(false);
            setImageSrc(null);
            onCropComplete(croppedFile);
        } catch {
            setError('Failed to crop image. Please try again.');
        }
    }, [imageSrc, croppedAreaPixels, onCropComplete]);

    const handleCancel = useCallback(() => {
        setDialogOpen(false);
        setImageSrc(null);
    }, []);

    return (
        <div className='space-y-2'>
            <div className='text-sm font-medium'>{label}</div>
            <p className='text-xs text-muted-foreground'>Recommended: {recommendedSize}</p>

            {/* Current image preview or upload zone */}
            <div
                className='relative flex cursor-pointer items-center justify-center overflow-hidden rounded-lg border-2 border-dashed border-muted-foreground/25 transition-colors hover:border-muted-foreground/50'
                style={{ aspectRatio: aspectRatio, maxHeight: aspectRatio >= 2 ? '150px' : '200px' }}
                onClick={() => !uploading && inputRef.current?.click()}
            >
                {uploading ? (
                    <div className='flex flex-col items-center gap-2 text-muted-foreground'>
                        <Loader2 className='h-8 w-8 animate-spin' />
                        <span className='text-sm'>Uploading...</span>
                    </div>
                ) : currentImageUrl ? (
                    <>
                        <img
                            src={currentImageUrl}
                            alt={label}
                            className='h-full w-full object-cover'
                        />
                        <div className='absolute inset-0 flex items-center justify-center bg-black/0 transition-colors hover:bg-black/40'>
                            <span className='text-sm font-medium text-white opacity-0 transition-opacity hover:opacity-100'>
                                Click to replace
                            </span>
                        </div>
                    </>
                ) : (
                    <div className='flex flex-col items-center gap-2 p-4 text-muted-foreground'>
                        <ImageIcon className='h-8 w-8' />
                        <span className='text-sm'>Click to upload</span>
                    </div>
                )}
            </div>

            <input
                ref={inputRef}
                type='file'
                accept={ACCEPTED_TYPES.join(',')}
                className='hidden'
                onChange={onFileSelect}
            />

            {error ? <p className='text-sm text-destructive'>{error}</p> : null}

            {/* Crop dialog */}
            <Dialog open={dialogOpen} onOpenChange={(open) => !open && handleCancel()}>
                <DialogContent className='sm:max-w-lg'>
                    <DialogHeader>
                        <DialogTitle>Crop {label}</DialogTitle>
                        <DialogDescription>
                            Drag to reposition, scroll to zoom. The cropped area will be uploaded.
                        </DialogDescription>
                    </DialogHeader>
                    <div className='relative h-[300px] w-full'>
                        {imageSrc ? (
                            <Cropper
                                image={imageSrc}
                                crop={crop}
                                zoom={zoom}
                                aspect={aspectRatio}
                                onCropChange={setCrop}
                                onZoomChange={setZoom}
                                onCropComplete={(_croppedArea, croppedAreaPixels) =>
                                    setCroppedAreaPixels(croppedAreaPixels)
                                }
                            />
                        ) : null}
                    </div>
                    <div className='flex items-center gap-3 px-1'>
                        <span className='text-xs text-muted-foreground'>Zoom</span>
                        <input
                            type='range'
                            value={zoom}
                            min={1}
                            max={3}
                            step={0.1}
                            onChange={(e) => setZoom(Number(e.target.value))}
                            className='flex-1'
                        />
                    </div>
                    <DialogFooter>
                        <Button type='button' variant='outline' onClick={handleCancel}>
                            <X className='mr-2 h-4 w-4' />
                            Cancel
                        </Button>
                        <Button type='button' onClick={handleCropConfirm}>
                            <Upload className='mr-2 h-4 w-4' />
                            Crop & Upload
                        </Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </div>
    );
}
