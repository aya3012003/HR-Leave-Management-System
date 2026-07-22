import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import Layout from './components/Layout';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import DashboardPage from './pages/DashboardPage';
import EmployeesPage from './pages/EmployeesPage';
import DepartmentsPage from './pages/DepartmentsPage';
import LeaveRequestsPage from './pages/LeaveRequestsPage';
import LeaveTypesPage from './pages/LeaveTypesPage';
import LeaveBalancesPage from './pages/LeaveBalancesPage';
import MyProfilePage from './pages/MyProfilePage';
import MyLeaveRequestsPage from './pages/MyLeaveRequestsPage';
import MyLeaveBalancesPage from './pages/MyLeaveBalancesPage';

function HomeRedirect() {
  const { isAdmin, isManager } = useAuth();
  return <Navigate to={isAdmin || isManager ? '/dashboard' : '/my-leave-requests'} replace />;
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route element={<ProtectedRoute><Layout /></ProtectedRoute>}>
            <Route index element={<ProtectedRoute><HomeRedirect /></ProtectedRoute>} />
            <Route path="dashboard" element={<ProtectedRoute requireAdminOrManager><DashboardPage /></ProtectedRoute>} />
            <Route path="employees" element={<ProtectedRoute requireAdminOrManager><EmployeesPage /></ProtectedRoute>} />
            <Route path="departments" element={<ProtectedRoute requireAdmin><DepartmentsPage /></ProtectedRoute>} />
            <Route path="leave-requests" element={<ProtectedRoute requireAdminOrManager><LeaveRequestsPage /></ProtectedRoute>} />
            <Route path="leave-types" element={<ProtectedRoute requireAdmin><LeaveTypesPage /></ProtectedRoute>} />
            <Route path="leave-balances" element={<ProtectedRoute requireAdminOrManager><LeaveBalancesPage /></ProtectedRoute>} />
            <Route path="my-profile" element={<ProtectedRoute><MyProfilePage /></ProtectedRoute>} />
            <Route path="my-leave-requests" element={<ProtectedRoute><MyLeaveRequestsPage /></ProtectedRoute>} />
            <Route path="my-leave-balances" element={<ProtectedRoute><MyLeaveBalancesPage /></ProtectedRoute>} />
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
