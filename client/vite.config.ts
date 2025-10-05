import { defineConfig } from 'vite';
import path from "path";
import { svelte } from '@sveltejs/vite-plugin-svelte';
import tailwindcss from '@tailwindcss/vite';

// https://vitejs.dev/config/
export default defineConfig({
    base: './',
    build: {
        target: 'esnext',
        rollupOptions: {
            output: {
                entryFileNames: "[name].js",
                assetFileNames: "[name][extname]"
            }
        }
    },
    plugins: [svelte(), tailwindcss()],
    resolve: {
        alias: {
            $lib: path.resolve("./src/lib"),
            $api: path.resolve("./src/api"),
        },
    }
});
