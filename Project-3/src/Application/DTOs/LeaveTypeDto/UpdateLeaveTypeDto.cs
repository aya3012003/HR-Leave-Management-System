using System.ComponentModel.DataAnnotations;

namespace Project_3.src.Application.DTOs.LeaveTypeDto
{
    public class UpdateLeaveTypeDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 365)]
        public int DefaultDays { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
