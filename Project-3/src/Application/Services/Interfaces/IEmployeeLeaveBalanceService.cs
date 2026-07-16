using Project_3.src.Application.DTOs;
using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.EmployeeLeaveBalanceDtos;

namespace Project_3.src.Application.Services.Interfaces
{
    public interface IEmployeeLeaveBalanceService
    {
        Task<PagedResult<EmployeeLeaveBalanceDto>>GetPagedAsync(EmployeeLeaveBalanceQueryParams query);
        Task<EmployeeLeaveBalanceDto> GetByIdAsync(int id);
        Task<IEnumerable<EmployeeLeaveBalanceDto>> GetMyBalancesAsync(string userId);

        Task<EmployeeLeaveBalanceDto> CreateAsync(CreateEmployeeLeaveBalanceDto dto);

        Task<EmployeeLeaveBalanceDto> UpdateAsync(int id, UpdateEmployeeLeaveBalanceDto dto);

        Task DeleteAsync(int id);
    }
}
