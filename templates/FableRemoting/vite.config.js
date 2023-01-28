import { defineConfig } from 'vite'

export default defineConfig({
    plugins: [
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