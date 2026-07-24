import { Link, useLocation } from "wouter";
import { useAuth } from "@/contexts/useAuth";
import { cn } from "@/lib/utils";
import { 
  LayoutDashboard, 
  CalendarDays, 
  WalletCards, 
  Palmtree, 
  UserCircle,
  Users,
  Building2,
  ListTodo,
  LogOut,
  Menu,
  X
} from "lucide-react";
import { useState } from "react";
import { initials } from "@/lib/utils";

export function Sidebar() {
  const { user, isAdmin, isManager, logout } = useAuth();
  const [location] = useLocation();
  const [isOpen, setIsOpen] = useState(false);

  const closeSidebar = () => setIsOpen(false);

  const links = [
    { href: "/dashboard", label: "Dashboard", icon: LayoutDashboard, visible: true },
    { href: "/my-leave-requests", label: "My Requests", icon: CalendarDays, visible: true },
    { href: "/my-leave-balances", label: "My Balances", icon: WalletCards, visible: true },
    { href: "/holidays", label: "Holidays", icon: Palmtree, visible: true },
    { href: "/profile", label: "My Profile", icon: UserCircle, visible: true },
    
    // Admin / Manager
    { href: "/leave-requests", label: "All Requests", icon: ListTodo, visible: isAdmin || isManager },
    { href: "/employees", label: "Employees", icon: Users, visible: isAdmin || isManager },
    { href: "/leave-balances", label: "All Balances", icon: WalletCards, visible: isAdmin || isManager },
    
    // Admin only
    { href: "/departments", label: "Departments", icon: Building2, visible: isAdmin },
    { href: "/leave-types", label: "Leave Types", icon: CalendarDays, visible: isAdmin },
  ];

  return (
    <>
      <button 
        className="md:hidden fixed top-4 left-4 z-50 p-2 bg-sidebar text-sidebar-foreground rounded-md shadow-md"
        onClick={() => setIsOpen(true)}
      >
        <Menu size={20} />
      </button>

      {isOpen && (
        <div 
          className="fixed inset-0 bg-black/50 z-40 md:hidden"
          onClick={closeSidebar}
        />
      )}

      <div className={cn(
        "fixed inset-y-0 left-0 z-50 w-64 bg-sidebar text-sidebar-foreground flex flex-col transition-transform duration-200 ease-in-out md:translate-x-0 md:static md:flex-shrink-0",
        isOpen ? "translate-x-0" : "-translate-x-full"
      )}>
        <div className="h-16 flex items-center px-6 border-b border-sidebar-border justify-between">
          <span className="font-bold text-lg tracking-tight">Acme HR</span>
          <button className="md:hidden p-1 text-sidebar-foreground" onClick={closeSidebar}>
            <X size={20} />
          </button>
        </div>

        <div className="flex-1 overflow-y-auto py-4 px-3 space-y-1">
          {links.filter(l => l.visible).map(link => {
            const isActive = location === link.href || location.startsWith(link.href + "/");
            return (
              <Link 
                key={link.href} 
                href={link.href}
                onClick={closeSidebar}
                className={cn(
                  "flex items-center gap-3 px-3 py-2 rounded-md text-sm font-medium transition-colors",
                  isActive 
                    ? "bg-sidebar-accent text-sidebar-accent-foreground" 
                    : "text-sidebar-foreground/80 hover:bg-sidebar-accent/50 hover:text-sidebar-foreground"
                )}
              >
                <link.icon size={18} />
                {link.label}
              </Link>
            );
          })}
        </div>

        <div className="p-4 border-t border-sidebar-border">
          <div className="flex items-center gap-3 mb-4">
            <div className="w-8 h-8 rounded-full bg-sidebar-primary text-sidebar-primary-foreground flex items-center justify-center font-bold text-sm">
              {initials(user?.userName || "")}
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium truncate">{user?.userName}</p>
              <p className="text-xs text-sidebar-foreground/60 truncate">
                {user?.roles?.join(", ")}
              </p>
            </div>
          </div>
          <button 
            onClick={() => { closeSidebar(); logout(); }}
            className="w-full flex items-center gap-2 justify-center px-3 py-2 rounded-md text-sm font-medium text-destructive-foreground bg-destructive/90 hover:bg-destructive transition-colors"
          >
            <LogOut size={16} />
            Log Out
          </button>
        </div>
      </div>
    </>
  );
}
