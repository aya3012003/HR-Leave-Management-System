namespace Project_3.src.Application.DTOs.LeaveRequestDTOs
{
    public class LeaveRequestDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int WorkingDays { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ManagerComment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
