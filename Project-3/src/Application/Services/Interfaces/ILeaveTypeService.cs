using Project_3.src.Application.DTOs;
using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.LeaveTypeDto;

namespace Project_3.src.Application.Services.Interfaces
{
    public interface ILeaveTypeService
    {
        Task<PagedResult<LeaveTypeDto>> GetPagedAsync(QueryParams query);

        Task<LeaveTypeDto> GetByIdAsync(int id);

        Task<LeaveTypeDto> CreateAsync(CreateLeaveTypeDto dto);

        Task<LeaveTypeDto> UpdateAsync(int id, UpdateLeaveTypeDto dto);

        Task DeleteAsync(int id);
    }
}
