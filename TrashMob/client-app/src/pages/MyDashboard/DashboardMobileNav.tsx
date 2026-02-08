import { useCallback, useEffect, useRef } from 'react';
import { cn } from '@/lib/utils';
import { DashboardNavItem } from './DashboardScrollNav';

interface DashboardMobileNavProps {
    items: DashboardNavItem[];
    activeId: string;
}

export function DashboardMobileNav({ items, activeId }: DashboardMobileNavProps) {
    const scrollRef = useRef<HTMLDivElement>(null);
    const activeButtonRef = useRef<HTMLButtonElement>(null);

    useEffect(() => {
        if (activeButtonRef.current && scrollRef.current) {
            activeButtonRef.current.scrollIntoView({
                behavior: 'smooth',
                block: 'nearest',
                inline: 'center',
            });
        }
    }, [activeId]);

    const handleClick = useCallback((id: string) => {
        const el = document.getElementById(id);
        if (el) {
            el.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
    }, []);

    return (
        <nav
            ref={scrollRef}
            aria-label='Dashboard sections'
            className='sticky top-0 z-30 flex gap-2 overflow-x-auto bg-background/95 backdrop-blur px-4 py-2 border-b scrollbar-hide'
        >
            {items.map((item) => {
                const active = activeId === item.id;
                return (
                    <button
                        key={item.id}
                        ref={active ? activeButtonRef : undefined}
                        type='button'
                        onClick={() => handleClick(item.id)}
                        className={cn(
                            'shrink-0 rounded-full px-3 py-1.5 text-xs font-medium transition-colors whitespace-nowrap',
                            active
                                ? 'bg-primary text-primary-foreground'
                                : 'bg-muted text-muted-foreground hover:bg-muted/80',
                        )}
                    >
                        {item.label}
                    </button>
                );
            })}
        </nav>
    );
}
