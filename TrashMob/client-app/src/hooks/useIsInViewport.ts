import { useState, useCallback, useEffect, useRef } from 'react';

export function useIsInViewport<T extends HTMLElement = HTMLDivElement>() {
    const [isIntersecting, setIntersecting] = useState(false);
    const observerRef = useRef<IntersectionObserver | null>(null);

    const ref = useCallback((node: T | null) => {
        if (observerRef.current) {
            observerRef.current.disconnect();
        }

        if (node) {
            observerRef.current = new IntersectionObserver(([entry]) => setIntersecting(entry.isIntersecting));
            observerRef.current.observe(node);
        }
    }, []);

    useEffect(() => {
        return () => observerRef.current?.disconnect();
    }, []);

    return { ref, isInViewPort: isIntersecting };
}
