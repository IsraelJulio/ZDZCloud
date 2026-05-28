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

const config = useRuntimeConfig()
const apiBase = config.public.apiBase

const form = reactive({ nome: '', descricao: '', preco: 0, categoriaId: '' })
const nomeTocado = ref(false)
const categoriaTocada = ref(false)

const { data: produtos, refresh } = await useFetch<Produto[]>(`${apiBase}/api/produtos`)
const { data: categorias } = await useFetch<Categoria[]>(`${apiBase}/api/categorias`)

const showModal = ref(false)
const produtoEditando = ref<Produto | null>(null)
const showConfirm = ref(false)
const produtoParaDeletar = ref<Produto | null>(null)
const errorToast = ref<string | null>(null)

let toastTimer: ReturnType<typeof setTimeout> | null = null

function mostrarErro(mensagem: string) {
  errorToast.value = mensagem
  if (toastTimer) clearTimeout(toastTimer)
  toastTimer = setTimeout(() => { errorToast.value = null }, 5000)
}

async function salvar() {
  await $fetch(`${apiBase}/api/produtos`, {
    method: 'POST',
    body: {
      nome: form.nome,
      descricao: form.descricao,
      preco: form.preco,
      categoriaId: Number(form.categoriaId)
    }
  })
  form.nome = ''
  form.descricao = ''
  form.preco = 0
  form.categoriaId = ''
  nomeTocado.value = false
  categoriaTocada.value = false
  await refresh()
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
  } catch (e: any) {
    mostrarErro(e.data ?? 'Erro ao excluir produto.')
  }
}
</script>

<template>
  <div>
    <h1 class="page-title">Produtos</h1>

    <div v-if="errorToast" class="toast-error">
      {{ errorToast }}
      <button @click="errorToast = null" class="toast-close">✕</button>
    </div>

    <div class="form-card">
      <h2 class="form-title">Novo Produto</h2>
      <div class="form-group">
        <label>Nome</label>
        <input
          v-model="form.nome"
          type="text"
          placeholder="Nome do produto"
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
      <div class="form-group">
        <label>Preço</label>
        <input v-model.number="form.preco" type="number" step="0.01" min="0" placeholder="0.00" />
      </div>
      <div class="form-group">
        <label>Categoria</label>
        <select
          v-model="form.categoriaId"
          :class="{ 'input-error': categoriaTocada && !form.categoriaId }"
          @change="categoriaTocada = true"
        >
          <option value="" disabled>Selecione uma categoria</option>
          <option v-for="cat in categorias" :key="cat.id" :value="cat.id">
            {{ cat.nome }}
          </option>
        </select>
        <span v-if="categoriaTocada && !form.categoriaId" class="field-hint error">
          Selecione uma categoria
        </span>
      </div>
      <button
        @click="salvar"
        :disabled="form.nome.length < 5 || !form.categoriaId"
        :title="form.nome.length < 5 ? 'O nome deve ter pelo menos 5 caracteres' : !form.categoriaId ? 'Selecione uma categoria' : ''"
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
            <th>Preço</th>
            <th>Categoria</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="!produtos || produtos.length === 0">
            <td colspan="6" class="empty">Nenhum produto cadastrado.</td>
          </tr>
          <tr v-for="prod in produtos" :key="prod.id">
            <td>{{ prod.id }}</td>
            <td>{{ prod.nome }}</td>
            <td>{{ prod.descricao }}</td>
            <td>{{ prod.preco?.toFixed(2) ?? '0.00' }}</td>
            <td>{{ prod.categoria?.nome }}</td>
            <td class="acoes">
              <button @click="abrirModal(prod)" class="btn btn-edit">Editar</button>
              <button @click="confirmarDelete(prod)" class="btn btn-delete">Excluir</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

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

<style scoped>
.page-title {
  font-size: 1.5rem;
  font-weight: 700;
  margin-bottom: 1.5rem;
  color: #1e293b;
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

.form-group input,
.form-group select {
  padding: 0.5rem 0.75rem;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 0.9rem;
  outline: none;
  transition: border-color 0.15s;
  background: white;
}

.form-group input:focus,
.form-group select:focus {
  border-color: #3b82f6;
}

.form-group input.input-error,
.form-group select.input-error {
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
