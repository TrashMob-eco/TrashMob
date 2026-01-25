import { Guid } from 'guid-typescript';
import { PhotoPicker } from "./components/PhotoPicker"
import { Tabs, TabsContent } from '@/components/ui/tabs';
import { zodResolver } from '@hookform/resolvers/zod';
import { useFieldArray, useForm, useWatch } from 'react-hook-form';
import { number, z } from 'zod';
import moment from 'moment';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { useCallback, useEffect, useState } from "react";
import { GoogleMap } from '@/components/Map/GoogleMap';
import { APIProvider, MapMouseEvent, AdvancedMarker, useMap, Marker } from '@vis.gl/react-google-maps';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import {
    AzureSearchLocationInputWithKey as AzureSearchLocationInput,
    SearchLocationOption,
} from '@/components/Map/AzureSearchLocationInput';
import { Textarea } from "@/components/ui/textarea";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Header } from "./components/Header";
import { ChevronLeft } from "lucide-react";
import * as exifr from "exifr";
import { TrashMarker } from "./components/TrashMarker";
import LitterReportData from "@/components/Models/LitterReportData";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { CreateLitterReport, GetLitterReports, UploadLitterImage } from "@/services/litter-report";
import { useToast } from "@/hooks/use-toast";
import { useNavigate } from "react-router";
import { useLogin } from "@/hooks/useLogin";
import LitterImageData from "@/components/Models/LitterImageData";

const photoSchema = z.object({
  file: z.instanceof(File),
  previewUrl: z.string(),
  gps: z.object({
    latitude: z.number(),
    longitude: z.number()
  }).optional()
})

const createLitterSchema = z.object({
    title: z.string(),
    description: z.string(),
    photos: z.array(photoSchema).min(1),
    location: z.object({
      latitude: z.number(),
      longitude: z.number(),
      address: z.string()
    })
});

export const CreateLitterPage = () => {

  const { toast } = useToast();
  const { currentUser } = useLogin()
  // const queryClient = useQueryClient();
  // const navigate = useNavigate();

  const steps = [
    { key: 'photos', label: 'Photos' },
    { key: 'location', label: 'Location' },
    { key: 'detail', label: 'Edit Detail' },
  ];
  const [stepIndex, setStepIndex] = useState<number>(0);

  const { data: litterReports } = useQuery({
    queryKey: GetLitterReports().key,
    queryFn: GetLitterReports().service
  })
  console.log({ litterReports })

  const createLitterReport = useMutation({
      mutationKey: CreateLitterReport().key,
      mutationFn: CreateLitterReport().service,
      onSuccess: (data, variable) => {
          toast({
              duration: 10000,
              variant: 'primary',
              title: `LitterReport ${variable.name} submitted!`,
              description: `${moment(variable.createdDate).format('dddd, MMMM Do YYYY [at] h:mm a')}`,
          });
          
      },
  });

  const uploadLitterImage = useMutation({
      mutationKey: UploadLitterImage().key,
      mutationFn: UploadLitterImage().service,
      // onSuccess: (data, variable) => {
      //     toast({
      //         duration: 10000,
      //         variant: 'primary',
      //         title: `LitterReport ${variable.name} submitted!`,
      //         description: `${moment(variable.createdDate).format('dddd, MMMM Do YYYY [at] h:mm a')}`,
      //     }); 
      // },
  });


  
  const backToPreviousStep = useCallback(() => {
    const previousStepIndex = stepIndex - 1
    setStepIndex(previousStepIndex)
  }, [setStepIndex, stepIndex])

  const goToNextStep = useCallback(() => {
    const nextStepIndex = stepIndex + 1
    setStepIndex(nextStepIndex)
  }, [setStepIndex, stepIndex])

  const step = steps[stepIndex]

  const form = useForm<z.infer<typeof createLitterSchema>>({
      resolver: zodResolver(createLitterSchema),
      defaultValues: {
        location: {
          latitude: 13.731037815756007,
          longitude: 100.54156989301707,
          address: 'Lumphini Park'
        },
        photos: []
        // photos: [
        //   { previewUrl: 'https://www.mordeo.org/files/uploads/2018/09/Cute-Dream-Asian-Girl-4K-Ultra-HD-Mobile-Wallpaper-950x1689.jpg' },
        //   { previewUrl: 'https://img.goodfon.com/wallpaper/big/f/bf/photo-asian-girl-xin-xin-devushka-aziatka-milaia-aziatka.webp' },
        //   { previewUrl: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRpl8EdlzN7huHg-HMW-xVB3mpZ1tvvKNobgg&s' }
        // ]
      },
  });

  const {
    fields: photos,
    append,
  } = useFieldArray({
    control: form.control,
    name: "photos",
  });

  const watchedLocation = useWatch({ control: form.control, name: 'location' })

  function onSubmit(formValues: z.infer<typeof createLitterSchema>) {
    // 1. Create Litter Report
    console.log('onSubmit', formValues)
    // const reportBody = new LitterReportData();
    // reportBody.name = formValues.title;
    // reportBody.description = formValues.description;
    // reportBody.createdDate = new Date();
    // reportBody.lastUpdatedDate = new Date();
    // reportBody.litterReportStatusId = 1
    // reportBody.createdByUserId = currentUser.id
    // reportBody.lastUpdatedByUserId = currentUser.id
    // reportBody.litterImages = formValues.photos.map(photo => {
    //   const litterImageBody = new LitterImageData()
    //   litterImageBody.id = Guid.create().toString();
    //   litterImageBody.azureBlobURL = '';
    //   litterImageBody.streetAddress = '';
    //   litterImageBody.city = '';
    //   litterImageBody.region = '';
    //   litterImageBody.country = '';
    //   litterImageBody.postalCode = '';
    //   litterImageBody.latitude = formValues.location.latitude
    //   litterImageBody.longitude = formValues.location.longitude
    //   return litterImageBody
    // })
    // createLitterReport.mutate(reportBody);

    // litter report id
    //  "a2d4239b-ead1-4cf7-bb95-b9268f641a0e"

    // Image id
    // "2832e89e-5f32-a2fb-328e-3c5f6c898ad7"

    uploadLitterImage.mutateAsync({
      // litterReportId: "a2d4239b-ead1-4cf7-bb95-b9268f641a0e",
      id: "2832e89e-5f32-a2fb-328e-3c5f6c898ad7",
      file: formValues.photos[0].file
    })

    // 2. Create Litter Report Images
  }
  
  const onPhotoAdded = async (files: FileList) => {
    for (const file of Array.from(files)) {

      const gps = await exifr.gps(file)
      const previewUrl = URL.createObjectURL(file)
      append({ file, previewUrl, gps })
    }
  }

  /** Find default location from photos' gps */
  useEffect(() => {
    const gpsList = (photos || []).map(({ gps }) => gps).filter(Boolean)
    const len = gpsList.length
    if (len > 0) {
      const sumLatitude = gpsList.reduce((sum, gps) => sum + gps.latitude, 0)
      const sumLongitude = gpsList.reduce((sum, gps) => sum + gps.longitude, 0)
      const location = {
        latitude: sumLatitude / len,
        longitude: sumLongitude / len,
        address: ''
      }
      form.setValue('location', location)
    }
  }, [photos])

  const map = useMap('locationPicker');
  
  const handleSelectSearchLocation = useCallback(
      (location: SearchLocationOption) => {
          const { lat, lon } = location.position;
          form.setValue('location.latitude', lat);
          form.setValue('location.longitude', lon);
      },
      [map],
  );

  const handleClickMap = useCallback((e: MapMouseEvent) => {
      if (e.detail.latLng) {
          const lat = e.detail.latLng.lat;
          const lng = e.detail.latLng.lng;
          form.setValue('location.latitude', lat);
          form.setValue('location.longitude', lng);
      }
  }, []);

  const handleMarkerDragEnd = useCallback(async (e: google.maps.MapMouseEvent) => {
      if (e.latLng) {
          const lat = e.latLng.lat();
          const lng = e.latLng.lng();
          form.setValue('location.latitude', lat);
          form.setValue('location.longitude', lng);
      }
  }, []);      

  return (
    <div className="fixed inset-0">
      {/** Header */}
      {/* <Header
        leftButton={stepIndex > 0 
          ? <Button variant='ghost' size="icon" onClick={backToPreviousStep}><ChevronLeft /></Button> 
          : null
        }
      /> */}
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-2'>

          {/** Footer */}
          <div className="fixed w-full bottom-0 left-0 right-0 p-3 bg-card">
            <div className="flex justify-end gap-4">
              {stepIndex === steps.length - 1 
                ? <Button className="w-full" type="submit" disabled={createLitterReport.isLoading}>Post</Button> 
                : <Button className="w-full" onClick={goToNextStep}>Next</Button>
              }
            </div>
          </div>

          <div className="mt-[80px] h-[calc(100dvh-140px)] overflow-scroll">
            <Tabs value={step.key}>
                <TabsContent value='photos'>
                  <h2 className="mt-0 p-4">Add litter photos</h2>
                  <div className="p-4 pt-0 flex flex-col gap-4">
                    <PhotoPicker 
                      onChange={onPhotoAdded}
                    />
                    {photos.map((p, index) => (
                      <img key={`photo-${index}`} src={p.previewUrl} alt={`Litter ${index + 1}`} />
                    ))}
                  </div>
                </TabsContent>
                <TabsContent value='location'>
                  <div>
                    <h2 className="mt-0 p-4">Where is the litter location</h2>
                    <FormItem>
                      <div className='relative w-full select-none touch-none'>
                        <GoogleMap
                            id='locationPicker'
                            defaultCenter={{ lat: watchedLocation.latitude, lng: watchedLocation.longitude }}
                            defaultZoom={16}
                            style={{ width: '100%', height: '500px' }}
                            // onClick={handleClickMap}
                        >
                          {/**  Trash markers */}
                          {photos
                            .filter(photo => !!photo.gps)
                            .map(photo => (
                              <TrashMarker {...photo} key={photo.previewUrl} />
                          ))}
                          <AdvancedMarker
                            position={{ lat: watchedLocation.latitude, lng: watchedLocation.longitude }}
                            draggable
                            onDragEnd={handleMarkerDragEnd}
                          />
                        </GoogleMap>
                        <div className="absolute top-8 left-8 right-8">
                          <AzureSearchLocationInput
                            className='w-full'
                            placeholder='Search for address'
                            onSelectLocation={handleSelectSearchLocation}
                          />
                        </div>
                      </div>
                      <FormDescription className="p-4">
                          Drag marker or click on map to move marker to precise location.
                      </FormDescription>
                    </FormItem>
                  </div>
                </TabsContent>
                <TabsContent value='detail'>
                  <h2 className="mt-0 p-4">Create new litter report</h2>
                  <div className='flex gap-4 overflow-x-scroll px-4'>
                    {photos.map((p, index) => (
                      <img
                        key={`photo-${index}`}
                        src={p.previewUrl}
                        alt={`Litter ${index + 1}`}
                        className='w-auto h-48 rounded-lg mt-1 mb-4'
                      />
                    ))}
                  </div>
                  <div className="bg-card py-4">
                    <div className='grid gap-4 grid-cols-12'>
                      <FormField
                        control={form.control}
                        name='location.address'
                        render={({ field }) => (
                            <FormItem className='col-span-12 px-4'>
                                <FormLabel>Location</FormLabel>
                                <FormControl>
                                    <Input
                                      {...field}
                                      readOnly
                                      disabled
                                    />
                                </FormControl>
                            </FormItem>
                        )}
                      />
                      <FormField
                        control={form.control}
                        name='title'
                        render={({ field }) => (
                            <FormItem className='col-span-12 px-4'>
                                <FormLabel>Title</FormLabel>
                                <FormControl>
                                    <Input
                                        placeholder='Type your message here.'
                                        autoFocus={false}
                                        {...field}
                                    />
                                </FormControl>
                            </FormItem>
                        )}
                      />
                      <FormField
                        control={form.control}
                        name='description'
                        render={({ field }) => (
                            <FormItem className='col-span-12 px-4'>
                                <FormLabel>Description</FormLabel>
                                <FormControl>
                                    <Textarea
                                        placeholder='Type your message here.'
                                        autoFocus={false}
                                        className='resize-none h-48'
                                        {...field}
                                    />
                                </FormControl>
                            </FormItem>
                        )}
                      />
                    </div>
                  </div>
                </TabsContent>
            </Tabs>
          </div>
        </form>
      </Form>
    </div>
  )
}

export const CreateLitterWithMapKey = () => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();

    if (isLoading) return null;

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <CreateLitterPage />
        </APIProvider>
    );
};
