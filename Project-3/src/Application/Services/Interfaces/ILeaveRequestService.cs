using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.LeaveRequestDTOs;

namespace Project_3.src.Application.Services.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<PagedResult<LeaveRequestDto>> GetAllAsync(LeaveRequestQueryParams query);

        Task<PagedResult<LeaveRequestDto>> GetMyRequestsAsync(string userId, LeaveRequestQueryParams query);

        Task<LeaveRequestDto> GetByIdAsync(int id);

        Task<LeaveRequestDto> CreateAsync(string userId, CreateLeaveRequestDto dto);

        Task<LeaveRequestDto> ApproveAsync(int id, string managerId, LeaveRequestActionDto dto);

        Task<LeaveRequestDto> RejectAsync(int id, string managerId, LeaveRequestActionDto dto);

        Task<LeaveRequestDto> CancelAsync(int id, string userId);
    }
}