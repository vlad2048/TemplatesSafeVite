import { resolve } from 'path'
import { defineConfig } from 'vite'

console.log('DD=', __dirname);
console.log('F0=', resolve(__dirname, 'index.html'));
console.log('F1=', resolve(__dirname, 'src/Client/index.html'));

export default defineConfig({
    plugins: [
    ],
    root: "./src/Client",
    //publicDir: "output",
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