namespace Project_3.src.Application.DTOs.EmployeeLeaveBalanceDtos
{
    public class CreateEmployeeLeaveBalanceDto
    {
        public string UserId { get; set; } = string.Empty;

        public int LeaveTypeId { get; set; }

        public int RemainingDays { get; set; }
    }
}
