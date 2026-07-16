namespace Project_3.src.Application.DTOs.LeaveTypeDto
{
    public class LeaveTypeDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int DefaultDays { get; set; }

        public string? Description { get; set; }
    }
}
