import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { login as apiLogin, logout as apiLogout } from '../api/auth';

interface User {
  userName: string;
  roles: string[];
}

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  isManager: boolean;
  isEmployee: boolean;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    const userName = localStorage.getItem('userName');
    const roles = JSON.parse(localStorage.getItem('roles') || '[]');
    if (token && userName) {
      setUser({ userName, roles });
    }
    setLoading(false);
  }, []);

  const login = async (email: string, password: string) => {
    const res = await apiLogin(email, password);
    const { accessToken, refreshToken, userName, roles } = res.data;
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    localStorage.setItem('userName', userName);
    localStorage.setItem('roles', JSON.stringify(roles));
    setUser({ userName, roles });
  };

  const logout = async () => {
    const refreshToken = localStorage.getItem('refreshToken') || '';
    try { await apiLogout(refreshToken); } catch { /* ignore */ }
    localStorage.clear();
    setUser(null);
  };

  const roles = user?.roles ?? [];
  return (
    <AuthContext.Provider value={{
      user,
      isAuthenticated: !!user,
      isAdmin: roles.includes('Admin'),
      isManager: roles.includes('Manager'),
      isEmployee: roles.includes('Employee'),
      loading,
      login,
      logout,
    }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
