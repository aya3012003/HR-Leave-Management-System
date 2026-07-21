import { useEffect, useState } from 'react';
import { getLeaveTypes, createLeaveType, updateLeaveType, deleteLeaveType } from '../api/leaveTypes';
import { LeaveTypeDto } from '../types';
import Pagination from '../components/Pagination';
import Modal from '../components/Modal';

interface Form { name: string; defaultDays: number; description: string; }
const empty: Form = { name: '', defaultDays: 0, description: '' };

export default function LeaveTypesPage() {
  const [items, setItems] = useState<LeaveTypeDto[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [modal, setModal] = useState<'create' | 'edit' | null>(null);
  const [selected, setSelected] = useState<LeaveTypeDto | null>(null);
  const [form, setForm] = useState<Form>(empty);
  const [saving, setSaving] = useState(false);

  const load = async (p = page) => {
    setLoading(true);
    try {
      const res = await getLeaveTypes({ pageNumber: p, pageSize: 10 });
      setItems(res.data.items);
      setTotalPages(res.data.totalPages);
    } catch { setError('Failed to load leave types.'); }
    finally { setLoading(false); }
  };

  useEffect(() => { load(); }, [page]);

  const openCreate = () => { setForm(empty); setSelected(null); setModal('create'); };
  const openEdit = (lt: LeaveTypeDto) => {
    setForm({ name: lt.name, defaultDays: lt.defaultDays, description: lt.description ?? '' });
    setSelected(lt); setModal('edit');
  };
  const closeModal = () => { setModal(null); setSelected(null); };
  const set = (field: keyof Form) => (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) =>
    setForm(prev => ({ ...prev, [field]: field === 'defaultDays' ? Number(e.target.value) : e.target.value }));

  const handleSave = async () => {
    setSaving(true);
    try {
      if (modal === 'create') await createLeaveType(form);
      else if (selected) await updateLeaveType(selected.id, form);
      closeModal(); load();
    } catch { setError('Save failed.'); }
    finally { setSaving(false); }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this leave type?')) return;
    try { await deleteLeaveType(id); load(); }
    catch { setError('Delete failed.'); }
  };

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Leave Types</h1>
        <button onClick={openCreate} className="bg-indigo-600 hover:bg-indigo-700 text-white text-sm font-medium px-4 py-2 rounded-lg">
          + Add Leave Type
        </button>
      </div>

      {error && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">{error}</div>}

      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 text-gray-500 uppercase text-xs tracking-wider">
            <tr>
              <th className="px-6 py-3 text-left">Name</th>
              <th className="px-6 py-3 text-left">Default Days</th>
              <th className="px-6 py-3 text-left">Description</th>
              <th className="px-6 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {loading ? (
              <tr><td colSpan={4} className="px-6 py-8 text-center text-gray-400">Loading…</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={4} className="px-6 py-8 text-center text-gray-400">No leave types found.</td></tr>
            ) : items.map(lt => (
              <tr key={lt.id} className="hover:bg-gray-50">
                <td className="px-6 py-3 font-medium text-gray-800">{lt.name}</td>
                <td className="px-6 py-3 text-gray-600">{lt.defaultDays}</td>
                <td className="px-6 py-3 text-gray-500 max-w-xs truncate">{lt.description ?? '—'}</td>
                <td className="px-6 py-3 text-right space-x-2">
                  <button onClick={() => openEdit(lt)} className="text-indigo-600 hover:underline text-xs font-medium">Edit</button>
                  <button onClick={() => handleDelete(lt.id)} className="text-red-500 hover:underline text-xs font-medium">Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="px-6 py-4 border-t border-gray-100">
          <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>
      </div>

      {modal && (
        <Modal title={modal === 'create' ? 'New Leave Type' : 'Edit Leave Type'} onClose={closeModal}>
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Name</label>
              <input value={form.name} onChange={set('name')}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                placeholder="e.g. Annual Leave" />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Default Days</label>
              <input type="number" min={0} value={form.defaultDays} onChange={set('defaultDays')}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Description</label>
              <textarea value={form.description} onChange={set('description')} rows={3}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                placeholder="Optional description" />
            </div>
            <div className="flex justify-end gap-3 pt-2">
              <button onClick={closeModal} className="px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50">Cancel</button>
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
