export interface EmployeeWorkdaySummaryDto {
  employeeId: number
  employeeName: string
  monthlyWorkdays: number
  yearlyWorkdays: number
}

export interface MonthlyWorkdayRankingDto {
  rank: number
  employeeId: number
  employeeName: string
  workdays: number
}

export interface ScheduledEmployeeDto {
  employeeId: number
  employeeName: string
}

export interface DailyScheduleDto {
  workDate: string
  holidayName: string | null
  employees: ScheduledEmployeeDto[]
}

export interface StatisticsDashboardDto {
  year: number
  month: number
  employeeWorkdays: EmployeeWorkdaySummaryDto[]
  monthlyRanking: MonthlyWorkdayRankingDto[]
  dailySchedules: DailyScheduleDto[]
}

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? ''

export async function getDashboard(year: number, month: number): Promise<StatisticsDashboardDto> {
  const response = await fetch(`${apiBaseUrl}/api/boss/dashboard?year=${year}&month=${month}`)

  if (!response.ok) {
    throw new Error(`無法取得 Dashboard 資料（${response.status}）。`)
  }

  return response.json() as Promise<StatisticsDashboardDto>
}