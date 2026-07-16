using System.ComponentModel.DataAnnotations;

namespace Project_3.src.Application.DTOs.EmployeeLeaveBalanceDtos
{
    public class EmployeeLeaveBalanceDto
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        [Required]
        public string EmployeeName { get; set; } = string.Empty;

        public int LeaveTypeId { get; set; }
        [Required]
        public string LeaveTypeName { get; set; } = string.Empty;

        public int RemainingDays { get; set; }
    }
}
