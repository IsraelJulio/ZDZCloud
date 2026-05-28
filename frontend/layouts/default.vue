<template>
  <div>
    <nav class="navbar">
      <span class="navbar-brand">ZDZCloud</span>
      <div class="nav-links">
        <NuxtLink to="/categorias" class="nav-link">Categorias</NuxtLink>
        <NuxtLink to="/produtos" class="nav-link">Produtos</NuxtLink>
      </div>
    </nav>
    <main class="main-content">
      <slot />
    </main>
  </div>
</template>

<style scoped>
.navbar {
  display: flex;
  align-items: center;
  gap: 2rem;
  padding: 1rem 2rem;
  background-color: #1e293b;
  color: white;
}

.navbar-brand {
  font-size: 1.25rem;
  font-weight: 700;
}

.nav-links {
  display: flex;
  gap: 1rem;
}

.nav-link {
  color: #94a3b8;
  text-decoration: none;
  font-size: 0.95rem;
}

.nav-link:hover,
.nav-link.router-link-active {
  color: white;
}

.main-content {
  padding: 2rem;
}
</style>
