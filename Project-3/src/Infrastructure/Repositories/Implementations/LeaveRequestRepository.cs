using Microsoft.EntityFrameworkCore;
using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.Models;
using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3.src.Infrastructure.Repositories.Implementations
{
    public class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(AppDbContext context) : base(context) { }

        public async Task<PagedResult<LeaveRequest>> GetPagedAsync(LeaveRequestQueryParams query)
        {
            IQueryable<LeaveRequest> requests = _dbSet
                .Include(r => r.User)
                .Include(r => r.LeaveType)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.UserId))
                requests = requests.Where(r => r.UserId == query.UserId);

            if (query.Status.HasValue)
                requests = requests.Where(r => r.Status == query.Status.Value);

            var totalCount = await requests.CountAsync();
            var items = await requests
                .OrderByDescending(r => r.CreatedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<LeaveRequest>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }
    }
}