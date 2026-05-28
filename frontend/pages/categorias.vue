<script setup lang="ts">
interface Categoria {
  id: number
  nome: string
  descricao: string
}

const config = useRuntimeConfig()
const apiBase = config.public.apiBase

const form = reactive({ nome: '', descricao: '' })

const { data: categorias, refresh } = await useFetch<Categoria[]>(`${apiBase}/api/categorias`)

const showModal = ref(false)
const categoriaEditando = ref<Categoria | null>(null)

async function salvar() {
  await $fetch(`${apiBase}/api/categorias`, {
    method: 'POST',
    body: { nome: form.nome, descricao: form.descricao }
  })
  form.nome = ''
  form.descricao = ''
  await refresh()
}

function abrirModal(categoria: Categoria) {
  categoriaEditando.value = { ...categoria }
  showModal.value = true
}

function fecharModal() {
  showModal.value = false
  categoriaEditando.value = null
}
</script>

<template>
  <div>
    <h1 class="page-title">Categorias</h1>

    <div class="form-card">
      <h2 class="form-title">Nova Categoria</h2>
      <div class="form-group">
        <label>Nome</label>
        <input v-model="form.nome" type="text" placeholder="Nome da categoria" />
      </div>
      <div class="form-group">
        <label>Descrição</label>
        <input v-model="form.descricao" type="text" placeholder="Descrição" />
      </div>
      <button @click="salvar" :disabled="form.nome.length < 5" class="btn btn-primary">
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
              <button class="btn btn-delete">Excluir</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <CategoriaModal
      v-if="showModal && categoriaEditando"
      :categoria="categoriaEditando"
      @fechar="fecharModal"
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
