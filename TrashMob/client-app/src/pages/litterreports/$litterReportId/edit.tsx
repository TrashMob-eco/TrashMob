import { useCallback, useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Guid } from 'guid-typescript';
import { AxiosResponse } from 'axios';
import { ArrowLeft, Loader2 } from 'lucide-react';
import { APIProvider } from '@vis.gl/react-google-maps';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { ImageUploader, ImageWithLocation } from '@/components/litterreports/image-uploader';
import { ImageLocationEditor } from '@/components/litterreports/image-location-editor';
import LitterReportData from '@/components/Models/LitterReportData';
import LitterImageData from '@/components/Models/LitterImageData';
import { LitterReportStatusEnum, LitterReportStatusLabels } from '@/components/Models/LitterReportStatus';
import { MAX_LITTER_REPORT_NAME_LENGTH, MAX_LITTER_REPORT_DESC_LENGTH } from '@/components/Models/Constants';
import { CharacterCounter } from '@/components/ui/character-counter';
import { GetLitterReport, UpdateLitterReport, UploadLitterImage, GetUserLitterReports } from '@/services/litter-report';
import { useLogin } from '@/hooks/useLogin';
import { useToast } from '@/hooks/use-toast';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';

const formSchema = z.object({
    name: z
        .string()
        .min(1, 'Name is required')
        .max(MAX_LITTER_REPORT_NAME_LENGTH, `Name must be less than ${MAX_LITTER_REPORT_NAME_LENGTH} characters`),
    description: z
        .string()
        .max(
            MAX_LITTER_REPORT_DESC_LENGTH,
            `Description must be less than ${MAX_LITTER_REPORT_DESC_LENGTH} characters`,
        ),
    litterReportStatusId: z.string(),
});

type FormInputs = z.infer<typeof formSchema>;

/** Convert existing LitterImageData from the API into ImageWithLocation for the ImageUploader. */
function existingImageToImageWithLocation(img: LitterImageData): ImageWithLocation {
    return {
        id: img.id,
        previewUrl: img.imageUrl,
        imageUrl: img.imageUrl,
        latitude: img.latitude,
        longitude: img.longitude,
        streetAddress: img.streetAddress || '',
        city: img.city || '',
        region: img.region || '',
        country: img.country || '',
        postalCode: img.postalCode || '',
        isLoadingLocation: false,
        locationSource: img.latitude && img.longitude ? 'exif' : 'none',
    };
}

const LitterReportEditPageInner = () => {
    const { litterReportId } = useParams<{ litterReportId: string }>() as { litterReportId: string };
    const { currentUser, isUserLoaded } = useLogin();
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const [images, setImages] = useState<ImageWithLocation[]>([]);
    const [editingImageId, setEditingImageId] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [imagesInitialized, setImagesInitialized] = useState(false);

    const { data: litterReport, isLoading } = useQuery<AxiosResponse<LitterReportData>, unknown, LitterReportData>({
        queryKey: GetLitterReport({ litterReportId }).key,
        queryFn: GetLitterReport({ litterReportId }).service,
        select: (res) => res.data,
        enabled: !!litterReportId,
    });

    const canEdit =
        isUserLoaded && litterReport && (litterReport.createdByUserId === currentUser.id || currentUser.isSiteAdmin);

    const uploadMutation = useMutation({
        mutationKey: UploadLitterImage().key,
        mutationFn: UploadLitterImage().service,
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: '',
            description: '',
            litterReportStatusId: '1',
        },
    });

    const reportName = form.watch('name');
    const reportDescription = form.watch('description');

    // Initialize form and images from loaded report
    useEffect(() => {
        if (litterReport && !imagesInitialized) {
            form.reset({
                name: litterReport.name || '',
                description: litterReport.description || '',
                litterReportStatusId: String(litterReport.litterReportStatusId),
            });

            const existingImages = (litterReport.images || []).map(
                existingImageToImageWithLocation,
            );
            setImages(existingImages);
            setImagesInitialized(true);
        }
    }, [litterReport, form, imagesInitialized]);

    const handleImagesChange = useCallback(
        (newImages: ImageWithLocation[] | ((prev: ImageWithLocation[]) => ImageWithLocation[])) => {
            if (typeof newImages === 'function') {
                setImages(newImages);
            } else {
                setImages(newImages);
            }
        },
        [],
    );

    const handleEditLocation = useCallback((imageId: string) => {
        setEditingImageId(imageId);
    }, []);

    const handleSaveLocation = useCallback((imageId: string, location: Partial<ImageWithLocation>) => {
        setImages((prev) => prev.map((img) => (img.id === imageId ? { ...img, ...location } : img)));
    }, []);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        async (formValues) => {
            if (!litterReport) return;

            if (images.length === 0) {
                toast({
                    variant: 'destructive',
                    title: 'No images',
                    description: 'A litter report must have at least one photo.',
                });
                return;
            }

            // Validate all images have locations
            const imagesWithoutLocation = images.filter((img) => img.latitude === null || img.longitude === null);
            if (imagesWithoutLocation.length > 0) {
                toast({
                    variant: 'destructive',
                    title: 'Missing locations',
                    description: `${imagesWithoutLocation.length} image(s) need a location. Click on them to set the location.`,
                });
                return;
            }

            setIsSubmitting(true);

            try {
                // Build updated report with image metadata
                const updatedReport: LitterReportData = {
                    ...litterReport,
                    name: formValues.name,
                    description: formValues.description,
                    litterReportStatusId: parseInt(formValues.litterReportStatusId, 10),
                    lastUpdatedByUserId: currentUser.id,
                    images: images.map((img) => {
                        const litterImage = new LitterImageData();
                        litterImage.id = img.imageUrl ? img.id : Guid.create().toString();
                        litterImage.litterReportId = litterReport.id;
                        litterImage.latitude = img.latitude;
                        litterImage.longitude = img.longitude;
                        litterImage.streetAddress = img.streetAddress;
                        litterImage.city = img.city;
                        litterImage.region = img.region;
                        litterImage.country = img.country;
                        litterImage.postalCode = img.postalCode;
                        litterImage.createdByUserId = currentUser.id;
                        litterImage.lastUpdatedByUserId = currentUser.id;
                        return litterImage;
                    }),
                };

                // Update the report (backend handles image additions/deletions)
                const updateService = UpdateLitterReport({ litterReport: updatedReport });
                await updateService.service();

                // Upload new image files
                const newImages = images.filter((img) => img.file);
                for (const img of newImages) {
                    // Find the matching image in the updated report by coordinates
                    const matchingImage = updatedReport.images.find(
                        (ci) =>
                            ci.latitude === img.latitude &&
                            ci.longitude === img.longitude &&
                            ci.streetAddress === img.streetAddress,
                    );
                    if (matchingImage && img.file) {
                        await uploadMutation.mutateAsync({
                            litterImageId: matchingImage.id,
                            file: img.file,
                        });
                    }
                }

                await queryClient.invalidateQueries({
                    queryKey: GetLitterReport({ litterReportId }).key,
                });
                await queryClient.invalidateQueries({
                    queryKey: GetUserLitterReports({ userId: currentUser.id }).key,
                });

                toast({
                    title: 'Litter report updated',
                    description: 'The litter report has been successfully updated.',
                });

                navigate(`/litterreports/${litterReportId}`);
            } catch {
                toast({
                    variant: 'destructive',
                    title: 'Error',
                    description: 'Failed to update the litter report. Please try again.',
                });
            } finally {
                setIsSubmitting(false);
            }
        },
        [litterReport, images, currentUser.id, queryClient, litterReportId, navigate, toast, uploadMutation],
    );

    if (isLoading) {
        return (
            <div>
                <HeroSection Title='Edit Litter Report' Description='Loading...' />
                <div className='container py-8 text-center'>Loading litter report...</div>
            </div>
        );
    }

    if (!litterReport) {
        return (
            <div>
                <HeroSection Title='Edit Litter Report' Description='Not Found' />
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

    if (!canEdit) {
        return (
            <div>
                <HeroSection Title='Edit Litter Report' Description='Access Denied' />
                <div className='container py-8 text-center'>
                    <p className='mb-4'>You do not have permission to edit this litter report.</p>
                    <Button asChild>
                        <Link to={`/litterreports/${litterReportId}`}>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Report
                        </Link>
                    </Button>
                </div>
            </div>
        );
    }

    const statusOptions = [
        { value: String(LitterReportStatusEnum.New), label: LitterReportStatusLabels[LitterReportStatusEnum.New] },
        {
            value: String(LitterReportStatusEnum.Assigned),
            label: LitterReportStatusLabels[LitterReportStatusEnum.Assigned],
        },
        {
            value: String(LitterReportStatusEnum.Cleaned),
            label: LitterReportStatusLabels[LitterReportStatusEnum.Cleaned],
        },
        {
            value: String(LitterReportStatusEnum.Cancelled),
            label: LitterReportStatusLabels[LitterReportStatusEnum.Cancelled],
        },
    ];

    const editingImage = editingImageId ? images.find((img) => img.id === editingImageId) || null : null;

    return (
        <div>
            <HeroSection Title='Edit Litter Report' Description={litterReport.name || 'Update litter report details'} />
            <div className='container py-8'>
                <div className='mb-4'>
                    <Button variant='outline' asChild>
                        <Link to={`/litterreports/${litterReportId}`}>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Report
                        </Link>
                    </Button>
                </div>

                <Card className='max-w-3xl'>
                    <Form {...form}>
                        <form onSubmit={form.handleSubmit(onSubmit)}>
                            <CardHeader>
                                <CardTitle>Edit Litter Report</CardTitle>
                            </CardHeader>
                            <CardContent className='space-y-6'>
                                <FormField
                                    control={form.control}
                                    name='name'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Report Name</FormLabel>
                                            <FormControl>
                                                <Input
                                                    {...field}
                                                    placeholder='e.g., Trash pile on Main Street'
                                                    maxLength={MAX_LITTER_REPORT_NAME_LENGTH}
                                                />
                                            </FormControl>
                                            <CharacterCounter
                                                currentLength={reportName?.length || 0}
                                                maxLength={MAX_LITTER_REPORT_NAME_LENGTH}
                                            />
                                        </FormItem>
                                    )}
                                />

                                <FormField
                                    control={form.control}
                                    name='description'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Description</FormLabel>
                                            <FormControl>
                                                <Textarea
                                                    {...field}
                                                    placeholder='Describe the litter location and what you observed...'
                                                    rows={4}
                                                    maxLength={MAX_LITTER_REPORT_DESC_LENGTH}
                                                />
                                            </FormControl>
                                            <CharacterCounter
                                                currentLength={reportDescription?.length || 0}
                                                maxLength={MAX_LITTER_REPORT_DESC_LENGTH}
                                            />
                                        </FormItem>
                                    )}
                                />

                                <FormField
                                    control={form.control}
                                    name='litterReportStatusId'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Status</FormLabel>
                                            <Select onValueChange={field.onChange} value={field.value}>
                                                <FormControl>
                                                    <SelectTrigger>
                                                        <SelectValue placeholder='Select status' />
                                                    </SelectTrigger>
                                                </FormControl>
                                                <SelectContent>
                                                    {statusOptions.map((option) => (
                                                        <SelectItem key={option.value} value={option.value}>
                                                            {option.label}
                                                        </SelectItem>
                                                    ))}
                                                </SelectContent>
                                            </Select>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />

                                <div>
                                    <FormLabel required>Photos</FormLabel>
                                    <p className='text-sm text-muted-foreground mb-3'>
                                        Add or remove photos. A litter report must have at least one photo.
                                    </p>
                                    <ImageUploader
                                        images={images}
                                        onImagesChange={handleImagesChange}
                                        onEditLocation={handleEditLocation}
                                        minImages={1}
                                    />
                                </div>
                            </CardContent>
                            <CardFooter className='flex justify-end gap-2'>
                                <Button variant='outline' asChild>
                                    <Link to={`/litterreports/${litterReportId}`}>Cancel</Link>
                                </Button>
                                <Button type='submit' disabled={isSubmitting || images.length === 0}>
                                    {isSubmitting ? <Loader2 className='h-4 w-4 mr-1 animate-spin' /> : null}
                                    Save Changes
                                </Button>
                            </CardFooter>
                        </form>
                    </Form>
                </Card>

                {/* Location editor dialog */}
                <ImageLocationEditor
                    open={editingImageId !== null}
                    onOpenChange={(open) => !open && setEditingImageId(null)}
                    image={editingImage}
                    onSave={handleSaveLocation}
                />
            </div>
        </div>
    );
};

export const LitterReportEditPage = () => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();

    if (isLoading) {
        return (
            <div className='flex justify-center items-center py-16'>
                <Loader2 className='animate-spin mr-2' /> Loading...
            </div>
        );
    }

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <LitterReportEditPageInner />
        </APIProvider>
    );
};

export default LitterReportEditPage;
