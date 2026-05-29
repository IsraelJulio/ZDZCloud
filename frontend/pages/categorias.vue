<script setup lang="ts">
import type { TableColumn } from '@nuxt/ui'

interface Categoria {
  id: number
  nome: string
  descricao: string
}

const config = useRuntimeConfig()
const apiBase = config.public.apiBase
const toast = useToast()

const form = reactive({ nome: '', descricao: '' })
const nomeTocado = ref(false)

const { data: categorias, refresh } = await useFetch<Categoria[]>(`${apiBase}/api/categorias`)

const showModal = ref(false)
const categoriaEditando = ref<Categoria | null>(null)
const showConfirm = ref(false)
const categoriaParaDeletar = ref<Categoria | null>(null)

const columns: TableColumn<Categoria>[] = [
  { accessorKey: 'id', header: 'ID' },
  { accessorKey: 'nome', header: 'Nome' },
  { accessorKey: 'descricao', header: 'Descrição' },
  { id: 'acoes', header: 'Ações' }
]

async function salvar() {
  await $fetch(`${apiBase}/api/categorias`, {
    method: 'POST',
    body: { nome: form.nome, descricao: form.descricao }
  })
  form.nome = ''
  form.descricao = ''
  nomeTocado.value = false
  await refresh()
  toast.add({ title: 'Categoria criada com sucesso.', color: 'success', icon: 'i-lucide-check' })
}

function abrirModal(categoria: Categoria) {
  categoriaEditando.value = { ...categoria }
  showModal.value = true
}

function fecharModal() {
  showModal.value = false
  categoriaEditando.value = null
}

function salvarEdicao(atualizada: Categoria) {
  if (!categorias.value) return
  categorias.value = categorias.value.map(c => c.id === atualizada.id ? atualizada : c)
  fecharModal()
  toast.add({ title: 'Categoria atualizada com sucesso.', color: 'success', icon: 'i-lucide-check' })
}

function confirmarDelete(categoria: Categoria) {
  categoriaParaDeletar.value = categoria
  showConfirm.value = true
}

function cancelarDelete() {
  showConfirm.value = false
  categoriaParaDeletar.value = null
}

async function excluir() {
  if (!categoriaParaDeletar.value) return
  const id = categoriaParaDeletar.value.id
  showConfirm.value = false
  categoriaParaDeletar.value = null

  try {
    await $fetch(`${apiBase}/api/categorias/${id}`, { method: 'DELETE' })
    if (categorias.value) {
      categorias.value = categorias.value.filter(c => c.id !== id)
    }
    toast.add({ title: 'Categoria excluída com sucesso.', color: 'success', icon: 'i-lucide-check' })
  } catch (e: any) {
    toast.add({
      title: 'Erro ao excluir',
      description: e.data ?? 'Erro ao excluir categoria.',
      color: 'error',
      icon: 'i-lucide-alert-circle'
    })
  }
}
</script>

<template>
  <div class="space-y-6">
    <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Categorias</h1>

    <UCard>
      <template #header>
        <h2 class="text-base font-semibold text-gray-700 dark:text-gray-200">Nova Categoria</h2>
      </template>

      <div class="grid gap-4 max-w-md">
        <UFormField
          label="Nome"
          :error="nomeTocado && form.nome.length < 5 ? `Mínimo 5 caracteres (${form.nome.length}/5)` : undefined"
        >
          <UInput
            v-model="form.nome"
            placeholder="Nome da categoria"
            @input="nomeTocado = true"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Descrição">
          <UInput v-model="form.descricao" placeholder="Descrição" class="w-full" />
        </UFormField>
      </div>

      <template #footer>
        <div class="flex justify-end">
          <UButton
            icon="i-lucide-plus"
            :disabled="form.nome.length < 5"
            @click="salvar"
          >
            Salvar
          </UButton>
        </div>
      </template>
    </UCard>

    <UCard>
      <template #header>
        <h2 class="text-base font-semibold text-gray-700 dark:text-gray-200">Lista de Categorias</h2>
      </template>

      <UTable :data="categorias ?? []" :columns="columns">
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
        v-if="!categorias || categorias.length === 0"
        class="text-center text-sm text-gray-400 italic py-6"
      >
        Nenhuma categoria cadastrada.
      </p>
    </UCard>

    <CategoriaModal
      v-if="showModal && categoriaEditando"
      :categoria="categoriaEditando"
      @fechar="fecharModal"
      @salvo="salvarEdicao"
    />

    <ConfirmDialog
      v-if="showConfirm"
      mensagem="Tem certeza que deseja excluir esta categoria?"
      @confirmar="excluir"
      @cancelar="cancelarDelete"
    />
  </div>
</template>
