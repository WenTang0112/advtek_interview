<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import {
  scheduleApi,
  type ScheduleCalendarDayDto,
} from '../services/scheduleApi'

const props = defineProps<{
  employeeId: number
}>()

const days = ref<ScheduleCalendarDayDto[]>([])
const errorMessage = ref('')
const isLoading = ref(true)
const pendingWorkDate = ref<string | null>(null)

const scheduledDays = computed(() => days.value.filter((day) => day.isMine).length)
const leadingBlankDays = computed(() => {
  const firstDay = days.value[0]?.workDate

  return firstDay ? new Date(`${firstDay}T00:00:00`).getDay() : 0
})
const monthLabel = computed(() => {
  const firstDay = days.value[0]?.workDate

  if (!firstDay) {
    return '下個月'
  }

  return new Intl.DateTimeFormat('zh-TW', {
    year: 'numeric',
    month: 'long',
  }).format(new Date(`${firstDay}T00:00:00`))
})

onMounted(loadCalendar)

async function loadCalendar() {
  isLoading.value = true
  errorMessage.value = ''

  try {
    days.value = await scheduleApi.getNextMonthCalendar(props.employeeId)
  } catch (error) {
    errorMessage.value = getErrorMessage(error)
  } finally {
    isLoading.value = false
  }
}

async function handleDayAction(day: ScheduleCalendarDayDto) {
  if (pendingWorkDate.value || (!day.canSchedule && !day.isMine)) {
    return
  }

  pendingWorkDate.value = day.workDate
  errorMessage.value = ''

  try {
    if (day.isMine && day.myScheduleId) {
      await scheduleApi.cancelSchedule(props.employeeId, day.myScheduleId)
    } else if (day.canSchedule) {
      await scheduleApi.createSchedule(props.employeeId, day.workDate)
    }

    await loadCalendar()
  } catch (error) {
    errorMessage.value = getErrorMessage(error)
  } finally {
    pendingWorkDate.value = null
  }
}

function getStatusLabels(day: ScheduleCalendarDayDto): string[] {
  const labels: string[] = []

  if (day.isMine) labels.push('我已排班')
  if (day.scheduledEmployeeCount >= 2) labels.push('當天已滿 2 人')
  if (day.isSaturday) labels.push('星期六')
  if (day.isSunday) labels.push('星期日')
  if (day.holidayName) labels.push(`國定假日：${day.holidayName}`)
  if (day.canSchedule) labels.push('可以排班')

  return labels.length > 0 ? labels : ['不可排班']
}

function getErrorMessage(error: unknown): string {
  return error instanceof Error ? error.message : '無法完成排班操作，請稍後再試。'
}
</script>

<template>
  <section class="schedule-calendar" aria-labelledby="schedule-calendar-title">
    <header class="calendar-header">
      <div>
        <h2 id="schedule-calendar-title">{{ monthLabel }}排班</h2>
        <p>請選擇可排班日期；規則將由系統再次確認。</p>
      </div>
      <dl class="schedule-limits" aria-label="排班天數限制">
        <div><dt>目前已排</dt><dd>{{ scheduledDays }} 天</dd></div>
        <div><dt>最低</dt><dd>6 天</dd></div>
        <div><dt>最高</dt><dd>15 天</dd></div>
      </dl>
    </header>

    <p v-if="errorMessage" class="error-message" role="alert">{{ errorMessage }}</p>
    <p v-if="isLoading" class="loading-message" aria-live="polite">正在載入月曆...</p>

    <div v-else class="calendar-grid" role="list" aria-label="下個月排班日期">
      <div v-for="weekday in ['日', '一', '二', '三', '四', '五', '六']" :key="weekday" class="weekday">
        星期{{ weekday }}
      </div>
      <span v-for="index in leadingBlankDays" :key="`blank-${index}`" class="calendar-blank" aria-hidden="true" />
      <article v-for="day in days" :key="day.workDate" class="calendar-day" role="listitem">
        <time :datetime="day.workDate">{{ day.workDate.slice(8) }} 日</time>
        <ul class="status-list" aria-label="日期狀態">
          <li v-for="label in getStatusLabels(day)" :key="label">{{ label }}</li>
        </ul>
        <button
          type="button"
          :disabled="(!day.canSchedule && !day.isMine) || pendingWorkDate === day.workDate"
          @click="handleDayAction(day)"
        >
          {{ pendingWorkDate === day.workDate ? '處理中' : day.isMine ? '取消排班' : '新增排班' }}
        </button>
      </article>
    </div>
  </section>
</template>

<style scoped>
.schedule-calendar {
  background: var(--surface);
  border-top: 3px solid var(--blue);
  box-shadow: 0 14px 30px rgb(23 58 94 / 8%);
  padding: 2rem;
  text-align: left;
}

.calendar-header {
  align-items: start;
  display: flex;
  gap: 2rem;
  justify-content: space-between;
  margin-bottom: 2rem;
}

.calendar-header h2 {
  color: var(--navy);
  font-size: 1.4rem;
  margin: 0 0 0.45rem;
}

.calendar-header p {
  color: var(--muted);
  font-size: 0.9rem;
  margin: 0;
}

.schedule-limits {
  border-left: 1px solid var(--border);
  display: flex;
  gap: 1.3rem;
  margin: 0;
  padding-left: 1.5rem;
}

.schedule-limits div {
  min-width: 4.5rem;
}

.schedule-limits dt {
  color: var(--muted);
  font-size: 0.8rem;
}

.schedule-limits dd {
  color: var(--navy);
  font-size: 1.15rem;
  font-weight: 700;
  margin: 0.2rem 0 0;
}

.calendar-grid {
  display: grid;
  border-top: 1px solid var(--border);
  gap: 0;
  grid-template-columns: repeat(7, minmax(0, 1fr));
}

.calendar-day {
  border-bottom: 1px solid var(--border);
  border-right: 1px solid var(--border);
  display: flex;
  flex-direction: column;
  gap: 0.7rem;
  min-height: 9.5rem;
  padding: 0.8rem;
}

.weekday {
  background: #e8f1f4;
  border-bottom: 1px solid var(--border);
  color: var(--navy);
  font-size: 0.8rem;
  font-weight: 700;
  padding: 0.75rem 0.25rem;
  text-align: center;
}

.calendar-blank {
  min-height: 1px;
}

.calendar-day time {
  color: var(--navy);
  font-weight: 700;
}

.status-list {
  flex: 1;
  font-size: 0.76rem;
  list-style: none;
  margin: 0;
  padding: 0;
}

.status-list li + li {
  margin-top: 0.35rem;
}

button {
  background: var(--navy);
  border: 1px solid var(--navy);
  color: #fff;
  cursor: pointer;
  font-size: 0.78rem;
  font-weight: 700;
  min-height: 2.15rem;
}

button:hover:not(:disabled) {
  background: var(--blue);
  border-color: var(--blue);
}

button:focus-visible {
  outline: 3px solid var(--teal);
  outline-offset: 2px;
}

button:disabled {
  background: #f2f5f6;
  border-color: #dce5e9;
  color: #7a8b97;
  cursor: not-allowed;
}

.error-message {
  background: #fff7f6;
  border-left: 3px solid #b44941;
  color: #81332e;
  margin: 0 0 1rem;
  padding-left: 0.75rem;
}

@media (max-width: 760px) {
  .calendar-header {
    flex-direction: column;
  }

  .calendar-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  .weekday:nth-child(n + 3),
  .calendar-blank {
    display: none;
  }

  .schedule-limits {
    border-left: 0;
    padding-left: 0;
  }
}
</style>