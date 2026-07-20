using Microsoft.AspNetCore.Identity;
using Project_3.src.Application.Models;

namespace Project_3.src.Infrastructure.Repositories.Interfaces
{
    public interface IUnitOfWork
    {

        IDepartmentRepository Departments { get; }
        ILeaveRequestRepository LeaveRequests { get; }
        ILeaveTypeRepository LeaveTypes { get; }
        IHolidayRepository Holidays { get; }
        IEmployeeLeaveBalanceRepository LeaveBalances { get; }
        Task<int> SaveChangesAsync();

    }
}
