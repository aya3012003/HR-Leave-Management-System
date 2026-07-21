export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  userName: string;
  roles: string[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface DepartmentDto {
  id: number;
  name: string;
}

export interface EmployeeDto {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  departmentId?: number;
  departmentName?: string;
  hireDate?: string;
  dateOfBirth?: string;
  roles: string[];
}

export interface LeaveTypeDto {
  id: number;
  name: string;
  defaultDays: number;
  description?: string;
}

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
  status: string;
  managerComment?: string;
  createdAt: string;
}

export interface EmployeeLeaveBalanceDto {
  id: number;
  userId: string;
  employeeName: string;
  leaveTypeId: number;
  leaveTypeName: string;
  remainingDays: number;
}

export interface DashboardStatisticsDto {
  totalEmployees: number;
  totalDepartments: number;
  pendingLeaveRequests: number;
  approvedLeaveRequests: number;
  rejectedLeaveRequests: number;
  cancelledLeaveRequests: number;
}

export interface LeaveSummaryDto {
  leaveType: string;
  count: number;
}

export interface DepartmentLeaveUsageDto {
  departmentName: string;
  totalDays: number;
}

export interface MostUsedLeaveTypeDto {
  leaveType: string;
  count: number;
}
