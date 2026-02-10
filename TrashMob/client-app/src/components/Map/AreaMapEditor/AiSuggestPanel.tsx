import { useState, useRef } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Sparkles, Check, Pencil, RotateCcw, Loader2, AlertCircle } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { SuggestArea, SuggestArea_Response } from '@/services/adoptable-areas';

interface AiSuggestPanelProps {
    partnerId: string;
    communityCenter?: { lat: number; lng: number } | null;
    communityName?: string;
    onSuggestionAccepted: (geoJson: string, suggestedName?: string, suggestedAreaType?: string) => void;
    onSuggestionPreview: (geoJson: string | null) => void;
    onRequestEditMode?: () => void;
}

export const AiSuggestPanel = ({
    partnerId,
    communityCenter,
    communityName,
    onSuggestionAccepted,
    onSuggestionPreview,
    onRequestEditMode,
}: AiSuggestPanelProps) => {
    const [description, setDescription] = useState('');
    const [suggestion, setSuggestion] = useState<SuggestArea_Response | null>(null);
    const inputRef = useRef<HTMLInputElement>(null);

    const { mutate, isPending, error } = useMutation({
        mutationKey: SuggestArea().key,
        mutationFn: (desc: string) =>
            SuggestArea().service(
                { partnerId },
                {
                    description: desc,
                    centerLatitude: communityCenter?.lat,
                    centerLongitude: communityCenter?.lng,
                    communityName,
                },
            ),
        onSuccess: (response) => {
            const data = response.data;
            setSuggestion(data);
            if (data.geoJson) {
                onSuggestionPreview(data.geoJson);
            }
        },
    });

    const handleSuggest = () => {
        if (!description.trim() || isPending) return;
        setSuggestion(null);
        mutate(description.trim());
    };

    const handleAccept = () => {
        if (!suggestion?.geoJson) return;
        onSuggestionPreview(null);
        onSuggestionAccepted(
            suggestion.geoJson,
            suggestion.suggestedName ?? undefined,
            suggestion.suggestedAreaType ?? undefined,
        );
        setSuggestion(null);
        setDescription('');
    };

    const handleAdjust = () => {
        if (!suggestion?.geoJson) return;
        onSuggestionPreview(null);
        onSuggestionAccepted(
            suggestion.geoJson,
            suggestion.suggestedName ?? undefined,
            suggestion.suggestedAreaType ?? undefined,
        );
        onRequestEditMode?.();
        setSuggestion(null);
        setDescription('');
    };

    const handleTryAgain = () => {
        onSuggestionPreview(null);
        setSuggestion(null);
        inputRef.current?.focus();
    };

    const errorMessage = suggestion?.message || (error ? 'An unexpected error occurred.' : null);
    const hasSuggestion = suggestion?.geoJson != null;
    const confidencePercent = suggestion ? Math.round(suggestion.confidence * 100) : 0;

    return (
        <div className='border rounded-t-lg bg-background'>
            <div className='flex items-center gap-2 p-2'>
                <Sparkles className='h-4 w-4 text-violet-500 shrink-0' />
                <Input
                    ref={inputRef}
                    type='text'
                    placeholder='Describe the area, e.g. "200 block of Main St from Oak to Elm"'
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    onKeyDown={(e) => {
                        if (e.key === 'Enter') handleSuggest();
                    }}
                    disabled={isPending}
                    className='h-8 text-sm'
                />
                <Button type='button' size='sm' onClick={handleSuggest} disabled={!description.trim() || isPending}>
                    {isPending ? <Loader2 className='h-4 w-4 animate-spin' /> : 'Suggest'}
                </Button>
            </div>
            {hasSuggestion ? (
                <div className='flex items-center justify-between gap-2 px-2 pb-2'>
                    <span className='text-sm text-muted-foreground truncate'>
                        Suggested: <strong>{suggestion.suggestedName}</strong>
                        {suggestion.suggestedAreaType ? ` (${suggestion.suggestedAreaType})` : ''}
                        {confidencePercent > 0 ? ` \u2014 ${confidencePercent}% confidence` : ''}
                    </span>
                    <div className='flex gap-1 shrink-0'>
                        <Button type='button' size='sm' variant='default' onClick={handleAccept}>
                            <Check className='h-3.5 w-3.5 mr-1' />
                            Accept
                        </Button>
                        <Button type='button' size='sm' variant='outline' onClick={handleAdjust}>
                            <Pencil className='h-3.5 w-3.5 mr-1' />
                            Adjust
                        </Button>
                        <Button type='button' size='sm' variant='ghost' onClick={handleTryAgain}>
                            <RotateCcw className='h-3.5 w-3.5 mr-1' />
                            Retry
                        </Button>
                    </div>
                </div>
            ) : null}
            {errorMessage && !hasSuggestion ? (
                <div className='flex items-center gap-2 px-2 pb-2 text-sm text-destructive'>
                    <AlertCircle className='h-4 w-4 shrink-0' />
                    <span>{errorMessage}</span>
                </div>
            ) : null}
        </div>
    );
};
