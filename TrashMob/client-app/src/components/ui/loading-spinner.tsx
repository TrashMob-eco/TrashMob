import { Loader2 } from 'lucide-react';
import { cn } from '@/lib/utils';

interface LoadingSpinnerProps {
    /** Additional CSS classes */
    className?: string;
    /** Size of the spinner */
    size?: 'sm' | 'md' | 'lg';
    /** Optional loading message for screen readers */
    srText?: string;
}

const sizeClasses = {
    sm: 'h-4 w-4',
    md: 'h-6 w-6',
    lg: 'h-8 w-8',
};

/**
 * LoadingSpinner component for consistent loading states
 *
 * @example
 * ```tsx
 * // Basic usage
 * <LoadingSpinner />
 *
 * // With size and custom message
 * <LoadingSpinner size="lg" srText="Loading events..." />
 * ```
 */
export const LoadingSpinner = ({ className, size = 'md', srText = 'Loading...' }: LoadingSpinnerProps) => (
    <div className={cn('flex items-center justify-center', className)} role='status' aria-live='polite'>
        <Loader2 className={cn('animate-spin text-muted-foreground', sizeClasses[size])} />
        <span className='sr-only'>{srText}</span>
    </div>
);

/**
 * FullPageLoader component for page-level loading states
 *
 * @example
 * ```tsx
 * if (isLoading) return <FullPageLoader />;
 * ```
 */
export const FullPageLoader = ({ message = 'Loading...' }: { message?: string }) => (
    <div className='flex flex-col items-center justify-center min-h-[400px] gap-4' role='status' aria-live='polite'>
        <Loader2 className='h-8 w-8 animate-spin text-primary' />
        <p className='text-muted-foreground'>{message}</p>
    </div>
);
