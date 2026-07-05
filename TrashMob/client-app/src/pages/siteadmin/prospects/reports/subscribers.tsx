import { useMemo, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Send, Trash2, UserPlus } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useToast } from '@/hooks/use-toast';
import { getErrorMessage } from '@/lib/api-errors';
import {
    AddSalesReportSubscriber,
    DeleteSalesReportSubscriber,
    GetSalesReportSubscribers,
    UpdateSalesReportSubscriber,
    type SalesReportSubscriberDto,
} from '@/services/sales-report-subscribers';
import { GetAllUsers } from '@/services/users';

/**
 * Sales Report Subscribers — distribution list management (Project 63 Phase 4b).
 * SiteAdmin only. The daily job reads from this table when deciding who
 * receives the weekly Monday-morning report and the monthly 1st-of-month
 * report.
 *
 * Route: /siteadmin/prospects/reports/subscribers
 */
export const SiteAdminProspectSubscribers = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();

    const [selectedUserId, setSelectedUserId] = useState<string>('');
    const [addWeekly, setAddWeekly] = useState(true);
    const [addMonthly, setAddMonthly] = useState(true);
    const [userSearch, setUserSearch] = useState<string>('');

    const { data: subscribers, isLoading } = useQuery({
        queryKey: GetSalesReportSubscribers().key,
        queryFn: GetSalesReportSubscribers().service,
        select: (res) => res.data,
    });

    const { data: users } = useQuery({
        queryKey: GetAllUsers().key,
        queryFn: GetAllUsers().service,
        select: (res) => res.data.items,
    });

    const subscribedUserIds = useMemo(() => new Set((subscribers ?? []).map((s) => s.userId)), [subscribers]);

    const availableUsers = useMemo(() => {
        const list = (users ?? []).filter((u) => !subscribedUserIds.has(u.id));
        const search = userSearch.trim().toLowerCase();
        if (!search) return list.slice(0, 25);
        return list
            .filter(
                (u) =>
                    (u.userName ?? '').toLowerCase().includes(search) ||
                    ((u as unknown as { email?: string }).email ?? '').toLowerCase().includes(search),
            )
            .slice(0, 25);
    }, [users, subscribedUserIds, userSearch]);

    const invalidate = () => queryClient.invalidateQueries({ queryKey: GetSalesReportSubscribers().key });

    const addMutation = useMutation({
        mutationKey: AddSalesReportSubscriber().key,
        mutationFn: AddSalesReportSubscriber().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Subscriber added' });
            invalidate();
            setSelectedUserId('');
            setUserSearch('');
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Add failed', description: getErrorMessage(error) });
        },
    });

    const deleteMutation = useMutation({
        mutationKey: DeleteSalesReportSubscriber().key,
        mutationFn: DeleteSalesReportSubscriber().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Subscriber removed' });
            invalidate();
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Remove failed', description: getErrorMessage(error) });
        },
    });

    const handleToggleWeekly = (sub: SalesReportSubscriberDto, checked: boolean) => {
        UpdateSalesReportSubscriber({ subscriptionId: sub.id })
            .service({ includeWeekly: checked, includeMonthly: sub.includeMonthly })
            .then(() => invalidate())
            .catch((error: Error) =>
                toast({
                    variant: 'destructive',
                    title: 'Update failed',
                    description: getErrorMessage(error),
                }),
            );
    };

    const handleToggleMonthly = (sub: SalesReportSubscriberDto, checked: boolean) => {
        UpdateSalesReportSubscriber({ subscriptionId: sub.id })
            .service({ includeWeekly: sub.includeWeekly, includeMonthly: checked })
            .then(() => invalidate())
            .catch((error: Error) =>
                toast({
                    variant: 'destructive',
                    title: 'Update failed',
                    description: getErrorMessage(error),
                }),
            );
    };

    const submitAdd = () => {
        if (!selectedUserId) return;
        addMutation.mutate({ userId: selectedUserId, includeWeekly: addWeekly, includeMonthly: addMonthly });
    };

    return (
        <div className='space-y-6'>
            <Card>
                <CardHeader>
                    <CardTitle>Sales Report Subscribers</CardTitle>
                    <CardDescription>
                        The distribution list for the weekly Monday-morning and monthly 1st-of-month emails. The daily
                        job reads from here — changes apply on the next send day.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <h3 className='mb-3 text-sm font-medium text-muted-foreground'>Current subscribers</h3>
                    {isLoading ? (
                        <p className='text-sm text-muted-foreground'>Loading…</p>
                    ) : (subscribers ?? []).length === 0 ? (
                        <p className='text-sm text-muted-foreground'>
                            No subscribers yet. Add one below and the next Monday's report will send to them.
                        </p>
                    ) : (
                        <ul className='divide-y'>
                            {(subscribers ?? []).map((sub) => (
                                <li key={sub.id} className='flex items-center justify-between py-3'>
                                    <div>
                                        <p className='font-medium'>{sub.userName || '—'}</p>
                                        <p className='text-xs text-muted-foreground'>{sub.email || 'No email'}</p>
                                    </div>
                                    <div className='flex items-center gap-4'>
                                        <label className='flex items-center gap-2 text-sm'>
                                            <Checkbox
                                                checked={sub.includeWeekly}
                                                onCheckedChange={(checked) => handleToggleWeekly(sub, checked === true)}
                                            />
                                            Weekly
                                        </label>
                                        <label className='flex items-center gap-2 text-sm'>
                                            <Checkbox
                                                checked={sub.includeMonthly}
                                                onCheckedChange={(checked) =>
                                                    handleToggleMonthly(sub, checked === true)
                                                }
                                            />
                                            Monthly
                                        </label>
                                        <Button
                                            variant='ghost'
                                            size='sm'
                                            onClick={() => deleteMutation.mutate({ subscriptionId: sub.id })}
                                            disabled={deleteMutation.isPending}
                                        >
                                            <Trash2 className='h-4 w-4' />
                                        </Button>
                                    </div>
                                </li>
                            ))}
                        </ul>
                    )}
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Add a subscriber</CardTitle>
                    <CardDescription>
                        Only existing TrashMob users can be added — the daily job needs a valid email address on file.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <div className='grid grid-cols-1 gap-4 md:grid-cols-3'>
                        <div className='md:col-span-2'>
                            <Label htmlFor='user-search'>Search users</Label>
                            <Input
                                id='user-search'
                                value={userSearch}
                                onChange={(e) => setUserSearch(e.target.value)}
                                placeholder='Type a name or email fragment…'
                            />
                            <div className='mt-2'>
                                <Select value={selectedUserId} onValueChange={setSelectedUserId}>
                                    <SelectTrigger>
                                        <SelectValue placeholder='Select a user…' />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {availableUsers.length === 0 ? (
                                            <SelectItem value='__none__' disabled>
                                                No matching users.
                                            </SelectItem>
                                        ) : (
                                            availableUsers.map((u) => (
                                                <SelectItem key={u.id} value={u.id}>
                                                    {u.userName}
                                                </SelectItem>
                                            ))
                                        )}
                                    </SelectContent>
                                </Select>
                            </div>
                        </div>
                        <div className='space-y-3'>
                            <label className='flex items-center gap-2 text-sm'>
                                <Checkbox
                                    checked={addWeekly}
                                    onCheckedChange={(checked) => setAddWeekly(checked === true)}
                                />
                                Weekly Monday email
                            </label>
                            <label className='flex items-center gap-2 text-sm'>
                                <Checkbox
                                    checked={addMonthly}
                                    onCheckedChange={(checked) => setAddMonthly(checked === true)}
                                />
                                Monthly 1st-of-month email
                            </label>
                            <Button
                                onClick={submitAdd}
                                disabled={!selectedUserId || addMutation.isPending}
                                className='w-full'
                            >
                                {addMutation.isPending ? (
                                    <Send className='mr-1 h-4 w-4 animate-pulse' />
                                ) : (
                                    <UserPlus className='mr-1 h-4 w-4' />
                                )}
                                Add subscriber
                            </Button>
                        </div>
                    </div>
                </CardContent>
            </Card>
        </div>
    );
};
