import * as React from 'react';
import { Link, useLocation } from 'react-router';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Home, ArrowLeft, Search } from 'lucide-react';

interface NotFoundProps {
    /** Custom title for the 404 page */
    title?: string;
    /** Custom message to display */
    message?: string;
    /** Show the attempted URL path */
    showPath?: boolean;
}

/**
 * NotFound/404 page component
 *
 * Displays a user-friendly error page when a route is not found.
 * Can be customized with title, message, and whether to show the attempted path.
 *
 * @example
 * ```tsx
 * // Default usage in router
 * <Route path="*" element={<NoMatch />} />
 *
 * // Customized for specific not-found scenarios
 * <NoMatch
 *   title="Event Not Found"
 *   message="The event you're looking for doesn't exist or has been removed."
 * />
 * ```
 */
export const NoMatch: React.FC<NotFoundProps> = ({
    title = 'Page Not Found',
    message = "We couldn't find the page you're looking for. It may have been moved, deleted, or never existed.",
    showPath = true,
}) => {
    const location = useLocation();

    const handleGoBack = () => {
        window.history.back();
    };

    return (
        <div className='flex min-h-[60vh] items-center justify-center p-4'>
            <Card className='w-full max-w-lg text-center'>
                <CardHeader className='space-y-4'>
                    <div className='mx-auto flex h-20 w-20 items-center justify-center rounded-full bg-muted'>
                        <Search className='h-10 w-10 text-muted-foreground' />
                    </div>
                    <CardTitle className='text-2xl'>
                        <span className='block text-6xl font-bold text-primary mb-2'>404</span>
                        {title}
                    </CardTitle>
                    <CardDescription className='text-base'>{message}</CardDescription>
                </CardHeader>
                <CardContent>
                    {showPath && location.pathname !== '/' ? <p className='text-sm text-muted-foreground'>
                            Attempted path: <code className='rounded bg-muted px-2 py-1'>{location.pathname}</code>
                        </p> : null}
                </CardContent>
                <CardFooter className='flex flex-col gap-3 sm:flex-row sm:justify-center'>
                    <Button variant='outline' onClick={handleGoBack}>
                        <ArrowLeft className='mr-2 h-4 w-4' />
                        Go Back
                    </Button>
                    <Button asChild>
                        <Link to='/'>
                            <Home className='mr-2 h-4 w-4' />
                            Back to Home
                        </Link>
                    </Button>
                </CardFooter>
                <div className='border-t px-6 py-4'>
                    <p className='text-sm text-muted-foreground'>
                        Think this is a bug?{' '}
                        <Link to='/contactus' className='text-primary underline underline-offset-4 hover:text-primary/80'>
                            Let us know
                        </Link>
                    </p>
                </div>
            </Card>
        </div>
    );
};

export default NoMatch;
