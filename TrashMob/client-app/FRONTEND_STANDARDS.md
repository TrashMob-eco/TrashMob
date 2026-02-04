# TrashMob Frontend Coding Standards

This document outlines the coding standards, patterns, and best practices for the TrashMob React frontend, following Vercel's recommended patterns for modern React development.

## Table of Contents

- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Component Patterns](#component-patterns)
- [Data Fetching](#data-fetching)
- [State Management](#state-management)
- [Error Handling](#error-handling)
- [Loading States](#loading-states)
- [Form Handling](#form-handling)
- [TypeScript Guidelines](#typescript-guidelines)
- [Styling](#styling)
- [Performance](#performance)
- [Testing](#testing)
- [Accessibility](#accessibility)

---

## Technology Stack

| Category       | Technology            | Version |
| -------------- | --------------------- | ------- |
| Framework      | React                 | 18.x    |
| Language       | TypeScript            | 5.8+    |
| Build Tool     | Vite                  | 7.x     |
| Routing        | React Router          | 7.x     |
| State (Server) | TanStack Query        | 4.x     |
| HTTP Client    | Axios                 | 1.x     |
| UI Components  | Radix UI              | Latest  |
| Styling        | Tailwind CSS          | 4.x     |
| Forms          | React Hook Form + Zod | Latest  |

---

## Project Structure

```
src/
├── components/           # Reusable UI components
│   ├── ui/              # Radix UI primitives (buttons, dialogs, etc.)
│   ├── [feature]/       # Feature-specific components
│   └── ErrorBoundary.tsx # Global error boundary
├── pages/               # Route pages (file-based routing)
│   └── [route]/         # Page components
├── services/            # API service definitions
│   ├── index.ts         # Axios instances & ApiService
│   └── [domain].ts      # Domain-specific services
├── hooks/               # Custom React hooks
│   └── use[Feature].ts  # Feature-specific hooks
├── store/               # Global state stores
├── lib/                 # Utility functions
├── config/              # Configuration files
├── enums/               # TypeScript enums
└── App.tsx              # Main application component
```

### Component Organization

For complex components, use a folder structure:

```
src/components/events/EventCard/
├── EventCard.tsx        # Main component
├── EventCard.test.tsx   # Tests
├── EventCard.types.ts   # TypeScript types (if needed)
└── index.ts             # Public export
```

---

## Component Patterns

### Functional Components with TypeScript

Always use functional components with explicit prop types:

```tsx
interface EventCardProps {
    event: EventData;
    onSelect?: (event: EventData) => void;
    className?: string;
}

export const EventCard: React.FC<EventCardProps> = ({ event, onSelect, className }) => {
    const handleClick = () => {
        onSelect?.(event);
    };

    return (
        <Card onClick={handleClick} className={cn('cursor-pointer', className)}>
            <CardHeader>
                <CardTitle>{event.name}</CardTitle>
            </CardHeader>
        </Card>
    );
};
```

### Custom Hooks

Extract reusable logic into custom hooks following the `use` prefix convention:

```tsx
// hooks/useEvent.ts
export const useEvent = (eventId: string) => {
    return useQuery({
        queryKey: GetEventById({ eventId }).key,
        queryFn: GetEventById({ eventId }).service,
        select: (res) => res.data,
    });
};

// Usage
const { data: event, isLoading, error } = useEvent(eventId);
```

### Composition Over Configuration

Prefer composable components over configuration-heavy ones:

```tsx
// Good: Composable
<Card>
    <CardHeader>
        <CardTitle>Event Name</CardTitle>
        <CardDescription>Event details</CardDescription>
    </CardHeader>
    <CardContent>
        {/* Content */}
    </CardContent>
    <CardFooter>
        <Button>Join Event</Button>
    </CardFooter>
</Card>

// Avoid: Configuration-heavy
<Card
    title="Event Name"
    description="Event details"
    content={<div>...</div>}
    footer={<Button>Join Event</Button>}
/>
```

### Controlled vs Uncontrolled Components

Prefer controlled components for form inputs:

```tsx
// Controlled (preferred)
const [value, setValue] = useState('');
<Input value={value} onChange={(e) => setValue(e.target.value)} />;

// Uncontrolled (use for simple cases)
const inputRef = useRef<HTMLInputElement>(null);
<Input ref={inputRef} defaultValue='' />;
```

---

## Data Fetching

### Service Definition Pattern

Define services with type-safe keys and functions:

```tsx
// services/events.ts
export type GetEventById_Params = { eventId: string };
export type GetEventById_Response = EventData;

export const GetEventById = (params: GetEventById_Params) => ({
    key: ['/Events/', params] as const,
    service: async () =>
        ApiService('public').fetchData<GetEventById_Response>({
            url: `/Events/${params.eventId}`,
            method: 'get',
        }),
});
```

### React Query Hooks

Wrap services in custom hooks with proper configuration:

```tsx
// hooks/useGetEvent.ts
export const useGetEvent = (eventId: string, options?: { enabled?: boolean }) => {
    return useQuery({
        queryKey: GetEventById({ eventId }).key,
        queryFn: GetEventById({ eventId }).service,
        select: (res) => res.data,
        staleTime: 5 * 60 * 1000, // 5 minutes
        enabled: options?.enabled ?? true,
    });
};
```

### Mutations with Optimistic Updates

Use mutations for create/update/delete with proper cache management:

```tsx
export const useCreateEvent = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (event: CreateEventRequest) =>
            ApiService('protected').fetchData({
                url: '/Events',
                method: 'post',
                data: event,
            }),
        onSuccess: () => {
            // Invalidate related queries
            queryClient.invalidateQueries({ queryKey: ['/Events'] });
        },
        onError: (error) => {
            // Handle error (show toast, etc.)
            console.error('Failed to create event:', error);
        },
    });
};
```

### Parallel Data Fetching

Fetch multiple resources in parallel when possible:

```tsx
const EventPage = ({ eventId }: { eventId: string }) => {
    // These queries run in parallel
    const eventQuery = useEvent(eventId);
    const attendeesQuery = useEventAttendees(eventId);
    const commentsQuery = useEventComments(eventId);

    if (eventQuery.isLoading) return <LoadingSpinner />;
    if (eventQuery.error) return <ErrorMessage error={eventQuery.error} />;

    return (
        <div>
            <EventDetails event={eventQuery.data} />
            <AttendeeList attendees={attendeesQuery.data} isLoading={attendeesQuery.isLoading} />
            <Comments comments={commentsQuery.data} isLoading={commentsQuery.isLoading} />
        </div>
    );
};
```

---

## State Management

### Server State vs Client State

Distinguish between server state (data from API) and client state (UI state):

| State Type          | Solution       | Examples                       |
| ------------------- | -------------- | ------------------------------ |
| Server State        | TanStack Query | User data, events, attendees   |
| Global Client State | Context/Store  | Auth state, theme, preferences |
| Local Client State  | useState       | Form inputs, modal open/close  |
| URL State           | React Router   | Filters, pagination, search    |

### Avoid Prop Drilling

Use composition or context for deeply nested props:

```tsx
// Option 1: Composition (preferred for simple cases)
<EventList>
    {events.map((event) => (
        <EventCard key={event.id} event={event} />
    ))}
</EventList>;

// Option 2: Context (for truly global state)
const EventContext = createContext<EventContextValue | null>(null);

export const useEventContext = () => {
    const context = useContext(EventContext);
    if (!context) throw new Error('useEventContext must be used within EventProvider');
    return context;
};
```

---

## Error Handling

### Error Boundaries

Wrap components that may fail with ErrorBoundary:

```tsx
import { ErrorBoundary } from '@/components/ErrorBoundary';

const MyPage = () => (
    <ErrorBoundary errorMessage='Failed to load events'>
        <EventList />
    </ErrorBoundary>
);
```

### Query Error Handling

Handle errors at the component level:

```tsx
const { data, isLoading, error } = useGetEvent(eventId);

if (error) {
    // Check error type for specific handling
    if (error.response?.status === 404) {
        return <NotFound message='Event not found' />;
    }
    return <ErrorMessage error={error} />;
}
```

### Form Submission Errors

Handle mutation errors with user feedback:

```tsx
const createEvent = useCreateEvent();

const onSubmit = async (data: EventFormData) => {
    try {
        await createEvent.mutateAsync(data);
        toast({ title: 'Success', description: 'Event created!' });
        navigate('/events');
    } catch (error) {
        toast({
            title: 'Error',
            description: 'Failed to create event. Please try again.',
            variant: 'destructive',
        });
    }
};
```

---

## Loading States

### Skeleton Loaders

Use skeleton components for better perceived performance:

```tsx
const EventCard = ({ event }: { event?: EventData }) => {
    if (!event) {
        return (
            <Card>
                <CardHeader>
                    <Skeleton className='h-6 w-3/4' />
                    <Skeleton className='h-4 w-1/2' />
                </CardHeader>
            </Card>
        );
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle>{event.name}</CardTitle>
                <CardDescription>{event.description}</CardDescription>
            </CardHeader>
        </Card>
    );
};
```

### Loading Patterns

```tsx
// Pattern 1: Loading component
if (isLoading) return <LoadingSpinner />;

// Pattern 2: Conditional rendering
<Button disabled={isSubmitting}>
    {isSubmitting ? 'Saving...' : 'Save'}
</Button>

// Pattern 3: Skeleton list
<div className="space-y-4">
    {isLoading
        ? Array.from({ length: 3 }).map((_, i) => <EventCardSkeleton key={i} />)
        : events.map(event => <EventCard key={event.id} event={event} />)
    }
</div>
```

---

## Form Handling

### React Hook Form with Zod

Use React Hook Form with Zod for type-safe validation:

```tsx
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';

const eventSchema = z.object({
    name: z.string().min(1, 'Name is required').max(100),
    description: z.string().min(10, 'Description must be at least 10 characters'),
    date: z.date({ required_error: 'Date is required' }),
    maxAttendees: z.number().min(1).max(1000).optional(),
});

type EventFormData = z.infer<typeof eventSchema>;

const EventForm = () => {
    const form = useForm<EventFormData>({
        resolver: zodResolver(eventSchema),
        defaultValues: {
            name: '',
            description: '',
        },
    });

    const onSubmit = (data: EventFormData) => {
        // Handle submission
    };

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)}>
                <FormField
                    control={form.control}
                    name='name'
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Event Name</FormLabel>
                            <FormControl>
                                <Input {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <Button type='submit'>Create Event</Button>
            </form>
        </Form>
    );
};
```

---

## TypeScript Guidelines

### Strict Mode

TypeScript strict mode is enabled. Follow these practices:

```tsx
// Use explicit types for function parameters and returns
const formatDate = (date: Date): string => {
    return date.toLocaleDateString();
};

// Use interfaces for object shapes
interface EventData {
    id: string;
    name: string;
    date: string;
}

// Use type for unions/intersections
type EventStatus = 'pending' | 'active' | 'completed' | 'cancelled';

// Avoid 'any' - use 'unknown' and type guards instead
const parseJSON = (json: string): unknown => {
    return JSON.parse(json);
};

// Type guards
const isEventData = (obj: unknown): obj is EventData => {
    return typeof obj === 'object' && obj !== null && 'id' in obj && 'name' in obj;
};
```

### Path Aliases

Use `@/` for absolute imports:

```tsx
// Good
import { Button } from '@/components/ui/button';
import { useEvent } from '@/hooks/useEvent';

// Avoid
import { Button } from '../../../components/ui/button';
```

### Generic Components

Use generics for reusable components:

```tsx
interface SelectProps<T> {
    options: T[];
    value: T | null;
    onChange: (value: T) => void;
    getLabel: (option: T) => string;
    getValue: (option: T) => string;
}

function Select<T>({ options, value, onChange, getLabel, getValue }: SelectProps<T>) {
    // Implementation
}
```

---

## Styling

### Tailwind CSS

Use Tailwind utility classes consistently:

```tsx
<div className='flex items-center justify-between gap-4 rounded-lg bg-card p-4 shadow-sm'>
    <span className='text-sm font-medium text-muted-foreground'>{event.date}</span>
</div>
```

### Component Variants with CVA

Use `class-variance-authority` for component variants:

```tsx
import { cva, type VariantProps } from 'class-variance-authority';

const buttonVariants = cva('inline-flex items-center justify-center rounded-md font-medium transition-colors', {
    variants: {
        variant: {
            default: 'bg-primary text-primary-foreground hover:bg-primary/90',
            destructive: 'bg-destructive text-destructive-foreground hover:bg-destructive/90',
            outline: 'border border-input bg-background hover:bg-accent',
            ghost: 'hover:bg-accent hover:text-accent-foreground',
        },
        size: {
            default: 'h-10 px-4 py-2',
            sm: 'h-9 px-3',
            lg: 'h-11 px-8',
            icon: 'h-10 w-10',
        },
    },
    defaultVariants: {
        variant: 'default',
        size: 'default',
    },
});

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement>, VariantProps<typeof buttonVariants> {}

const Button = ({ className, variant, size, ...props }: ButtonProps) => (
    <button className={cn(buttonVariants({ variant, size }), className)} {...props} />
);
```

### Class Merging

Use `cn` utility for conditional classes:

```tsx
import { cn } from '@/lib/utils';

<div
    className={cn(
        'base-classes',
        condition && 'conditional-classes',
        isActive ? 'active-classes' : 'inactive-classes',
        className,
    )}
/>;
```

---

## Performance

### Memoization

Use memoization judiciously:

```tsx
// Memoize expensive calculations
const sortedEvents = useMemo(
    () => events.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime()),
    [events],
);

// Memoize callbacks passed to children
const handleSelect = useCallback((event: EventData) => {
    setSelectedEvent(event);
}, []);

// Memoize components that receive stable props
const MemoizedEventCard = memo(EventCard);
```

### Code Splitting

Use dynamic imports for large components:

```tsx
import { lazy, Suspense } from 'react';

const AdminDashboard = lazy(() => import('./AdminDashboard'));

const App = () => (
    <Suspense fallback={<LoadingSpinner />}>
        <AdminDashboard />
    </Suspense>
);
```

### Image Optimization

Use proper image sizing and lazy loading:

```tsx
<img src={event.imageUrl} alt={event.name} loading='lazy' width={400} height={300} className='object-cover' />
```

---

## Testing

### Test Structure

Place tests alongside components:

```
EventCard/
├── EventCard.tsx
└── EventCard.test.tsx
```

### Testing Patterns

```tsx
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

const createWrapper = () => {
    const queryClient = new QueryClient({
        defaultOptions: { queries: { retry: false } },
    });
    return ({ children }) => <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>;
};

describe('EventCard', () => {
    it('renders event name', () => {
        render(<EventCard event={mockEvent} />, { wrapper: createWrapper() });
        expect(screen.getByText(mockEvent.name)).toBeInTheDocument();
    });

    it('calls onSelect when clicked', async () => {
        const user = userEvent.setup();
        const onSelect = vi.fn();

        render(<EventCard event={mockEvent} onSelect={onSelect} />, {
            wrapper: createWrapper(),
        });

        await user.click(screen.getByRole('article'));
        expect(onSelect).toHaveBeenCalledWith(mockEvent);
    });

    it('handles async data loading', async () => {
        render(<EventDetails eventId='123' />, { wrapper: createWrapper() });

        // Wait for loading to complete
        await waitFor(() => {
            expect(screen.queryByText('Loading...')).not.toBeInTheDocument();
        });

        expect(screen.getByText('Event Name')).toBeInTheDocument();
    });
});
```

---

## Accessibility

### WCAG 2.2 AA Compliance

Follow these guidelines:

- Use semantic HTML (`<nav>`, `<main>`, `<article>`, etc.)
- Provide alt text for all images
- Ensure keyboard navigation works
- Maintain 4.5:1 color contrast ratio
- Use ARIA attributes when needed

### Radix UI

Radix UI components handle accessibility automatically:

```tsx
import { Dialog, DialogTrigger, DialogContent, DialogTitle } from '@/components/ui/dialog';

// Focus management, keyboard nav, and ARIA handled automatically
<Dialog>
    <DialogTrigger asChild>
        <Button>Open Dialog</Button>
    </DialogTrigger>
    <DialogContent>
        <DialogTitle>Modal Title</DialogTitle>
        {/* Content */}
    </DialogContent>
</Dialog>;
```

### Screen Reader Support

```tsx
// Provide context for screen readers
<Button aria-label="Close dialog">
    <X className="h-4 w-4" />
</Button>

// Use visually hidden text for context
<span className="sr-only">Loading events</span>

// Announce dynamic content
<div role="status" aria-live="polite">
    {message}
</div>
```

---

## Quick Reference

### Creating a New Feature

1. Create service in `src/services/[feature].ts`
2. Create hook in `src/hooks/use[Feature].ts`
3. Create component in `src/components/[feature]/`
4. Add page in `src/pages/[route]/`
5. Add tests alongside components

### Common Commands

```bash
npm start          # Start dev server (port 3000)
npm run build      # Production build
npm test           # Run tests
npm run lint       # Lint and fix
npm run format     # Format code with Prettier
```

### Environment Variables

| Variable       | Description                                 |
| -------------- | ------------------------------------------- |
| `VITE_API_URL` | API base URL (optional, defaults to `/api`) |

---

## Resources

- [React Documentation](https://react.dev)
- [Vercel React Best Practices](https://vercel.com/docs)
- [TanStack Query Documentation](https://tanstack.com/query)
- [Radix UI Documentation](https://www.radix-ui.com)
- [Tailwind CSS Documentation](https://tailwindcss.com)

---

**Last Updated:** February 3, 2026
**Maintained By:** TrashMob Engineering Team
