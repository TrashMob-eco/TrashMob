import { RefObject, useState, useMemo, useEffect, useRef } from 'react';

export function useIsInViewport<T extends HTMLElement = HTMLDivElement>() {
    const ref = useRef<T>(null);
    const [isIntersecting, setIntersecting] = useState(false);

    const observer = useMemo(() => new IntersectionObserver(([entry]) => setIntersecting(entry.isIntersecting)), []);

    useEffect(() => {
        if (ref.current) {
            observer.observe(ref.current);
        }
        return () => observer.disconnect();
    }, []);

    return { ref, isInViewPort: isIntersecting };
}
