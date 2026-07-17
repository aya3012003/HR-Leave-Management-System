using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.Models;

namespace Project_3.src.Infrastructure.Repositories.Interfaces
{
    public interface ILeaveRequestRepository : IRepository<LeaveRequest>
    {
        Task<PagedResult<LeaveRequest>> GetPagedAsync(LeaveRequestQueryParams query);
    }
}
