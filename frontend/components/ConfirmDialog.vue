<script setup lang="ts">
defineProps<{ mensagem: string }>()
const emit = defineEmits<{ confirmar: []; cancelar: [] }>()
</script>

<template>
  <div class="overlay" @click.self="emit('cancelar')">
    <div class="dialog">
      <p class="mensagem">{{ mensagem }}</p>
      <div class="actions">
        <button @click="emit('cancelar')" class="btn btn-secondary">Cancelar</button>
        <button @click="emit('confirmar')" class="btn btn-danger">Confirmar</button>
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
  z-index: 200;
}

.dialog {
  background: white;
  border-radius: 10px;
  padding: 1.75rem;
  width: 100%;
  max-width: 380px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
}

.mensagem {
  font-size: 0.95rem;
  color: #334155;
  margin-bottom: 1.5rem;
  line-height: 1.5;
}

.actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
}

.btn {
  padding: 0.5rem 1.25rem;
  border: none;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.15s;
}

.btn-secondary {
  background-color: #e2e8f0;
  color: #475569;
}

.btn-secondary:hover {
  background-color: #cbd5e1;
}

.btn-danger {
  background-color: #ef4444;
  color: white;
}

.btn-danger:hover {
  background-color: #dc2626;
}
</style>
