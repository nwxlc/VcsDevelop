import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
    plugins: [react()],

    server: {
        host: '0.0.0.0',
        port: 5173,

        allowedHosts: ['frontend'],

        proxy: {
            '/api': {
                target: 'http://webapi:8080',
                changeOrigin: true,
                secure: false,
                ws: true,

                configure: (proxy) => {
                    proxy.on('error', (err) => {
                        console.log('proxy error', err)
                    })
                }
            },

            '/scalar': {
                target: 'http://webapi:8080',
                changeOrigin: true,
                secure: false
            }
        }
    }
})