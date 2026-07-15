namespace Project_3.src.Application.DTOs.EmployeeDTOs
{
    public class EmployeeDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
