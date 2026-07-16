using Project_3.src.Application.DTOs.Pagination;

namespace Project_3.src.Application.DTOs.EmployeeLeaveBalanceDtos
{
    public class EmployeeLeaveBalanceQueryParams : PaginationParams
    {
        public string? Search { get; set; }

        public string? UserId { get; set; }

        public int? LeaveTypeId { get; set; }
    }
}
