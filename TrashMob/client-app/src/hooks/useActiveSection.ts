import { useState, useEffect, useRef } from 'react';

interface UseActiveSectionOptions {
    sectionIds: string[];
    rootMargin?: string;
}

export function useActiveSection({
    sectionIds,
    rootMargin = '-80px 0px -60% 0px',
}: UseActiveSectionOptions) {
    const [activeId, setActiveId] = useState<string>('');
    const observerRef = useRef<IntersectionObserver | null>(null);

    useEffect(() => {
        const visibleEntries = new Map<string, number>();

        observerRef.current = new IntersectionObserver(
            (entries) => {
                entries.forEach((entry) => {
                    visibleEntries.set(entry.target.id, entry.intersectionRatio);
                });

                for (const id of sectionIds) {
                    const ratio = visibleEntries.get(id);
                    if (ratio && ratio > 0) {
                        setActiveId(id);
                        return;
                    }
                }
            },
            {
                rootMargin,
                threshold: [0, 0.1, 0.2, 0.5],
            },
        );

        const elements = sectionIds.map((id) => document.getElementById(id)).filter(Boolean) as HTMLElement[];

        elements.forEach((el) => observerRef.current?.observe(el));

        return () => {
            observerRef.current?.disconnect();
        };
    }, [sectionIds, rootMargin]);

    return activeId;
}
