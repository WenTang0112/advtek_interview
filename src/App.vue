<script setup lang="ts">
import { ref } from 'vue'
import BossDashboard from './components/BossDashboard.vue'
import ScheduleCalendar from './components/ScheduleCalendar.vue'

const employeeId = 1
const activeView = ref<'employee' | 'boss'>('boss')
</script>

<template>
  <header class="site-header">
    <a class="brand" href="/" aria-label="Shift Planner 首頁">
      <span class="brand-mark">SP</span>
      <span>SHIFT PLANNER</span>
    </a>
    <nav aria-label="主要導覽">
      <button type="button" :class="{ 'active-nav': activeView === 'employee' }" @click="activeView = 'employee'">員工專區</button>
      <button type="button" :class="{ 'active-nav': activeView === 'boss' }" @click="activeView = 'boss'">管理專區</button>
    </nav>
  </header>

  <main class="app-shell">
    <section v-if="activeView === 'employee'" class="page-heading" aria-labelledby="page-title">
      <p class="eyebrow">EMPLOYEE SELF-SERVICE</p>
      <h1 id="page-title">我的排班</h1>
      <p>下個月工作日安排</p>
    </section>
    <ScheduleCalendar v-if="activeView === 'employee'" :employee-id="employeeId" />
    <BossDashboard v-else />
  </main>

  <footer class="site-footer">SHIFT PLANNER</footer>
</template>

<style scoped>
.app-shell {
  margin: 0 auto;
  max-width: 76rem;
  padding: 3.5rem 2rem 4.5rem;
}

.site-header {
  align-items: center;
  background: #fff;
  border-bottom: 1px solid #dce5e9;
  display: flex;
  justify-content: space-between;
  min-height: 5.25rem;
  padding: 0 5vw;
}

.brand {
  align-items: center;
  color: var(--navy);
  display: inline-flex;
  font-size: 1rem;
  font-weight: 800;
  gap: 0.7rem;
  letter-spacing: 0.08em;
  text-decoration: none;
}

.brand-mark {
  align-items: center;
  background: var(--blue);
  color: #fff;
  display: inline-flex;
  font-size: 0.72rem;
  height: 2rem;
  justify-content: center;
  letter-spacing: 0.04em;
  width: 2rem;
}

nav {
  align-items: center;
  color: var(--muted);
  display: flex;
  font-size: 0.9rem;
  gap: 2rem;
}

nav button {
  background: transparent;
  border: 0;
  color: inherit;
  cursor: pointer;
  font-size: inherit;
  padding: 0;
}

nav button:focus-visible {
  outline: 2px solid var(--teal);
  outline-offset: 4px;
}

.active-nav {
  color: #137a94;
  font-weight: 700;
}

.page-heading {
  border-left: 4px solid var(--teal);
  margin-bottom: 2.5rem;
  padding-left: 1.25rem;
}

.eyebrow {
  color: var(--blue);
  font-size: 0.75rem;
  font-weight: 800;
  letter-spacing: 0.12em;
  margin: 0 0 0.55rem;
}

h1 {
  color: var(--navy);
  font-size: 2rem;
  font-weight: 700;
  margin: 0 0 0.4rem;
}

.page-heading > p:last-child {
  color: var(--muted);
  margin: 0;
}

.site-footer {
  background: var(--navy);
  color: #b9d9e1;
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.12em;
  padding: 1.4rem 5vw;
}

@media (max-width: 640px) {
  .site-header {
    align-items: flex-start;
    flex-direction: column;
    gap: 1rem;
    padding: 1.15rem 1.25rem;
  }

  nav {
    font-size: 0.82rem;
    gap: 1.2rem;
  }

  .app-shell {
    padding: 2.25rem 1rem 3rem;
  }
}
</style>
