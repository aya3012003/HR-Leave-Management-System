namespace Project_3.src.Application.DTOs.DashboardDto
{
    public class DashboardStatisticsDto
    {
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalLeaveRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
    }
}
