import moment from 'moment';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { Guid } from 'guid-typescript';
import * as ToolTips from '@/store/ToolTips';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Input } from '@/components/ui/input';
import { useGetEvent } from '@/hooks/useGetEvent';
import { Link, Outlet, useMatch, useNavigate, useParams } from 'react-router';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { useEffect, useState } from 'react';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import {
    DeleteEventPickupLocationById,
    GetEventPickupLocations,
    GetHaulingPartnerLocation,
    SubmitEventPickupLocations,
} from '@/services/locations';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Badge } from '@/components/ui/badge';
import { Ellipsis, Loader2, Pencil, Plus, SquareX } from 'lucide-react';
import { Label } from '@/components/ui/label';
import { useToast } from '@/hooks/use-toast';
import EventSummaryData from '@/components/Models/EventSummaryData';
import { useLogin } from '@/hooks/useLogin';
import { CreateEventSummary, GetEventSummaryById, UpdateEventSummary } from '@/services/events';

const upsertEventSummarySchema = z.object({
    actualNumberOfAttendees: z.number(),
    durationInMinutes: z.number(),
    numberOfBags: z.number(),
    numberOfBuckets: z.number(),
    notes: z.string(),
    createdByUserId: z.string(),
});

export const EditEventSummary = () => {
    const { eventId } = useParams<{ eventId: string }>() as { eventId: string };
    const { currentUser } = useLogin();
    const navigate = useNavigate();
    const { toast } = useToast();
    const { data: event } = useGetEvent(eventId);

    const { data: eventSummary } = useQuery({
        queryKey: GetEventSummaryById({ eventId }).key,
        queryFn: GetEventSummaryById({ eventId }).service,
        select: (res) => res.data,
    });

    const createEventSummary = useMutation({
        mutationKey: CreateEventSummary().key,
        mutationFn: CreateEventSummary().service,
        onSuccess: () => {
            toast({
                variant: 'primary',
                title: 'Event summary created',
            });
        },
    });

    const updateEventSummary = useMutation({
        mutationKey: UpdateEventSummary().key,
        mutationFn: UpdateEventSummary().service,
        onSuccess: () => {
            toast({
                variant: 'primary',
                title: 'Event summary updated',
            });
        },
    });

    /** Pickup Locations */
    const queryClient = useQueryClient();
    const isCreate = useMatch(`/eventsummary/:eventId/pickup-locations/create`);
    const isEdit = useMatch(`/eventsummary/:eventId/pickup-locations/:locationId/edit`);

    const { data: pickupLocations } = useQuery({
        queryKey: GetEventPickupLocations({ eventId }).key,
        queryFn: GetEventPickupLocations({ eventId }).service,
        select: (res) => res.data,
    });

    const deleteEventPickupLocationById = useMutation({
        mutationKey: DeleteEventPickupLocationById().key,
        mutationFn: DeleteEventPickupLocationById().service,
        onSuccess: async () => {
            toast({
                variant: 'primary',
                title: 'Pickup location deleted',
            });
            await queryClient.invalidateQueries({
                queryKey: GetEventPickupLocations({ eventId }).key,
                refetchType: 'all',
            });
        },
        onError: async (error: Error) => {
            toast({
                variant: 'destructive',
                title: 'Pickup location delete fail',
                description: error.message,
            });
        },
    });

    const submitEventPickupLocations = useMutation({
        mutationKey: SubmitEventPickupLocations().key,
        mutationFn: SubmitEventPickupLocations().service,
        onSuccess: async () => {
            toast({
                variant: 'primary',
                title: 'Pickup locations submitted',
            });
            await queryClient.invalidateQueries({
                queryKey: GetEventPickupLocations({ eventId }).key,
                refetchType: 'all',
            });
        },
    });

    const [isDeletingId, setIsDeletingId] = useState<string | null>(null);
    function removePickupLocation(locationId: string, name: string) {
        if (!window.confirm(`Please confirm that you want to remove pickup location: '${name}' from this event ?`))
            return;
        setIsDeletingId(locationId);

        deleteEventPickupLocationById.mutateAsync({ locationId }).finally(() => {
            setIsDeletingId(null);
        });
    }
    /** End of Pickup Locations */

    /** Hauling Partners */
    const { data: haulingPartnerLocation } = useQuery({
        queryKey: GetHaulingPartnerLocation({ eventId }).key,
        queryFn: GetHaulingPartnerLocation({ eventId }).service,
        select: (res) => res.data,
    });

    /** End of Hauling Partners */

    const form = useForm<z.infer<typeof upsertEventSummarySchema>>({
        resolver: zodResolver(upsertEventSummarySchema),
        defaultValues: {},
    });

    useEffect(() => {
        if (event) {
            form.reset({
                actualNumberOfAttendees: eventSummary?.actualNumberOfAttendees ?? event.maxNumberOfParticipants,
                durationInMinutes: eventSummary?.durationInMinutes ?? event.durationHours * 60 + event.durationMinutes,
                numberOfBags: eventSummary?.numberOfBags ?? 0,
                numberOfBuckets: eventSummary?.numberOfBuckets ?? 0,
                notes: eventSummary?.notes ?? '',
                createdByUserId: eventSummary?.createdByUserId ?? Guid.EMPTY,
            });
        }
    }, [eventSummary, event]);

    function onSubmit(formValues: z.infer<typeof upsertEventSummarySchema>) {
        const body = new EventSummaryData();
        body.eventId = eventId;
        body.actualNumberOfAttendees = formValues.actualNumberOfAttendees;
        body.numberOfBags = formValues.numberOfBags;
        body.numberOfBuckets = formValues.numberOfBuckets;
        body.durationInMinutes = formValues.durationInMinutes;
        body.notes = formValues.notes ?? '';
        body.createdByUserId = currentUser.id;
        body.createdDate = new Date();

        if (formValues.createdByUserId !== Guid.EMPTY) {
            updateEventSummary.mutateAsync(body);
        } else {
            body.createdByUserId = currentUser.id;
            createEventSummary.mutateAsync(body);
        }
    }

    const submitPickupLocations = () => {
        if (
            !window.confirm(
                'Please confirm that you want to submit these pickup locations? Once submitted, they cannot be updated.',
            )
        )
            return;

        submitEventPickupLocations.mutate({ eventId });
    };

    if (!event) return null;

    const isSubmitting = createEventSummary.isLoading || updateEventSummary.isLoading;
    return (
        <div className='tailwind'>
            <div className='max-w-2xl mx-auto py-8 space-y-8'>
                <Card>
                    <CardHeader>
                        <CardTitle>Enter Event Summary Information</CardTitle>
                        <CardDescription>Please enter information about how the event went.</CardDescription>
                    </CardHeader>
                    <Form {...form}>
                        <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-2'>
                            <CardContent>
                                <div className='grid grid-cols-2'>
                                    <div className='col-span-1'>
                                        <Label>Event Name</Label>
                                        <p className='text-muted text-sm'>{event.name}</p>
                                    </div>
                                    <div className='col-span-1'>
                                        <Label>Event Date</Label>
                                        <p className='text-muted text-sm'>{moment(event?.eventDate).format('LL')}</p>
                                    </div>
                                </div>
                                <div className='grid gap-4 grid-cols-12'>
                                    <FormField
                                        control={form.control}
                                        name='actualNumberOfAttendees'
                                        render={({ field }) => (
                                            <FormItem className='col-span-12 sm:col-span-6'>
                                                <FormLabel
                                                    required
                                                    tooltip={ToolTips.EventSummaryActualNumberOfAttendees}
                                                >
                                                    Actual Number of Attendees
                                                </FormLabel>
                                                <FormControl>
                                                    <Input
                                                        {...field}
                                                        type='number'
                                                        onChange={(e) => field.onChange(e.target.valueAsNumber)}
                                                    />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='durationInMinutes'
                                        render={({ field }) => (
                                            <FormItem className='col-span-12 sm:col-span-6'>
                                                <FormLabel required tooltip={ToolTips.EventSummaryDurationInMinutes}>
                                                    Actual duration in minutes
                                                </FormLabel>
                                                <FormControl>
                                                    <Input
                                                        {...field}
                                                        type='number'
                                                        onChange={(e) => field.onChange(e.target.valueAsNumber)}
                                                    />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='numberOfBags'
                                        render={({ field }) => (
                                            <FormItem className='col-span-12 sm:col-span-6'>
                                                <FormLabel tooltip={ToolTips.EventSummaryNumberOfBags}>
                                                    Number of Bags
                                                </FormLabel>
                                                <FormControl>
                                                    <Input
                                                        {...field}
                                                        type='number'
                                                        onChange={(e) => field.onChange(e.target.valueAsNumber)}
                                                    />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='numberOfBuckets'
                                        render={({ field }) => (
                                            <FormItem className='col-span-12 sm:col-span-6'>
                                                <FormLabel tooltip={ToolTips.EventSummaryNumberOfBuckets}>
                                                    Number of Buckets
                                                </FormLabel>
                                                <FormControl>
                                                    <Input
                                                        {...field}
                                                        type='number'
                                                        onChange={(e) => field.onChange(e.target.valueAsNumber)}
                                                    />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='notes'
                                        render={({ field }) => (
                                            <FormItem className='col-span-12'>
                                                <FormLabel tooltip={ToolTips.EventSummaryNotes}>Notes</FormLabel>
                                                <FormControl>
                                                    <Textarea {...field} rows={5} />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                </div>
                            </CardContent>
                            <CardFooter className='justify-between'>
                                <Button variant='outline'>Cancel</Button>
                                <Button type='submit' disabled={isSubmitting}>
                                    {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                                    Save
                                </Button>
                            </CardFooter>
                        </form>
                    </Form>
                </Card>
                <Card>
                    <CardHeader>
                        <CardTitle>Pickup Locations</CardTitle>
                        <CardDescription>
                            If you have garbage that needs to be hauled, and have previously requested help from a
                            partner with hauling, enter the locations as accurately as possible of the piles you need
                            hauled. Leave additional notes as needed to help the partner locate the trash. You can add
                            as many locations as needed, but the request will not be sent until you have saved the
                            entries and then hit the Submit button!
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Name & Location</TableHead>
                                    <TableHead>Notes</TableHead>
                                    <TableHead>Status</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {(pickupLocations || []).map((loc) => {
                                    return (
                                        <TableRow className={isDeletingId === loc.id ? 'opacity-20' : ''} key={loc.id}>
                                            <TableCell>
                                                <div>{loc.name}</div>
                                                <div className='text-sm text-muted-foreground'>
                                                    {loc.streetAddress} {loc.city}
                                                </div>
                                            </TableCell>
                                            <TableCell>{loc.notes}</TableCell>
                                            <TableCell>
                                                {loc.hasBeenPickedUp ? (
                                                    <Badge variant='success'>Picked Up</Badge>
                                                ) : loc.hasBeenSubmitted ? (
                                                    <Badge variant='secondary'>Submitted</Badge>
                                                ) : (
                                                    <Badge>Not submitted</Badge>
                                                )}
                                            </TableCell>
                                            <TableCell>
                                                <DropdownMenu>
                                                    <DropdownMenuTrigger asChild>
                                                        <Button variant='ghost' size='icon'>
                                                            <Ellipsis />
                                                        </Button>
                                                    </DropdownMenuTrigger>
                                                    <DropdownMenuContent className='w-56'>
                                                        <DropdownMenuItem asChild>
                                                            <Link to={`pickup-locations/${loc.id}/edit`}>
                                                                <Pencil />
                                                                Edit Pickup Location
                                                            </Link>
                                                        </DropdownMenuItem>
                                                        {true || loc.hasBeenSubmitted ? (
                                                            <DropdownMenuItem
                                                                onClick={() => removePickupLocation(loc.id, loc.name)}
                                                            >
                                                                <SquareX />
                                                                Remove Pickup Location
                                                            </DropdownMenuItem>
                                                        ) : null}
                                                    </DropdownMenuContent>
                                                </DropdownMenu>
                                            </TableCell>
                                        </TableRow>
                                    );
                                })}
                                <TableRow>
                                    <TableCell colSpan={4}>
                                        <div className='flex justify-between gap-2'>
                                            <Button variant='ghost' asChild>
                                                <Link to='pickup-locations/create'>
                                                    <Plus /> Add Pickup Location
                                                </Link>
                                            </Button>
                                            <Button
                                                variant='default'
                                                disabled={submitEventPickupLocations.isLoading}
                                                onClick={submitPickupLocations}
                                            >
                                                {submitEventPickupLocations.isLoading ? (
                                                    <Loader2 className='animate-spin' />
                                                ) : null}
                                                Submit locations (
                                                {(pickupLocations || []).filter((loc) => !loc.hasBeenSubmitted).length})
                                            </Button>
                                        </div>
                                    </TableCell>
                                </TableRow>
                            </TableBody>
                        </Table>
                    </CardContent>
                    <Dialog open={!!isCreate} onOpenChange={() => navigate(`/eventsummary/${eventId}`)}>
                        <DialogContent className='sm:max-w-[680px]' onOpenAutoFocus={(e) => e.preventDefault()}>
                            <DialogHeader>
                                <DialogTitle>Create Pickup Location</DialogTitle>
                            </DialogHeader>
                            <div>
                                <Outlet />
                            </div>
                        </DialogContent>
                    </Dialog>
                    <Dialog open={!!isEdit} onOpenChange={() => navigate(`/eventsummary/${eventId}`)}>
                        <DialogContent className='sm:max-w-[680px]' onOpenAutoFocus={(e) => e.preventDefault()}>
                            <DialogHeader>
                                <DialogTitle>Edit Pickup Location</DialogTitle>
                            </DialogHeader>
                            <div>
                                <Outlet />
                            </div>
                        </DialogContent>
                    </Dialog>
                </Card>
                <Card>
                    <CardHeader>
                        <CardTitle>Hauling Partner Contacts</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Name</TableHead>
                                    <TableHead>Email</TableHead>
                                    <TableHead>Phone</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {(haulingPartnerLocation?.partnerLocationContacts || []).map((contact) => {
                                    return (
                                        <TableRow key={contact.id}>
                                            <TableCell>{contact.name}</TableCell>
                                            <TableCell>{contact.email}</TableCell>
                                            <TableCell>{contact.phone}</TableCell>
                                        </TableRow>
                                    );
                                })}
                            </TableBody>
                        </Table>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
};
