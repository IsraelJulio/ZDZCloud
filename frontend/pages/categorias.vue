<script setup lang="ts">
interface Categoria {
  id: number
  nome: string
  descricao: string
}

const config = useRuntimeConfig()
const apiBase = config.public.apiBase

const form = reactive({ nome: '', descricao: '' })
const nomeTocado = ref(false)

const { data: categorias, refresh } = await useFetch<Categoria[]>(`${apiBase}/api/categorias`)

const showModal = ref(false)
const categoriaEditando = ref<Categoria | null>(null)
const showConfirm = ref(false)
const categoriaParaDeletar = ref<Categoria | null>(null)
const errorToast = ref<string | null>(null)
const successToast = ref<string | null>(null)

let toastTimer: ReturnType<typeof setTimeout> | null = null
let successTimer: ReturnType<typeof setTimeout> | null = null

function mostrarErro(mensagem: string) {
  errorToast.value = mensagem
  if (toastTimer) clearTimeout(toastTimer)
  toastTimer = setTimeout(() => { errorToast.value = null }, 5000)
}

function mostrarSucesso(mensagem: string) {
  successToast.value = mensagem
  if (successTimer) clearTimeout(successTimer)
  successTimer = setTimeout(() => { successToast.value = null }, 3000)
}

async function salvar() {
  await $fetch(`${apiBase}/api/categorias`, {
    method: 'POST',
    body: { nome: form.nome, descricao: form.descricao }
  })
  form.nome = ''
  form.descricao = ''
  nomeTocado.value = false
  await refresh()
  mostrarSucesso('Categoria criada com sucesso.')
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
  const idx = categorias.value.findIndex(c => c.id === atualizada.id)
  if (idx !== -1) categorias.value[idx] = atualizada
  fecharModal()
  mostrarSucesso('Categoria atualizada com sucesso.')
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
    mostrarSucesso('Categoria excluída com sucesso.')
  } catch (e: any) {
    mostrarErro(e.data ?? 'Erro ao excluir categoria.')
  }
}
</script>

<template>
  <div>
    <h1 class="page-title">Categorias</h1>

    <div v-if="successToast" class="toast-success">
      {{ successToast }}
      <button @click="successToast = null" class="toast-close">✕</button>
    </div>

    <div v-if="errorToast" class="toast-error">
      {{ errorToast }}
      <button @click="errorToast = null" class="toast-close">✕</button>
    </div>

    <div class="form-card">
      <h2 class="form-title">Nova Categoria</h2>
      <div class="form-group">
        <label>Nome</label>
        <input
          v-model="form.nome"
          type="text"
          placeholder="Nome da categoria"
          :class="{ 'input-error': nomeTocado && form.nome.length < 5 }"
          @input="nomeTocado = true"
        />
        <span v-if="nomeTocado && form.nome.length < 5" class="field-hint error">
          Mínimo 5 caracteres ({{ form.nome.length }}/5)
        </span>
      </div>
      <div class="form-group">
        <label>Descrição</label>
        <input v-model="form.descricao" type="text" placeholder="Descrição" />
      </div>
      <button
        @click="salvar"
        :disabled="form.nome.length < 5"
        :title="form.nome.length < 5 ? 'O nome deve ter pelo menos 5 caracteres' : ''"
        class="btn btn-primary"
      >
        Salvar
      </button>
    </div>

    <div class="table-wrapper">
      <table class="data-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Nome</th>
            <th>Descrição</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="!categorias || categorias.length === 0">
            <td colspan="4" class="empty">Nenhuma categoria cadastrada.</td>
          </tr>
          <tr v-for="cat in categorias" :key="cat.id">
            <td>{{ cat.id }}</td>
            <td>{{ cat.nome }}</td>
            <td>{{ cat.descricao }}</td>
            <td class="acoes">
              <button @click="abrirModal(cat)" class="btn btn-edit">Editar</button>
              <button @click="confirmarDelete(cat)" class="btn btn-delete">Excluir</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

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

<style scoped>
.page-title {
  font-size: 1.5rem;
  font-weight: 700;
  margin-bottom: 1.5rem;
  color: #1e293b;
}

.toast-success {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background-color: #f0fdf4;
  border: 1px solid #86efac;
  border-radius: 8px;
  padding: 0.75rem 1rem;
  margin-bottom: 1.25rem;
  color: #15803d;
  font-size: 0.9rem;
}

.toast-success .toast-close {
  color: #15803d;
}

.toast-error {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background-color: #fef2f2;
  border: 1px solid #fca5a5;
  border-radius: 8px;
  padding: 0.75rem 1rem;
  margin-bottom: 1.25rem;
  color: #b91c1c;
  font-size: 0.9rem;
}

.toast-close {
  background: none;
  border: none;
  cursor: pointer;
  color: #b91c1c;
  font-size: 1rem;
  padding: 0 0.25rem;
  line-height: 1;
}

.form-card {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  padding: 1.5rem;
  margin-bottom: 2rem;
  max-width: 480px;
}

.form-title {
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: #334155;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  margin-bottom: 1rem;
}

.form-group label {
  font-size: 0.875rem;
  font-weight: 500;
  color: #475569;
}

.form-group input {
  padding: 0.5rem 0.75rem;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 0.9rem;
  outline: none;
  transition: border-color 0.15s;
}

.form-group input:focus {
  border-color: #3b82f6;
}

.form-group input.input-error {
  border-color: #ef4444;
}

.field-hint {
  font-size: 0.78rem;
  color: #94a3b8;
  margin-top: 0.1rem;
}

.field-hint.error {
  color: #ef4444;
}

.btn {
  padding: 0.5rem 1.25rem;
  border: none;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.15s, opacity 0.15s;
}

.btn:disabled {
  opacity: 0.45;
  cursor: not-allowed;
}

.btn-primary {
  background-color: #3b82f6;
  color: white;
}

.btn-primary:not(:disabled):hover {
  background-color: #2563eb;
}

.btn-edit {
  background-color: #f59e0b;
  color: white;
  margin-right: 0.5rem;
}

.btn-edit:hover {
  background-color: #d97706;
}

.btn-delete {
  background-color: #ef4444;
  color: white;
}

.btn-delete:hover {
  background-color: #dc2626;
}

.table-wrapper {
  overflow-x: auto;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  overflow: hidden;
}

.data-table th {
  background-color: #f8fafc;
  padding: 0.75rem 1rem;
  text-align: left;
  font-size: 0.8rem;
  font-weight: 600;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.03em;
  border-bottom: 1px solid #e2e8f0;
}

.data-table td {
  padding: 0.75rem 1rem;
  font-size: 0.9rem;
  color: #334155;
  border-bottom: 1px solid #f1f5f9;
}

.data-table tr:last-child td {
  border-bottom: none;
}

.data-table tr:hover td {
  background-color: #f8fafc;
}

.acoes {
  white-space: nowrap;
}

.empty {
  text-align: center;
  color: #94a3b8;
  font-style: italic;
}
</style>
