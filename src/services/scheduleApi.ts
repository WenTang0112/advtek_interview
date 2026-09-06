export interface ScheduleDto {
  id: number
  workDate: string
  createdAt: string
}

export interface ScheduleCountDto {
  scheduledDays: number
}

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? ''

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, init)

  if (!response.ok) {
    throw new Error(`API request failed with status ${response.status}.`)
  }

  return response.json() as Promise<T>
}

export const scheduleApi = {
  getNextMonthSchedules(employeeId: number): Promise<ScheduleDto[]> {
    return request<ScheduleDto[]>(`/api/employees/${employeeId}/schedules/next-month`)
  },

  getCurrentMonthCount(employeeId: number): Promise<ScheduleCountDto> {
    return request<ScheduleCountDto>(`/api/employees/${employeeId}/schedules/current-month/count`)
  },
}