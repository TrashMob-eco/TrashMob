import { useCallback } from 'react';
import { cn } from '@/lib/utils';
import { LucideIcon } from 'lucide-react';

export interface DashboardNavItem {
    id: string;
    label: string;
    icon?: LucideIcon;
}

export interface DashboardNavGroup {
    title: string;
    items: DashboardNavItem[];
}

interface DashboardScrollNavProps {
    groups: DashboardNavGroup[];
    activeId: string;
    className?: string;
}

export function DashboardScrollNav({ groups, activeId, className }: DashboardScrollNavProps) {
    const handleClick = useCallback((id: string) => {
        const el = document.getElementById(id);
        if (el) {
            el.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
    }, []);

    return (
        <nav aria-label='Dashboard sections' className={cn('space-y-6', className)}>
            {groups.map((group) => (
                <div key={group.title}>
                    <h3 className='mb-2 px-3 text-xs font-semibold uppercase tracking-wider text-muted-foreground'>
                        {group.title}
                    </h3>
                    <ul className='space-y-1'>
                        {group.items.map((item) => {
                            const active = activeId === item.id;
                            const Icon = item.icon;
                            return (
                                <li key={item.id}>
                                    <button
                                        type='button'
                                        onClick={() => handleClick(item.id)}
                                        className={cn(
                                            'flex w-full items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors text-left',
                                            active
                                                ? 'bg-primary text-primary-foreground'
                                                : 'text-muted-foreground hover:bg-muted hover:text-foreground',
                                        )}
                                    >
                                        {Icon ? <Icon className='h-4 w-4 shrink-0' /> : null}
                                        <span>{item.label}</span>
                                    </button>
                                </li>
                            );
                        })}
                    </ul>
                </div>
            ))}
        </nav>
    );
}
