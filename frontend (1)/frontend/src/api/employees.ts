import api from './client';
import { EmployeeDto, PagedResult } from '../types';

export const getEmployees = (params?: { page?: number; pageSize?: number; search?: string; deptId?: number }) =>
  api.get<PagedResult<EmployeeDto>>('/v1/Employees', { params });

export const getEmployee = (id: string) =>
  api.get<EmployeeDto>(`/v1/Employees/${id}`);

export const getMyProfile = () =>
  api.get<EmployeeDto>('/v1/Employees/me');

export const createEmployee = (data: {
  firstName: string; lastName: string; email: string; password: string;
  departmentId?: number; role: string; employeeType?: string;
}) => api.post('/v1/Employees', data);

export const updateEmployee = (id: string, data: { firstName?: string; lastName?: string; departmentId?: number; employeeType?: string }) =>
  api.put<EmployeeDto>(`/v1/Employees/${id}`, data);

export const updateMyProfile = (data: { firstName?: string; lastName?: string; departmentId?: number; employeeType?: string }) =>
  api.put<EmployeeDto>('/v1/Employees/me', data);

export const deleteEmployee = (id: string) =>
  api.delete(`/v1/Employees/${id}`);
