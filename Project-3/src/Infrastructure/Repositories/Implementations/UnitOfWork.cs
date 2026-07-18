using Project_3.src.Application.Models;
using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3.src.Infrastructure.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IDepartmentRepository Departments { get; }

        public ILeaveRequestRepository LeaveRequests { get; }

        public ILeaveTypeRepository LeaveTypes { get; }

        public IEmployeeLeaveBalanceRepository LeaveBalances { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;


            Departments = new DepartmentRepository(context);
            LeaveRequests = new LeaveRequestRepository(context);
            LeaveTypes = new LeaveTypeRepository(context);
            LeaveBalances = new EmployeeLeaveBalanceRepository(context);
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

    }
}
