using Project_3.src.Infrastructure.Shared.Enums;

namespace Project_3.src.Application.DTOs.DashboardDto
{
    public class EmployeeLeaveHistoryDto
    {
        public string LeaveType { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int WorkingDays { get; set; }

        public LeaveStatus Status { get; set; }
    }
}
