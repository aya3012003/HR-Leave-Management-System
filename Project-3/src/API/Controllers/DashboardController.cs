using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.Services.Interfaces;

namespace Project_3.src.API.Controllers
{
    /// <summary>
    /// Provides dashboard statistics and leave management reports.
    /// Accessible only by users with the Admin or Manager role.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="service">Dashboard service.</param>
        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves overall dashboard statistics.
        /// </summary>
        /// <returns>General statistics such as employees, leave requests, and departments.</returns>
        /// <response code="200">Dashboard statistics retrieved successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            return Ok(await _service.GetDashboardStatisticsAsync());
        }

        /// <summary>
        /// Retrieves a summary of leave requests grouped by leave type.
        /// </summary>
        /// <returns>Summary of leave requests by leave type.</returns>
        /// <response code="200">Leave summary retrieved successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpGet("leave-summary")]
        public async Task<IActionResult> LeaveSummary()
        {
            return Ok(await _service.GetLeaveTypeSummaryAsync());
        }

        /// <summary>
        /// Retrieves leave usage statistics for each department.
        /// </summary>
        /// <returns>Total leave days used by each department.</returns>
        /// <response code="200">Department leave usage retrieved successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpGet("department-leave-usage")]
        public async Task<IActionResult> DepartmentLeaveUsage()
        {
            return Ok(await _service.GetLeaveDaysOfDepartment());
        }

        /// <summary>
        /// Retrieves the leave history of a specific employee.
        /// </summary>
        /// <param name="userId">The unique identifier of the employee.</param>
        /// <returns>Employee leave history.</returns>
        /// <response code="200">Employee history retrieved successfully.</response>
        /// <response code="404">Employee not found.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpGet("employee-history/{userId}")]
        public async Task<IActionResult> EmployeeHistory(string userId)
        {
            return Ok(await _service.GetEmployeeLeaveHistoryAsync(userId));
        }

        /// <summary>
        /// Retrieves the most frequently used leave type.
        /// </summary>
        /// <returns>The leave type with the highest number of requests.</returns>
        /// <response code="200">Most used leave type retrieved successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpGet("most-used-leave-type")]
        public async Task<IActionResult> MostUsedLeaveType()
        {
            return Ok(await _service.GetMostUsedLeaveTypeAsync());
        }
    }
}