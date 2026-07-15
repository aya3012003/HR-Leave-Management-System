using System.ComponentModel.DataAnnotations;

namespace Project_3.src.Application.DTOs.EmployeeDTOs
{
    public class UpdateEmployeeDto
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        public int? DepartmentId { get; set; }
        public string? EmployeeType { get; set; }
    }
}
