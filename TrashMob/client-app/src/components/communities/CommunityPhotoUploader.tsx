import { FC, useRef, useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { UploadCommunityPhoto } from '@/services/community-photos';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Camera, Upload, X, Loader2, ImagePlus } from 'lucide-react';
import { useToast } from '@/hooks/use-toast';

interface CommunityPhotoUploaderProps {
    slug: string;
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

interface PhotoToUpload {
    file: File;
    preview: string;
}

const MAX_FILE_SIZE = 10 * 1024 * 1024; // 10MB
const ACCEPTED_TYPES = ['image/jpeg', 'image/png', 'image/webp', 'image/heic', 'image/heif'];

export const CommunityPhotoUploader: FC<CommunityPhotoUploaderProps> = ({ slug, open, onOpenChange }) => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [photos, setPhotos] = useState<PhotoToUpload[]>([]);
    const [isUploading, setIsUploading] = useState(false);
    const [uploadProgress, setUploadProgress] = useState(0);

    const uploadMutation = useMutation({
        mutationFn: async (photo: PhotoToUpload) => {
            return UploadCommunityPhoto().service({ slug }, photo.file);
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/communities', slug, 'photos'] });
        },
    });

    const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
        const files = e.target.files;
        if (!files) return;

        const newPhotos: PhotoToUpload[] = [];

        for (let i = 0; i < files.length; i++) {
            const file = files[i];

            if (!ACCEPTED_TYPES.includes(file.type)) {
                toast({
                    title: 'Invalid file type',
                    description: `${file.name} is not a supported image format.`,
                    variant: 'destructive',
                });
            } else if (file.size > MAX_FILE_SIZE) {
                toast({
                    title: 'File too large',
                    description: `${file.name} exceeds the 10MB limit.`,
                    variant: 'destructive',
                });
            } else {
                newPhotos.push({
                    file,
                    preview: URL.createObjectURL(file),
                });
            }
        }

        setPhotos((prev) => [...prev, ...newPhotos]);

        // Reset input
        if (fileInputRef.current) {
            fileInputRef.current.value = '';
        }
    };

    const handleRemovePhoto = (index: number) => {
        setPhotos((prev) => {
            const newPhotos = [...prev];
            URL.revokeObjectURL(newPhotos[index].preview);
            newPhotos.splice(index, 1);
            return newPhotos;
        });
    };

    const handleUpload = async () => {
        if (photos.length === 0) return;

        setIsUploading(true);
        setUploadProgress(0);

        let successCount = 0;
        let errorCount = 0;

        for (let i = 0; i < photos.length; i++) {
            try {
                await uploadMutation.mutateAsync(photos[i]);
                successCount++;
            } catch (error) {
                errorCount++;
                console.error('Failed to upload photo:', error);
            }
            setUploadProgress(((i + 1) / photos.length) * 100);
        }

        setIsUploading(false);
        setUploadProgress(0);

        // Clean up previews
        photos.forEach((photo) => URL.revokeObjectURL(photo.preview));
        setPhotos([]);

        if (errorCount === 0) {
            toast({
                title: 'Photos uploaded',
                description: `Successfully uploaded ${successCount} photo${successCount > 1 ? 's' : ''}.`,
            });
            onOpenChange(false);
        } else {
            toast({
                title: 'Upload partially completed',
                description: `${successCount} uploaded, ${errorCount} failed.`,
                variant: 'destructive',
            });
        }
    };

    const handleClose = () => {
        if (isUploading) return;
        photos.forEach((photo) => URL.revokeObjectURL(photo.preview));
        setPhotos([]);
        onOpenChange(false);
    };

    return (
        <Dialog open={open} onOpenChange={handleClose}>
            <DialogContent className='max-w-2xl max-h-[90vh] overflow-y-auto'>
                <DialogHeader>
                    <DialogTitle className='flex items-center gap-2'>
                        <Camera className='h-5 w-5' />
                        Upload Community Photos
                    </DialogTitle>
                    <DialogDescription>Share photos showcasing your community's cleanup efforts.</DialogDescription>
                </DialogHeader>

                <div className='space-y-4 py-4'>
                    {/* Drop zone / file input */}
                    <div
                        className='border-2 border-dashed rounded-lg p-8 text-center cursor-pointer hover:border-primary transition-colors'
                        onClick={() => fileInputRef.current?.click()}
                    >
                        <input
                            ref={fileInputRef}
                            type='file'
                            accept='image/*'
                            multiple
                            onChange={handleFileSelect}
                            className='hidden'
                        />
                        <ImagePlus className='h-10 w-10 mx-auto text-muted-foreground mb-2' />
                        <p className='text-sm text-muted-foreground'>Click to select photos or drag and drop</p>
                        <p className='text-xs text-muted-foreground mt-1'>JPEG, PNG, WebP up to 10MB each</p>
                    </div>

                    {/* Photo previews */}
                    {photos.length > 0 ? (
                        <div className='space-y-4'>
                            <h4 className='font-medium'>Selected Photos ({photos.length})</h4>
                            <div className='grid grid-cols-3 gap-4'>
                                {photos.map((photo, index) => (
                                    <div key={index} className='relative'>
                                        <img
                                            src={photo.preview}
                                            alt={`Preview ${index + 1}`}
                                            className='w-full h-24 object-cover rounded-md'
                                        />
                                        <Button
                                            variant='destructive'
                                            size='icon'
                                            className='absolute -top-2 -right-2 h-6 w-6'
                                            onClick={() => handleRemovePhoto(index)}
                                            disabled={isUploading}
                                        >
                                            <X className='h-3 w-3' />
                                        </Button>
                                    </div>
                                ))}
                            </div>
                        </div>
                    ) : null}

                    {/* Upload progress */}
                    {isUploading ? (
                        <div className='space-y-2'>
                            <div className='flex items-center justify-between text-sm'>
                                <span>Uploading...</span>
                                <span>{Math.round(uploadProgress)}%</span>
                            </div>
                            <div className='h-2 bg-muted rounded-full overflow-hidden'>
                                <div
                                    className='h-full bg-primary transition-all duration-300'
                                    style={{ width: `${uploadProgress}%` }}
                                />
                            </div>
                        </div>
                    ) : null}
                </div>

                <DialogFooter>
                    <Button variant='outline' onClick={handleClose} disabled={isUploading}>
                        Cancel
                    </Button>
                    <Button onClick={handleUpload} disabled={photos.length === 0 || isUploading}>
                        {isUploading ? (
                            <>
                                <Loader2 className='h-4 w-4 mr-2 animate-spin' />
                                Uploading...
                            </>
                        ) : (
                            <>
                                <Upload className='h-4 w-4 mr-2' />
                                Upload {photos.length} Photo{photos.length !== 1 ? 's' : ''}
                            </>
                        )}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};
