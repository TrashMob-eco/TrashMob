import { FC, useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Camera, ChevronLeft, ChevronRight, X, Trash2, Upload, Loader2 } from 'lucide-react';
import { GetCommunityPhotos, DeleteCommunityPhoto } from '@/services/community-photos';
import PartnerPhotoData from '@/components/Models/PartnerPhotoData';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent } from '@/components/ui/dialog';
import { useToast } from '@/hooks/use-toast';

interface CommunityPhotoGalleryProps {
    slug: string;
    canUpload?: boolean;
    canDelete?: boolean;
    onUploadClick?: () => void;
}

export const CommunityPhotoGallery: FC<CommunityPhotoGalleryProps> = ({
    slug,
    canUpload = false,
    canDelete = false,
    onUploadClick,
}) => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [selectedPhotoIndex, setSelectedPhotoIndex] = useState<number | null>(null);

    const { data: photos = [], isLoading } = useQuery<
        AxiosResponse<PartnerPhotoData[]>,
        unknown,
        PartnerPhotoData[]
    >({
        queryKey: GetCommunityPhotos({ slug }).key,
        queryFn: GetCommunityPhotos({ slug }).service,
        select: (res) => res.data,
        enabled: !!slug,
    });

    const deleteMutation = useMutation({
        mutationFn: async (photoId: string) => {
            return DeleteCommunityPhoto().service({ slug, photoId });
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/communities', slug, 'photos'] });
            toast({
                title: 'Photo deleted',
                description: 'The photo has been removed from the gallery.',
            });
            setSelectedPhotoIndex(null);
        },
        onError: () => {
            toast({
                title: 'Error',
                description: 'Failed to delete the photo. Please try again.',
                variant: 'destructive',
            });
        },
    });

    const handlePrevious = () => {
        if (selectedPhotoIndex !== null && selectedPhotoIndex > 0) {
            setSelectedPhotoIndex(selectedPhotoIndex - 1);
        }
    };

    const handleNext = () => {
        if (selectedPhotoIndex !== null && selectedPhotoIndex < photos.length - 1) {
            setSelectedPhotoIndex(selectedPhotoIndex + 1);
        }
    };

    const handleDelete = (photoId: string) => {
        if (confirm('Are you sure you want to delete this photo?')) {
            deleteMutation.mutate(photoId);
        }
    };

    const selectedPhoto = selectedPhotoIndex !== null ? photos[selectedPhotoIndex] : null;

    if (isLoading) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <Camera className='h-5 w-5' />
                        Photo Gallery
                    </CardTitle>
                </CardHeader>
                <CardContent>
                    <div className='flex items-center justify-center py-8'>
                        <Loader2 className='h-6 w-6 animate-spin' />
                    </div>
                </CardContent>
            </Card>
        );
    }

    return (
        <>
            <Card>
                <CardHeader>
                    <div className='flex items-center justify-between'>
                        <CardTitle className='flex items-center gap-2'>
                            <Camera className='h-5 w-5' />
                            Photo Gallery
                        </CardTitle>
                        {canUpload && onUploadClick ? (
                            <Button variant='outline' size='sm' onClick={onUploadClick}>
                                <Upload className='h-4 w-4 mr-2' />
                                Upload Photos
                            </Button>
                        ) : null}
                    </div>
                </CardHeader>
                <CardContent>
                    {photos.length === 0 ? (
                        <div className='text-center py-8 text-muted-foreground'>
                            <Camera className='h-12 w-12 mx-auto mb-2 opacity-50' />
                            <p>No photos yet.</p>
                            {canUpload && onUploadClick ? (
                                <Button variant='link' onClick={onUploadClick} className='mt-2'>
                                    Upload the first photo
                                </Button>
                            ) : null}
                        </div>
                    ) : (
                        <div className='grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4'>
                            {photos.map((photo, index) => (
                                <div
                                    key={photo.id}
                                    className='relative aspect-square cursor-pointer group overflow-hidden rounded-lg'
                                    onClick={() => setSelectedPhotoIndex(index)}
                                >
                                    <img
                                        src={photo.imageUrl}
                                        alt={photo.caption || 'Community photo'}
                                        className='w-full h-full object-cover transition-transform group-hover:scale-105'
                                    />
                                    <div className='absolute inset-0 bg-black/0 group-hover:bg-black/20 transition-colors' />
                                </div>
                            ))}
                        </div>
                    )}
                </CardContent>
            </Card>

            {/* Lightbox Dialog */}
            <Dialog open={selectedPhotoIndex !== null} onOpenChange={() => setSelectedPhotoIndex(null)}>
                <DialogContent className='max-w-4xl p-0 overflow-hidden'>
                    {selectedPhoto ? (
                        <div className='relative'>
                            {/* Close button */}
                            <Button
                                variant='ghost'
                                size='icon'
                                className='absolute top-2 right-2 z-10 bg-black/50 hover:bg-black/70 text-white'
                                onClick={() => setSelectedPhotoIndex(null)}
                            >
                                <X className='h-4 w-4' />
                            </Button>

                            {/* Navigation buttons */}
                            {selectedPhotoIndex !== null && selectedPhotoIndex > 0 ? (
                                <Button
                                    variant='ghost'
                                    size='icon'
                                    className='absolute left-2 top-1/2 -translate-y-1/2 z-10 bg-black/50 hover:bg-black/70 text-white'
                                    onClick={handlePrevious}
                                >
                                    <ChevronLeft className='h-6 w-6' />
                                </Button>
                            ) : null}

                            {selectedPhotoIndex !== null && selectedPhotoIndex < photos.length - 1 ? (
                                <Button
                                    variant='ghost'
                                    size='icon'
                                    className='absolute right-2 top-1/2 -translate-y-1/2 z-10 bg-black/50 hover:bg-black/70 text-white'
                                    onClick={handleNext}
                                >
                                    <ChevronRight className='h-6 w-6' />
                                </Button>
                            ) : null}

                            {/* Image */}
                            <img
                                src={selectedPhoto.imageUrl}
                                alt={selectedPhoto.caption || 'Community photo'}
                                className='w-full max-h-[80vh] object-contain'
                            />

                            {/* Caption and actions */}
                            <div className='p-4 bg-background'>
                                <div className='flex items-center justify-between'>
                                    <div>
                                        {selectedPhoto.caption ? (
                                            <p className='text-sm'>{selectedPhoto.caption}</p>
                                        ) : (
                                            <p className='text-sm text-muted-foreground'>No caption</p>
                                        )}
                                        <p className='text-xs text-muted-foreground mt-1'>
                                            {new Date(selectedPhoto.uploadedDate).toLocaleDateString()}
                                        </p>
                                    </div>
                                    {canDelete ? (
                                        <Button
                                            variant='destructive'
                                            size='sm'
                                            onClick={() => handleDelete(selectedPhoto.id)}
                                            disabled={deleteMutation.isPending}
                                        >
                                            {deleteMutation.isPending ? (
                                                <Loader2 className='h-4 w-4 animate-spin' />
                                            ) : (
                                                <>
                                                    <Trash2 className='h-4 w-4 mr-2' />
                                                    Delete
                                                </>
                                            )}
                                        </Button>
                                    ) : null}
                                </div>
                            </div>
                        </div>
                    ) : null}
                </DialogContent>
            </Dialog>
        </>
    );
};
