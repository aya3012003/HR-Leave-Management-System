import api from './client';
import { AuthResponse } from '../types';

export const login = (email: string, password: string) =>
  api.post<AuthResponse>('/Auth/login', { email, password });

export const register = (userName: string, email: string, password: string) =>
  api.post('/Auth/register', { userName, email, password });

export const refresh = (accessToken: string, refreshToken: string) =>
  api.post<AuthResponse>('/Auth/refresh', { accessToken, refreshToken });

export const logout = (refreshToken: string) =>
  api.post('/Auth/logout', { refreshToken });
