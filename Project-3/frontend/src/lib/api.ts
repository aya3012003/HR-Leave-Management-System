/**
 * Base API client for the HR Leave Management System .NET backend.
 * Backend runs at http://localhost:7000 (configurable via VITE_API_BASE_URL).
 *
 * All endpoints use the Authorization: Bearer <token> header except
 * the public auth endpoints (login, register, refresh).
 */

// In local dev the Vite proxy forwards /api/* to the backend (no CORS).
// In production / Replit, set VITE_API_BASE_URL to the full backend origin.
const BASE_URL = (import.meta.env.VITE_API_BASE_URL ?? '').replace(/\/$/, '');

function getToken(): string | null {
    return localStorage.getItem('access_token');
}

function getRefreshToken(): string | null {
    return localStorage.getItem('refresh_token');
}

export function setTokens(accessToken: string, refreshToken: string): void {
    localStorage.setItem('access_token', accessToken);
    localStorage.setItem('refresh_token', refreshToken);
}

export function clearTokens(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user');
}

async function refreshAccessToken(): Promise<boolean> {
    const accessToken = getToken();
    const refreshToken = getRefreshToken();
    if (!accessToken || !refreshToken) return false;
    try {
        const res = await fetch(`${BASE_URL}/api/Auth/refresh`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ accessToken, refreshToken }),
        });
        if (!res.ok) return false;
        const data = await res.json();
        setTokens(data.accessToken, data.refreshToken);
        return true;
    } catch {
        return false;
    }
}

export class ApiError extends Error {
    constructor(
        public status: number,
        public statusText: string,
        public data: unknown,
    ) {
        super(`API Error ${status}: ${statusText}`);
        this.name = 'ApiError';
    }
}

async function request<T>(
    path: string,
    options: RequestInit = {},
    retry = true,
): Promise<T> {
    const token = getToken();
    const headers: Record<string, string> = {
        'Content-Type': 'application/json',
        ...(options.headers as Record<string, string>),
    };
    if (token) headers['Authorization'] = `Bearer ${token}`;

    const res = await fetch(`${BASE_URL}${path}`, { ...options, headers });

    if (res.status === 401 && retry) {
        const refreshed = await refreshAccessToken();
        if (refreshed) return request<T>(path, options, false);
        clearTokens();
        window.dispatchEvent(new CustomEvent('auth:logout'));
        throw new ApiError(401, 'Unauthorized', null);
    }

    if (!res.ok) {
        let data: unknown;
        try { data = await res.json(); } catch { data = await res.text(); }
        throw new ApiError(res.status, res.statusText, data);
    }

    if (res.status === 204) return undefined as T;

    return res.json() as Promise<T>;
}

// ──────────────────────────────────────────────────────
// Auth
// ──────────────────────────────────────────────────────
import type {
    LoginDto, RegisterDto, AuthResponseDto, LogoutDto,
    EmployeeDto, CreateEmployeeDto, UpdateEmployeeDto, PagedResult,
    DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto,
    LeaveTypeDto, CreateLeaveTypeDto, UpdateLeaveTypeDto,
    LeaveRequestDto, CreateLeaveRequestDto, LeaveRequestActionDto,
    EmployeeLeaveBalanceDto, CreateEmployeeLeaveBalanceDto, UpdateEmployeeLeaveBalanceDto,
    DashboardStatisticsDto, LeaveSummaryDto, DepartmentLeaveUsageDto,
    EmployeeLeaveHistoryDto, MostUsedLeaveTypeDto, HolidayDto,
} from '@/types';

export const authApi = {
    login: (dto: LoginDto) =>
        request<AuthResponseDto>('/api/Auth/login', { method: 'POST', body: JSON.stringify(dto) }),

    register: (dto: RegisterDto) =>
        request<{ message: string }>('/api/Auth/register', { method: 'POST', body: JSON.stringify(dto) }),

    logout: (dto: LogoutDto) =>
        request<void>('/api/Auth/logout', { method: 'POST', body: JSON.stringify(dto) }),
};

// ──────────────────────────────────────────────────────
// Dashboard
// ──────────────────────────────────────────────────────
export const dashboardApi = {
    getStats: () =>
        request<DashboardStatisticsDto>('/api/Dashboard/dashboard'),

    getLeaveSummary: () =>
        request<LeaveSummaryDto[]>('/api/Dashboard/leave-summary'),

    getDepartmentLeaveUsage: () =>
        request<DepartmentLeaveUsageDto[]>('/api/Dashboard/department-leave-usage'),

    getEmployeeHistory: (userId: string) =>
        request<EmployeeLeaveHistoryDto[]>(`/api/Dashboard/employee-history/${userId}`),

    getMostUsedLeaveType: () =>
        request<MostUsedLeaveTypeDto[]>('/api/Dashboard/most-used-leave-type'),
};

// ──────────────────────────────────────────────────────
// Departments
// ──────────────────────────────────────────────────────
export const departmentsApi = {
    getAll: () =>
        request<PagedResult<DepartmentDto>>('/api/Department').then(res => res.items),

    getById: (id: number) =>
        request<DepartmentDto>(`/api/Department/${id}`),

    create: (dto: CreateDepartmentDto) =>
        request<DepartmentDto>('/api/Department', { method: 'POST', body: JSON.stringify(dto) }),

    update: (id: number, dto: UpdateDepartmentDto) =>
        request<DepartmentDto>(`/api/Department/${id}`, { method: 'PUT', body: JSON.stringify(dto) }),

    delete: (id: number) =>
        request<void>(`/api/Department/${id}`, { method: 'DELETE' }),
};

// ──────────────────────────────────────────────────────
// Employees
// ──────────────────────────────────────────────────────
export const employeesApi = {
    getAll: (params?: { page?: number; pageSize?: number; deptId?: number; search?: string }) => {
        const q = new URLSearchParams();
        if (params?.page) q.set('page', String(params.page));
        if (params?.pageSize) q.set('pageSize', String(params.pageSize));
        if (params?.deptId) q.set('deptId', String(params.deptId));
        if (params?.search) q.set('search', params.search);
        return request<PagedResult<EmployeeDto>>(`/api/v1/Employees?${q}`);
    },

    getMe: () =>
        request<EmployeeDto>('/api/v1/Employees/me'),

    getById: (id: string) =>
        request<EmployeeDto>(`/api/v1/Employees/${id}`),

    create: (dto: CreateEmployeeDto) =>
        request<EmployeeDto>('/api/v1/Employees', { method: 'POST', body: JSON.stringify(dto) }),

    update: (id: string, dto: UpdateEmployeeDto) =>
        request<EmployeeDto>(`/api/v1/Employees/${id}`, { method: 'PUT', body: JSON.stringify(dto) }),

    updateMe: (dto: UpdateEmployeeDto) =>
        request<EmployeeDto>('/api/v1/Employees/me', { method: 'PUT', body: JSON.stringify(dto) }),

    delete: (id: string) =>
        request<void>(`/api/v1/Employees/${id}`, { method: 'DELETE' }),
};

// ──────────────────────────────────────────────────────
// Leave Types
// ──────────────────────────────────────────────────────
export const leaveTypesApi = {
    getAll: (params?: { page?: number; pageSize?: number; search?: string }) => {
        const q = new URLSearchParams();
        if (params?.page) q.set('page', String(params.page));
        if (params?.pageSize) q.set('pageSize', String(params.pageSize));
        if (params?.search) q.set('search', params.search ?? '');
        return request<PagedResult<LeaveTypeDto>>(`/api/LeaveTypes?${q}`);
    },

    getById: (id: number) =>
        request<LeaveTypeDto>(`/api/LeaveTypes/${id}`),

    create: (dto: CreateLeaveTypeDto) =>
        request<LeaveTypeDto>('/api/LeaveTypes', { method: 'POST', body: JSON.stringify(dto) }),

    update: (id: number, dto: UpdateLeaveTypeDto) =>
        request<LeaveTypeDto>(`/api/LeaveTypes/${id}`, { method: 'PUT', body: JSON.stringify(dto) }),

    delete: (id: number) =>
        request<void>(`/api/LeaveTypes/${id}`, { method: 'DELETE' }),
};

// ──────────────────────────────────────────────────────
// Leave Requests
// ──────────────────────────────────────────────────────
export const leaveRequestsApi = {
    getAll: (params?: { page?: number; pageSize?: number; status?: string; userId?: string }) => {
        const q = new URLSearchParams();
        if (params?.page) q.set('page', String(params.page));
        if (params?.pageSize) q.set('pageSize', String(params.pageSize));
        if (params?.status) q.set('status', params.status);
        if (params?.userId) q.set('userId', params.userId);
        return request<PagedResult<LeaveRequestDto>>(`/api/v1/LeaveRequests?${q}`);
    },

    getMy: () =>
        request<PagedResult<LeaveRequestDto>>('/api/v1/LeaveRequests/my?pageSize=100').then(res => res.items),

    getById: (id: number) =>
        request<LeaveRequestDto>(`/api/v1/LeaveRequests/${id}`),

    create: (dto: CreateLeaveRequestDto) =>
        request<LeaveRequestDto>('/api/v1/LeaveRequests', { method: 'POST', body: JSON.stringify(dto) }),

    approve: (id: number, dto: LeaveRequestActionDto) =>
        request<LeaveRequestDto>(`/api/v1/LeaveRequests/${id}/approve`, { method: 'PUT', body: JSON.stringify(dto) }),

    reject: (id: number, dto: LeaveRequestActionDto) =>
        request<LeaveRequestDto>(`/api/v1/LeaveRequests/${id}/reject`, { method: 'PUT', body: JSON.stringify(dto) }),

    cancel: (id: number) =>
        request<LeaveRequestDto>(`/api/v1/LeaveRequests/${id}/cancel`, { method: 'PUT', body: JSON.stringify({}) }),
};

// ──────────────────────────────────────────────────────
// Employee Leave Balances
// ──────────────────────────────────────────────────────
export const leaveBalancesApi = {
    getAll: (params?: { page?: number; pageSize?: number; userId?: string }) => {
        const q = new URLSearchParams();
        if (params?.page) q.set('page', String(params.page));
        if (params?.pageSize) q.set('pageSize', String(params.pageSize));
        if (params?.userId) q.set('userId', params.userId);
        return request<PagedResult<EmployeeLeaveBalanceDto>>(`/api/EmployeeLeaveBalances?${q}`);
    },

    getById: (id: number) =>
        request<EmployeeLeaveBalanceDto>(`/api/EmployeeLeaveBalances/${id}`),

    create: (dto: CreateEmployeeLeaveBalanceDto) =>
        request<EmployeeLeaveBalanceDto>('/api/EmployeeLeaveBalances', { method: 'POST', body: JSON.stringify(dto) }),

    update: (id: number, dto: UpdateEmployeeLeaveBalanceDto) =>
        request<EmployeeLeaveBalanceDto>(`/api/EmployeeLeaveBalances/${id}`, { method: 'PUT', body: JSON.stringify(dto) }),

    delete: (id: number) =>
        request<void>(`/api/EmployeeLeaveBalances/${id}`, { method: 'DELETE' }),

    // My leave balances (for employees)
    getMy: () =>
        request<EmployeeLeaveBalanceDto[]>('/api/my-leave-balances'),
};

// ──────────────────────────────────────────────────────
// Holidays
// ──────────────────────────────────────────────────────
export const holidaysApi = {
    getByYear: (year: number) =>
        request<HolidayDto[]>(`/api/Holiday/${year}`),
};
