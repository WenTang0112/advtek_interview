<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { getDashboard, type DailyScheduleDto, type StatisticsDashboardDto } from '../services/statisticsApi'

interface CalendarCell {
  key: string
  day: DailyScheduleDto | null
}

const selectedMonth = ref(new Date(new Date().getFullYear(), new Date().getMonth(), 1))
const dashboard = ref<StatisticsDashboardDto | null>(null)
const errorMessage = ref('')
const isLoading = ref(false)

const monthLabel = computed(() => new Intl.DateTimeFormat('zh-TW', {
  year: 'numeric',
  month: 'long',
}).format(selectedMonth.value))

const calendarCells = computed<CalendarCell[]>(() => {
  const data = dashboard.value

  if (!data) return []

  const firstDay = new Date(data.year, data.month - 1, 1)
  const leadingBlanks = Array.from({ length: firstDay.getDay() }, (_, index) => ({
    key: `blank-${index}`,
    day: null,
  }))
  const days = data.dailySchedules.map((day) => ({ key: day.workDate, day }))

  return [...leadingBlanks, ...days]
})

onMounted(loadDashboard)

async function loadDashboard() {
  isLoading.value = true
  errorMessage.value = ''

  try {
    dashboard.value = await getDashboard(
      selectedMonth.value.getFullYear(),
      selectedMonth.value.getMonth() + 1,
    )
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : '無法取得 Dashboard 資料。'
  } finally {
    isLoading.value = false
  }
}

function changeMonth(offset: number) {
  selectedMonth.value = new Date(
    selectedMonth.value.getFullYear(),
    selectedMonth.value.getMonth() + offset,
    1,
  )
  void loadDashboard()
}

function dayNumber(day: DailyScheduleDto) {
  return new Date(`${day.workDate}T00:00:00`).getDate()
}
</script>

<template>
  <section class="dashboard" aria-labelledby="dashboard-title">
    <header class="dashboard-heading">
      <div>
        <p class="eyebrow">MANAGEMENT OVERVIEW</p>
        <h1 id="dashboard-title">排班總覽</h1>
        <p>掌握團隊出勤分布與每月排班狀態</p>
      </div>
      <div class="month-picker" aria-label="月份切換">
        <button type="button" aria-label="上個月" @click="changeMonth(-1)">&#8249;</button>
        <strong>{{ monthLabel }}</strong>
        <button type="button" aria-label="下個月" @click="changeMonth(1)">&#8250;</button>
      </div>
    </header>

    <p v-if="errorMessage" class="error-message" role="alert">{{ errorMessage }}</p>
    <p v-if="isLoading" class="loading-message" aria-live="polite">正在更新 Dashboard...</p>

    <template v-else-if="dashboard">
      <section class="overview-grid" aria-label="員工出勤統計">
        <article class="data-panel employee-summary">
          <div class="panel-heading">
            <div>
              <p class="panel-kicker">TEAM WORKDAYS</p>
              <h2>員工出勤統計</h2>
            </div>
            <span>{{ dashboard.employeeWorkdays.length }} 位員工</span>
          </div>
          <div class="table-scroll">
            <table>
              <thead><tr><th>員工</th><th>當月</th><th>年度累計</th></tr></thead>
              <tbody>
                <tr v-for="employee in dashboard.employeeWorkdays" :key="employee.employeeId">
                  <th scope="row">{{ employee.employeeName }}</th>
                  <td>{{ employee.monthlyWorkdays }} 天</td>
                  <td>{{ employee.yearlyWorkdays }} 天</td>
                </tr>
              </tbody>
            </table>
          </div>
        </article>

        <article class="data-panel ranking-panel">
          <div class="panel-heading">
            <div>
              <p class="panel-kicker">MONTHLY RANKING</p>
              <h2>當月上班排行</h2>
            </div>
          </div>
          <ol class="ranking-list">
            <li v-for="employee in dashboard.monthlyRanking" :key="employee.employeeId">
              <span class="rank">{{ String(employee.rank).padStart(2, '0') }}</span>
              <span class="ranking-name">{{ employee.employeeName }}</span>
              <strong>{{ employee.workdays }} <small>天</small></strong>
            </li>
          </ol>
        </article>
      </section>

      <section class="data-panel calendar-panel" aria-labelledby="calendar-title">
        <div class="panel-heading">
          <div>
            <p class="panel-kicker">MONTHLY SCHEDULE</p>
            <h2 id="calendar-title">{{ monthLabel }}完整班表</h2>
          </div>
          <span>每格顯示當日排班人員</span>
        </div>
        <div class="calendar-scroll">
          <div class="schedule-grid" role="grid" aria-label="指定月份完整班表">
            <div v-for="weekday in ['日', '一', '二', '三', '四', '五', '六']" :key="weekday" class="weekday" role="columnheader">星期{{ weekday }}</div>
            <template v-for="cell in calendarCells" :key="cell.key">
              <div v-if="!cell.day" class="calendar-blank" aria-hidden="true" />
              <article v-else class="schedule-day" role="gridcell">
                <time :datetime="cell.day.workDate">{{ dayNumber(cell.day) }}</time>
                <span v-if="cell.day.holidayName" class="holiday-name">國定假日：{{ cell.day.holidayName }}</span>
                <ul v-if="cell.day.employees.length" class="assigned-employees">
                  <li v-for="employee in cell.day.employees" :key="employee.employeeId">{{ employee.employeeName }}</li>
                </ul>
                <span v-else class="no-schedule">尚無排班</span>
              </article>
            </template>
          </div>
        </div>
      </section>
    </template>
  </section>
</template>

<style scoped>
.dashboard { color: var(--ink); }
.dashboard-heading { align-items: end; border-left: 4px solid var(--teal); display: flex; justify-content: space-between; margin-bottom: 2.5rem; padding-left: 1.25rem; }
.eyebrow, .panel-kicker { color: var(--blue); font-size: 0.72rem; font-weight: 800; letter-spacing: 0.12em; margin: 0 0 0.55rem; }
h1, h2 { color: var(--navy); margin: 0; }
h1 { font-size: 2rem; font-weight: 700; }
h2 { font-size: 1.15rem; }
.dashboard-heading > div > p:last-child { color: var(--muted); margin: 0.45rem 0 0; }
.month-picker { align-items: center; border: 1px solid var(--border); display: flex; min-height: 2.8rem; }
.month-picker strong { color: var(--navy); font-size: 0.95rem; min-width: 7rem; text-align: center; }
.month-picker button { background: #fff; border: 0; color: var(--blue); cursor: pointer; font-size: 1.7rem; height: 2.7rem; line-height: 1; width: 2.7rem; }
.month-picker button:hover, .month-picker button:focus-visible { background: #e8f1f4; outline: 0; }
.overview-grid { display: grid; gap: 1.5rem; grid-template-columns: minmax(0, 1.65fr) minmax(16rem, 1fr); margin-bottom: 1.5rem; }
.data-panel { background: var(--surface); border-top: 3px solid var(--blue); box-shadow: 0 12px 26px rgb(23 58 94 / 7%); padding: 1.5rem; }
.panel-heading { align-items: start; display: flex; justify-content: space-between; margin-bottom: 1.25rem; }
.panel-heading > span { color: var(--muted); font-size: 0.78rem; }
.table-scroll { overflow-x: auto; }
table { border-collapse: collapse; min-width: 28rem; width: 100%; }
th, td { border-bottom: 1px solid var(--border); padding: 0.8rem 0.65rem; text-align: left; }
thead th { background: #e8f1f4; color: var(--navy); font-size: 0.78rem; }
tbody th { color: var(--ink); font-size: 0.88rem; font-weight: 600; }
td { color: var(--muted); font-size: 0.88rem; }
.ranking-list { list-style: none; margin: 0; padding: 0; }
.ranking-list li { align-items: center; border-bottom: 1px solid var(--border); display: flex; gap: 0.75rem; min-height: 3.2rem; }
.rank { color: var(--teal); font-size: 0.72rem; font-weight: 800; width: 1.6rem; }
.ranking-name { flex: 1; font-size: 0.88rem; }
.ranking-list strong { color: var(--navy); font-size: 1rem; }
.ranking-list small { color: var(--muted); font-size: 0.72rem; font-weight: 400; }
.calendar-panel { margin-top: 1.5rem; }
.calendar-scroll { overflow-x: auto; }
.schedule-grid { border-left: 1px solid var(--border); border-top: 1px solid var(--border); display: grid; grid-template-columns: repeat(7, minmax(8.5rem, 1fr)); min-width: 59.5rem; }
.weekday { background: #e8f1f4; border-bottom: 1px solid var(--border); border-right: 1px solid var(--border); color: var(--navy); font-size: 0.78rem; font-weight: 700; padding: 0.75rem; text-align: center; }
.calendar-blank, .schedule-day { border-bottom: 1px solid var(--border); border-right: 1px solid var(--border); min-height: 8.2rem; }
.schedule-day { display: flex; flex-direction: column; padding: 0.75rem; }
.schedule-day time { color: var(--navy); font-size: 0.9rem; font-weight: 800; margin-bottom: 0.65rem; }
.holiday-name { color: #9a4e19; font-size: 0.72rem; font-weight: 700; margin: -0.2rem 0 0.55rem; }
.assigned-employees { display: grid; gap: 0.35rem; list-style: none; margin: 0; padding: 0; }
.assigned-employees li { background: #eaf6f5; border-left: 2px solid var(--teal); color: #27636d; font-size: 0.76rem; padding: 0.3rem 0.4rem; }
.no-schedule { color: #93a1ac; font-size: 0.74rem; }
.error-message { background: #fff7f6; border-left: 3px solid #b44941; color: #81332e; margin: 0 0 1.5rem; padding: 0.75rem; }
.loading-message { color: var(--muted); padding: 2rem 0; }
@media (max-width: 760px) {
  .dashboard-heading { align-items: start; flex-direction: column; gap: 1.25rem; }
  .overview-grid { grid-template-columns: 1fr; }
  .data-panel { padding: 1.1rem; }
}
</style>