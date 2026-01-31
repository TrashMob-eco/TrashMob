import { useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { MapPin, Calendar, User, ArrowLeft, Image as ImageIcon, Pencil, Trash2, Loader2 } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import LitterReportData from '@/components/Models/LitterReportData';
import {
    LitterReportStatusEnum,
    LitterReportStatusLabels,
    LitterReportStatusColors,
} from '@/components/Models/LitterReportStatus';
import { GetLitterReport, DeleteLitterReport, GetUserLitterReports } from '@/services/litter-report';
import { useLogin } from '@/hooks/useLogin';
import { useToast } from '@/hooks/use-toast';

const formatDate = (date: Date | null) => {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: 'numeric',
        minute: '2-digit',
    });
};

const getFullAddress = (image: LitterReportData['litterImages'][0]) => {
    if (!image) return '-';
    const parts = [image.streetAddress, image.city, image.region, image.postalCode, image.country].filter(Boolean);
    return parts.join(', ') || '-';
};

export const LitterReportDetailPage = () => {
    const { litterReportId } = useParams<{ litterReportId: string }>() as { litterReportId: string };
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { currentUser, isUserLoaded } = useLogin();
    const { toast } = useToast();
    const [showDeleteDialog, setShowDeleteDialog] = useState(false);

    const { data: litterReport, isLoading } = useQuery<AxiosResponse<LitterReportData>, unknown, LitterReportData>({
        queryKey: GetLitterReport({ litterReportId }).key,
        queryFn: GetLitterReport({ litterReportId }).service,
        select: (res) => res.data,
        enabled: !!litterReportId,
    });

    const deleteMutation = useMutation({
        mutationKey: DeleteLitterReport({ litterReportId }).key,
        mutationFn: DeleteLitterReport({ litterReportId }).service,
        onSuccess: async () => {
            toast({
                title: 'Litter report deleted',
                description: 'The litter report has been successfully deleted.',
            });
            await queryClient.invalidateQueries({
                queryKey: GetUserLitterReports({ userId: currentUser.id }).key,
            });
            navigate('/litterreports');
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to delete the litter report. Please try again.',
            });
        },
    });

    const canEditOrDelete =
        isUserLoaded && litterReport && (litterReport.createdByUserId === currentUser.id || currentUser.isSiteAdmin);

    const handleDelete = () => {
        deleteMutation.mutate();
        setShowDeleteDialog(false);
    };

    if (isLoading) {
        return (
            <div>
                <HeroSection Title='Litter Report' Description='Loading...' />
                <div className='container py-8 text-center'>Loading litter report details...</div>
            </div>
        );
    }

    if (!litterReport) {
        return (
            <div>
                <HeroSection Title='Litter Report' Description='Not Found' />
                <div className='container py-8 text-center'>
                    <p className='mb-4'>This litter report could not be found.</p>
                    <Button asChild>
                        <Link to='/litterreports'>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Litter Reports
                        </Link>
                    </Button>
                </div>
            </div>
        );
    }

    const statusId = litterReport.litterReportStatusId as LitterReportStatusEnum;
    const statusLabel = LitterReportStatusLabels[statusId] || 'Unknown';
    const statusColor = LitterReportStatusColors[statusId] || 'bg-gray-500';
    const firstImage = litterReport.litterImages?.[0];

    return (
        <div>
            <HeroSection Title='Litter Report' Description={litterReport.name || 'View litter report details'} />
            <div className='container py-8'>
                <div className='mb-4'>
                    <Button variant='outline' asChild>
                        <Link to='/litterreports'>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Litter Reports
                        </Link>
                    </Button>
                </div>

                <div className='grid grid-cols-1 lg:grid-cols-3 gap-6'>
                    {/* Main content */}
                    <div className='lg:col-span-2 space-y-6'>
                        <Card>
                            <CardHeader>
                                <div className='flex items-center justify-between'>
                                    <CardTitle className='text-2xl'>{litterReport.name || 'Untitled Report'}</CardTitle>
                                    <div className='flex items-center gap-2'>
                                        <Badge variant='outline' className={`${statusColor} text-white border-0`}>
                                            {statusLabel}
                                        </Badge>
                                        {canEditOrDelete ? (
                                            <>
                                                <Button variant='outline' size='sm' asChild>
                                                    <Link to={`/litterreports/${litterReportId}/edit`}>
                                                        <Pencil className='h-4 w-4 mr-1' /> Edit
                                                    </Link>
                                                </Button>
                                                <Button
                                                    variant='destructive'
                                                    size='sm'
                                                    onClick={() => setShowDeleteDialog(true)}
                                                >
                                                    <Trash2 className='h-4 w-4 mr-1' /> Delete
                                                </Button>
                                            </>
                                        ) : null}
                                    </div>
                                </div>
                            </CardHeader>
                            <CardContent>
                                <div className='space-y-4'>
                                    <div>
                                        <h3 className='font-semibold mb-2'>Description</h3>
                                        <p className='text-muted-foreground'>
                                            {litterReport.description || 'No description provided.'}
                                        </p>
                                    </div>

                                    {firstImage ? (
                                        <div>
                                            <h3 className='font-semibold mb-2 flex items-center gap-2'>
                                                <MapPin className='h-4 w-4' /> Location
                                            </h3>
                                            <p className='text-muted-foreground'>{getFullAddress(firstImage)}</p>
                                        </div>
                                    ) : null}
                                </div>
                            </CardContent>
                        </Card>

                        {/* Photos */}
                        {litterReport.litterImages && litterReport.litterImages.length > 0 ? (
                            <Card>
                                <CardHeader>
                                    <CardTitle className='flex items-center gap-2'>
                                        <ImageIcon className='h-5 w-5' /> Photos ({litterReport.litterImages.length})
                                    </CardTitle>
                                </CardHeader>
                                <CardContent>
                                    <div className='grid grid-cols-2 md:grid-cols-3 gap-4'>
                                        {litterReport.litterImages.map((image) => (
                                            <div
                                                key={image.id}
                                                className='aspect-square relative rounded-lg overflow-hidden bg-muted'
                                            >
                                                {image.imageURL ? (
                                                    <img
                                                        src={image.imageURL}
                                                        alt='Litter'
                                                        className='object-cover w-full h-full'
                                                    />
                                                ) : (
                                                    <div className='flex items-center justify-center h-full text-muted-foreground'>
                                                        <ImageIcon className='h-8 w-8' />
                                                    </div>
                                                )}
                                            </div>
                                        ))}
                                    </div>
                                </CardContent>
                            </Card>
                        ) : null}
                    </div>

                    {/* Sidebar */}
                    <div className='space-y-6'>
                        <Card>
                            <CardHeader>
                                <CardTitle>Details</CardTitle>
                            </CardHeader>
                            <CardContent className='space-y-4'>
                                <div className='flex items-center gap-2'>
                                    <Calendar className='h-4 w-4 text-muted-foreground' />
                                    <div>
                                        <p className='text-sm text-muted-foreground'>Reported</p>
                                        <p className='font-medium'>{formatDate(litterReport.createdDate)}</p>
                                    </div>
                                </div>

                                {litterReport.createdByUserName ? (
                                    <div className='flex items-center gap-2'>
                                        <User className='h-4 w-4 text-muted-foreground' />
                                        <div>
                                            <p className='text-sm text-muted-foreground'>Reported by</p>
                                            <p className='font-medium'>{litterReport.createdByUserName}</p>
                                        </div>
                                    </div>
                                ) : null}

                                {litterReport.lastUpdatedDate ? (
                                    <div className='flex items-center gap-2'>
                                        <Calendar className='h-4 w-4 text-muted-foreground' />
                                        <div>
                                            <p className='text-sm text-muted-foreground'>Last updated</p>
                                            <p className='font-medium'>{formatDate(litterReport.lastUpdatedDate)}</p>
                                        </div>
                                    </div>
                                ) : null}
                            </CardContent>
                        </Card>

                        {/* Location details for each image */}
                        {litterReport.litterImages && litterReport.litterImages.length > 1 ? (
                            <Card>
                                <CardHeader>
                                    <CardTitle>Photo Locations</CardTitle>
                                </CardHeader>
                                <CardContent className='space-y-3'>
                                    {litterReport.litterImages.map((image, index) => (
                                        <div key={image.id} className='text-sm'>
                                            <p className='font-medium'>Photo {index + 1}</p>
                                            <p className='text-muted-foreground'>{getFullAddress(image)}</p>
                                        </div>
                                    ))}
                                </CardContent>
                            </Card>
                        ) : null}
                    </div>
                </div>

                {/* Delete Confirmation Dialog */}
                <Dialog open={showDeleteDialog} onOpenChange={setShowDeleteDialog}>
                    <DialogContent>
                        <DialogHeader>
                            <DialogTitle>Delete Litter Report</DialogTitle>
                            <DialogDescription>
                                Are you sure you want to delete "{litterReport.name || 'this litter report'}"? This
                                action cannot be undone.
                            </DialogDescription>
                        </DialogHeader>
                        <DialogFooter>
                            <Button variant='outline' onClick={() => setShowDeleteDialog(false)}>
                                Cancel
                            </Button>
                            <Button
                                variant='destructive'
                                onClick={handleDelete}
                                disabled={deleteMutation.isPending}
                            >
                                {deleteMutation.isPending ? <Loader2 className='h-4 w-4 mr-1 animate-spin' /> : null}
                                Delete
                            </Button>
                        </DialogFooter>
                    </DialogContent>
                </Dialog>
            </div>
        </div>
    );
};

export default LitterReportDetailPage;
