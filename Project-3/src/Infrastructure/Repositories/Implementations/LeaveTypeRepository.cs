using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.LeaveTypeDto;
using Project_3.src.Application.Models;
using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;



namespace Project_3.src.Infrastructure.Repositories.Implementations
{
    public class LeaveTypeRepository : Repository<LeaveType>, ILeaveTypeRepository
    {
        public LeaveTypeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<LeaveType>> GetPagedAsync(LeaveTypeQueryParams p)
        {
            IQueryable<LeaveType> query = _dbSet;

            if (!string.IsNullOrWhiteSpace(p.Search))
            {
                query = query.Where(x =>
                    x.Name.Contains(p.Search) ||
                    (x.Description != null && x.Description.Contains(p.Search)));
            }

            query = p.SortBy.ToLower() switch
            {
                "days" => p.SortDir == "desc"
                    ? query.OrderByDescending(x => x.DefaultDays)
                    : query.OrderBy(x => x.DefaultDays),

                "name" => p.SortDir == "desc"
                    ? query.OrderByDescending(x => x.Name)
                    : query.OrderBy(x => x.Name),

                _ => query.OrderBy(x => x.Id)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((p.PageNumber - 1) * p.PageSize)
                .Take(p.PageSize)
                .ToListAsync();

            return new PagedResult<LeaveType>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = p.PageNumber,
                PageSize = p.PageSize
            };
        }
    }
}
