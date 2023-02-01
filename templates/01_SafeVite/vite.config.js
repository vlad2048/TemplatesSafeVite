import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// https://vitejs.dev/config/
export default defineConfig({
	root: 'src/Client',
    plugins: [
		react()
	],
	server: {
		proxy: {
			'/api': { target: 'http://localhost:5000', ws: true },
			//'/socket': { target: 'http://localhost:5000', ws: false }
		}
	}
});
