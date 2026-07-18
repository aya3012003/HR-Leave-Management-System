using System.ComponentModel.DataAnnotations;

namespace Project_3.src.Application.DTOs.LeaveRequestDTOs
{
    public class CreateLeaveRequestDto
    {
        [Required] public int LeaveTypeId { get; set; }
        [Required] public DateOnly StartDate { get; set; }
        [Required] public DateOnly EndDate { get; set; }
        [Required, MaxLength(300)] public string Reason { get; set; } = string.Empty;
    }
}
