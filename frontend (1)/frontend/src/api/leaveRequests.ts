import api from './client';
import { LeaveRequestDto, PagedResult } from '../types';

export const getLeaveRequests = (params?: { pageNumber?: number; pageSize?: number; status?: string; userId?: string }) =>
  api.get<PagedResult<LeaveRequestDto>>('/v1/LeaveRequests', { params });

export const getMyLeaveRequests = (params?: { pageNumber?: number; pageSize?: number; status?: string }) =>
  api.get<PagedResult<LeaveRequestDto>>('/v1/LeaveRequests/my', { params });

export const getLeaveRequest = (id: number) =>
  api.get<LeaveRequestDto>(`/v1/LeaveRequests/${id}`);

export const createLeaveRequest = (data: { leaveTypeId: number; startDate: string; endDate: string; reason: string }) =>
  api.post('/v1/LeaveRequests', data);

export const approveLeaveRequest = (id: number, managerComment: string) =>
  api.put(`/v1/LeaveRequests/${id}/approve`, { managerComment });

export const rejectLeaveRequest = (id: number, managerComment: string) =>
  api.put(`/v1/LeaveRequests/${id}/reject`, { managerComment });

export const cancelLeaveRequest = (id: number) =>
  api.put(`/v1/LeaveRequests/${id}/cancel`);
