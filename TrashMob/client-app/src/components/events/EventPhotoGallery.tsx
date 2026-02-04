import { FC, useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { GetEventPhotos, DeleteEventPhoto, FlagEventPhoto } from '@/services/event-photos';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { Camera, X, ChevronLeft, ChevronRight, Flag, Trash2, MoreVertical, Loader2, ImageOff } from 'lucide-react';
import { useToast } from '@/hooks/use-toast';
import { getEventPhotoTypeLabel } from '@/components/Models/EventPhotoData';

interface EventPhotoGalleryProps {
    eventId: string;
    canUpload?: boolean;
    canDelete?: boolean;
    currentUserId?: string;
    onUploadClick?: () => void;
}

export const EventPhotoGallery: FC<EventPhotoGalleryProps> = ({
    eventId,
    canUpload = false,
    canDelete = false,
    currentUserId,
    onUploadClick,
}) => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [selectedPhotoIndex, setSelectedPhotoIndex] = useState<number | null>(null);
    const [filterType, setFilterType] = useState<number | undefined>(undefined);
    const [flagDialogOpen, setFlagDialogOpen] = useState(false);
    const [flagReason, setFlagReason] = useState('');
    const [photoToFlag, setPhotoToFlag] = useState<string | null>(null);

    const { data: photos, isLoading } = useQuery({
        queryKey: GetEventPhotos({ eventId, photoType: filterType }).key,
        queryFn: GetEventPhotos({ eventId, photoType: filterType }).service,
        select: (res) => res.data,
        enabled: !!eventId,
    });

    const deleteMutation = useMutation({
        mutationFn: async (photoId: string) => {
            return DeleteEventPhoto().service({ eventId, photoId });
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/events', eventId, 'photos'] });
            toast({ title: 'Photo deleted', description: 'The photo has been removed.' });
        },
        onError: () => {
            toast({ title: 'Error', description: 'Failed to delete photo.', variant: 'destructive' });
        },
    });

    const flagMutation = useMutation({
        mutationFn: async ({ photoId, reason }: { photoId: string; reason: string }) => {
            return FlagEventPhoto().service({ eventId, photoId }, { reason });
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/events', eventId, 'photos'] });
            toast({ title: 'Photo reported', description: 'The photo has been flagged for review.' });
            setFlagDialogOpen(false);
            setFlagReason('');
            setPhotoToFlag(null);
        },
        onError: () => {
            toast({ title: 'Error', description: 'Failed to report photo.', variant: 'destructive' });
        },
    });

    const handlePrevPhoto = () => {
        if (selectedPhotoIndex !== null && photos) {
            setSelectedPhotoIndex((selectedPhotoIndex - 1 + photos.length) % photos.length);
        }
    };

    const handleNextPhoto = () => {
        if (selectedPhotoIndex !== null && photos) {
            setSelectedPhotoIndex((selectedPhotoIndex + 1) % photos.length);
        }
    };

    const handleDeletePhoto = (photoId: string) => {
        if (confirm('Are you sure you want to delete this photo?')) {
            deleteMutation.mutate(photoId);
            setSelectedPhotoIndex(null);
        }
    };

    const handleFlagPhoto = (photoId: string) => {
        setPhotoToFlag(photoId);
        setFlagDialogOpen(true);
    };

    const submitFlag = () => {
        if (photoToFlag && flagReason.trim()) {
            flagMutation.mutate({ photoId: photoToFlag, reason: flagReason });
        }
    };

    const selectedPhoto = selectedPhotoIndex !== null && photos ? photos[selectedPhotoIndex] : null;
    const canDeletePhoto = (photo: { uploadedByUserId: string }) =>
        canDelete || (currentUserId && photo.uploadedByUserId === currentUserId);

    const photoTypeFilters = [
        { value: undefined, label: 'All Photos' },
        { value: 0, label: 'Before' },
        { value: 1, label: 'During' },
        { value: 2, label: 'After' },
    ];

    return (
        <>
            <Card>
                <CardHeader>
                    <div className='flex items-center justify-between'>
                        <div>
                            <CardTitle className='flex items-center gap-2'>
                                <Camera className='h-5 w-5' />
                                Event Photos ({photos?.length || 0})
                            </CardTitle>
                            <CardDescription>Photos from this cleanup event</CardDescription>
                        </div>
                        {canUpload && onUploadClick ? (
                            <Button variant='outline' onClick={onUploadClick}>
                                <Camera className='h-4 w-4 mr-2' />
                                Add Photos
                            </Button>
                        ) : null}
                    </div>
                </CardHeader>
                <CardContent>
                    {/* Filter buttons */}
                    <div className='flex gap-2 mb-4 flex-wrap'>
                        {photoTypeFilters.map((filter) => (
                            <Button
                                key={filter.label}
                                variant={filterType === filter.value ? 'default' : 'outline'}
                                size='sm'
                                onClick={() => setFilterType(filter.value)}
                            >
                                {filter.label}
                            </Button>
                        ))}
                    </div>

                    {isLoading ? (
                        <div className='text-center py-8'>
                            <Loader2 className='h-8 w-8 animate-spin mx-auto text-muted-foreground' />
                        </div>
                    ) : photos && photos.length > 0 ? (
                        <div className='grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4'>
                            {photos.map((photo, index) => (
                                <div
                                    key={photo.id}
                                    className='relative group rounded-lg overflow-hidden border cursor-pointer bg-muted'
                                    onClick={() => setSelectedPhotoIndex(index)}
                                >
                                    <img
                                        src={photo.thumbnailUrl || photo.imageUrl}
                                        alt={photo.caption || 'Event photo'}
                                        className='w-full h-32 object-cover transition-transform group-hover:scale-105'
                                    />
                                    <Badge variant='secondary' className='absolute bottom-2 left-2 text-xs'>
                                        {getEventPhotoTypeLabel(photo.photoType)}
                                    </Badge>
                                    {photo.inReview ? (
                                        <Badge variant='destructive' className='absolute top-2 left-2 text-xs'>
                                            Under Review
                                        </Badge>
                                    ) : null}
                                </div>
                            ))}
                        </div>
                    ) : (
                        <div className='text-center py-8 text-muted-foreground'>
                            <ImageOff className='h-12 w-12 mx-auto mb-2 opacity-50' />
                            <p>No photos yet for this event.</p>
                            {canUpload && onUploadClick ? (
                                <Button variant='link' onClick={onUploadClick} className='mt-2'>
                                    Be the first to add photos!
                                </Button>
                            ) : null}
                        </div>
                    )}
                </CardContent>
            </Card>

            {/* Photo Lightbox */}
            <Dialog open={selectedPhotoIndex !== null} onOpenChange={() => setSelectedPhotoIndex(null)}>
                <DialogContent className='max-w-4xl p-0 overflow-hidden'>
                    {selectedPhoto ? (
                        <>
                            <div className='relative bg-black'>
                                <img
                                    src={selectedPhoto.imageUrl}
                                    alt={selectedPhoto.caption || 'Event photo'}
                                    className='w-full max-h-[70vh] object-contain'
                                />
                                {/* Navigation arrows */}
                                {photos && photos.length > 1 ? (
                                    <>
                                        <Button
                                            variant='ghost'
                                            size='icon'
                                            className='absolute left-2 top-1/2 -translate-y-1/2 bg-black/50 hover:bg-black/70 text-white'
                                            onClick={(e) => {
                                                e.stopPropagation();
                                                handlePrevPhoto();
                                            }}
                                        >
                                            <ChevronLeft className='h-6 w-6' />
                                        </Button>
                                        <Button
                                            variant='ghost'
                                            size='icon'
                                            className='absolute right-2 top-1/2 -translate-y-1/2 bg-black/50 hover:bg-black/70 text-white'
                                            onClick={(e) => {
                                                e.stopPropagation();
                                                handleNextPhoto();
                                            }}
                                        >
                                            <ChevronRight className='h-6 w-6' />
                                        </Button>
                                    </>
                                ) : null}
                                {/* Close button */}
                                <Button
                                    variant='ghost'
                                    size='icon'
                                    className='absolute top-2 right-2 bg-black/50 hover:bg-black/70 text-white'
                                    onClick={() => setSelectedPhotoIndex(null)}
                                >
                                    <X className='h-5 w-5' />
                                </Button>
                            </div>
                            <div className='p-4'>
                                <div className='flex items-start justify-between'>
                                    <div>
                                        <Badge variant='secondary' className='mb-2'>
                                            {getEventPhotoTypeLabel(selectedPhoto.photoType)}
                                        </Badge>
                                        {selectedPhoto.caption ? (
                                            <p className='text-sm text-foreground'>{selectedPhoto.caption}</p>
                                        ) : null}
                                        {selectedPhoto.takenAt ? (
                                            <p className='text-xs text-muted-foreground mt-1'>
                                                Taken: {new Date(selectedPhoto.takenAt).toLocaleString()}
                                            </p>
                                        ) : null}
                                    </div>
                                    <DropdownMenu>
                                        <DropdownMenuTrigger asChild>
                                            <Button variant='ghost' size='icon'>
                                                <MoreVertical className='h-4 w-4' />
                                            </Button>
                                        </DropdownMenuTrigger>
                                        <DropdownMenuContent align='end'>
                                            <DropdownMenuItem onClick={() => handleFlagPhoto(selectedPhoto.id)}>
                                                <Flag className='h-4 w-4 mr-2' />
                                                Report Photo
                                            </DropdownMenuItem>
                                            {canDeletePhoto(selectedPhoto) && (
                                                <DropdownMenuItem
                                                    onClick={() => handleDeletePhoto(selectedPhoto.id)}
                                                    className='text-destructive'
                                                >
                                                    <Trash2 className='h-4 w-4 mr-2' />
                                                    Delete Photo
                                                </DropdownMenuItem>
                                            )}
                                        </DropdownMenuContent>
                                    </DropdownMenu>
                                </div>
                                {photos && photos.length > 1 ? (
                                    <p className='text-xs text-muted-foreground mt-2'>
                                        {(selectedPhotoIndex ?? 0) + 1} of {photos.length}
                                    </p>
                                ) : null}
                            </div>
                        </>
                    ) : null}
                </DialogContent>
            </Dialog>

            {/* Flag Photo Dialog */}
            <Dialog open={flagDialogOpen} onOpenChange={setFlagDialogOpen}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>Report Photo</DialogTitle>
                        <DialogDescription>
                            Please describe why this photo is inappropriate or violates our community guidelines.
                        </DialogDescription>
                    </DialogHeader>
                    <div className='py-4'>
                        <Label htmlFor='flagReason'>Reason for reporting</Label>
                        <Textarea
                            id='flagReason'
                            value={flagReason}
                            onChange={(e) => setFlagReason(e.target.value)}
                            placeholder='Please describe the issue...'
                            className='mt-2'
                        />
                    </div>
                    <DialogFooter>
                        <Button variant='outline' onClick={() => setFlagDialogOpen(false)}>
                            Cancel
                        </Button>
                        <Button onClick={submitFlag} disabled={!flagReason.trim() || flagMutation.isPending}>
                            {flagMutation.isPending ? <Loader2 className='h-4 w-4 mr-2 animate-spin' /> : null}
                            Submit Report
                        </Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </>
    );
};
