import { FC, useRef, useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { UploadEventPhoto } from '@/services/event-photos';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Camera, Upload, X, Loader2, ImagePlus } from 'lucide-react';
import { useToast } from '@/hooks/use-toast';

interface EventPhotoUploaderProps {
    eventId: string;
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

interface PhotoToUpload {
    file: File;
    preview: string;
    caption: string;
    photoType: number;
}

const MAX_FILE_SIZE = 10 * 1024 * 1024; // 10MB
const ACCEPTED_TYPES = ['image/jpeg', 'image/png', 'image/webp', 'image/heic', 'image/heif'];

export const EventPhotoUploader: FC<EventPhotoUploaderProps> = ({ eventId, open, onOpenChange }) => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [photos, setPhotos] = useState<PhotoToUpload[]>([]);
    const [isUploading, setIsUploading] = useState(false);
    const [uploadProgress, setUploadProgress] = useState(0);

    const uploadMutation = useMutation({
        mutationFn: async (photo: PhotoToUpload) => {
            return UploadEventPhoto().service({ eventId }, photo.file);
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/events', eventId, 'photos'] });
        },
    });

    const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
        const files = e.target.files;
        if (!files) return;

        const newPhotos: PhotoToUpload[] = [];

        for (let i = 0; i < files.length; i++) {
            const file = files[i];

            // Validate file type
            if (!ACCEPTED_TYPES.includes(file.type)) {
                toast({
                    title: 'Invalid file type',
                    description: `${file.name} is not a supported image format.`,
                    variant: 'destructive',
                });
            } else if (file.size > MAX_FILE_SIZE) {
                // Validate file size
                toast({
                    title: 'File too large',
                    description: `${file.name} exceeds the 10MB limit.`,
                    variant: 'destructive',
                });
            } else {
                newPhotos.push({
                    file,
                    preview: URL.createObjectURL(file),
                    caption: '',
                    photoType: 1, // Default to "During"
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

    const handleUpdatePhotoType = (index: number, photoType: string) => {
        setPhotos((prev) => {
            const newPhotos = [...prev];
            newPhotos[index] = { ...newPhotos[index], photoType: parseInt(photoType) };
            return newPhotos;
        });
    };

    const handleUpdateCaption = (index: number, caption: string) => {
        setPhotos((prev) => {
            const newPhotos = [...prev];
            newPhotos[index] = { ...newPhotos[index], caption };
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
                        Upload Event Photos
                    </DialogTitle>
                    <DialogDescription>
                        Share photos from this cleanup event. Mark each photo as Before, During, or After.
                    </DialogDescription>
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
                    {photos.length > 0 && (
                        <div className='space-y-4'>
                            <h4 className='font-medium'>Selected Photos ({photos.length})</h4>
                            {photos.map((photo, index) => (
                                <div key={index} className='flex gap-4 p-3 border rounded-lg bg-muted/30'>
                                    {/* Thumbnail */}
                                    <div className='relative flex-shrink-0'>
                                        <img
                                            src={photo.preview}
                                            alt={`Preview ${index + 1}`}
                                            className='w-24 h-24 object-cover rounded-md'
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

                                    {/* Photo details */}
                                    <div className='flex-1 space-y-2'>
                                        <div>
                                            <Label htmlFor={`type-${index}`} className='text-xs'>
                                                Photo Type
                                            </Label>
                                            <Select
                                                value={photo.photoType.toString()}
                                                onValueChange={(value) => handleUpdatePhotoType(index, value)}
                                                disabled={isUploading}
                                            >
                                                <SelectTrigger id={`type-${index}`} className='h-8'>
                                                    <SelectValue />
                                                </SelectTrigger>
                                                <SelectContent>
                                                    <SelectItem value='0'>Before</SelectItem>
                                                    <SelectItem value='1'>During</SelectItem>
                                                    <SelectItem value='2'>After</SelectItem>
                                                </SelectContent>
                                            </Select>
                                        </div>
                                        <div>
                                            <Label htmlFor={`caption-${index}`} className='text-xs'>
                                                Caption (optional)
                                            </Label>
                                            <Input
                                                id={`caption-${index}`}
                                                value={photo.caption}
                                                onChange={(e) => handleUpdateCaption(index, e.target.value)}
                                                placeholder='Add a caption...'
                                                className='h-8'
                                                disabled={isUploading}
                                            />
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}

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
