import { useEffect, useState } from 'react';
import { getStatistics, getLeaveSummary, getDepartmentLeaveUsage, getMostUsedLeaveType } from '../api/dashboard';
import { DashboardStatisticsDto, LeaveSummaryDto, DepartmentLeaveUsageDto, MostUsedLeaveTypeDto } from '../types';

function StatCard({ label, value, color }: { label: string; value: number | string; color: string }) {
  return (
    <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
      <p className="text-sm text-gray-500 font-medium">{label}</p>
      <p className={`text-3xl font-bold mt-1 ${color}`}>{value}</p>
    </div>
  );
}

export default function DashboardPage() {
  const [stats, setStats] = useState<DashboardStatisticsDto | null>(null);
  const [summary, setSummary] = useState<LeaveSummaryDto[]>([]);
  const [deptUsage, setDeptUsage] = useState<DepartmentLeaveUsageDto[]>([]);
  const [mostUsed, setMostUsed] = useState<MostUsedLeaveTypeDto | null>(null);
  const [error, setError] = useState('');

  useEffect(() => {
    Promise.all([
      getStatistics().then(r => setStats(r.data)),
      getLeaveSummary().then(r => setSummary(r.data)),
      getDepartmentLeaveUsage().then(r => setDeptUsage(r.data)),
      getMostUsedLeaveType().then(r => setMostUsed(r.data)),
    ]).catch(() => setError('Failed to load dashboard data.'));
  }, []);

  return (
    <div className="p-8">
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Dashboard</h1>
      {error && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">{error}</div>}

      {stats && (
        <div className="grid grid-cols-2 lg:grid-cols-3 gap-4 mb-8">
          <StatCard label="Total Employees" value={stats.totalEmployees} color="text-indigo-600" />
          <StatCard label="Departments" value={stats.totalDepartments} color="text-indigo-600" />
          <StatCard label="Pending Requests" value={stats.pendingLeaveRequests} color="text-amber-600" />
          <StatCard label="Approved Requests" value={stats.approvedLeaveRequests} color="text-green-600" />
          <StatCard label="Rejected Requests" value={stats.rejectedLeaveRequests} color="text-red-600" />
          <StatCard label="Cancelled Requests" value={stats.cancelledLeaveRequests} color="text-gray-500" />
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Leave Summary */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
          <h2 className="text-base font-semibold text-gray-800 mb-4">Leave Requests by Type</h2>
          {summary.length === 0 ? (
            <p className="text-gray-400 text-sm">No data available.</p>
          ) : (
            <div className="space-y-3">
              {summary.map(s => (
                <div key={s.leaveType} className="flex items-center justify-between">
                  <span className="text-sm text-gray-600">{s.leaveType}</span>
                  <span className="text-sm font-semibold text-indigo-600 bg-indigo-50 px-2 py-0.5 rounded-full">{s.count}</span>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Department Usage */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
          <h2 className="text-base font-semibold text-gray-800 mb-4">Leave Days by Department</h2>
          {deptUsage.length === 0 ? (
            <p className="text-gray-400 text-sm">No data available.</p>
          ) : (
            <div className="space-y-3">
              {deptUsage.map(d => (
                <div key={d.departmentName} className="flex items-center justify-between">
                  <span className="text-sm text-gray-600">{d.departmentName}</span>
                  <span className="text-sm font-semibold text-gray-800">{d.totalDays} days</span>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Most Used Leave Type */}
        {mostUsed && (
          <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
            <h2 className="text-base font-semibold text-gray-800 mb-2">Most Used Leave Type</h2>
            <p className="text-2xl font-bold text-indigo-600">{mostUsed.leaveType}</p>
            <p className="text-sm text-gray-500 mt-1">{mostUsed.count} requests</p>
          </div>
        )}
      </div>
    </div>
  );
}
