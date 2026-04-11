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
                    manualChunks(id) {
                        if (
                            id.includes('node_modules/react/') ||
                            id.includes('node_modules/react-dom/') ||
                            id.includes('node_modules/react-router/')
                        )
                            return 'vendor-react';
                        if (id.includes('node_modules/@azure/msal-')) return 'vendor-msal';
                        if (id.includes('node_modules/@tanstack/react-query') || id.includes('node_modules/axios/'))
                            return 'vendor-query';
                        if (id.includes('node_modules/@vis.gl/react-google-maps')) return 'vendor-maps';
                        if (id.includes('node_modules/@radix-ui/')) return 'vendor-radix';
                        if (id.includes('node_modules/lucide-react/')) return 'vendor-icons';
                        if (
                            id.includes('node_modules/react-hook-form/') ||
                            id.includes('node_modules/@hookform/') ||
                            id.includes('node_modules/zod/')
                        )
                            return 'vendor-forms';
                        if (id.includes('node_modules/@tanstack/react-table')) return 'vendor-tables';
                        if (
                            id.includes('node_modules/lodash/') ||
                            id.includes('node_modules/moment/') ||
                            id.includes('node_modules/clsx/') ||
                            id.includes('node_modules/class-variance-authority/')
                        )
                            return 'vendor-utils';
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
