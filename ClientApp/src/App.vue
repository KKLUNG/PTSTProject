<template>
  <Suspense>
    <template #default>
      <LoginForm />
    </template>
    <template #fallback>
      <div class="loading">載入中...</div>
    </template>
  </Suspense>
</template>

<script setup lang="ts">
import { defineAsyncComponent } from 'vue'

const LoginForm = defineAsyncComponent(() => import('./views/CMSLogin.vue'))
</script>

<style>
.loading { max-width: 420px; margin: 40px auto; font-family: system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif; }
</style>
