import { useQuery } from "@tanstack/react-query";
import { dashboardApi, leaveBalancesApi, leaveRequestsApi } from "@/lib/api";
import { useAuth } from "@/contexts/useAuth";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from "recharts";
import { Users, Building2, CalendarRange, CheckCircle2, Clock, XCircle, ArrowRight } from "lucide-react";
import { Link } from "wouter";
import { Button } from "@/components/ui/button";
import { statusColor, formatDate } from "@/lib/utils";

const COLORS = ['#2563eb', '#16a34a', '#d97706', '#9333ea', '#dc2626'];

export default function DashboardPage() {
  const { isAdminOrManager, isManager } = useAuth();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Dashboard</h1>
        <p className="text-muted-foreground mt-1">Welcome to the HR Portal.</p>
      </div>

      {isAdminOrManager ? <ManagerDashboard /> : <EmployeeDashboard />}
    </div>
  );
}

function ManagerDashboard() {
  const { data: stats, isLoading: statsLoading } = useQuery({
    queryKey: ['dashboard', 'stats'],
    queryFn: dashboardApi.getStats,
  });

  const { data: deptUsage } = useQuery({
    queryKey: ['dashboard', 'dept-usage'],
    queryFn: dashboardApi.getDepartmentLeaveUsage,
  });

  const { data: mostUsed } = useQuery({
    queryKey: ['dashboard', 'most-used'],
    queryFn: dashboardApi.getMostUsedLeaveType,
  });

  if (statsLoading) return <div>Loading dashboard...</div>;

  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
        <StatCard title="Total Employees" value={stats?.totalEmployees} icon={Users} />
        <StatCard title="Departments" value={stats?.totalDepartments} icon={Building2} />
        <StatCard title="Total Requests" value={stats?.totalLeaveRequests} icon={CalendarRange} />
        <StatCard title="Pending" value={stats?.pendingRequests} icon={Clock} color="text-amber-500" />
        <StatCard title="Approved" value={stats?.approvedRequests} icon={CheckCircle2} color="text-emerald-500" />
        <StatCard title="Rejected" value={stats?.rejectedRequests} icon={XCircle} color="text-red-500" />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Department Leave Usage (Days)</CardTitle>
          </CardHeader>
          <CardContent className="h-[300px]">
            {deptUsage && deptUsage.length > 0 ? (
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={deptUsage}>
                  <XAxis dataKey="departmentName" fontSize={12} tickLine={false} axisLine={false} />
                  <YAxis fontSize={12} tickLine={false} axisLine={false} />
                  <Tooltip cursor={{ fill: 'transparent' }} />
                  <Bar dataKey="totalDays" fill="hsl(var(--primary))" radius={[4, 4, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            ) : (
              <div className="h-full flex items-center justify-center text-muted-foreground">No data</div>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Most Used Leave Types</CardTitle>
          </CardHeader>
          <CardContent className="h-[300px]">
            {mostUsed && mostUsed.length > 0 ? (
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={mostUsed}
                    dataKey="count"
                    nameKey="leaveType"
                    cx="50%"
                    cy="50%"
                    outerRadius={100}
                    label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                  >
                    {mostUsed.map((_, index) => (
                      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip />
                </PieChart>
              </ResponsiveContainer>
            ) : (
              <div className="h-full flex items-center justify-center text-muted-foreground">No data</div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

function EmployeeDashboard() {
  const { data: balances, isLoading: balancesLoading } = useQuery({
    queryKey: ['my-leave-balances'],
    queryFn: leaveBalancesApi.getMy,
  });

  const { data: requests, isLoading: requestsLoading } = useQuery({
    queryKey: ['my-leave-requests'],
    queryFn: leaveRequestsApi.getMy,
  });

  if (balancesLoading || requestsLoading) return <div>Loading...</div>;

  return (
    <div className="space-y-6">
      <div className="flex justify-end">
        <Link href="/my-leave-requests/new" className="inline-flex">
          <Button>Apply for Leave</Button>
        </Link>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {balances?.map(balance => (
          <Card key={balance.id}>
            <CardContent className="p-6 flex flex-col items-center justify-center text-center">
              <p className="text-sm font-medium text-muted-foreground uppercase tracking-wider mb-2">{balance.leaveTypeName}</p>
              <p className="text-4xl font-bold">{balance.remainingDays}</p>
              <p className="text-sm text-muted-foreground mt-1">days remaining</p>
            </CardContent>
          </Card>
        ))}
      </div>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Recent Requests</CardTitle>
          <Link href="/my-leave-requests" className="text-sm text-primary hover:underline flex items-center gap-1">
            View All <ArrowRight size={14} />
          </Link>
        </CardHeader>
        <CardContent>
          {requests && requests.length > 0 ? (
            <div className="space-y-4">
              {requests.slice(0, 5).map(req => (
                <div key={req.id} className="flex items-center justify-between p-3 border rounded-lg">
                  <div>
                    <p className="font-medium">{req.leaveTypeName}</p>
                    <p className="text-sm text-muted-foreground">
                      {formatDate(req.startDate)} to {formatDate(req.endDate)} ({req.workingDays} days)
                    </p>
                  </div>
                  <span className={`px-2.5 py-1 rounded-full text-xs font-medium ${statusColor(req.status)}`}>
                    {req.status}
                  </span>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-sm text-muted-foreground text-center py-4">No recent leave requests.</p>
          )}
        </CardContent>
      </Card>
    </div>
  );
}

function StatCard({ title, value, icon: Icon, color = "text-muted-foreground" }: any) {
  return (
    <Card>
      <CardContent className="p-6">
        <div className="flex items-center justify-between space-x-2">
          <p className="text-sm font-medium text-muted-foreground">{title}</p>
          <Icon className={`h-4 w-4 ${color}`} />
        </div>
        <div className="mt-4">
          <span className="text-3xl font-bold">{value ?? '-'}</span>
        </div>
      </CardContent>
    </Card>
  );
}
