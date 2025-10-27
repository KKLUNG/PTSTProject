/// <reference types="vite/client" />

// Vite 環境變數型別定義
interface ImportMetaEnv {
  readonly VITE_DEVEXTREME_LICENSE_KEY?: string
  readonly VITE_API_BASE_URL?: string
  readonly VITE_APP_TITLE?: string
  readonly VITE_DEBUG?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}

