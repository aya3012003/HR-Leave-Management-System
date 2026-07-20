using Project_3.src.Infrastructure.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Project_3.src.Application.Models
{
    public class LeaveRequest : BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int WorkingDays { get; set; }
        [Required]
        [MaxLength(300)]
        public string Reason { get; set; } = null!;
        
        public LeaveStatus Status { get; set; }= LeaveStatus.Pending;

        [MaxLength(500)]
        public string? ManagerComment { get; set; }

        public string UserId { get; set; } = string.Empty;

        public User User { get; set; } = null!;

        public int LeaveTypeId { get; set; }

        public LeaveType LeaveType { get; set; } = null!;
    }
}
