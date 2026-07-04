import { useEffect, useRef, useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Check, Loader2 } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/hooks/use-toast';
import { getErrorMessage } from '@/lib/api-errors';

/**
 * Autosave-on-blur narrative panel used by both the Weekly and Monthly report
 * screens (Project 63 Phase 4a). The parent supplies the current on-disk value
 * plus a mutation hook that persists it; this component owns the local draft
 * and fires the mutation when the textarea loses focus, if the draft actually
 * changed.
 *
 * The parent also owns the query invalidation strategy — this component only
 * cares about the current value + save function.
 */
interface SalesReportNarrativePanelProps {
    title: string;
    description: string;
    placeholder: string;
    persistedValue: string | null | undefined;
    mutationKey: readonly unknown[];
    save: (value: string | null) => Promise<unknown>;
    invalidateQueryKey: readonly unknown[];
}

export const SalesReportNarrativePanel = ({
    title,
    description,
    placeholder,
    persistedValue,
    mutationKey,
    save,
    invalidateQueryKey,
}: SalesReportNarrativePanelProps) => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [draft, setDraft] = useState<string>(persistedValue ?? '');
    // Track the last committed value so we can detect real changes on blur
    // regardless of intermediate keystrokes.
    const lastCommitted = useRef<string>(persistedValue ?? '');

    useEffect(() => {
        // When the parent's persisted value updates (new period selected, refetch),
        // reset both the draft and the committed marker so the next blur only
        // fires when the salesperson actually edits.
        setDraft(persistedValue ?? '');
        lastCommitted.current = persistedValue ?? '';
    }, [persistedValue]);

    const mutation = useMutation({
        mutationKey: [...mutationKey],
        mutationFn: (value: string | null) => save(value),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: [...invalidateQueryKey] });
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Save failed', description: getErrorMessage(error) });
        },
    });

    const commitIfChanged = () => {
        const trimmed = draft.trim();
        const currentValue = trimmed.length === 0 ? null : trimmed;
        const lastValue = lastCommitted.current.trim().length === 0 ? null : lastCommitted.current.trim();
        if (currentValue === lastValue) return;
        lastCommitted.current = trimmed;
        mutation.mutate(currentValue);
    };

    return (
        <Card>
            <CardHeader className='flex flex-row items-center justify-between'>
                <div>
                    <CardTitle>{title}</CardTitle>
                    <CardDescription>{description}</CardDescription>
                </div>
                <span className='flex items-center gap-1 text-xs text-muted-foreground'>
                    {mutation.isPending ? (
                        <>
                            <Loader2 className='h-3 w-3 animate-spin' /> Saving…
                        </>
                    ) : mutation.isSuccess || (persistedValue ?? '').trim().length > 0 ? (
                        <>
                            <Check className='h-3 w-3 text-green-600' /> Saved
                        </>
                    ) : null}
                </span>
            </CardHeader>
            <CardContent>
                <Textarea
                    rows={4}
                    value={draft}
                    onChange={(e) => setDraft(e.target.value)}
                    onBlur={commitIfChanged}
                    placeholder={placeholder}
                    maxLength={2000}
                />
                <p className='mt-1 text-right text-xs text-muted-foreground'>{draft.length} / 2000</p>
            </CardContent>
        </Card>
    );
};
