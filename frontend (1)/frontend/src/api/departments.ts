import api from './client';
import { DepartmentDto, PagedResult } from '../types';

export const getDepartments = (params?: { pageNumber?: number; pageSize?: number; search?: string }) =>
  api.get<PagedResult<DepartmentDto>>('/Department', { params });

export const getDepartment = (id: number) =>
  api.get<DepartmentDto>(`/Department/${id}`);

export const createDepartment = (name: string) =>
  api.post<DepartmentDto>('/Department', { name });

export const updateDepartment = (id: number, name: string) =>
  api.put<DepartmentDto>(`/Department/${id}`, { name });

export const deleteDepartment = (id: number) =>
  api.delete(`/Department/${id}`);
