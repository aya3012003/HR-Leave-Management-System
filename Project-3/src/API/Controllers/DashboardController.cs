using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.Services.Interfaces;

namespace Project_3.src.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public ReportsController(IDashboardService service)
        {
            _service = service;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            return Ok(await _service.GetDashboardStatisticsAsync());
        }

        [HttpGet("leave-summary")]
        public async Task<IActionResult> LeaveSummary()
        {
            return Ok(await _service.GetLeaveTypeSummaryAsync());
        }

        [HttpGet("department-leave-usage")]
        public async Task<IActionResult> DepartmentLeaveUsage()
        {
            return Ok(await _service.GetLeaveDaysOfDepartment());
        }

        [HttpGet("employee-history/{userId}")]
        public async Task<IActionResult> EmployeeHistory(string userId)
        {
            return Ok(await _service.GetEmployeeLeaveHistoryAsync(userId));
        }

        [HttpGet("most-used-leave-type")]
        public async Task<IActionResult> MostUsedLeaveType()
        {
            return Ok(await _service.GetMostUsedLeaveTypeAsync());
        }
    }
}
