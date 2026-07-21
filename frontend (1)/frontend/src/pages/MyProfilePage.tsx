import { useEffect, useState } from 'react';
import { getMyProfile, updateMyProfile } from '../api/employees';
import { EmployeeDto } from '../types';

export default function MyProfilePage() {
  const [profile, setProfile] = useState<EmployeeDto | null>(null);
  const [editing, setEditing] = useState(false);
  const [form, setForm] = useState({ firstName: '', lastName: '' });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  useEffect(() => {
    getMyProfile().then(r => {
      setProfile(r.data);
      setForm({ firstName: r.data.firstName, lastName: r.data.lastName });
    }).catch(() => setError('Failed to load profile.'));
  }, []);

  const handleSave = async () => {
    setSaving(true); setError(''); setSuccess('');
    try {
      const updated = await updateMyProfile({
        firstName: form.firstName, lastName: form.lastName,
      });
      setProfile(updated.data);
      setEditing(false);
      setSuccess('Profile updated successfully.');
    } catch { setError('Update failed.'); }
    finally { setSaving(false); }
  };

  const set = (f: string) => (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
    setForm(p => ({ ...p, [f]: e.target.value }));

  const inputCls = "w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500";

  return (
    <div className="p-8 max-w-2xl">
      <h1 className="text-2xl font-bold text-gray-900 mb-6">My Profile</h1>

      {error && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">{error}</div>}
      {success && <div className="mb-4 p-3 bg-green-50 border border-green-200 rounded-lg text-green-700 text-sm">{success}</div>}

      {profile && (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 space-y-5">
          <div className="flex items-center justify-between">
            <div>
              <h2 className="text-lg font-semibold text-gray-900">{profile.fullName || `${profile.firstName} ${profile.lastName}`}</h2>
              <p className="text-sm text-gray-500">{profile.email}</p>
              <div className="flex gap-1 mt-1">
                {profile.roles.map(r => (
                  <span key={r} className="text-xs bg-indigo-50 text-indigo-600 px-2 py-0.5 rounded-full font-medium">{r}</span>
                ))}
              </div>
            </div>
            {!editing && (
              <button onClick={() => setEditing(true)} className="px-4 py-2 text-sm bg-indigo-600 text-white rounded-lg hover:bg-indigo-700">
                Edit
              </button>
            )}
          </div>

          {!editing ? (
            <dl className="grid grid-cols-2 gap-4 pt-4 border-t border-gray-100">
              <div><dt className="text-xs text-gray-500 mb-0.5">First Name</dt><dd className="text-sm font-medium text-gray-800">{profile.firstName}</dd></div>
              <div><dt className="text-xs text-gray-500 mb-0.5">Last Name</dt><dd className="text-sm font-medium text-gray-800">{profile.lastName}</dd></div>
              <div><dt className="text-xs text-gray-500 mb-0.5">Department</dt><dd className="text-sm font-medium text-gray-800">{profile.departmentName ?? '—'}</dd></div>
              {profile.hireDate && <div><dt className="text-xs text-gray-500 mb-0.5">Hire Date</dt><dd className="text-sm font-medium text-gray-800">{profile.hireDate}</dd></div>}
            </dl>
          ) : (
            <div className="space-y-4 pt-4 border-t border-gray-100">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">First Name</label>
                  <input value={form.firstName} onChange={set('firstName')} className={inputCls} />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Last Name</label>
                  <input value={form.lastName} onChange={set('lastName')} className={inputCls} />
                </div>
              </div>
              <div className="flex gap-3 pt-2">
                <button onClick={handleSave} disabled={saving}
                  className="px-4 py-2 text-sm bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 disabled:opacity-60">
                  {saving ? 'Saving…' : 'Save'}
                </button>
                <button onClick={() => setEditing(false)} className="px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50">
                  Cancel
                </button>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
