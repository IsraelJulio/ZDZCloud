<script setup lang="ts">
import type { TableColumn } from '@nuxt/ui'

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

const config = useRuntimeConfig()
const apiBase = config.public.apiBase
const toast = useToast()

const form = reactive({ nome: '', descricao: '', preco: 0, categoriaId: null as number | null })
const nomeTocado = ref(false)
const categoriaTocada = ref(false)

const { data: produtos, refresh } = await useFetch<Produto[]>(`${apiBase}/api/produtos`)
const { data: categorias } = await useFetch<Categoria[]>(`${apiBase}/api/categorias`)

const showModal = ref(false)
const produtoEditando = ref<Produto | null>(null)
const showConfirm = ref(false)
const produtoParaDeletar = ref<Produto | null>(null)

const categoriasSelectItems = computed(() =>
  (categorias.value ?? []).map(c => ({ label: c.nome, value: c.id }))
)

const columns: TableColumn<Produto>[] = [
  { accessorKey: 'id', header: 'ID' },
  { accessorKey: 'nome', header: 'Nome' },
  { accessorKey: 'descricao', header: 'Descrição' },
  {
    id: 'preco',
    header: 'Preço',
    cell: ({ row }) => `R$ ${row.original.preco?.toFixed(2) ?? '0.00'}`
  },
  {
    id: 'categoria',
    header: 'Categoria',
    cell: ({ row }) => row.original.categoria?.nome ?? '-'
  },
  { id: 'acoes', header: 'Ações' }
]

async function salvar() {
  await $fetch(`${apiBase}/api/produtos`, {
    method: 'POST',
    body: {
      nome: form.nome,
      descricao: form.descricao,
      preco: form.preco,
      categoriaId: form.categoriaId
    }
  })
  form.nome = ''
  form.descricao = ''
  form.preco = 0
  form.categoriaId = null
  nomeTocado.value = false
  categoriaTocada.value = false
  await refresh()
  toast.add({ title: 'Produto criado com sucesso.', color: 'success', icon: 'i-lucide-check' })
}

function abrirModal(produto: Produto) {
  produtoEditando.value = { ...produto }
  showModal.value = true
}

function fecharModal() {
  showModal.value = false
  produtoEditando.value = null
}

function salvarEdicao(atualizado: Produto) {
  if (!produtos.value) return
  const idx = produtos.value.findIndex(p => p.id === atualizado.id)
  if (idx !== -1) produtos.value[idx] = atualizado
  fecharModal()
  toast.add({ title: 'Produto atualizado com sucesso.', color: 'success', icon: 'i-lucide-check' })
}

function confirmarDelete(produto: Produto) {
  produtoParaDeletar.value = produto
  showConfirm.value = true
}

function cancelarDelete() {
  showConfirm.value = false
  produtoParaDeletar.value = null
}

async function excluir() {
  if (!produtoParaDeletar.value) return
  const id = produtoParaDeletar.value.id
  showConfirm.value = false
  produtoParaDeletar.value = null

  try {
    await $fetch(`${apiBase}/api/produtos/${id}`, { method: 'DELETE' })
    if (produtos.value) {
      produtos.value = produtos.value.filter(p => p.id !== id)
    }
    toast.add({ title: 'Produto excluído com sucesso.', color: 'success', icon: 'i-lucide-check' })
  } catch (e: any) {
    toast.add({
      title: 'Erro ao excluir',
      description: e.data ?? 'Erro ao excluir produto.',
      color: 'error',
      icon: 'i-lucide-alert-circle'
    })
  }
}
</script>

<template>
  <div class="space-y-6">
    <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Produtos</h1>

    <UCard>
      <template #header>
        <h2 class="text-base font-semibold text-gray-700 dark:text-gray-200">Novo Produto</h2>
      </template>

      <div class="grid gap-4 max-w-md">
        <UFormField
          label="Nome"
          :error="nomeTocado && form.nome.length < 5 ? `Mínimo 5 caracteres (${form.nome.length}/5)` : undefined"
        >
          <UInput
            v-model="form.nome"
            placeholder="Nome do produto"
            @input="nomeTocado = true"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Descrição">
          <UInput v-model="form.descricao" placeholder="Descrição" class="w-full" />
        </UFormField>

        <UFormField label="Preço">
          <UInputNumber
            v-model="form.preco"
            :min="0"
            :step="0.01"
            placeholder="0.00"
            class="w-full"
          />
        </UFormField>

        <UFormField
          label="Categoria"
          :error="categoriaTocada && !form.categoriaId ? 'Selecione uma categoria' : undefined"
        >
          <USelect
            v-model="form.categoriaId"
            :items="categoriasSelectItems"
            placeholder="Selecione uma categoria"
            @change="categoriaTocada = true"
            class="w-full"
          />
        </UFormField>
      </div>

      <template #footer>
        <div class="flex justify-end">
          <UButton
            icon="i-lucide-plus"
            :disabled="form.nome.length < 5 || !form.categoriaId"
            @click="salvar"
          >
            Salvar
          </UButton>
        </div>
      </template>
    </UCard>

    <UCard>
      <template #header>
        <h2 class="text-base font-semibold text-gray-700 dark:text-gray-200">Lista de Produtos</h2>
      </template>

      <UTable :data="produtos ?? []" :columns="columns">
        <template #acoes-cell="{ row }">
          <div class="flex gap-2">
            <UButton
              size="sm"
              color="warning"
              variant="soft"
              icon="i-lucide-pencil"
              @click="abrirModal(row.original)"
            >
              Editar
            </UButton>
            <UButton
              size="sm"
              color="error"
              variant="soft"
              icon="i-lucide-trash-2"
              @click="confirmarDelete(row.original)"
            >
              Excluir
            </UButton>
          </div>
        </template>
      </UTable>

      <p
        v-if="!produtos || produtos.length === 0"
        class="text-center text-sm text-gray-400 italic py-6"
      >
        Nenhum produto cadastrado.
      </p>
    </UCard>

    <ProdutoModal
      v-if="showModal && produtoEditando"
      :produto="produtoEditando"
      :categorias="categorias ?? []"
      @fechar="fecharModal"
      @salvo="salvarEdicao"
    />

    <ConfirmDialog
      v-if="showConfirm"
      mensagem="Tem certeza que deseja excluir este produto?"
      @confirmar="excluir"
      @cancelar="cancelarDelete"
    />
  </div>
</template>
