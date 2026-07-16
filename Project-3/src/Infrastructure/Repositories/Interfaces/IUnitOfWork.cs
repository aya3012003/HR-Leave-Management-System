using Microsoft.AspNetCore.Identity;
using Project_3.src.Application.Models;

namespace Project_3.src.Infrastructure.Repositories.Interfaces
{
    public interface IUnitOfWork
    {


        IDepartmentRepository Departments { get; }
        IRepository<LeaveRequest> LeaveRequests { get; }

        ILeaveTypeRepository LeaveTypes { get; }

        IEmployeeLeaveBalanceRepository LeaveBalances { get; }
        Task<int> SaveChangesAsync();

    }
}
