using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_3.src.Application.DTOs.DashboardDto;
using Project_3.src.Application.DTOs.LeaveTypeDto;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Interfaces;
using Project_3.src.Infrastructure.Repositories.Implementations;
using Project_3.src.Infrastructure.Repositories.Interfaces;
using Project_3.src.Shared.Enums;

namespace Project_3.src.Application.Services.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public DashboardService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
        {
            return new DashboardStatisticsDto
            {
                TotalEmployees =  await _userManager.Users.CountAsync(),
                TotalDepartments = await _unitOfWork.Departments.CountAsync(),
                TotalLeaveRequests = await _unitOfWork.LeaveRequests.CountAsync(),
                PendingRequests = await _unitOfWork.LeaveRequests.CountAsync(lr => lr.Status == LeaveStatus.Pending),
                ApprovedRequests = await _unitOfWork.LeaveRequests.CountAsync(lr => lr.Status == LeaveStatus.Approved),
                RejectedRequests = await _unitOfWork.LeaveRequests.CountAsync(lr => lr.Status == LeaveStatus.Rejected)
            };
        }

     

        // return all request groub by leave type and the count of each leavetype
        public async Task<IEnumerable<LeaveSummaryDto>>GetLeaveTypeSummaryAsync()
        {
            var requests = await _unitOfWork.LeaveRequests.GetAllAsync(l => l.LeaveType);
            return requests.GroupBy(x => x.LeaveType.Name).Select(x => new LeaveSummaryDto
            {
                LeaveType = x.Key,
                count = x.Count()
            });
        }
        // TODO: Load User.Department using ThenInclude in LeaveRequestRepository.
        // Returns the total approved leave days for each department.
        public async  Task<IEnumerable<DepartmentLeaveUsageDto>> GetLeaveDaysOfDepartment()
        {
            var requests = await _unitOfWork.LeaveRequests.GetAllAsync(l => l.User);
            return requests.Where(x => x.Status == LeaveStatus.Approved).GroupBy(x=>x.User.Department?.Name).Select(x => new DepartmentLeaveUsageDto
            {
                DepartmentName = x.Key,
                TotalDays = x.Sum(y => y.WorkingDays)
            });

        }

        public async Task<IEnumerable<EmployeeLeaveHistoryDto>> GetEmployeeLeaveHistoryAsync(string userId)
        {
            var requests = await _unitOfWork.LeaveRequests.GetAllAsync(x => x.LeaveType);

            return requests
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new EmployeeLeaveHistoryDto
                {
                    LeaveType = x.LeaveType.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    WorkingDays = x.WorkingDays,
                    Status = x.Status
                })
                .ToList();
        }
        // return the name of the mostused leave type
        public async Task<MostUsedLeaveTypeDto?> GetMostUsedLeaveTypeAsync()
        {
            var requests = await _unitOfWork.LeaveRequests.GetAllAsync(x => x.LeaveType);

            return requests
                .GroupBy(x => x.LeaveType.Name)
                .Select(x => new MostUsedLeaveTypeDto
                {
                    LeaveType = x.Key,
                    Count = x.Count()
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();
        }
    }
}
