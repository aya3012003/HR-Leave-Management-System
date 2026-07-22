import { NavLink, useNavigate, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

interface NavItem { to: string; label: string; }

export default function Layout() {
  const { user, isAdmin, isManager, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const adminItems: NavItem[] = [
    { to: '/dashboard', label: 'Dashboard' },
    { to: '/employees', label: 'Employees' },
    { to: '/departments', label: 'Departments' },
    { to: '/leave-requests', label: 'Leave Requests' },
    { to: '/leave-types', label: 'Leave Types' },
    { to: '/leave-balances', label: 'Leave Balances' },
  ];

  const managerItems: NavItem[] = [
    { to: '/dashboard', label: 'Dashboard' },
    { to: '/employees', label: 'Employees' },
    { to: '/leave-requests', label: 'Leave Requests' },
    { to: '/leave-balances', label: 'Leave Balances' },
  ];

  const employeeItems: NavItem[] = [
    { to: '/my-leave-requests', label: 'My Leave Requests' },
    { to: '/my-leave-balances', label: 'My Leave Balances' },
    { to: '/my-profile', label: 'My Profile' },
  ];

  const navItems = isAdmin ? adminItems : isManager ? managerItems : employeeItems;
  // Shared "My" section for admin/manager too
  const personalItems: NavItem[] = [
    { to: '/my-profile', label: 'My Profile' },
    { to: '/my-leave-requests', label: 'My Leave Requests' },
  ];

  const linkClass = ({ isActive }: { isActive: boolean }) =>
    `block px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
      isActive ? 'bg-indigo-700 text-white' : 'text-indigo-100 hover:bg-indigo-700/60'
    }`;

  return (
    <div className="flex h-screen bg-slate-50">
      {/* Sidebar */}
      <aside className="w-60 flex-shrink-0 bg-indigo-900 flex flex-col">
        <div className="px-6 py-5 border-b border-indigo-800">
          <p className="text-white font-bold text-lg leading-tight">HR Leave</p>
          <p className="text-indigo-300 text-xs mt-0.5">Management System</p>
        </div>
        <nav className="flex-1 overflow-y-auto px-3 py-4 space-y-1">
          {(isAdmin || isManager) && (
            <>
              <p className="px-4 py-1 text-xs text-indigo-400 uppercase tracking-wider font-semibold">Management</p>
              {navItems.map(item => (
                <NavLink key={item.to} to={item.to} className={linkClass}>{item.label}</NavLink>
              ))}
              <p className="px-4 py-1 mt-3 text-xs text-indigo-400 uppercase tracking-wider font-semibold">Personal</p>
              {personalItems.map(item => (
                <NavLink key={item.to} to={item.to} className={linkClass}>{item.label}</NavLink>
              ))}
            </>
          )}
          {!isAdmin && !isManager && navItems.map(item => (
            <NavLink key={item.to} to={item.to} className={linkClass}>{item.label}</NavLink>
          ))}
        </nav>
        <div className="px-4 py-4 border-t border-indigo-800">
          <p className="text-indigo-200 text-sm font-medium truncate">{user?.userName}</p>
          <p className="text-indigo-400 text-xs">{user?.roles.join(', ')}</p>
          <button
            onClick={handleLogout}
            className="mt-3 w-full text-left text-sm text-indigo-300 hover:text-white transition-colors"
          >
            Sign out →
          </button>
        </div>
      </aside>

      {/* Main content */}
      <main className="flex-1 overflow-y-auto">
        <Outlet />
      </main>
    </div>
  );
}
