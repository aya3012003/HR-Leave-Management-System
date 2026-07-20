using Project_3.src.Application.DTOs.Pagination;
using Project_3.src.Infrastructure.Shared.Enums;

namespace Project_3.src.Application.DTOs.LeaveRequestDTOs
{
    public class LeaveRequestQueryParams : PaginationParams
    {
        public LeaveStatus? Status { get; set; }
        public string? UserId { get; set; }
    }
}
