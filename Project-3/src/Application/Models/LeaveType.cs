using System.ComponentModel.DataAnnotations;

namespace Project_3.src.Application.Models
{
    public class LeaveType : BaseEntity
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;

        public int DefaultDays { get; set; }

        public string? Description { get; set; }

        public ICollection<LeaveRequest> LeaveRequests { get; set; }= new List<LeaveRequest>();

        public ICollection<EmployeeLeaveBalance> LeaveBalances { get; set; } = new List<EmployeeLeaveBalance>();

    }
}
