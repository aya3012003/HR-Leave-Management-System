import { useEffect, useState } from 'react';
import { getLeaveRequests, approveLeaveRequest, rejectLeaveRequest } from '../api/leaveRequests';
import { LeaveRequestDto } from '../types';
import Pagination from '../components/Pagination';
import Modal from '../components/Modal';

const STATUS_COLORS: Record<string, string> = {
  Pending: 'bg-amber-100 text-amber-700',
  Approved: 'bg-green-100 text-green-700',
  Rejected: 'bg-red-100 text-red-700',
  Cancelled: 'bg-gray-100 text-gray-600',
};

export default function LeaveRequestsPage() {
  const [items, setItems] = useState<LeaveRequestDto[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [statusFilter, setStatusFilter] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [actionModal, setActionModal] = useState<{ type: 'approve' | 'reject'; request: LeaveRequestDto } | null>(null);
  const [comment, setComment] = useState('');
  const [saving, setSaving] = useState(false);

  const load = async (p = page, s = statusFilter) => {
    setLoading(true);
    try {
      const res = await getLeaveRequests({ pageNumber: p, pageSize: 10, status: s || undefined });
      setItems(res.data.items);
      setTotalPages(res.data.totalPages);
    } catch { setError('Failed to load leave requests.'); }
    finally { setLoading(false); }
  };

  useEffect(() => { load(); }, [page, statusFilter]);

  const openAction = (type: 'approve' | 'reject', req: LeaveRequestDto) => { setComment(''); setActionModal({ type, request: req }); };

  const handleAction = async () => {
    if (!actionModal) return;
    setSaving(true);
    try {
      if (actionModal.type === 'approve') await approveLeaveRequest(actionModal.request.id, comment);
      else await rejectLeaveRequest(actionModal.request.id, comment);
      setActionModal(null);
      load();
    } catch { setError('Action failed.'); }
    finally { setSaving(false); }
  };

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Leave Requests</h1>
        <select value={statusFilter} onChange={e => { setStatusFilter(e.target.value); setPage(1); }}
          className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500">
          <option value="">All Statuses</option>
          <option>Pending</option><option>Approved</option><option>Rejected</option><option>Cancelled</option>
        </select>
      </div>

      {error && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">{error}</div>}

      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 text-gray-500 uppercase text-xs tracking-wider">
            <tr>
              <th className="px-6 py-3 text-left">Employee</th>
              <th className="px-6 py-3 text-left">Leave Type</th>
              <th className="px-6 py-3 text-left">Start</th>
              <th className="px-6 py-3 text-left">End</th>
              <th className="px-6 py-3 text-left">Days</th>
              <th className="px-6 py-3 text-left">Status</th>
              <th className="px-6 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {loading ? (
              <tr><td colSpan={7} className="px-6 py-8 text-center text-gray-400">Loading…</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={7} className="px-6 py-8 text-center text-gray-400">No leave requests found.</td></tr>
            ) : items.map(req => (
              <tr key={req.id} className="hover:bg-gray-50">
                <td className="px-6 py-3 font-medium text-gray-800">{req.employeeName}</td>
                <td className="px-6 py-3 text-gray-600">{req.leaveTypeName}</td>
                <td className="px-6 py-3 text-gray-600">{req.startDate}</td>
                <td className="px-6 py-3 text-gray-600">{req.endDate}</td>
                <td className="px-6 py-3 text-gray-600">{req.workingDays}</td>
                <td className="px-6 py-3">
                  <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${STATUS_COLORS[req.status] ?? 'bg-gray-100 text-gray-600'}`}>
                    {req.status}
                  </span>
                </td>
                <td className="px-6 py-3 text-right space-x-2">
                  {req.status === 'Pending' && (
                    <>
                      <button onClick={() => openAction('approve', req)} className="text-green-600 hover:underline text-xs font-medium">Approve</button>
                      <button onClick={() => openAction('reject', req)} className="text-red-500 hover:underline text-xs font-medium">Reject</button>
                    </>
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

      {actionModal && (
        <Modal title={actionModal.type === 'approve' ? 'Approve Request' : 'Reject Request'} onClose={() => setActionModal(null)}>
          <div className="space-y-4">
            <p className="text-sm text-gray-600">
              {actionModal.type === 'approve' ? 'Approve' : 'Reject'} leave request for{' '}
              <strong>{actionModal.request.employeeName}</strong> ({actionModal.request.leaveTypeName},{' '}
              {actionModal.request.startDate} – {actionModal.request.endDate})?
            </p>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Manager Comment</label>
              <textarea value={comment} onChange={e => setComment(e.target.value)} rows={3}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                placeholder="Optional comment…" />
            </div>
            <div className="flex justify-end gap-3 pt-2">
              <button onClick={() => setActionModal(null)} className="px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50">Cancel</button>
              <button onClick={handleAction} disabled={saving}
                className={`px-4 py-2 text-sm text-white rounded-lg disabled:opacity-60 ${actionModal.type === 'approve' ? 'bg-green-600 hover:bg-green-700' : 'bg-red-600 hover:bg-red-700'}`}>
                {saving ? 'Saving…' : actionModal.type === 'approve' ? 'Approve' : 'Reject'}
              </button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}
