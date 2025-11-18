import { defineConfig } from "vite"
import vue from "@vitejs/plugin-vue"
import { fileURLToPath, URL } from "node:url"

import appConfig from "./app-config"

const { publicPath, outputDir } = appConfig

export default defineConfig({
  base: publicPath || "/",
  plugins: [vue()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  css: {
    preprocessorOptions: {
      scss: {
        // 抑制 legacy-js-api 棄用警告
        // 這個警告來自 sass-embedded，在 Vite 中暫時無法完全避免
        // 功能正常，只是 API 會在未來版本中改變
        silenceDeprecations: ['legacy-js-api'],
      },
    },
  },
  build: {
    outDir: outputDir,
    sourcemap: false,
  },
})
