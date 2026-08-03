using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Interfaces;
using System.Security.Claims;

/// <summary>
/// Provides endpoints for employees to view their own leave balances.
/// Accessible only by users with the Employee role.
/// </summary>
[ApiController]
[Route("api/my-leave-balances")]
[Authorize(Roles = "Admin,Manager,Employee")]
public class MyLeaveBalancesController : ControllerBase
{
    private readonly IEmployeeLeaveBalanceService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyLeaveBalancesController"/> class.
    /// </summary>
    /// <param name="service">Employee leave balance service.</param>
    public MyLeaveBalancesController(IEmployeeLeaveBalanceService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves the leave balances of the currently authenticated employee.
    /// </summary>
    /// <returns>The authenticated employee's leave balances.</returns>
    /// <response code="200">Leave balances retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpGet]
    public async Task<IActionResult> GetMyBalances()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var balances = await _service.GetMyBalancesAsync(userId);

        return Ok(balances);
    }
}