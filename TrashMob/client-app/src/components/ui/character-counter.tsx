import { cn } from '@/lib/utils';
import { FormDescription, FormMessage } from './form';

interface CharacterCounterProps {
    /** Current length of the text */
    currentLength: number;
    /** Maximum allowed length */
    maxLength: number;
    /** Optional className for the container */
    className?: string;
    /** Whether to show the FormMessage (error) alongside the counter */
    showFormMessage?: boolean;
}

/**
 * A reusable character counter component that displays current/max character count.
 * Shows in red when the limit is exceeded.
 *
 * @example
 * ```tsx
 * const description = form.watch('description');
 *
 * <FormControl>
 *   <Textarea {...field} maxLength={MAX_DESC_LENGTH} />
 * </FormControl>
 * <CharacterCounter currentLength={description?.length || 0} maxLength={MAX_DESC_LENGTH} />
 * ```
 */
export function CharacterCounter({
    currentLength,
    maxLength,
    className,
    showFormMessage = true,
}: CharacterCounterProps) {
    const isOverLimit = currentLength > maxLength;

    return (
        <div className={cn('flex justify-between', className)}>
            {showFormMessage ? <div className='grow'>
                    <FormMessage />
                </div> : null}
            <FormDescription
                className={cn('text-right', {
                    'text-destructive': isOverLimit,
                })}
            >
                {currentLength}/{maxLength}
            </FormDescription>
        </div>
    );
}
