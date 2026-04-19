import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
    plugins: [react()],
    server: {
        port: 5237,
        proxy: {
            '/api': {
                target: 'https://localhost:7031',
                changeOrigin: true,
                secure: false
            },
            '/scalar': {
                target: 'https://localhost:7031',
                changeOrigin: true,
                secure: false,
            }
        }
    }
})