import { useEffect, useState } from 'react';
import { getLeaveBalances, updateLeaveBalance, deleteLeaveBalance } from '../api/leaveBalances';
import { EmployeeLeaveBalanceDto } from '../types';
import Pagination from '../components/Pagination';
import Modal from '../components/Modal';
import { useAuth } from '../context/AuthContext';

export default function LeaveBalancesPage() {
  const { isAdmin } = useAuth();
  const [items, setItems] = useState<EmployeeLeaveBalanceDto[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [editModal, setEditModal] = useState<EmployeeLeaveBalanceDto | null>(null);
  const [days, setDays] = useState(0);
  const [saving, setSaving] = useState(false);

  const load = async (p = page) => {
    setLoading(true);
    try {
      const res = await getLeaveBalances({ pageNumber: p, pageSize: 15 });
      setItems(res.data.items);
      setTotalPages(res.data.totalPages);
    } catch { setError('Failed to load leave balances.'); }
    finally { setLoading(false); }
  };

  useEffect(() => { load(); }, [page]);

  const openEdit = (b: EmployeeLeaveBalanceDto) => { setDays(b.remainingDays); setEditModal(b); };

  const handleSave = async () => {
    if (!editModal) return;
    setSaving(true);
    try { await updateLeaveBalance(editModal.id, days); setEditModal(null); load(); }
    catch { setError('Update failed.'); }
    finally { setSaving(false); }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this leave balance record?')) return;
    try { await deleteLeaveBalance(id); load(); }
    catch { setError('Delete failed.'); }
  };

  return (
    <div className="p-8">
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Leave Balances</h1>
      {error && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">{error}</div>}

      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 text-gray-500 uppercase text-xs tracking-wider">
            <tr>
              <th className="px-6 py-3 text-left">Employee</th>
              <th className="px-6 py-3 text-left">Leave Type</th>
              <th className="px-6 py-3 text-left">Remaining Days</th>
              <th className="px-6 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {loading ? (
              <tr><td colSpan={4} className="px-6 py-8 text-center text-gray-400">Loading…</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={4} className="px-6 py-8 text-center text-gray-400">No balances found.</td></tr>
            ) : items.map(b => (
              <tr key={b.id} className="hover:bg-gray-50">
                <td className="px-6 py-3 font-medium text-gray-800">{b.employeeName}</td>
                <td className="px-6 py-3 text-gray-600">{b.leaveTypeName}</td>
                <td className="px-6 py-3">
                  <span className={`font-semibold ${b.remainingDays <= 3 ? 'text-red-600' : 'text-green-600'}`}>
                    {b.remainingDays}
                  </span>
                </td>
                <td className="px-6 py-3 text-right space-x-2">
                  <button onClick={() => openEdit(b)} className="text-indigo-600 hover:underline text-xs font-medium">Edit</button>
                  {isAdmin && (
                    <button onClick={() => handleDelete(b.id)} className="text-red-500 hover:underline text-xs font-medium">Delete</button>
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

      {editModal && (
        <Modal title="Edit Leave Balance" onClose={() => setEditModal(null)}>
          <div className="space-y-4">
            <p className="text-sm text-gray-600">
              Editing <strong>{editModal.employeeName}</strong> – {editModal.leaveTypeName}
            </p>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Remaining Days</label>
              <input type="number" min={0} value={days} onChange={e => setDays(Number(e.target.value))}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            </div>
            <div className="flex justify-end gap-3 pt-2">
              <button onClick={() => setEditModal(null)} className="px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50">Cancel</button>
              <button onClick={handleSave} disabled={saving}
                className="px-4 py-2 text-sm bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 disabled:opacity-60">
                {saving ? 'Saving…' : 'Save'}
              </button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}
