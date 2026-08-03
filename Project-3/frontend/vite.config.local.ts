/**
 * Local development config — use this when running outside of Replit.
 * Compatible with Node.js 18+.
 *
 * API calls are proxied through Vite to avoid CORS issues.
 * Set VITE_BACKEND_URL in .env.local to your backend origin.
 * Defaults to https://localhost:7001 (ASP.NET Core HTTPS default).
 *
 * Run with: npm run dev:local
 */
import { fileURLToPath } from 'node:url';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';
import { defineConfig, loadEnv } from 'vite';

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '');
  const backendUrl = env.VITE_BACKEND_URL || 'https://localhost:7001';

  return {
    base: '/',
    plugins: [react(), tailwindcss()],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
      },
      dedupe: ['react', 'react-dom'],
    },
    build: {
      outDir: 'dist/public',
      emptyOutDir: true,
    },
    server: {
      port: 5173,
      host: '0.0.0.0',
      open: true,
      // Proxy /api/* to the .NET backend — eliminates CORS and cert issues
      proxy: {
        '/api': {
          target: backendUrl,
          changeOrigin: true,
          secure: false, // allows self-signed certs on localhost
        },
      },
    },
  };
});
