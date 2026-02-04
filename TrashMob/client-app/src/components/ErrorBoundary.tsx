/**
 * Error Boundary Component
 *
 * React error boundary for catching JavaScript errors anywhere in the child
 * component tree, logging those errors, and displaying a fallback UI.
 *
 * @see https://react.dev/reference/react/Component#catching-rendering-errors-with-an-error-boundary
 */

import React, { Component, ErrorInfo, ReactNode } from 'react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';

interface ErrorBoundaryProps {
    /** Child components to render */
    children: ReactNode;
    /** Optional custom fallback UI */
    fallback?: ReactNode;
    /** Optional callback when an error is caught */
    onError?: (error: Error, errorInfo: ErrorInfo) => void;
    /** Optional custom error message */
    errorMessage?: string;
}

interface ErrorBoundaryState {
    hasError: boolean;
    error: Error | null;
}

/**
 * Error Boundary component that catches JavaScript errors in child components
 *
 * @example
 * ```tsx
 * <ErrorBoundary
 *   fallback={<CustomErrorUI />}
 *   onError={(error) => logErrorToService(error)}
 * >
 *   <MyComponent />
 * </ErrorBoundary>
 * ```
 */
export class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
    constructor(props: ErrorBoundaryProps) {
        super(props);
        this.state = {
            hasError: false,
            error: null,
        };
    }

    static getDerivedStateFromError(error: Error): Partial<ErrorBoundaryState> {
        return { hasError: true, error };
    }

    componentDidCatch(error: Error, errorInfo: ErrorInfo): void {
        // Log error to console in development
        if (import.meta.env.DEV) {
            console.error('ErrorBoundary caught an error:', error, errorInfo);
        }

        // Call optional error callback
        this.props.onError?.(error, errorInfo);
    }

    handleReset = (): void => {
        this.setState({
            hasError: false,
            error: null,
        });
    };

    handleReload = (): void => {
        window.location.reload();
    };

    render(): ReactNode {
        const { hasError, error } = this.state;
        const { children, fallback, errorMessage } = this.props;

        if (hasError) {
            // Use custom fallback if provided
            if (fallback) {
                return fallback;
            }

            // Default error UI
            return (
                <div className='flex min-h-[400px] items-center justify-center p-4'>
                    <Card className='w-full max-w-md'>
                        <CardHeader>
                            <CardTitle className='text-destructive'>Something went wrong</CardTitle>
                            <CardDescription>
                                {errorMessage || 'An unexpected error occurred. Please try again.'}
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            {import.meta.env.DEV && error ? (
                                <details className='mt-2'>
                                    <summary className='cursor-pointer text-sm text-muted-foreground'>
                                        Error details (development only)
                                    </summary>
                                    <pre className='mt-2 overflow-auto rounded bg-muted p-2 text-xs'>
                                        {error.message}
                                        {error.stack ? `\n\n${error.stack}` : null}
                                    </pre>
                                </details>
                            ) : null}
                        </CardContent>
                        <CardFooter className='flex gap-2'>
                            <Button variant='outline' onClick={this.handleReset}>
                                Try Again
                            </Button>
                            <Button onClick={this.handleReload}>Reload Page</Button>
                        </CardFooter>
                    </Card>
                </div>
            );
        }

        return children;
    }
}

/**
 * Higher-order component to wrap a component with an error boundary
 *
 * @example
 * ```tsx
 * const SafeComponent = withErrorBoundary(MyComponent, {
 *   errorMessage: 'Failed to load component'
 * });
 * ```
 */
export function withErrorBoundary<P extends object>(
    WrappedComponent: React.ComponentType<P>,
    errorBoundaryProps?: Omit<ErrorBoundaryProps, 'children'>,
): React.FC<P> {
    const displayName = WrappedComponent.displayName || WrappedComponent.name || 'Component';

    const ComponentWithErrorBoundary: React.FC<P> = (props) => (
        <ErrorBoundary {...errorBoundaryProps}>
            <WrappedComponent {...props} />
        </ErrorBoundary>
    );

    ComponentWithErrorBoundary.displayName = `withErrorBoundary(${displayName})`;

    return ComponentWithErrorBoundary;
}

export default ErrorBoundary;
