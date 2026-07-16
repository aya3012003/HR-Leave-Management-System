using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.EmployeeLeaveBalanceDtos;
using Project_3.src.Application.Models;

namespace Project_3.src.Infrastructure.Repositories.Interfaces
{
    public interface IEmployeeLeaveBalanceRepository: IRepository<EmployeeLeaveBalance>
    {
        Task<PagedResult<EmployeeLeaveBalance>> GetPagedAsync(EmployeeLeaveBalanceQueryParams query);
        Task<IEnumerable<EmployeeLeaveBalance>> GetByUserIdAsync(string userId);

        Task<EmployeeLeaveBalance?> GetByUserAndLeaveTypeAsync(string userId,int leaveTypeId);
    }
}
