import mkcert from 'vite-plugin-mkcert'
import { splitVendorChunkPlugin } from 'vite';

export default {
    // config options
    server: {
        https: true,
        port: 3000,
        proxy: {
            '/signin': {
                target: 'https://localhost:5001/',
                changeOrigin: true,
                secure: false,
                ws: true,
            },
            '/signout': {
                target: 'https://localhost:5001/',
                changeOrigin: true,
                secure: false,
                ws: true,
            },
            '/api': {
                 target: 'https://localhost:5001/',
                 changeOrigin: true,
                 secure: false,
                 ws: true,
             }
        }
    },
    plugins: [
        mkcert(),
        splitVendorChunkPlugin()
    ]
}