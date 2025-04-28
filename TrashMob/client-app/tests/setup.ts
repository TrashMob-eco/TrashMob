import { vi } from 'vitest';
import '@testing-library/jest-dom';

vi.stubGlobal('scrollTo', vi.fn);

// Mock URL.createObjectURL (used by azure-map-control)
const mockURL = {
    createObjectURL: vi.fn(),
};
vi.stubGlobal('URL', mockURL);

// Mock crypto (used by azure-map-control & msal)
vi.stubGlobal('crypto', {
    subtle: vi.fn(),
    getRandomValues: (arr: number[]) => {
        // Mock getRandomValues with a basic implementation
        for (let i = 0; i < arr.length; i++) {
            arr[i] = Math.floor(Math.random() * 256);
        }
        return arr;
    },
});

vi.stubGlobal(
    'ResizeObserver',
    class {
        observe = vi.fn();
        unobserve = vi.fn();
        disconnect = vi.fn();
    },
);
