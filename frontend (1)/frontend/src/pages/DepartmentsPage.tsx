import { useEffect, useState } from 'react';
import { getDepartments, createDepartment, updateDepartment, deleteDepartment } from '../api/departments';
import { DepartmentDto } from '../types';
import Pagination from '../components/Pagination';
import Modal from '../components/Modal';

export default function DepartmentsPage() {
  const [items, setItems] = useState<DepartmentDto[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [modal, setModal] = useState<'create' | 'edit' | null>(null);
  const [selected, setSelected] = useState<DepartmentDto | null>(null);
  const [name, setName] = useState('');
  const [saving, setSaving] = useState(false);

  const load = async (p = page, s = search) => {
    setLoading(true);
    try {
      const res = await getDepartments({ pageNumber: p, pageSize: 10, search: s });
      setItems(res.data.items);
      setTotalPages(res.data.totalPages);
    } catch { setError('Failed to load departments.'); }
    finally { setLoading(false); }
  };

  useEffect(() => { load(); }, [page]);

  const openCreate = () => { setName(''); setSelected(null); setModal('create'); };
  const openEdit = (d: DepartmentDto) => { setName(d.name); setSelected(d); setModal('edit'); };
  const closeModal = () => { setModal(null); setSelected(null); };

  const handleSave = async () => {
    if (!name.trim()) return;
    setSaving(true);
    try {
      if (modal === 'create') await createDepartment(name.trim());
      else if (selected) await updateDepartment(selected.id, name.trim());
      closeModal();
      load();
    } catch { setError('Save failed.'); }
    finally { setSaving(false); }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this department?')) return;
    try { await deleteDepartment(id); load(); }
    catch { setError('Delete failed.'); }
  };

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Departments</h1>
        <button onClick={openCreate} className="bg-indigo-600 hover:bg-indigo-700 text-white text-sm font-medium px-4 py-2 rounded-lg transition-colors">
          + Add Department
        </button>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-100">
          <input value={search} onChange={e => setSearch(e.target.value)}
            onKeyDown={e => e.key === 'Enter' && load(1, search)}
            placeholder="Search departments…"
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 w-64" />
        </div>

        {error && <div className="mx-6 my-3 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">{error}</div>}

        <table className="w-full text-sm">
          <thead className="bg-gray-50 text-gray-500 uppercase text-xs tracking-wider">
            <tr>
              <th className="px-6 py-3 text-left">ID</th>
              <th className="px-6 py-3 text-left">Name</th>
              <th className="px-6 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {loading ? (
              <tr><td colSpan={3} className="px-6 py-8 text-center text-gray-400">Loading…</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={3} className="px-6 py-8 text-center text-gray-400">No departments found.</td></tr>
            ) : items.map(d => (
              <tr key={d.id} className="hover:bg-gray-50">
                <td className="px-6 py-3 text-gray-500">{d.id}</td>
                <td className="px-6 py-3 font-medium text-gray-800">{d.name}</td>
                <td className="px-6 py-3 text-right space-x-2">
                  <button onClick={() => openEdit(d)} className="text-indigo-600 hover:underline text-xs font-medium">Edit</button>
                  <button onClick={() => handleDelete(d.id)} className="text-red-500 hover:underline text-xs font-medium">Delete</button>
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
        <Modal title={modal === 'create' ? 'New Department' : 'Edit Department'} onClose={closeModal}>
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Name</label>
              <input value={name} onChange={e => setName(e.target.value)}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                placeholder="Department name" />
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
