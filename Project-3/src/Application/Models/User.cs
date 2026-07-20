using Microsoft.AspNetCore.Identity;
using Project_3.src.Infrastructure.identity;

using System.ComponentModel.DataAnnotations;

namespace Project_3.src.Application.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }

        public Department? Department { get; set; } = null!;

        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

        public ICollection<EmployeeLeaveBalance> LeaveBalances { get; set; } = new List<EmployeeLeaveBalance>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }

}
