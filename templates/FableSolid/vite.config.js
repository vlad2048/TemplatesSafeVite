import { defineConfig } from 'vite'
import solidPlugin from 'vite-plugin-solid';

export default defineConfig({
    plugins: [
        solidPlugin()
    ],
    root: "./src/Client",
    server: {
        port: 8080,
        proxy: {
            '/api': 'http://localhost:5000',
        }
    },
    build: {
        outDir:"./",
    }
})