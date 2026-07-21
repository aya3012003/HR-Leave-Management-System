import api from './client';
import { EmployeeLeaveBalanceDto, PagedResult } from '../types';

export const getLeaveBalances = (params?: { pageNumber?: number; pageSize?: number; userId?: string }) =>
  api.get<PagedResult<EmployeeLeaveBalanceDto>>('/EmployeeLeaveBalances', { params });

export const getLeaveBalance = (id: number) =>
  api.get<EmployeeLeaveBalanceDto>(`/EmployeeLeaveBalances/${id}`);

export const updateLeaveBalance = (id: number, remainingDays: number) =>
  api.put<EmployeeLeaveBalanceDto>(`/EmployeeLeaveBalances/${id}`, { remainingDays });

export const deleteLeaveBalance = (id: number) =>
  api.delete(`/EmployeeLeaveBalances/${id}`);

export const getMyLeaveBalances = () =>
  api.get<EmployeeLeaveBalanceDto[]>('/my-leave-balances');
