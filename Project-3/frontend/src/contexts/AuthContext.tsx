import { createContext, useState, useEffect, useCallback } from 'react';
import { authApi, setTokens, clearTokens } from '@/lib/api';
import type { AuthResponseDto, LoginDto } from '@/types';

export interface AuthUser {
  userName: string;
  roles: string[];
  accessToken: string;
}

export interface AuthContextValue {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  isManager: boolean;
  isAdminOrManager: boolean;
  login: (dto: LoginDto) => Promise<void>;
  logout: () => Promise<void>;
  isLoading: boolean;
}

export const AuthContext = createContext<AuthContextValue | null>(null);

function parseUser(data: AuthResponseDto): AuthUser {
  return { userName: data.userName, roles: data.roles, accessToken: data.accessToken };
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(() => {
    try {
      const raw = localStorage.getItem('user');
      return raw ? JSON.parse(raw) : null;
    } catch { return null; }
  });
  const [isLoading, setIsLoading] = useState(false);

  const login = useCallback(async (dto: LoginDto) => {
    setIsLoading(true);
    try {
      const data = await authApi.login(dto);
      setTokens(data.accessToken, data.refreshToken);
      const u = parseUser(data);
      localStorage.setItem('user', JSON.stringify(u));
      setUser(u);
    } finally {
      setIsLoading(false);
    }
  }, []);

  const logout = useCallback(async () => {
    const refreshToken = localStorage.getItem('refresh_token');
    if (refreshToken) {
      try { await authApi.logout({ refreshToken }); } catch { /* ignore */ }
    }
    clearTokens();
    setUser(null);
  }, []);

  useEffect(() => {
    const handler = () => {
      clearTokens();
      setUser(null);
    };
    window.addEventListener('auth:logout', handler);
    return () => window.removeEventListener('auth:logout', handler);
  }, []);

  const roles = user?.roles ?? [];
  const isAdmin = roles.includes('Admin');
  const isManager = roles.includes('Manager');

  return (
    <AuthContext.Provider value={{
      user,
      isAuthenticated: !!user,
      isAdmin,
      isManager,
      isAdminOrManager: isAdmin || isManager,
      login,
      logout,
      isLoading,
    }}>
      {children}
    </AuthContext.Provider>
  );
}
