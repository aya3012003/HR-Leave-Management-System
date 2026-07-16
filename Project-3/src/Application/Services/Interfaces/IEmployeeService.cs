using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.EmployeeDTOs;

namespace Project_3.src.Application.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<PagedResult<EmployeeDto>> GetEmployeesAsync(int page, int pageSize, int? deptId, string? search);
        Task<EmployeeDto?> GetEmployeeByIdAsync(string id);
        Task<EmployeeDto?> GetEmployeeProfileAsync(string userId);
        Task<EmployeeDto?> CreateEmployeeAsync(CreateEmployeeDto dto);
        Task<EmployeeDto?> UpdateEmployeeAsync(string id, UpdateEmployeeDto dto);
        Task<bool> DeleteEmployeeAsync(string id);
    }
}
