# TrashMob Web API & Frontend — AI Assistant Context

> **Note:** For overall project context, architecture, and coding standards, see [/CLAUDE.md](../CLAUDE.md) at the repository root. This document covers patterns specific to the web application.

## Application Overview

This folder contains the main TrashMob web application:
- **ASP.NET Core Web API** — Backend services and REST endpoints
- **React SPA (TypeScript)** — Client-side web application in `client-app/`

## Folder Structure

```
TrashMob/
├── Controllers/              # API endpoints (REST controllers)
├── client-app/               # React TypeScript SPA
│   ├── src/
│   │   ├── components/       # Reusable React components
│   │   │   ├── ui/           # Radix UI primitives (shadcn/ui)
│   │   │   │   └── custom/   # Extended UI components (EnhancedFormLabel, etc.)
│   │   │   └── Models/       # TypeScript model classes with defaults
│   │   ├── pages/            # Page components (file-based routing convention)
│   │   │   └── siteadmin/    # Admin pages (DataTable + columns pattern)
│   │   ├── lib/              # Utilities and helpers
│   │   ├── hooks/            # Custom React hooks
│   │   └── services/         # API client service factories
│   ├── package.json
│   └── vite.config.ts
├── wwwroot/                  # Static assets
├── appsettings.json          # Configuration (DO NOT commit secrets)
└── Program.cs                # Application startup
```

## Controller Patterns

### V2 Controllers (89 controllers in `Controllers/V2/`)

**All new endpoints must be v2.** V1 controllers in `Controllers/` are legacy — do not add new endpoints there.

V2 controllers extend `ControllerBase` directly (not `SecureController`), use primary constructors, and return DTOs only.

### V2 Controller Checklist

- [ ] `[ApiController]`, `[ApiVersion("2.0")]`, `[EnableCors("_myAllowSpecificOrigins")]`
- [ ] Route: `[Route("api/v{version:apiVersion}/things")]`
- [ ] Use **primary constructor** with `ILogger<T>` and managers
- [ ] Add **`CancellationToken cancellationToken`** as last parameter on all async methods
- [ ] Add **XML doc comments** on all public methods (Swagger requires them)
- [ ] Add **`[ProducesResponseType]`** for every response (use DTOs, not entities)
- [ ] Add **`[Authorize]`** + **`[RequiredScope]`** on write operations
- [ ] Return **DTOs only** — use `.ToV2Dto()` mapping, never return raw EF entities
- [ ] Use **structured logging** with message templates (e.g., `"V2 GetThing Id={ThingId}"`)
- [ ] Paginated collections return **`PagedResponse<TDto>`** via `ToPagedAsync()`

### V1 Controller Hierarchy (Legacy)

- `BaseController` — Provides `Logger` property (lazy-loaded)
- `SecureController : BaseController` — Adds `UserId`, `AuthorizationService`, `IsAuthorizedAsync()`
- `KeyedController<T> : SecureController` — Generic CRUD for entities with GUID keys

## React Frontend Patterns

### Service Factory Pattern (API Calls)

All API calls use a factory pattern returning `{ key, service }` for TanStack React Query:

```typescript
// src/services/things.ts — ALL URLs must use /v2/ prefix
export type GetThing_Params = { id: string };

export const GetThing = (params: GetThing_Params) => ({
    key: ['/things', params.id],
    service: async () =>
        ApiService('protected').fetchData<ThingData>({
            url: `/v2/things/${params.id}`,
            method: 'get',
        }),
});

export const CreateThing = () => ({
    key: ['/things/create'],
    service: async (body: ThingData) =>
        ApiService('protected').fetchData<ThingData>({
            url: '/v2/things',
            method: 'post',
            data: body,
        }),
});
```

### List Page Pattern (DataTable)

Admin and list pages use a DataTable + columns factory pattern:

```tsx
// pages/siteadmin/things/columns.tsx
interface GetColumnsProps {
    onDelete: (id: string, name: string) => void;
}

export const getColumns = ({ onDelete }: GetColumnsProps): ColumnDef<ThingData>[] => [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
        cell: ({ row }) => (
            <Link to={`/siteadmin/things/${row.original.id}`} className='font-medium hover:underline'>
                {row.getValue('name')}
            </Link>
        ),
    },
    {
        accessorKey: 'status',
        header: 'Status',
        cell: ({ row }) => <StatusBadge status={row.getValue('status')} />,
    },
    // Actions column with dropdown menu...
];

// pages/siteadmin/things/page.tsx
export const SiteAdminThings = () => {
    const { data: things } = useQuery({
        queryKey: GetThings().key,
        queryFn: GetThings().service,
        select: (res) => res.data,
    });

    const columns = getColumns({ onDelete: handleDelete });

    return (
        <Card>
            <CardHeader><CardTitle>Things</CardTitle></CardHeader>
            <CardContent>
                <DataTable columns={columns} data={things || []} enableSearch searchPlaceholder='Search...' />
            </CardContent>
        </Card>
    );
};
```

### Detail Page Pattern

```tsx
// pages/siteadmin/things/$thingId.tsx
export const SiteAdminThingDetail = () => {
    const { thingId } = useParams<{ thingId: string }>() as { thingId: string };

    const { data: thing } = useQuery({
        queryKey: GetThing({ id: thingId }).key,
        queryFn: GetThing({ id: thingId }).service,
        select: (res) => res.data,
        enabled: !!thingId,
    });

    if (!thing) return null;

    return (
        <div className='space-y-6'>
            <div className='flex items-center gap-2'>
                <Button variant='ghost' size='sm' asChild>
                    <Link to='/siteadmin/things'><ArrowLeft className='mr-2 h-4 w-4' /> Back</Link>
                </Button>
            </div>
            <Card>
                <CardHeader><CardTitle>{thing.name}</CardTitle></CardHeader>
                <CardContent>
                    <dl className='grid grid-cols-1 gap-4 sm:grid-cols-2'>
                        <div><dt className='text-sm font-medium text-muted-foreground'>Field</dt>
                             <dd className='mt-1'>{thing.field}</dd></div>
                    </dl>
                </CardContent>
            </Card>
        </div>
    );
};
```

### Form Pattern (Zod + React Hook Form)

```tsx
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';

const schema = z.object({
    name: z.string().min(3, 'Name must be at least 3 characters.').max(100),
    description: z.string().max(500).optional(),
    isPublic: z.boolean(),
});

type FormValues = z.infer<typeof schema>;

export const CreateThing = () => {
    const form = useForm<FormValues>({
        resolver: zodResolver(schema),
        defaultValues: { name: '', description: '', isPublic: true },
    });

    const createThing = useMutation({
        mutationKey: CreateThing().key,
        mutationFn: CreateThing().service,
        onSuccess: () => { toast({ variant: 'primary', title: 'Created!' }); navigate('/things'); },
    });

    const onSubmit = (values: FormValues) => {
        const model = new ThingData();
        model.name = values.name;
        // ... map form values to model
        createThing.mutate(model);
    };

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-6'>
                <FormField control={form.control} name='name' render={({ field }) => (
                    <FormItem>
                        <FormLabel>Name *</FormLabel>
                        <FormControl><Input {...field} /></FormControl>
                        <FormMessage />
                    </FormItem>
                )} />
                <Button type='submit' disabled={createThing.isPending}>Create</Button>
            </form>
        </Form>
    );
};
```

### TypeScript Model Pattern

```typescript
// components/Models/ThingData.ts — classes with defaults, not interfaces
class ThingData {
    id: string = Guid.createEmpty().toString();
    name: string = '';
    description: string = '';
    isPublic: boolean = true;
    createdDate: Date = new Date();
    latitude?: number;  // Optional fields use ?
    longitude?: number;
}
```

### Frontend Checklist (New Pages)

- [ ] Use **`@/` path alias** for imports (not relative `../../../`)
- [ ] Use **service factory pattern** (`{ key, service }`) for API calls
- [ ] Use **Zod** for form validation, **React Hook Form** with `zodResolver`
- [ ] Use **Radix UI** components from `@/components/ui/` (Card, Button, Input, etc.)
- [ ] Use **Tailwind CSS** utility classes for styling
- [ ] Use **DataTable** + **columns factory** for list pages
- [ ] Use **`useMutation`** with `onSuccess` for toast notifications and query invalidation
- [ ] Add route in `App.tsx` with lazy import
- [ ] Handle loading and error states
- [ ] Use **`getErrorMessage()`** from `@/lib/api-errors` in `onError` callbacks (handles both v1 and v2 error formats)
- [ ] Run **`npm run check`** before committing (runs both ESLint + Prettier to match CI)

### API Error Handling

The Axios interceptor in `services/index.ts` normalizes API errors so `error.message` always contains the best available message (v2 Problem Details `.detail`, v1 `.message`, or fallback). For custom error extraction, use `getErrorMessage()`:

```typescript
import { getErrorMessage } from '@/lib/api-errors';

onError: (error: Error) => {
    toast({ variant: 'destructive', title: 'Error', description: error.message });
},
```

### CI Matching

**IMPORTANT:** CI runs both ESLint and Prettier. Running only `npm run lint` locally will miss Prettier formatting issues. Always run:

```bash
npm run check      # Runs: eslint . --fix && prettier . --write
```

## Authentication

### Getting User Context (Backend)

```csharp
// In SecureController-derived classes
var userId = UserId;  // From SecureController base class (extracted from JWT claims)

// Use authorization handler (preferred over manual comparison)
if (!await IsAuthorizedAsync(entity, AuthorizationPolicyConstants.UserOwnsEntity))
    return Forbid();
```

### Getting User Context (Frontend)

```tsx
import { useGetCurrentUser } from '@/hooks/useGetCurrentUser';

const { currentUser } = useGetCurrentUser();
// currentUser.id, currentUser.isSiteAdmin, currentUser.userName, etc.
```

## Quick Reference

### Run Locally

```bash
# Backend (from TrashMob folder)
dotnet run --environment Development

# Frontend (from TrashMob/client-app folder)
npm start
```

### Local URLs
- API: https://localhost:44332
- Swagger: https://localhost:44332/swagger/index.html
- Frontend: http://localhost:3000

---

**Related Documentation:**
- [Root CLAUDE.md](../CLAUDE.md) — Architecture, patterns, coding standards
- [Planning/README.md](../Planning/README.md) — 2026 roadmap
- [TrashMob.prd](./TrashMob.prd) — Product requirements

**Last Updated:** March 15, 2026
