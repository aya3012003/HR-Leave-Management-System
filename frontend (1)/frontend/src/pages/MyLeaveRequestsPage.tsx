import { useEffect, useState } from 'react';
import { getMyLeaveRequests, createLeaveRequest, cancelLeaveRequest } from '../api/leaveRequests';
import { getLeaveTypes } from '../api/leaveTypes';
import { LeaveRequestDto, LeaveTypeDto } from '../types';
import Pagination from '../components/Pagination';
import Modal from '../components/Modal';

const STATUS_COLORS: Record<string, string> = {
  Pending: 'bg-amber-100 text-amber-700',
  Approved: 'bg-green-100 text-green-700',
  Rejected: 'bg-red-100 text-red-700',
  Cancelled: 'bg-gray-100 text-gray-600',
};

interface Form { leaveTypeId: string; startDate: string; endDate: string; reason: string; }
const emptyForm: Form = { leaveTypeId: '', startDate: '', endDate: '', reason: '' };

export default function MyLeaveRequestsPage() {
  const [items, setItems] = useState<LeaveRequestDto[]>([]);
  const [leaveTypes, setLeaveTypes] = useState<LeaveTypeDto[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [statusFilter, setStatusFilter] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [showCreate, setShowCreate] = useState(false);
  const [form, setForm] = useState<Form>(emptyForm);
  const [saving, setSaving] = useState(false);

  const load = async (p = page, s = statusFilter) => {
    setLoading(true);
    try {
      const res = await getMyLeaveRequests({ pageNumber: p, pageSize: 10, status: s || undefined });
      setItems(res.data.items);
      setTotalPages(res.data.totalPages);
    } catch { setError('Failed to load leave requests.'); }
    finally { setLoading(false); }
  };

  useEffect(() => {
    load();
    getLeaveTypes({ pageSize: 100 }).then(r => setLeaveTypes(r.data.items));
  }, [page, statusFilter]);

  const handleCreate = async () => {
    setSaving(true); setError('');
    try {
      await createLeaveRequest({ leaveTypeId: Number(form.leaveTypeId), startDate: form.startDate, endDate: form.endDate, reason: form.reason });
      setShowCreate(false); setForm(emptyForm); load();
    } catch (err: unknown) {
      setError((err as { response?: { data?: { detail?: string } } })?.response?.data?.detail || 'Failed to create request.');
    } finally { setSaving(false); }
  };

  const handleCancel = async (id: number) => {
    if (!confirm('Cancel this leave request?')) return;
    try { await cancelLeaveRequest(id); load(); }
    catch { setError('Cancel failed.'); }
  };

  const set = (f: keyof Form) => (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) =>
    setForm(p => ({ ...p, [f]: e.target.value }));

  const inputCls = "w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500";

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">My Leave Requests</h1>
        <div className="flex items-center gap-3">
          <select value={statusFilter} onChange={e => { setStatusFilter(e.target.value); setPage(1); }}
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500">
            <option value="">All Statuses</option>
            <option>Pending</option><option>Approved</option><option>Rejected</option><option>Cancelled</option>
          </select>
          <button onClick={() => { setForm(emptyForm); setShowCreate(true); }}
            className="bg-indigo-600 hover:bg-indigo-700 text-white text-sm font-medium px-4 py-2 rounded-lg">
            + New Request
          </button>
        </div>
      </div>

      {error && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">{error}</div>}

      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 text-gray-500 uppercase text-xs tracking-wider">
            <tr>
              <th className="px-6 py-3 text-left">Leave Type</th>
              <th className="px-6 py-3 text-left">Start</th>
              <th className="px-6 py-3 text-left">End</th>
              <th className="px-6 py-3 text-left">Days</th>
              <th className="px-6 py-3 text-left">Status</th>
              <th className="px-6 py-3 text-left">Manager Note</th>
              <th className="px-6 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {loading ? (
              <tr><td colSpan={7} className="px-6 py-8 text-center text-gray-400">Loading…</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={7} className="px-6 py-8 text-center text-gray-400">No leave requests yet.</td></tr>
            ) : items.map(req => (
              <tr key={req.id} className="hover:bg-gray-50">
                <td className="px-6 py-3 font-medium text-gray-800">{req.leaveTypeName}</td>
                <td className="px-6 py-3 text-gray-600">{req.startDate}</td>
                <td className="px-6 py-3 text-gray-600">{req.endDate}</td>
                <td className="px-6 py-3 text-gray-600">{req.workingDays}</td>
                <td className="px-6 py-3">
                  <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${STATUS_COLORS[req.status] ?? 'bg-gray-100 text-gray-600'}`}>
                    {req.status}
                  </span>
                </td>
                <td className="px-6 py-3 text-gray-500 max-w-xs truncate">{req.managerComment ?? '—'}</td>
                <td className="px-6 py-3 text-right">
                  {req.status === 'Pending' && (
                    <button onClick={() => handleCancel(req.id)} className="text-red-500 hover:underline text-xs font-medium">Cancel</button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="px-6 py-4 border-t border-gray-100">
          <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>
      </div>

      {showCreate && (
        <Modal title="New Leave Request" onClose={() => setShowCreate(false)}>
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Leave Type</label>
              <select value={form.leaveTypeId} onChange={set('leaveTypeId')} className={inputCls}>
                <option value="">Select leave type…</option>
                {leaveTypes.map(lt => <option key={lt.id} value={lt.id}>{lt.name} ({lt.defaultDays} days)</option>)}
              </select>
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Start Date</label>
                <input type="date" value={form.startDate} onChange={set('startDate')} className={inputCls} />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">End Date</label>
                <input type="date" value={form.endDate} onChange={set('endDate')} className={inputCls} />
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Reason</label>
              <textarea value={form.reason} onChange={set('reason')} rows={3} className={inputCls} placeholder="Reason for leave…" />
            </div>
            <div className="flex justify-end gap-3 pt-2">
              <button onClick={() => setShowCreate(false)} className="px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50">Cancel</button>
              <button onClick={handleCreate} disabled={saving || !form.leaveTypeId || !form.startDate || !form.endDate}
                className="px-4 py-2 text-sm bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 disabled:opacity-60">
                {saving ? 'Submitting…' : 'Submit'}
              </button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}
