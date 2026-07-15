using System.ComponentModel.DataAnnotations;

namespace Project_3.src.Application.DTOs.DepartmentDTOs
{
    public class UpdateDepartmentDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
    }
}
