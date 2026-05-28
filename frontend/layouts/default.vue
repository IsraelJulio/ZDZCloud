<script setup lang="ts">
const colorMode = useColorMode()
const isDark = computed({
  get: () => colorMode.value === 'dark',
  set: (val) => { colorMode.preference = val ? 'dark' : 'light' }
})
</script>

<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-950">
    <header class="bg-gray-900 border-b border-gray-800 sticky top-0 z-50">
      <div class="max-w-7xl mx-auto px-6 h-16 flex items-center gap-8">
        <span class="text-white font-bold text-xl tracking-tight">ZDZCloud</span>
        <nav class="flex gap-1 flex-1">
          <NuxtLink
            to="/categorias"
            class="px-3 py-1.5 rounded-md text-sm font-medium text-gray-400 hover:text-white hover:bg-gray-800 transition-colors"
            active-class="text-white bg-gray-800"
          >
            Categorias
          </NuxtLink>
          <NuxtLink
            to="/produtos"
            class="px-3 py-1.5 rounded-md text-sm font-medium text-gray-400 hover:text-white hover:bg-gray-800 transition-colors"
            active-class="text-white bg-gray-800"
          >
            Produtos
          </NuxtLink>
        </nav>
        <UButton
          :icon="isDark ? 'i-lucide-sun' : 'i-lucide-moon'"
          variant="ghost"
          color="neutral"
          size="sm"
          :aria-label="isDark ? 'Mudar para tema claro' : 'Mudar para tema escuro'"
          @click="isDark = !isDark"
        />
      </div>
    </header>
    <main class="max-w-7xl mx-auto px-6 py-8">
      <slot />
    </main>
  </div>
</template>
