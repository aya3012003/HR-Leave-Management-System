using Microsoft.EntityFrameworkCore;
using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.EmployeeLeaveBalanceDtos;
using Project_3.src.Application.Models;
using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3.src.Infrastructure.Repositories.Implementations
{
    public class EmployeeLeaveBalanceRepository : Repository<EmployeeLeaveBalance> , IEmployeeLeaveBalanceRepository
    {
        public EmployeeLeaveBalanceRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<PagedResult<EmployeeLeaveBalance>> GetPagedAsync(EmployeeLeaveBalanceQueryParams queryParams)
        {
            IQueryable<EmployeeLeaveBalance> query = _dbSet.AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.LeaveType);

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                query = query.Where(x =>
                    x.User.FirstName.Contains(queryParams.Search) ||
                    x.User.LastName.Contains(queryParams.Search) ||
                    x.LeaveType.Name.Contains(queryParams.Search));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.UserId))
            {
                query = query.Where(x => x.UserId == queryParams.UserId);
            }

            if (queryParams.LeaveTypeId.HasValue)
            {
                query = query.Where(x => x.LeaveTypeId == queryParams.LeaveTypeId);
            }

            query = query.OrderBy(x => x.User.FirstName);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return new PagedResult<EmployeeLeaveBalance>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };
        }
        public async Task<IEnumerable<EmployeeLeaveBalance>> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(x => x.LeaveType)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<EmployeeLeaveBalance?> GetByUserAndLeaveTypeAsync(string userId,int leaveTypeId)
        {
            return await _dbSet
                .Include(x => x.LeaveType)
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.LeaveTypeId == leaveTypeId);
        }
    }
}

