namespace Project_3.src.Application.Models
{
    public class EmployeeLeaveBalance : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;

        public User User { get; set; } = null!;

        public int LeaveTypeId { get; set; }

        public LeaveType LeaveType { get; set; } = null!;

        public int RemainingDays { get; set; }
    }
}
