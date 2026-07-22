using Microsoft.EntityFrameworkCore;
using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.LeaveRequestDTOs;
using Project_3.src.Application.Models;
using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.Repositories.Interfaces;
using Project_3.src.Infrastructure.Shared.Enums;

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
        public async Task<IEnumerable<LeaveRequest>> GetAllWithUserDepartmentAsync()
        {
            return await _dbSet
                .Include(r => r.User)
                .ThenInclude(u => u.Department)
                .Include(r => r.LeaveType)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<bool> HasOverlappingRequestAsync(string userId, DateOnly startDate, DateOnly endDate)
        {
            return await _context.LeaveRequests.AnyAsync(r =>
                r.UserId == userId &&
                r.Status == LeaveStatus.Pending ||
                r.Status == LeaveStatus.Approved &&
                startDate <= r.EndDate &&
                endDate >= r.StartDate);
        }
    }
}