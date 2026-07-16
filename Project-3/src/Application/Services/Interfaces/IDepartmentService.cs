using Project_3.src.Application.DTOs;
using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.DepartmentDTOs;

namespace Project_3.src.Application.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<PagedResult<DepartmentDto>> GetPagedAsync(QueryParams query);

        Task<DepartmentDto> GetByIdAsync(int id);

        Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);

        Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto);

        Task DeleteAsync(int id);

    }
}
