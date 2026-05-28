<script setup lang="ts">
interface Categoria {
  id: number
  nome: string
}

interface Produto {
  id: number
  nome: string
  descricao: string
  preco: number
  categoriaId: number
  categoria: Categoria
}

const props = defineProps<{
  produto: Produto
  categorias: Categoria[]
}>()

const emit = defineEmits<{ fechar: []; salvo: [produto: Produto] }>()

const config = useRuntimeConfig()
const apiBase = config.public.apiBase

const form = reactive({
  nome: props.produto.nome,
  descricao: props.produto.descricao,
  preco: props.produto.preco,
  categoriaId: props.produto.categoriaId
})
const nomeTocado = ref(false)
const open = ref(true)

const categoriasSelectItems = computed(() =>
  props.categorias.map(c => ({ label: c.nome, value: c.id }))
)

watch(open, (val) => {
  if (!val) emit('fechar')
})

async function salvar() {
  const atualizado = await $fetch<Produto>(`${apiBase}/api/produtos/${props.produto.id}`, {
    method: 'PUT',
    body: {
      nome: form.nome,
      descricao: form.descricao,
      preco: form.preco,
      categoriaId: form.categoriaId
    }
  })
  emit('salvo', atualizado)
}
</script>

<template>
  <UModal v-model:open="open" title="Editar Produto">
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

        <UFormField label="Preço">
          <UInputNumber v-model="form.preco" :min="0" :step="0.01" class="w-full" />
        </UFormField>

        <UFormField label="Categoria">
          <USelect
            v-model="form.categoriaId"
            :items="categoriasSelectItems"
            class="w-full"
          />
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
