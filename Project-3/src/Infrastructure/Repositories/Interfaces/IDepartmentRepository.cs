using Project_3.src.Application.DTOs;
using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.Models;

namespace Project_3.src.Infrastructure.Repositories.Interfaces
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<PagedResult<Department>> GetPagedAsync(QueryParams query);

    }
}
