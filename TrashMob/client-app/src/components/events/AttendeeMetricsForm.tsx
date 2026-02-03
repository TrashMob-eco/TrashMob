import { FC, useEffect } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { Loader2, AlertCircle, CheckCircle2, XCircle, Clock } from 'lucide-react';

import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { Select, SelectItem, SelectContent, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useToast } from '@/hooks/use-toast';
import { useLogin } from '@/hooks/useLogin';
import { GetMyMetrics, SubmitMyMetrics } from '@/services/event-attendee-metrics';
import { GetWeightUnits } from '@/services/weight-units';
import { MetricsStatus } from '@/components/Models/EventAttendeeMetricsData';

const attendeeMetricsSchema = z.object({
    bagsCollected: z.number().min(0).optional(),
    pickedWeight: z.number().min(0).optional(),
    pickedWeightUnitId: z.number().optional(),
    durationMinutes: z.number().min(0).optional(),
    notes: z.string().max(2048).optional(),
});

type AttendeeMetricsFormValues = z.infer<typeof attendeeMetricsSchema>;

interface AttendeeMetricsFormProps {
    eventId: string;
    isEventCompleted: boolean;
    isAttending: boolean;
}

const statusConfig: Record<
    MetricsStatus,
    { icon: FC<{ className?: string }>; variant: 'default' | 'secondary' | 'destructive' | 'outline'; label: string }
> = {
    Pending: { icon: Clock, variant: 'secondary', label: 'Pending Review' },
    Approved: { icon: CheckCircle2, variant: 'default', label: 'Approved' },
    Rejected: { icon: XCircle, variant: 'destructive', label: 'Rejected' },
    Adjusted: { icon: CheckCircle2, variant: 'default', label: 'Approved (Adjusted)' },
};

export const AttendeeMetricsForm: FC<AttendeeMetricsFormProps> = ({ eventId, isEventCompleted, isAttending }) => {
    const { currentUser } = useLogin();
    const { toast } = useToast();
    const queryClient = useQueryClient();

    // Fetch existing metrics submission
    const {
        data: existingMetrics,
        isLoading: isLoadingMetrics,
        error: metricsError,
    } = useQuery({
        queryKey: GetMyMetrics({ eventId }).key,
        queryFn: GetMyMetrics({ eventId }).service,
        select: (res) => res.data,
        enabled: isEventCompleted && isAttending,
        retry: (failureCount, error) => {
            // Don't retry on 404 (no submission yet)
            if ((error as AxiosError)?.response?.status === 404) return false;
            return failureCount < 3;
        },
    });

    // Fetch weight units
    const { data: weightUnits } = useQuery({
        queryKey: GetWeightUnits().key,
        queryFn: GetWeightUnits().service,
        select: (res) => res.data,
    });

    // Submit mutation
    const submitMetrics = useMutation({
        mutationKey: SubmitMyMetrics().key,
        mutationFn: (values: AttendeeMetricsFormValues) => SubmitMyMetrics().service({ eventId }, values),
        onSuccess: () => {
            toast({
                variant: 'primary',
                title: 'Metrics submitted',
                description: 'Your metrics have been submitted for review.',
            });
            queryClient.invalidateQueries({ queryKey: GetMyMetrics({ eventId }).key });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to submit metrics. Please try again.',
            });
        },
    });

    // Determine default weight unit based on user preference (Pound=1, Kilogram=2)
    const defaultWeightUnitId = currentUser.prefersMetric ? 2 : 1;

    const form = useForm<AttendeeMetricsFormValues>({
        resolver: zodResolver(attendeeMetricsSchema),
        defaultValues: {
            bagsCollected: 0,
            pickedWeight: 0,
            pickedWeightUnitId: defaultWeightUnitId,
            durationMinutes: 0,
            notes: '',
        },
    });

    // Reset form when existing metrics load
    useEffect(() => {
        if (existingMetrics) {
            form.reset({
                bagsCollected: existingMetrics.bagsCollected ?? 0,
                pickedWeight: existingMetrics.pickedWeight ?? 0,
                pickedWeightUnitId: existingMetrics.pickedWeightUnitId ?? defaultWeightUnitId,
                durationMinutes: existingMetrics.durationMinutes ?? 0,
                notes: existingMetrics.notes ?? '',
            });
        }
    }, [existingMetrics, defaultWeightUnitId, form]);

    const onSubmit = (values: AttendeeMetricsFormValues) => {
        submitMetrics.mutate(values);
    };

    // Don't show if event is not completed or user is not attending
    if (!isEventCompleted || !isAttending) {
        return null;
    }

    if (isLoadingMetrics) {
        return (
            <Card>
                <CardContent className='py-8 text-center'>
                    <Loader2 className='h-8 w-8 animate-spin mx-auto' />
                </CardContent>
            </Card>
        );
    }

    const hasSubmission = existingMetrics && !((metricsError as AxiosError)?.response?.status === 404);
    const status = existingMetrics?.status as MetricsStatus | undefined;
    const isEditable = !status || status === 'Pending' || status === 'Rejected';
    const statusInfo = status ? statusConfig[status] : null;

    return (
        <Card>
            <CardHeader>
                <div className='flex items-center justify-between'>
                    <div>
                        <CardTitle>Your Event Metrics</CardTitle>
                        <CardDescription>
                            {hasSubmission
                                ? 'View or update your metrics submission for this event.'
                                : 'Submit your personal metrics from this event for the lead to review.'}
                        </CardDescription>
                    </div>
                    {statusInfo ? (
                        <Badge variant={statusInfo.variant} className='flex items-center gap-1'>
                            <statusInfo.icon className='h-3 w-3' />
                            {statusInfo.label}
                        </Badge>
                    ) : null}
                </div>
            </CardHeader>
            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)}>
                    <CardContent className='space-y-4'>
                        {status === 'Rejected' && existingMetrics?.rejectionReason ? (
                            <Alert variant='destructive'>
                                <AlertCircle className='h-4 w-4' />
                                <AlertTitle>Submission Rejected</AlertTitle>
                                <AlertDescription>{existingMetrics.rejectionReason}</AlertDescription>
                            </Alert>
                        ) : null}

                        {status === 'Adjusted' && existingMetrics?.adjustmentReason ? (
                            <Alert>
                                <AlertCircle className='h-4 w-4' />
                                <AlertTitle>Values Adjusted by Lead</AlertTitle>
                                <AlertDescription>
                                    {existingMetrics.adjustmentReason}
                                    <div className='mt-2 text-sm'>
                                        Adjusted values:{' '}
                                        {existingMetrics.adjustedBagsCollected ?? existingMetrics.bagsCollected} bags,{' '}
                                        {existingMetrics.adjustedPickedWeight ?? existingMetrics.pickedWeight}{' '}
                                        {weightUnits?.find(
                                            (u) =>
                                                u.id ===
                                                (existingMetrics.adjustedPickedWeightUnitId ??
                                                    existingMetrics.pickedWeightUnitId),
                                        )?.name || 'lbs'}
                                        , {existingMetrics.adjustedDurationMinutes ?? existingMetrics.durationMinutes}{' '}
                                        minutes
                                    </div>
                                </AlertDescription>
                            </Alert>
                        ) : null}

                        <div className='grid gap-4 grid-cols-12'>
                            <FormField
                                control={form.control}
                                name='bagsCollected'
                                render={({ field }) => (
                                    <FormItem className='col-span-12 sm:col-span-6'>
                                        <FormLabel>Bags Collected</FormLabel>
                                        <FormControl>
                                            <Input
                                                {...field}
                                                type='number'
                                                min={0}
                                                disabled={!isEditable}
                                                onChange={(e) => field.onChange(e.target.valueAsNumber || 0)}
                                            />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />

                            <FormField
                                control={form.control}
                                name='durationMinutes'
                                render={({ field }) => (
                                    <FormItem className='col-span-12 sm:col-span-6'>
                                        <FormLabel>Duration (minutes)</FormLabel>
                                        <FormControl>
                                            <Input
                                                {...field}
                                                type='number'
                                                min={0}
                                                disabled={!isEditable}
                                                onChange={(e) => field.onChange(e.target.valueAsNumber || 0)}
                                            />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />

                            <FormField
                                control={form.control}
                                name='pickedWeight'
                                render={({ field }) => (
                                    <FormItem className='col-span-12 sm:col-span-6'>
                                        <FormLabel>Weight Collected</FormLabel>
                                        <FormControl>
                                            <Input
                                                {...field}
                                                type='number'
                                                min={0}
                                                step={0.1}
                                                disabled={!isEditable}
                                                onChange={(e) => field.onChange(e.target.valueAsNumber || 0)}
                                            />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />

                            <FormField
                                control={form.control}
                                name='pickedWeightUnitId'
                                render={({ field }) => (
                                    <FormItem className='col-span-12 sm:col-span-6'>
                                        <FormLabel>Weight Unit</FormLabel>
                                        <FormControl>
                                            <Select
                                                value={`${field.value}`}
                                                onValueChange={(val) => field.onChange(Number(val))}
                                                disabled={!isEditable}
                                            >
                                                <SelectTrigger>
                                                    <SelectValue placeholder='Weight Unit' />
                                                </SelectTrigger>
                                                <SelectContent>
                                                    {(weightUnits || []).map((unit) => (
                                                        <SelectItem key={unit.id} value={`${unit.id}`}>
                                                            {unit.name}
                                                        </SelectItem>
                                                    ))}
                                                </SelectContent>
                                            </Select>
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
                                        <FormLabel>Notes (optional)</FormLabel>
                                        <FormControl>
                                            <Textarea
                                                {...field}
                                                disabled={!isEditable}
                                                rows={3}
                                                placeholder='Any additional notes about your contribution...'
                                            />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        </div>
                    </CardContent>
                    <CardFooter className='justify-end'>
                        {isEditable ? (
                            <Button type='submit' disabled={submitMetrics.isPending}>
                                {submitMetrics.isPending ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                                {hasSubmission ? 'Update Metrics' : 'Submit Metrics'}
                            </Button>
                        ) : null}
                    </CardFooter>
                </form>
            </Form>
        </Card>
    );
};

export default AttendeeMetricsForm;
