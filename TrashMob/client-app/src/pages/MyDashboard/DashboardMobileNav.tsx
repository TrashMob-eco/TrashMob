import { useCallback, useEffect, useRef } from 'react';
import { cn } from '@/lib/utils';
import { DashboardNavItem } from './DashboardScrollNav';

interface DashboardMobileNavProps {
    items: DashboardNavItem[];
    activeSection: string | null;
    onSectionChange: (id: string | null) => void;
}

export function DashboardMobileNav({ items, activeSection, onSectionChange }: DashboardMobileNavProps) {
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
    }, [activeSection]);

    const handleClick = useCallback(
        (id: string | null) => {
            onSectionChange(id);
            window.scrollTo({ top: 0, behavior: 'smooth' });
        },
        [onSectionChange],
    );

    const isAllActive = activeSection === null;

    return (
        <nav
            ref={scrollRef}
            aria-label='Dashboard sections'
            className='sticky top-0 z-30 flex gap-2 overflow-x-auto bg-background/95 backdrop-blur px-4 py-2 border-b scrollbar-hide'
        >
            <button
                ref={isAllActive ? activeButtonRef : undefined}
                type='button'
                onClick={() => handleClick(null)}
                className={cn(
                    'shrink-0 rounded-full px-3 py-1.5 text-xs font-medium transition-colors whitespace-nowrap',
                    isAllActive
                        ? 'bg-primary text-primary-foreground'
                        : 'bg-muted text-muted-foreground hover:bg-muted/80',
                )}
            >
                All
            </button>
            {items.map((item) => {
                const active = activeSection === item.id;
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
