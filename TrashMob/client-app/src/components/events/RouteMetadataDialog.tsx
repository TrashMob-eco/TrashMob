import { useCallback, useEffect } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/hooks/use-toast';
import { DisplayEventAttendeeRoute } from '@/components/Models/RouteData';
import {
    UpdateRouteMetadata,
    GetEventAttendeeRoutesByEventId,
    GetEventRoutes,
    GetEventRouteStats,
} from '@/services/event-routes';

const PRIVACY_OPTIONS = [
    { value: 'Private', label: 'Private' },
    { value: 'EventOnly', label: 'Event Only' },
    { value: 'Public', label: 'Public' },
];

const WEIGHT_UNITS = [
    { value: '1', label: 'lbs' },
    { value: '2', label: 'kg' },
];

const routeMetadataSchema = z.object({
    privacyLevel: z.string(),
    bagsCollected: z.coerce.number().int().min(0).optional().or(z.literal('')),
    weightCollected: z.coerce.number().min(0).optional().or(z.literal('')),
    weightUnitId: z.string(),
    notes: z.string().max(500).optional(),
});

type RouteMetadataFormInputs = z.infer<typeof routeMetadataSchema>;

interface RouteMetadataDialogProps {
    route: DisplayEventAttendeeRoute | null;
    open: boolean;
    onOpenChange: (open: boolean) => void;
    eventId: string;
}

export const RouteMetadataDialog = ({ route, open, onOpenChange, eventId }: RouteMetadataDialogProps) => {
    const { toast } = useToast();
    const queryClient = useQueryClient();

    const updateMutation = useMutation({
        mutationFn: async (values: RouteMetadataFormInputs) => {
            if (!route) return;
            const result = await UpdateRouteMetadata({ routeId: route.id }).service({
                privacyLevel: values.privacyLevel,
                trimStartMeters: route.trimStartMeters,
                trimEndMeters: route.trimEndMeters,
                notes: values.notes || null,
                bagsCollected:
                    values.bagsCollected !== '' && values.bagsCollected != null ? Number(values.bagsCollected) : null,
                weightCollected:
                    values.weightCollected !== '' && values.weightCollected != null
                        ? Number(values.weightCollected)
                        : null,
                weightUnitId:
                    values.weightCollected !== '' && values.weightCollected != null
                        ? Number(values.weightUnitId)
                        : null,
            });
            return result.data;
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetEventAttendeeRoutesByEventId({ eventId }).key });
            queryClient.invalidateQueries({ queryKey: GetEventRoutes({ eventId }).key });
            queryClient.invalidateQueries({ queryKey: GetEventRouteStats({ eventId }).key });
            toast({ variant: 'primary', title: 'Route updated' });
            onOpenChange(false);
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to update route. Please try again.' });
        },
    });

    const form = useForm<RouteMetadataFormInputs>({
        resolver: zodResolver(routeMetadataSchema),
        defaultValues: {
            privacyLevel: 'EventOnly',
            bagsCollected: '',
            weightCollected: '',
            weightUnitId: '1',
            notes: '',
        },
    });

    useEffect(() => {
        if (open && route) {
            form.reset({
                privacyLevel: route.privacyLevel || 'EventOnly',
                bagsCollected: route.bagsCollected ?? '',
                weightCollected: route.weightCollected ?? '',
                weightUnitId: (route.weightUnitId ?? 1).toString(),
                notes: route.notes || '',
            });
        }
    }, [open, route, form]);

    const onSubmit: SubmitHandler<RouteMetadataFormInputs> = useCallback(
        (values) => {
            updateMutation.mutate(values);
        },
        [updateMutation],
    );

    if (!route) return null;

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className='sm:max-w-[450px]' onOpenAutoFocus={(e) => e.preventDefault()}>
                <DialogHeader>
                    <DialogTitle>Edit Route</DialogTitle>
                </DialogHeader>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
                        <FormField
                            control={form.control}
                            name='privacyLevel'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Privacy</FormLabel>
                                    <Select onValueChange={field.onChange} value={field.value}>
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            {PRIVACY_OPTIONS.map((opt) => (
                                                <SelectItem key={opt.value} value={opt.value}>
                                                    {opt.label}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='bagsCollected'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Bags Collected</FormLabel>
                                    <FormControl>
                                        <Input {...field} type='number' min={0} step={1} placeholder='0' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <div className='flex gap-3'>
                            <FormField
                                control={form.control}
                                name='weightCollected'
                                render={({ field }) => (
                                    <FormItem className='flex-1'>
                                        <FormLabel>Weight Collected</FormLabel>
                                        <FormControl>
                                            <Input {...field} type='number' min={0} step={0.1} placeholder='0' />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name='weightUnitId'
                                render={({ field }) => (
                                    <FormItem className='w-24'>
                                        <FormLabel>Unit</FormLabel>
                                        <Select onValueChange={field.onChange} value={field.value}>
                                            <FormControl>
                                                <SelectTrigger>
                                                    <SelectValue />
                                                </SelectTrigger>
                                            </FormControl>
                                            <SelectContent>
                                                {WEIGHT_UNITS.map((u) => (
                                                    <SelectItem key={u.value} value={u.value}>
                                                        {u.label}
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        </div>
                        <FormField
                            control={form.control}
                            name='notes'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Notes</FormLabel>
                                    <FormControl>
                                        <Textarea
                                            {...field}
                                            placeholder='Optional notes about this route...'
                                            rows={3}
                                        />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <div className='flex justify-end gap-2'>
                            <Button type='button' variant='secondary' onClick={() => onOpenChange(false)}>
                                Cancel
                            </Button>
                            <Button type='submit' disabled={updateMutation.isPending}>
                                {updateMutation.isPending ? <Loader2 className='animate-spin' /> : null}
                                Save
                            </Button>
                        </div>
                    </form>
                </Form>
            </DialogContent>
        </Dialog>
    );
};
