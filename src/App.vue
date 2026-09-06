<script setup lang="ts">
import ScheduleCalendar from './components/ScheduleCalendar.vue'

const employeeId = 1
</script>

<template>
  <main class="app-shell">
    <h1>排班系統</h1>
    <ScheduleCalendar :employee-id="employeeId" />
  </main>
</template>

<style scoped>
.app-shell {
  margin: 0 auto;
  max-width: 72rem;
  padding: 2rem;
}
</style>
