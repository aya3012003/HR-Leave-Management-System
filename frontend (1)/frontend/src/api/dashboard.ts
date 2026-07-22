import api from './client';
import { DashboardStatisticsDto, LeaveSummaryDto, DepartmentLeaveUsageDto, MostUsedLeaveTypeDto, LeaveRequestDto } from '../types';

export const getStatistics = () =>
  api.get<DashboardStatisticsDto>('/Dashboard/dashboard');

export const getLeaveSummary = () =>
  api.get<LeaveSummaryDto[]>('/Dashboard/leave-summary');

export const getDepartmentLeaveUsage = () =>
  api.get<DepartmentLeaveUsageDto[]>('/Dashboard/department-leave-usage');

export const getEmployeeHistory = (userId: string) =>
  api.get<LeaveRequestDto[]>(`/Dashboard/employee-history/${userId}`);

export const getMostUsedLeaveType = () =>
  api.get<MostUsedLeaveTypeDto>('/Dashboard/most-used-leave-type');
