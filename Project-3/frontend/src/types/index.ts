// ──────────────────────────────────────────────────────
// Auth DTOs
// ──────────────────────────────────────────────────────
export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  userName: string;
  email: string;
  password: string;
}

export interface AuthResponseDto {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  userName: string;
  roles: string[];
}

export interface TokenRequestDto {
  accessToken: string;
  refreshToken: string;
}

export interface LogoutDto {
  refreshToken: string;
}

// ──────────────────────────────────────────────────────
// Common
// ──────────────────────────────────────────────────────
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// ──────────────────────────────────────────────────────
// Employee DTOs
// ──────────────────────────────────────────────────────
export interface EmployeeDto {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  departmentId: number | null;
  departmentName: string | null;
  hireDate: string;
  dateOfBirth: string;
  roles: string[];
}

export interface CreateEmployeeDto {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  departmentId: number;
  role: string;
  employeeType?: string;
}

export interface UpdateEmployeeDto {
  firstName?: string;
  lastName?: string;
  departmentId?: number;
  employeeType?: string;
}

// ──────────────────────────────────────────────────────
// Department DTOs
// ──────────────────────────────────────────────────────
export interface DepartmentDto {
  id: number;
  name: string;
}

export interface CreateDepartmentDto {
  name: string;
}

export interface UpdateDepartmentDto {
  name: string;
}

// ──────────────────────────────────────────────────────
// Leave Type DTOs
// ──────────────────────────────────────────────────────
export interface LeaveTypeDto {
  id: number;
  name: string;
  defaultDays: number;
  description?: string;
}

export interface CreateLeaveTypeDto {
  name: string;
  defaultDays: number;
  description?: string;
}

export interface UpdateLeaveTypeDto {
  name: string;
  defaultDays: number;
  description?: string;
}

// ──────────────────────────────────────────────────────
// Leave Request DTOs
// ──────────────────────────────────────────────────────
export type LeaveStatus = 'Pending' | 'Approved' | 'Rejected' | 'Cancelled';

export interface LeaveRequestDto {
  id: number;
  userId: string;
  employeeName: string;
  leaveTypeId: number;
  leaveTypeName: string;
  startDate: string;
  endDate: string;
  workingDays: number;
  reason: string;
  status: LeaveStatus;
  managerComment?: string;
  createdAt: string;
}

export interface CreateLeaveRequestDto {
  leaveTypeId: number;
  startDate: string;
  endDate: string;
  reason: string;
}

export interface LeaveRequestActionDto {
  managerComment?: string;
}

// ──────────────────────────────────────────────────────
// Employee Leave Balance DTOs
// ──────────────────────────────────────────────────────
export interface EmployeeLeaveBalanceDto {
  id: number;
  userId: string;
  employeeName: string;
  leaveTypeId: number;
  leaveTypeName: string;
  remainingDays: number;
}

export interface CreateEmployeeLeaveBalanceDto {
  userId: string;
  leaveTypeId: number;
  remainingDays: number;
}

export interface UpdateEmployeeLeaveBalanceDto {
  remainingDays: number;
}

// ──────────────────────────────────────────────────────
// Dashboard DTOs
// ──────────────────────────────────────────────────────
export interface DashboardStatisticsDto {
  totalEmployees: number;
  totalDepartments: number;
  totalLeaveRequests: number;
  pendingRequests: number;
  approvedRequests: number;
  rejectedRequests: number;
}

export interface LeaveSummaryDto {
  leaveType: string;
  count: number;
}

export interface DepartmentLeaveUsageDto {
  departmentName: string;
  totalDays: number;
}

export interface EmployeeLeaveHistoryDto {
  leaveType: string;
  startDate: string;
  endDate: string;
  workingDays: number;
  status: LeaveStatus;
}

export interface MostUsedLeaveTypeDto {
  leaveType: string;
  count: number;
}

// ──────────────────────────────────────────────────────
// Holiday DTO
// ──────────────────────────────────────────────────────
export interface HolidayDto {
  date: string;
  name: string;
  countryCode: string;
}
