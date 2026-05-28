<script setup lang="ts">
interface Categoria {
  id: number
  nome: string
  descricao: string
}

const props = defineProps<{ categoria: Categoria }>()
const emit = defineEmits<{ fechar: []; salvo: [categoria: Categoria] }>()

const config = useRuntimeConfig()
const apiBase = config.public.apiBase

const form = reactive({
  nome: props.categoria.nome,
  descricao: props.categoria.descricao
})
const nomeTocado = ref(false)
const open = ref(true)

watch(open, (val) => {
  if (!val) emit('fechar')
})

async function salvar() {
  const atualizada = await $fetch<Categoria>(`${apiBase}/api/categorias/${props.categoria.id}`, {
    method: 'PUT',
    body: { nome: form.nome, descricao: form.descricao }
  })
  emit('salvo', atualizada)
}
</script>

<template>
  <UModal v-model:open="open" title="Editar Categoria">
    <template #body>
      <div class="grid gap-4">
        <UFormField
          label="Nome"
          :error="nomeTocado && form.nome.length < 5 ? `Mínimo 5 caracteres (${form.nome.length}/5)` : undefined"
        >
          <UInput
            v-model="form.nome"
            @input="nomeTocado = true"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Descrição">
          <UInput v-model="form.descricao" class="w-full" />
        </UFormField>
      </div>
    </template>

    <template #footer>
      <div class="flex justify-end gap-3 w-full">
        <UButton color="neutral" variant="ghost" @click="open = false">Cancelar</UButton>
        <UButton :disabled="form.nome.length < 5" @click="salvar">Salvar</UButton>
      </div>
    </template>
  </UModal>
</template>
