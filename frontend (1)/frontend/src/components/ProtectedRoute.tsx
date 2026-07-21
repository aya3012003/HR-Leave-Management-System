import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { ReactNode } from 'react';

interface Props {
  children: ReactNode;
  requireAdmin?: boolean;
  requireAdminOrManager?: boolean;
}

export default function ProtectedRoute({ children, requireAdmin, requireAdminOrManager }: Props) {
  const { isAuthenticated, isAdmin, isManager, loading } = useAuth();

  if (loading) return <div className="flex items-center justify-center h-screen">Loading…</div>;
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  if (requireAdmin && !isAdmin) return <Navigate to="/" replace />;
  if (requireAdminOrManager && !isAdmin && !isManager) return <Navigate to="/" replace />;

  return <>{children}</>;
}
