using System.ComponentModel.DataAnnotations;

namespace Project_3.src.Application.DTOs.EmployeeDTOs
{
    public class CreateEmployeeDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;

        public string? EmployeeType { get; set; }
    }
}
