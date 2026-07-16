using Project_3.src.Application.Models;
using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3.src.Infrastructure.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

     

        public IRepository<Department> Departments { get; }

        public IRepository<LeaveRequest> LeaveRequests { get; }

          public   ILeaveTypeRepository LeaveTypes { get; }

        public IRepository<EmployeeLeaveBalance> LeaveBalances { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

           
            Departments = new Repository<Department>(context);
            LeaveRequests = new Repository<LeaveRequest>(context);
            LeaveTypes = new LeaveTypeRepository(context);
            LeaveBalances = new Repository<EmployeeLeaveBalance>(context);
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

    }
}
