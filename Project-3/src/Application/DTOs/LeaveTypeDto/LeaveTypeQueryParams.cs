using Project_3.src.Application.DTOs.Pagination;

namespace Project_3.src.Application.DTOs.LeaveTypeDto
{
    public class LeaveTypeQueryParams:PaginationParams
    {
        public string? Search { get; set; }

        public string SortBy { get; set; } = "Name";

        public string SortDir { get; set; } = "asc";


    }
}
