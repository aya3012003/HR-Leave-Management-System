import { useEffect, useState } from 'react';
import { getEmployees, createEmployee, updateEmployee, deleteEmployee } from '../api/employees';
import { getDepartments } from '../api/departments';
import { EmployeeDto, DepartmentDto } from '../types';
import Pagination from '../components/Pagination';
import Modal from '../components/Modal';
import { useAuth } from '../context/AuthContext';

const ROLES = ['Employee', 'Manager', 'Admin'];

export default function EmployeesPage() {
  const { isAdmin } = useAuth();
  const [items, setItems] = useState<EmployeeDto[]>([]);
  const [departments, setDepartments] = useState<DepartmentDto[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [modal, setModal] = useState<'create' | 'edit' | null>(null);
  const [selected, setSelected] = useState<EmployeeDto | null>(null);
  const [saving, setSaving] = useState(false);

  const [createForm, setCreateForm] = useState({ firstName: '', lastName: '', email: '', password: '', departmentId: '', role: 'Employee' });
  const [editForm, setEditForm] = useState({ firstName: '', lastName: '', departmentId: '' });

  const load = async (p = page, s = search) => {
    setLoading(true);
    try {
      const res = await getEmployees({ page: p, pageSize: 10, search: s });
      setItems(res.data.items);
      setTotalPages(res.data.totalPages);
    } catch { setError('Failed to load employees.'); }
    finally { setLoading(false); }
  };

  useEffect(() => {
    load();
    getDepartments({ pageSize: 100 }).then(r => setDepartments(r.data.items));
  }, [page]);

  const openCreate = () => { setCreateForm({ firstName: '', lastName: '', email: '', password: '', departmentId: '', role: 'Employee' }); setModal('create'); };
  const openEdit = (e: EmployeeDto) => {
    setSelected(e);
    setEditForm({ firstName: e.firstName, lastName: e.lastName, departmentId: e.departmentId?.toString() ?? '' });
    setModal('edit');
  };
  const closeModal = () => { setModal(null); setSelected(null); };

  const handleCreate = async () => {
    setSaving(true);
    try {
      await createEmployee({
        firstName: createForm.firstName, lastName: createForm.lastName,
        email: createForm.email, password: createForm.password,
        departmentId: createForm.departmentId ? Number(createForm.departmentId) : undefined,
        role: createForm.role,
      });
      closeModal(); load();
    } catch (err: unknown) {
      setError((err as { response?: { data?: { detail?: string } } })?.response?.data?.detail || 'Create failed.');
    } finally { setSaving(false); }
  };

  const handleEdit = async () => {
    if (!selected) return;
    setSaving(true);
    try {
      await updateEmployee(selected.id, {
        firstName: editForm.firstName, lastName: editForm.lastName,
        departmentId: editForm.departmentId ? Number(editForm.departmentId) : undefined,
      });
      closeModal(); load();
    } catch { setError('Update failed.'); }
    finally { setSaving(false); }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Delete this employee?')) return;
    try { await deleteEmployee(id); load(); }
    catch { setError('Delete failed.'); }
  };

  const setC = (f: string) => (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
    setCreateForm(p => ({ ...p, [f]: e.target.value }));
  const setE = (f: string) => (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
    setEditForm(p => ({ ...p, [f]: e.target.value }));

  const inputCls = "w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500";

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Employees</h1>
        {isAdmin && (
          <button onClick={openCreate} className="bg-indigo-600 hover:bg-indigo-700 text-white text-sm font-medium px-4 py-2 rounded-lg">
            + Add Employee
          </button>
        )}
      </div>

      {error && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">{error}</div>}

      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-100 flex gap-3">
          <input value={search} onChange={e => setSearch(e.target.value)}
            onKeyDown={e => e.key === 'Enter' && load(1, search)}
            placeholder="Search employees…"
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 w-64" />
          <button onClick={() => load(1, search)} className="px-4 py-2 text-sm bg-gray-100 hover:bg-gray-200 rounded-lg">Search</button>
        </div>

        <table className="w-full text-sm">
          <thead className="bg-gray-50 text-gray-500 uppercase text-xs tracking-wider">
            <tr>
              <th className="px-6 py-3 text-left">Name</th>
              <th className="px-6 py-3 text-left">Email</th>
              <th className="px-6 py-3 text-left">Department</th>
              <th className="px-6 py-3 text-left">Roles</th>
              {isAdmin && <th className="px-6 py-3 text-right">Actions</th>}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {loading ? (
              <tr><td colSpan={5} className="px-6 py-8 text-center text-gray-400">Loading…</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={5} className="px-6 py-8 text-center text-gray-400">No employees found.</td></tr>
            ) : items.map(emp => (
              <tr key={emp.id} className="hover:bg-gray-50">
                <td className="px-6 py-3 font-medium text-gray-800">{emp.fullName || `${emp.firstName} ${emp.lastName}`}</td>
                <td className="px-6 py-3 text-gray-500">{emp.email}</td>
                <td className="px-6 py-3 text-gray-600">{emp.departmentName ?? '—'}</td>
                <td className="px-6 py-3">
                  <span className="inline-flex gap-1">
                    {emp.roles.map(r => (
                      <span key={r} className="text-xs bg-indigo-50 text-indigo-600 px-2 py-0.5 rounded-full font-medium">{r}</span>
                    ))}
                  </span>
                </td>
                {isAdmin && (
                  <td className="px-6 py-3 text-right space-x-2">
                    <button onClick={() => openEdit(emp)} className="text-indigo-600 hover:underline text-xs font-medium">Edit</button>
                    <button onClick={() => handleDelete(emp.id)} className="text-red-500 hover:underline text-xs font-medium">Delete</button>
                  </td>
                )}
              </tr>
            ))}
          </tbody>
        </table>
        <div className="px-6 py-4 border-t border-gray-100">
          <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>
      </div>

      {/* Create Modal */}
      {modal === 'create' && (
        <Modal title="Add Employee" onClose={closeModal}>
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">First Name</label>
                <input value={createForm.firstName} onChange={setC('firstName')} className={inputCls} />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Last Name</label>
                <input value={createForm.lastName} onChange={setC('lastName')} className={inputCls} />
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
              <input type="email" value={createForm.email} onChange={setC('email')} className={inputCls} />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Password</label>
              <input type="password" value={createForm.password} onChange={setC('password')} className={inputCls} />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Department</label>
              <select value={createForm.departmentId} onChange={setC('departmentId')} className={inputCls}>
                <option value="">— None —</option>
                {departments.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Role</label>
              <select value={createForm.role} onChange={setC('role')} className={inputCls}>
                {ROLES.map(r => <option key={r} value={r}>{r}</option>)}
              </select>
            </div>
            <div className="flex justify-end gap-3 pt-2">
              <button onClick={closeModal} className="px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50">Cancel</button>
              <button onClick={handleCreate} disabled={saving}
                className="px-4 py-2 text-sm bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 disabled:opacity-60">
                {saving ? 'Creating…' : 'Create'}
              </button>
            </div>
          </div>
        </Modal>
      )}

      {/* Edit Modal */}
      {modal === 'edit' && (
        <Modal title="Edit Employee" onClose={closeModal}>
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">First Name</label>
                <input value={editForm.firstName} onChange={setE('firstName')} className={inputCls} />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Last Name</label>
                <input value={editForm.lastName} onChange={setE('lastName')} className={inputCls} />
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Department</label>
              <select value={editForm.departmentId} onChange={setE('departmentId')} className={inputCls}>
                <option value="">— None —</option>
                {departments.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}
              </select>
            </div>
            <div className="flex justify-end gap-3 pt-2">
              <button onClick={closeModal} className="px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50">Cancel</button>
              <button onClick={handleEdit} disabled={saving}
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
