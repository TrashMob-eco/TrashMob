import { useCallback, useRef, useState } from 'react';
import { Guid } from 'guid-typescript';
import exifr from 'exifr';
import { ImagePlus, X, MapPin, AlertCircle, Loader2 } from 'lucide-react';
import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import { AzureMapSearchAddressReverse } from '@/services/maps';
import * as MapStore from '@/store/MapStore';

export interface ImageWithLocation {
    id: string;
    file: File;
    previewUrl: string;
    latitude: number | null;
    longitude: number | null;
    streetAddress: string;
    city: string;
    region: string;
    country: string;
    postalCode: string;
    isLoadingLocation: boolean;
    locationSource: 'exif' | 'manual' | 'none';
}

interface ImageUploaderProps {
    images: ImageWithLocation[];
    onImagesChange: (images: ImageWithLocation[]) => void;
    onEditLocation: (imageId: string) => void;
    maxImages?: number;
    maxSizeMB?: number;
}

const ACCEPTED_TYPES = ['image/jpeg', 'image/png', 'image/webp', 'image/heic', 'image/heif'];

export const ImageUploader = ({
    images,
    onImagesChange,
    onEditLocation,
    maxImages = 10,
    maxSizeMB = 10,
}: ImageUploaderProps) => {
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [isDragging, setIsDragging] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const extractGpsAndReverseGeocode = async (file: File, imageId: string): Promise<Partial<ImageWithLocation>> => {
        try {
            // Try to extract GPS from EXIF
            const gps = await exifr.gps(file);

            if (gps?.latitude && gps?.longitude) {
                // Got GPS, now reverse geocode
                const opts = await MapStore.getOption();
                const response = await AzureMapSearchAddressReverse().service({
                    azureKey: opts.subscriptionKey,
                    lat: gps.latitude,
                    long: gps.longitude,
                });

                const result = response.data.addresses[0];
                return {
                    latitude: gps.latitude,
                    longitude: gps.longitude,
                    streetAddress: result?.address?.streetName || '',
                    city: result?.address?.municipality || '',
                    region: result?.address?.countrySubdivision || '',
                    country: result?.address?.country || '',
                    postalCode: result?.address?.postalCode || '',
                    locationSource: 'exif',
                    isLoadingLocation: false,
                };
            }
        } catch {
            // EXIF extraction failed, that's ok
        }

        return {
            latitude: null,
            longitude: null,
            streetAddress: '',
            city: '',
            region: '',
            country: '',
            postalCode: '',
            locationSource: 'none',
            isLoadingLocation: false,
        };
    };

    const processFiles = useCallback(
        async (files: FileList | File[]) => {
            setError(null);
            const fileArray = Array.from(files);

            // Validate total count
            if (images.length + fileArray.length > maxImages) {
                setError(`Maximum ${maxImages} images allowed`);
                return;
            }

            // Validate each file
            const validFiles = fileArray.filter((file) => {
                if (!ACCEPTED_TYPES.includes(file.type)) {
                    setError(`Invalid file type: ${file.name}. Please use JPEG, PNG, or WebP.`);
                    return false;
                }
                if (file.size > maxSizeMB * 1024 * 1024) {
                    setError(`File too large: ${file.name}. Maximum size is ${maxSizeMB}MB.`);
                    return false;
                }
                return true;
            });

            if (validFiles.length === 0) return;

            // Create initial image entries with loading state
            const newImages: ImageWithLocation[] = validFiles.map((file) => ({
                id: Guid.create().toString(),
                file,
                previewUrl: URL.createObjectURL(file),
                latitude: null,
                longitude: null,
                streetAddress: '',
                city: '',
                region: '',
                country: '',
                postalCode: '',
                isLoadingLocation: true,
                locationSource: 'none' as const,
            }));

            onImagesChange([...images, ...newImages]);

            // Process EXIF for each image
            for (const img of newImages) {
                const locationData = await extractGpsAndReverseGeocode(img.file, img.id);
                onImagesChange((prev) => prev.map((i) => (i.id === img.id ? { ...i, ...locationData } : i)));
            }
        },
        [images, maxImages, maxSizeMB, onImagesChange],
    );

    const handleDrop = useCallback(
        (e: React.DragEvent) => {
            e.preventDefault();
            setIsDragging(false);
            if (e.dataTransfer.files.length > 0) {
                processFiles(e.dataTransfer.files);
            }
        },
        [processFiles],
    );

    const handleDragOver = useCallback((e: React.DragEvent) => {
        e.preventDefault();
        setIsDragging(true);
    }, []);

    const handleDragLeave = useCallback((e: React.DragEvent) => {
        e.preventDefault();
        setIsDragging(false);
    }, []);

    const handleFileSelect = useCallback(
        (e: React.ChangeEvent<HTMLInputElement>) => {
            if (e.target.files && e.target.files.length > 0) {
                processFiles(e.target.files);
            }
            // Reset input so same file can be selected again
            e.target.value = '';
        },
        [processFiles],
    );

    const handleRemoveImage = useCallback(
        (imageId: string) => {
            const imageToRemove = images.find((img) => img.id === imageId);
            if (imageToRemove) {
                URL.revokeObjectURL(imageToRemove.previewUrl);
            }
            onImagesChange(images.filter((img) => img.id !== imageId));
        },
        [images, onImagesChange],
    );

    const handleClick = () => {
        fileInputRef.current?.click();
    };

    const canAddMore = images.length < maxImages;

    return (
        <div className='space-y-4'>
            {/* Dropzone */}
            {canAddMore ? (
                <div
                    className={cn(
                        'border-2 border-dashed rounded-lg p-8 text-center cursor-pointer transition-colors',
                        isDragging
                            ? 'border-primary bg-primary/5'
                            : 'border-muted-foreground/25 hover:border-primary/50',
                    )}
                    onDrop={handleDrop}
                    onDragOver={handleDragOver}
                    onDragLeave={handleDragLeave}
                    onClick={handleClick}
                    role='button'
                    tabIndex={0}
                    onKeyDown={(e) => {
                        if (e.key === 'Enter' || e.key === ' ') {
                            e.preventDefault();
                            handleClick();
                        }
                    }}
                >
                    <input
                        ref={fileInputRef}
                        type='file'
                        accept={ACCEPTED_TYPES.join(',')}
                        multiple
                        className='hidden'
                        onChange={handleFileSelect}
                    />
                    <ImagePlus className='h-12 w-12 mx-auto text-muted-foreground mb-4' />
                    <p className='text-sm font-medium'>Drop photos here or click to upload</p>
                    <p className='text-xs text-muted-foreground mt-1'>
                        JPEG, PNG, or WebP up to {maxSizeMB}MB each (max {maxImages} photos)
                    </p>
                </div>
            ) : null}

            {/* Error message */}
            {error ? (
                <div className='flex items-center gap-2 text-destructive text-sm'>
                    <AlertCircle className='h-4 w-4' />
                    {error}
                </div>
            ) : null}

            {/* Image preview grid */}
            {images.length > 0 ? (
                <div className='grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4'>
                    {images.map((image) => (
                        <div key={image.id} className='relative group rounded-lg overflow-hidden border bg-muted'>
                            {/* Image preview */}
                            <div className='aspect-square'>
                                <img
                                    src={image.previewUrl}
                                    alt='Litter report'
                                    className='w-full h-full object-cover'
                                />
                            </div>

                            {/* Remove button */}
                            <button
                                type='button'
                                onClick={() => handleRemoveImage(image.id)}
                                className='absolute top-2 right-2 p-1 bg-destructive text-destructive-foreground rounded-full opacity-0 group-hover:opacity-100 transition-opacity'
                                aria-label='Remove image'
                            >
                                <X className='h-4 w-4' />
                            </button>

                            {/* Location status */}
                            <div className='absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/70 to-transparent p-2'>
                                {image.isLoadingLocation ? (
                                    <div className='flex items-center gap-1 text-white text-xs'>
                                        <Loader2 className='h-3 w-3 animate-spin' />
                                        <span>Reading location...</span>
                                    </div>
                                ) : image.latitude && image.longitude ? (
                                    <button
                                        type='button'
                                        onClick={() => onEditLocation(image.id)}
                                        className='flex items-center gap-1 text-white text-xs hover:text-primary-foreground'
                                    >
                                        <MapPin className='h-3 w-3' />
                                        <span className='truncate'>{image.city || 'Location set'}</span>
                                    </button>
                                ) : (
                                    <button
                                        type='button'
                                        onClick={() => onEditLocation(image.id)}
                                        className='flex items-center gap-1 text-amber-400 text-xs hover:text-amber-300'
                                    >
                                        <AlertCircle className='h-3 w-3' />
                                        <span>Set location</span>
                                    </button>
                                )}
                            </div>
                        </div>
                    ))}
                </div>
            ) : null}

            {/* Image count */}
            {images.length > 0 ? (
                <p className='text-sm text-muted-foreground'>
                    {images.length} of {maxImages} photos added
                </p>
            ) : null}
        </div>
    );
};
