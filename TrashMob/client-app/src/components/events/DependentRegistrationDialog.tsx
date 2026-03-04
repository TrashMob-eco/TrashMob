import { FC, useState } from 'react';
import { useQuery, useMutation } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Baby } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import { useToast } from '@/hooks/use-toast';
import DependentData from '@/components/Models/DependentData';
import { GetMyDependents, RegisterEventDependents } from '@/services/dependents';

interface DependentRegistrationDialogProps {
    eventId: string;
    userId: string;
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

export const DependentRegistrationDialog: FC<DependentRegistrationDialogProps> = ({
    eventId,
    userId,
    open,
    onOpenChange,
}) => {
    const { toast } = useToast();
    const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());

    const queryConfig = GetMyDependents({ userId });
    const { data: dependents } = useQuery<AxiosResponse<DependentData[]>, unknown, DependentData[]>({
        queryKey: queryConfig.key,
        queryFn: queryConfig.service,
        select: (res) => res.data,
        enabled: open && !!userId,
    });

    const registerMutation = useMutation({
        mutationFn: RegisterEventDependents({ eventId }).service,
        onSuccess: () => {
            const count = selectedIds.size;
            toast({
                variant: 'primary',
                title: `${count} dependent${count !== 1 ? 's' : ''} registered for this event!`,
            });
            setSelectedIds(new Set());
            onOpenChange(false);
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to register dependents.' });
        },
    });

    const toggleDependent = (id: string) => {
        setSelectedIds((prev) => {
            const next = new Set(prev);
            if (next.has(id)) {
                next.delete(id);
            } else {
                next.add(id);
            }
            return next;
        });
    };

    const handleRegister = () => {
        if (selectedIds.size === 0) return;
        registerMutation.mutate({ dependentIds: Array.from(selectedIds) });
    };

    const handleSkip = () => {
        setSelectedIds(new Set());
        onOpenChange(false);
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle className='flex items-center gap-2'>
                        <Baby className='h-5 w-5' />
                        Bring Dependents?
                    </DialogTitle>
                    <DialogDescription>
                        Select any dependents you plan to bring to this event.
                    </DialogDescription>
                </DialogHeader>

                <div className='space-y-3 py-2'>
                    {(dependents || []).map((dep) => (
                        <label
                            key={dep.id}
                            className='flex items-center gap-3 rounded-md border p-3 cursor-pointer hover:bg-accent'
                        >
                            <Checkbox
                                checked={selectedIds.has(dep.id)}
                                onCheckedChange={() => toggleDependent(dep.id)}
                            />
                            <div>
                                <div className='font-medium'>
                                    {dep.firstName} {dep.lastName}
                                </div>
                                <div className='text-sm text-muted-foreground'>{dep.relationship}</div>
                            </div>
                        </label>
                    ))}
                </div>

                <DialogFooter>
                    <Button variant='outline' onClick={handleSkip}>
                        Skip
                    </Button>
                    <Button onClick={handleRegister} disabled={selectedIds.size === 0 || registerMutation.isPending}>
                        {registerMutation.isPending
                            ? 'Registering...'
                            : `Register ${selectedIds.size > 0 ? `(${selectedIds.size})` : ''}`}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};
