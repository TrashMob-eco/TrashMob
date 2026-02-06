import { useCallback, useState } from 'react';
import { Link, useNavigate } from 'react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Guid } from 'guid-typescript';
import { ArrowLeft, Loader2 } from 'lucide-react';
import { APIProvider } from '@vis.gl/react-google-maps';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { ImageUploader, ImageWithLocation } from '@/components/litterreports/image-uploader';
import { ImageLocationEditor } from '@/components/litterreports/image-location-editor';
import LitterReportData from '@/components/Models/LitterReportData';
import LitterImageData from '@/components/Models/LitterImageData';
import { MAX_LITTER_REPORT_NAME_LENGTH, MAX_LITTER_REPORT_DESC_LENGTH } from '@/components/Models/Constants';
import { CharacterCounter } from '@/components/ui/character-counter';
import { CreateLitterReport, UploadLitterImage, GetUserLitterReports } from '@/services/litter-report';
import { useLogin } from '@/hooks/useLogin';
import { useToast } from '@/hooks/use-toast';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import { useFeatureMetrics } from '@/hooks/useFeatureMetrics';

const formSchema = z.object({
    name: z
        .string()
        .min(1, 'Report name is required')
        .max(MAX_LITTER_REPORT_NAME_LENGTH, `Name must be less than ${MAX_LITTER_REPORT_NAME_LENGTH} characters`),
    description: z
        .string()
        .max(MAX_LITTER_REPORT_DESC_LENGTH, `Description must be less than ${MAX_LITTER_REPORT_DESC_LENGTH} characters`)
        .optional()
        .default(''),
});

type FormInputs = z.infer<typeof formSchema>;

const CreateLitterReportPageInner = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { currentUser } = useLogin();
    const { toast } = useToast();
    const { trackLitterReport } = useFeatureMetrics();

    const [images, setImages] = useState<ImageWithLocation[]>([]);
    const [editingImageId, setEditingImageId] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: '',
            description: '',
        },
    });

    const reportName = form.watch('name');
    const reportDescription = form.watch('description');

    const createMutation = useMutation({
        mutationKey: CreateLitterReport().key,
        mutationFn: CreateLitterReport().service,
    });

    const uploadMutation = useMutation({
        mutationKey: UploadLitterImage().key,
        mutationFn: UploadLitterImage().service,
    });

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

    const onSubmit = async (formValues: FormInputs) => {
        // Validate images
        if (images.length === 0) {
            toast({
                variant: 'destructive',
                title: 'No images',
                description: 'Please add at least one photo.',
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
            // Build litter report data
            const litterReport = new LitterReportData();
            litterReport.id = Guid.create().toString();
            litterReport.name = formValues.name;
            litterReport.description = formValues.description || '';
            litterReport.litterReportStatusId = 1; // New
            litterReport.createdByUserId = currentUser.id;

            // Build litter image data array
            litterReport.litterImages = images.map((img) => {
                const litterImage = new LitterImageData();
                litterImage.id = img.id;
                litterImage.litterReportId = litterReport.id;
                litterImage.latitude = img.latitude;
                litterImage.longitude = img.longitude;
                litterImage.streetAddress = img.streetAddress;
                litterImage.city = img.city;
                litterImage.region = img.region;
                litterImage.country = img.country;
                litterImage.postalCode = img.postalCode;
                litterImage.createdByUserId = currentUser.id;
                return litterImage;
            });

            // Step 1: Create the litter report
            const response = await createMutation.mutateAsync(litterReport);
            const createdReport = response.data;

            // Step 2: Upload each image file
            for (const img of images) {
                // Find the corresponding image ID from the created report
                const createdImage = createdReport.litterImages.find(
                    (ci: LitterImageData) =>
                        ci.latitude === img.latitude &&
                        ci.longitude === img.longitude &&
                        ci.streetAddress === img.streetAddress,
                );

                if (createdImage) {
                    await uploadMutation.mutateAsync({
                        litterImageId: createdImage.id,
                        file: img.file,
                    });
                }
            }

            // Track litter report creation
            trackLitterReport('Create', createdReport.id, { imageCount: images.length });

            // Invalidate queries and navigate
            await queryClient.invalidateQueries({
                queryKey: GetUserLitterReports({ userId: currentUser.id }).key,
            });

            toast({
                title: 'Report submitted',
                description: 'Your litter report has been submitted successfully.',
            });

            navigate(`/litterreports/${createdReport.id}`);
        } catch (error) {
            console.error('Failed to create litter report:', error);
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to submit the report. Please try again.',
            });
        } finally {
            setIsSubmitting(false);
        }
    };

    const editingImage = editingImageId ? images.find((img) => img.id === editingImageId) || null : null;

    return (
        <div>
            <HeroSection
                Title='Report Litter'
                Description='Help clean up your community by reporting litter locations'
            />
            <div className='container py-8'>
                <div className='mb-4'>
                    <Button variant='outline' asChild>
                        <Link to='/litterreports'>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Litter Reports
                        </Link>
                    </Button>
                </div>

                <Card className='max-w-3xl'>
                    <Form {...form}>
                        <form onSubmit={form.handleSubmit(onSubmit)}>
                            <CardHeader>
                                <CardTitle>New Litter Report</CardTitle>
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

                                <div>
                                    <FormLabel required>Photos</FormLabel>
                                    <p className='text-sm text-muted-foreground mb-3'>
                                        Add photos of the litter. Location will be automatically detected from photo
                                        metadata when available.
                                    </p>
                                    <ImageUploader
                                        images={images}
                                        onImagesChange={handleImagesChange}
                                        onEditLocation={handleEditLocation}
                                    />
                                    {images.length === 0 ? (
                                        <p className='text-sm text-destructive mt-2'>At least one photo is required.</p>
                                    ) : null}
                                </div>
                            </CardContent>
                            <CardFooter className='flex justify-end gap-2'>
                                <Button variant='outline' asChild>
                                    <Link to='/litterreports'>Cancel</Link>
                                </Button>
                                <Button type='submit' disabled={isSubmitting}>
                                    {isSubmitting ? <Loader2 className='h-4 w-4 mr-1 animate-spin' /> : null}
                                    Submit Report
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

export const CreateLitterReportPage = () => {
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
            <CreateLitterReportPageInner />
        </APIProvider>
    );
};

export default CreateLitterReportPage;
