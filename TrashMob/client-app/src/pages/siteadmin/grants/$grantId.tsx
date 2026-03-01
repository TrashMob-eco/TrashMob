import { useState } from 'react';
import { Link, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Edit, ExternalLink, Loader2, Plus, SquareX } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { useToast } from '@/hooks/use-toast';
import { GrantStatusBadge, getGrantStatusLabel } from '@/components/contacts/contact-constants';
import { GrantTaskDialog } from '@/components/contacts/grant-task-dialog';
import GrantTaskData from '@/components/Models/GrantTaskData';
import { GetGrantById } from '@/services/grants';
import { CreateGrantTask, DeleteGrantTask, GetGrantTasks, UpdateGrantTask } from '@/services/grants';
import { GetContacts } from '@/services/contacts';

export const SiteAdminGrantDetail = () => {
    const { grantId } = useParams<{ grantId: string }>() as { grantId: string };
    const { toast } = useToast();
    const queryClient = useQueryClient();

    const [taskDialogOpen, setTaskDialogOpen] = useState(false);
    const [editingTask, setEditingTask] = useState<GrantTaskData | null>(null);

    const { data: grant, isLoading: grantLoading } = useQuery({
        queryKey: GetGrantById({ id: grantId }).key,
        queryFn: GetGrantById({ id: grantId }).service,
        select: (res) => res.data,
    });

    const { data: tasks } = useQuery({
        queryKey: GetGrantTasks({ grantId }).key,
        queryFn: GetGrantTasks({ grantId }).service,
        select: (res) => res.data,
    });

    const { data: contacts } = useQuery({
        queryKey: GetContacts().key,
        queryFn: GetContacts().service,
        select: (res) => res.data,
    });

    const invalidateTasks = () => {
        queryClient.invalidateQueries({ queryKey: ['/granttasks', grantId], refetchType: 'all' });
    };

    const createTask = useMutation({
        mutationKey: CreateGrantTask().key,
        mutationFn: CreateGrantTask().service,
        onSuccess: () => {
            invalidateTasks();
            setTaskDialogOpen(false);
            toast({ variant: 'primary', title: 'Task added' });
        },
    });

    const updateTask = useMutation({
        mutationKey: UpdateGrantTask().key,
        mutationFn: UpdateGrantTask().service,
        onSuccess: () => {
            invalidateTasks();
            setTaskDialogOpen(false);
            setEditingTask(null);
            toast({ variant: 'default', title: 'Task updated' });
        },
    });

    const deleteTask = useMutation({
        mutationKey: DeleteGrantTask().key,
        mutationFn: DeleteGrantTask().service,
        onSuccess: () => {
            invalidateTasks();
            toast({ variant: 'default', title: 'Task deleted' });
        },
    });

    const handleAddTask = () => {
        setEditingTask(null);
        setTaskDialogOpen(true);
    };

    const handleEditTask = (task: GrantTaskData) => {
        setEditingTask(task);
        setTaskDialogOpen(true);
    };

    const handleDeleteTask = (id: string) => {
        if (!window.confirm('Delete this task?')) return;
        deleteTask.mutate({ id });
    };

    const handleToggleComplete = (task: GrantTaskData) => {
        const body = { ...task };
        body.isCompleted = !task.isCompleted;
        body.completedDate = body.isCompleted ? new Date().toISOString() : null;
        updateTask.mutate(body);
    };

    const handleTaskSubmit = (values: { title: string; dueDate: string | null; notes: string }) => {
        if (editingTask) {
            const body = { ...editingTask };
            body.title = values.title;
            body.dueDate = values.dueDate;
            body.notes = values.notes;
            updateTask.mutate(body);
        } else {
            const body = new GrantTaskData();
            body.grantId = grantId;
            body.title = values.title;
            body.dueDate = values.dueDate;
            body.notes = values.notes;
            body.sortOrder = (tasks || []).length;
            createTask.mutate(body);
        }
    };

    const funderContact = grant?.funderContactId
        ? (contacts || []).find((c) => c.id === grant.funderContactId)
        : null;

    const funderContactName = funderContact
        ? [funderContact.firstName, funderContact.lastName].filter(Boolean).join(' ')
        : null;

    if (grantLoading) {
        return (
            <div className='flex justify-center items-center py-16'>
                <Loader2 className='animate-spin mr-2' /> Loading...
            </div>
        );
    }

    if (!grant) {
        return <p className='text-muted-foreground'>Grant not found.</p>;
    }

    const formatDate = (d: string | null) => (d ? new Date(d).toLocaleDateString() : '—');
    const formatAmount = (n: number | null) => (n != null ? `$${n.toLocaleString()}` : '—');

    return (
        <div className='space-y-6'>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <div>
                        <CardTitle>{grant.funderName}</CardTitle>
                        {grant.programName ? (
                            <p className='text-sm text-muted-foreground mt-1'>{grant.programName}</p>
                        ) : null}
                    </div>
                    <div className='flex items-center gap-2'>
                        <GrantStatusBadge status={grant.status} />
                        <Button variant='outline' size='sm' asChild>
                            <Link to={`/siteadmin/grants/${grantId}/edit`}>
                                <Edit /> Edit
                            </Link>
                        </Button>
                    </div>
                </CardHeader>
                <CardContent>
                    <div className='grid grid-cols-2 md:grid-cols-4 gap-4 text-sm'>
                        <div>
                            <div className='text-muted-foreground'>Status</div>
                            <div className='font-medium'>{getGrantStatusLabel(grant.status)}</div>
                        </div>
                        <div>
                            <div className='text-muted-foreground'>Submission Deadline</div>
                            <div className='font-medium'>{formatDate(grant.submissionDeadline)}</div>
                        </div>
                        <div>
                            <div className='text-muted-foreground'>Amount Range</div>
                            <div className='font-medium'>
                                {grant.amountMin != null || grant.amountMax != null
                                    ? `${formatAmount(grant.amountMin)} – ${formatAmount(grant.amountMax)}`
                                    : '—'}
                            </div>
                        </div>
                        <div>
                            <div className='text-muted-foreground'>Amount Awarded</div>
                            <div className='font-medium'>{formatAmount(grant.amountAwarded)}</div>
                        </div>
                        <div>
                            <div className='text-muted-foreground'>Award Date</div>
                            <div className='font-medium'>{formatDate(grant.awardDate)}</div>
                        </div>
                        <div>
                            <div className='text-muted-foreground'>Reporting Deadline</div>
                            <div className='font-medium'>{formatDate(grant.reportingDeadline)}</div>
                        </div>
                        <div>
                            <div className='text-muted-foreground'>Renewal Date</div>
                            <div className='font-medium'>{formatDate(grant.renewalDate)}</div>
                        </div>
                        <div>
                            <div className='text-muted-foreground'>Funder Contact</div>
                            <div className='font-medium'>
                                {funderContactName ? (
                                    <Link
                                        to={`/siteadmin/contacts/${grant.funderContactId}`}
                                        className='hover:underline'
                                    >
                                        {funderContactName}
                                    </Link>
                                ) : (
                                    '—'
                                )}
                            </div>
                        </div>
                    </div>
                    {grant.grantUrl ? (
                        <div className='mt-4'>
                            <a
                                href={grant.grantUrl}
                                target='_blank'
                                rel='noopener noreferrer'
                                className='text-sm text-primary hover:underline inline-flex items-center gap-1'
                            >
                                <ExternalLink className='h-3.5 w-3.5' /> Grant URL
                            </a>
                        </div>
                    ) : null}
                    {grant.description ? (
                        <div className='mt-4'>
                            <div className='text-sm text-muted-foreground'>Description</div>
                            <p className='text-sm mt-1 whitespace-pre-wrap'>{grant.description}</p>
                        </div>
                    ) : null}
                    {grant.notes ? (
                        <div className='mt-4'>
                            <div className='text-sm text-muted-foreground'>Notes</div>
                            <p className='text-sm mt-1 whitespace-pre-wrap'>{grant.notes}</p>
                        </div>
                    ) : null}
                </CardContent>
            </Card>

            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <CardTitle className='text-lg'>Tasks ({(tasks || []).length})</CardTitle>
                    <Button size='sm' onClick={handleAddTask}>
                        <Plus /> Add Task
                    </Button>
                </CardHeader>
                <CardContent>
                    {(tasks || []).length === 0 ? (
                        <p className='text-sm text-muted-foreground'>No tasks yet. Add one to track grant progress.</p>
                    ) : (
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead className='w-12'>Done</TableHead>
                                    <TableHead>Title</TableHead>
                                    <TableHead>Due Date</TableHead>
                                    <TableHead>Notes</TableHead>
                                    <TableHead className='text-right'>Actions</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {(tasks || []).map((t) => (
                                    <TableRow key={t.id} className={t.isCompleted ? 'opacity-60' : ''}>
                                        <TableCell>
                                            <Checkbox
                                                checked={t.isCompleted}
                                                onCheckedChange={() => handleToggleComplete(t)}
                                            />
                                        </TableCell>
                                        <TableCell className={t.isCompleted ? 'line-through' : 'font-medium'}>
                                            {t.title}
                                        </TableCell>
                                        <TableCell>{formatDate(t.dueDate)}</TableCell>
                                        <TableCell className='max-w-[200px] truncate'>{t.notes || '—'}</TableCell>
                                        <TableCell className='text-right'>
                                            <div className='flex justify-end gap-1'>
                                                <Button
                                                    variant='ghost'
                                                    size='icon'
                                                    onClick={() => handleEditTask(t)}
                                                >
                                                    <Edit className='h-4 w-4' />
                                                </Button>
                                                <Button
                                                    variant='ghost'
                                                    size='icon'
                                                    onClick={() => handleDeleteTask(t.id)}
                                                    className='text-destructive hover:text-destructive'
                                                >
                                                    <SquareX className='h-4 w-4' />
                                                </Button>
                                            </div>
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    )}
                </CardContent>
            </Card>

            <GrantTaskDialog
                open={taskDialogOpen}
                onOpenChange={setTaskDialogOpen}
                task={editingTask}
                isPending={createTask.isPending || updateTask.isPending}
                onSubmit={handleTaskSubmit}
            />
        </div>
    );
};
