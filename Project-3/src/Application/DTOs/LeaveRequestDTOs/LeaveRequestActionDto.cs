using System.ComponentModel.DataAnnotations;

namespace Project_3.src.Application.DTOs.LeaveRequestDTOs
{
    public class LeaveRequestActionDto
    {
        [MaxLength(500)] public string? ManagerComment { get; set; }
    }
}
