import api from './client';
import { LeaveTypeDto, PagedResult } from '../types';

export const getLeaveTypes = (params?: { pageNumber?: number; pageSize?: number; search?: string }) =>
  api.get<PagedResult<LeaveTypeDto>>('/LeaveTypes', { params });

export const getLeaveType = (id: number) =>
  api.get<LeaveTypeDto>(`/LeaveTypes/${id}`);

export const createLeaveType = (data: { name: string; defaultDays: number; description?: string }) =>
  api.post<LeaveTypeDto>('/LeaveTypes', data);

export const updateLeaveType = (id: number, data: { name: string; defaultDays: number; description?: string }) =>
  api.put<LeaveTypeDto>(`/LeaveTypes/${id}`, data);

export const deleteLeaveType = (id: number) =>
  api.delete(`/LeaveTypes/${id}`);
