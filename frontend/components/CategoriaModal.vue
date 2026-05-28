<script setup lang="ts">
interface Categoria {
  id: number
  nome: string
  descricao: string
}

const props = defineProps<{
  categoria: Categoria
}>()

const emit = defineEmits<{
  fechar: []
}>()

const form = reactive({
  nome: props.categoria.nome,
  descricao: props.categoria.descricao
})
</script>

<template>
  <div class="overlay" @click.self="emit('fechar')">
    <div class="modal">
      <h2 class="modal-title">Editar Categoria</h2>

      <div class="form-group">
        <label>Nome</label>
        <input v-model="form.nome" type="text" />
      </div>
      <div class="form-group">
        <label>Descrição</label>
        <input v-model="form.descricao" type="text" />
      </div>

      <div class="modal-actions">
        <button @click="emit('fechar')" class="btn btn-secondary">Cancelar</button>
        <button :disabled="form.nome.length < 5" class="btn btn-primary">Salvar</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.overlay {
  position: fixed;
  inset: 0;
  background-color: rgba(0, 0, 0, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
}

.modal {
  background: white;
  border-radius: 10px;
  padding: 1.75rem;
  width: 100%;
  max-width: 440px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
}

.modal-title {
  font-size: 1.1rem;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 1.25rem;
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

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  margin-top: 1.5rem;
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

.btn-secondary {
  background-color: #e2e8f0;
  color: #475569;
}

.btn-secondary:hover {
  background-color: #cbd5e1;
}
</style>
