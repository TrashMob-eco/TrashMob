import path from "path"
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig(() => {
    return {
        resolve: {
            alias: {
                "@": path.resolve(__dirname, "./src"),
            },
        },
        build: {
            outDir: 'build',
        },
        plugins: [react()],
        server: {
            port: 3000,
        },
        test: {
            environment: 'jsdom',
            globals: true,
            setupFiles: './tests/setup.ts',
        },
    };
});
