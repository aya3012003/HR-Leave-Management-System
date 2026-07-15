using Project_3.src.Application.DTOs.DepartmentDTOs;

namespace Project_3.src.Application.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();
        Task<(IEnumerable<DepartmentDto> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
        Task<DepartmentDto> GetByIdAsync(int id);

        Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);

        Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto);

        Task DeleteAsync(int id);

    }
}
