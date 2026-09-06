export interface ScheduleDto {
  id: number
  workDate: string
  createdAt: string
}

export interface ScheduleCountDto {
  scheduledDays: number
}

export interface ScheduleCalendarDayDto {
  workDate: string
  canSchedule: boolean
  isMine: boolean
  myScheduleId: number | null
  isSaturday: boolean
  isSunday: boolean
  scheduledEmployeeCount: number
  holidayName: string | null
}

interface ApiErrorDto {
  code?: string
  message?: string
}

export class ScheduleApiError extends Error {
  readonly status: number

  constructor(
    status: number,
    message: string,
  ) {
    super(message)
    this.status = status
  }
}

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? ''

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, init)

  if (!response.ok) {
    const error = await response.json().catch(() => null) as ApiErrorDto | null
    throw new ScheduleApiError(
      response.status,
      error?.message ?? `API request failed with status ${response.status}.`,
    )
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json() as Promise<T>
}

export const scheduleApi = {
  getNextMonthCalendar(employeeId: number): Promise<ScheduleCalendarDayDto[]> {
    return request<ScheduleCalendarDayDto[]>(`/api/employees/${employeeId}/schedules/calendar/next-month`)
  },

  getNextMonthSchedules(employeeId: number): Promise<ScheduleDto[]> {
    return request<ScheduleDto[]>(`/api/employees/${employeeId}/schedules/next-month`)
  },

  getCurrentMonthCount(employeeId: number): Promise<ScheduleCountDto> {
    return request<ScheduleCountDto>(`/api/employees/${employeeId}/schedules/current-month/count`)
  },

  createSchedule(employeeId: number, workDate: string): Promise<ScheduleDto> {
    return request<ScheduleDto>(`/api/employees/${employeeId}/schedules`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ workDate }),
    })
  },

  cancelSchedule(employeeId: number, scheduleId: number): Promise<void> {
    return request<void>(`/api/employees/${employeeId}/schedules/${scheduleId}`, {
      method: 'DELETE',
    })
  },
}