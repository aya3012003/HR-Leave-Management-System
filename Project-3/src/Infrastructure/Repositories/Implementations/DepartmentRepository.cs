using Microsoft.EntityFrameworkCore;
using Project_3.src.Application.DTOs;
using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.Models;
using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3.src.Infrastructure.Repositories.Implementations
{
    public class DepartmentRepository
        : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<PagedResult<Department>> GetPagedAsync(QueryParams query)
        {
            IQueryable<Department> departments = _dbSet;

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                departments = departments.Where(d =>
                    d.Name.Contains(query.Search));
            }

            departments = departments.OrderBy(d => d.Name);

            var totalCount = await departments.CountAsync();

            var items = await departments
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<Department>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }
    }
}