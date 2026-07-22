import { useEffect, useState } from 'react';
import { getMyLeaveBalances } from '../api/leaveBalances';
import { EmployeeLeaveBalanceDto } from '../types';

export default function MyLeaveBalancesPage() {
  const [balances, setBalances] = useState<EmployeeLeaveBalanceDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    setLoading(true);
    getMyLeaveBalances()
      .then(r => setBalances(r.data))
      .catch(() => setError('Failed to load your leave balances.'))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="p-8 max-w-2xl">
      <h1 className="text-2xl font-bold text-gray-900 mb-6">My Leave Balances</h1>
      {error && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">{error}</div>}

      {loading ? (
        <p className="text-gray-400 text-sm">Loading…</p>
      ) : balances.length === 0 ? (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-8 text-center text-gray-400">
          No leave balances found.
        </div>
      ) : (
        <div className="grid gap-4">
          {balances.map(b => (
            <div key={b.id} className="bg-white rounded-xl shadow-sm border border-gray-100 p-5 flex items-center justify-between">
              <div>
                <p className="font-semibold text-gray-800">{b.leaveTypeName}</p>
              </div>
              <div className="text-right">
                <p className={`text-3xl font-bold ${b.remainingDays <= 3 ? 'text-red-600' : 'text-green-600'}`}>
                  {b.remainingDays}
                </p>
                <p className="text-xs text-gray-500 mt-0.5">days remaining</p>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
