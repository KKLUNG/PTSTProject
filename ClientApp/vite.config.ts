import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

const publicPath = "";

export default defineConfig({
  base: publicPath,
  plugins: [vue()],
  server: {
    port: 8080,
    proxy: {
      '/api': {
        target: 'http://localhost:34794',
        changeOrigin: true
      }
    }
  }
})


