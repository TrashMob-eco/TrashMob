import react from '@vitejs/plugin-react';
import path from 'path';
import { defineConfig } from 'vite';

export default defineConfig(() => {
    return {
        resolve: {
            alias: {
                '@': path.resolve(__dirname, './src'),
            },
        },
        build: {
            outDir: 'build',
            rollupOptions: {
                output: {
                    manualChunks: {
                        // Core React
                        'vendor-react': ['react', 'react-dom', 'react-router'],
                        // Auth
                        'vendor-msal': ['@azure/msal-browser', '@azure/msal-react'],
                        // Data fetching
                        'vendor-query': ['@tanstack/react-query', 'axios'],
                        // Google Maps
                        'vendor-maps': ['@vis.gl/react-google-maps'],
                        // UI components
                        'vendor-radix': [
                            '@radix-ui/react-alert-dialog',
                            '@radix-ui/react-checkbox',
                            '@radix-ui/react-collapsible',
                            '@radix-ui/react-dialog',
                            '@radix-ui/react-dropdown-menu',
                            '@radix-ui/react-hover-card',
                            '@radix-ui/react-icons',
                            '@radix-ui/react-label',
                            '@radix-ui/react-navigation-menu',
                            '@radix-ui/react-popover',
                            '@radix-ui/react-progress',
                            '@radix-ui/react-radio-group',
                            '@radix-ui/react-select',
                            '@radix-ui/react-separator',
                            '@radix-ui/react-slot',
                            '@radix-ui/react-switch',
                            '@radix-ui/react-tabs',
                            '@radix-ui/react-toast',
                            '@radix-ui/react-toggle',
                            '@radix-ui/react-toggle-group',
                            '@radix-ui/react-tooltip',
                        ],
                        // Icons
                        'vendor-icons': ['lucide-react'],
                        // Forms
                        'vendor-forms': ['react-hook-form', '@hookform/resolvers', 'zod'],
                        // Tables
                        'vendor-tables': ['@tanstack/react-table'],
                        // Utilities
                        'vendor-utils': ['lodash', 'moment', 'clsx', 'class-variance-authority'],
                    },
                },
            },
        },
        plugins: [react()],
        server: {
            port: 3000,
            proxy: {
                '/api': {
                    target: 'https://localhost:44332',
                    changeOrigin: true,
                    secure: false, // Allow self-signed certs for local dev
                },
            },
        },
        test: {
            environment: 'jsdom',
            globals: true,
            setupFiles: './tests/setup.ts',
            exclude: ['**/node_modules/**', '**/e2e/**'],
        },
    };
});
