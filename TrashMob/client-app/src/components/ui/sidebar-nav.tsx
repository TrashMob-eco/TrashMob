import { useCallback } from 'react';
import { Link, useLocation } from 'react-router';
import { cn } from '@/lib/utils';
import { LucideIcon } from 'lucide-react';

export interface NavItem {
    name: string;
    href: string;
    icon?: LucideIcon;
}

export interface NavGroup {
    title: string;
    items: NavItem[];
}

interface SidebarNavProps {
    groups: NavGroup[];
    className?: string;
}

export function SidebarNav({ groups, className }: SidebarNavProps) {
    const location = useLocation();

    const isActive = useCallback(
        (href: string) => {
            // Exact match for index routes, prefix match for nested routes
            return location.pathname === href || location.pathname.startsWith(href + '/');
        },
        [location.pathname],
    );

    return (
        <nav className={cn('space-y-6', className)}>
            {groups.map((group) => (
                <div key={group.title}>
                    <h3 className='mb-2 px-3 text-xs font-semibold uppercase tracking-wider text-muted-foreground'>
                        {group.title}
                    </h3>
                    <ul className='space-y-1'>
                        {group.items.map((item) => {
                            const active = isActive(item.href);
                            const Icon = item.icon;
                            return (
                                <li key={item.href}>
                                    <Link
                                        to={item.href}
                                        className={cn(
                                            'flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors',
                                            active
                                                ? 'bg-primary text-primary-foreground'
                                                : 'text-muted-foreground hover:bg-muted hover:text-foreground',
                                        )}
                                    >
                                        {Icon ? <Icon className='h-4 w-4 shrink-0' /> : null}
                                        <span>{item.name}</span>
                                    </Link>
                                </li>
                            );
                        })}
                    </ul>
                </div>
            ))}
        </nav>
    );
}
