using Project_3.src.Application.DTOs.DashboardDto;
using Project_3.src.Application.DTOs.LeaveTypeDto;

namespace Project_3.src.Application.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatisticsDto> GetDashboardStatisticsAsync();
      Task<IEnumerable<LeaveSummaryDto>> GetLeaveTypeSummaryAsync();
        Task<IEnumerable<DepartmentLeaveUsageDto>> GetLeaveDaysOfDepartment();
        Task<IEnumerable<EmployeeLeaveHistoryDto>> GetEmployeeLeaveHistoryAsync(string userId);
        Task<MostUsedLeaveTypeDto?> GetMostUsedLeaveTypeAsync();

    }
}
