using Project_3.src.Application.Models;

namespace Project_3.src.Application.DTOs.DashboardDto
{
    public class LeaveSummaryDto
    {
        public string LeaveType { get; set; } = string.Empty;
        public int count { get; set; }
    }
}
