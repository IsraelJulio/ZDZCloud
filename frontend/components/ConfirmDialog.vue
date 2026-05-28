<script setup lang="ts">
defineProps<{ mensagem: string }>()
const emit = defineEmits<{ confirmar: []; cancelar: [] }>()

const open = ref(true)
let acao: 'confirmar' | 'cancelar' | null = null

function resolver(resultado: 'confirmar' | 'cancelar') {
  acao = resultado
  open.value = false
}

watch(open, (val) => {
  if (!val) emit(acao ?? 'cancelar')
})
</script>

<template>
  <UModal v-model:open="open" title="Confirmação" :dismissible="true">
    <template #body>
      <p class="text-sm text-gray-600 dark:text-gray-400">{{ mensagem }}</p>
    </template>

    <template #footer>
      <div class="flex justify-end gap-3 w-full">
        <UButton color="neutral" variant="ghost" @click="resolver('cancelar')">Cancelar</UButton>
        <UButton color="error" @click="resolver('confirmar')">Confirmar</UButton>
      </div>
    </template>
  </UModal>
</template>
