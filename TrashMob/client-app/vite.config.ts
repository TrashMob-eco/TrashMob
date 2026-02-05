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
