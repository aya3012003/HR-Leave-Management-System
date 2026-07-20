using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.DTOs.EmployeeLeaveBalanceDtos;
using Project_3.src.Application.Services.Interfaces;

/// <summary>
/// Provides endpoints for managing employee leave balances.
/// Accessible by administrators and managers.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin, Manager")]
public class EmployeeLeaveBalancesController : ControllerBase
{
    private readonly IEmployeeLeaveBalanceService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeLeaveBalancesController"/> class.
    /// </summary>
    /// <param name="service">Employee leave balance service.</param>
    public EmployeeLeaveBalancesController(IEmployeeLeaveBalanceService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves a paginated list of employee leave balances.
    /// </summary>
    /// <param name="query">Filtering and pagination parameters.</param>
    /// <returns>A paginated list of employee leave balances.</returns>
    /// <response code="200">Leave balances retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] EmployeeLeaveBalanceQueryParams query)
    {
        var result = await _service.GetPagedAsync(query);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves an employee leave balance by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the leave balance.</param>
    /// <returns>The requested employee leave balance.</returns>
    /// <response code="200">Leave balance retrieved successfully.</response>
    /// <response code="404">Leave balance not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var balance = await _service.GetByIdAsync(id);

        return Ok(balance);
    }

    /// <summary>
    /// Updates an existing employee leave balance.
    /// </summary>
    /// <param name="id">The unique identifier of the leave balance.</param>
    /// <param name="dto">The updated leave balance information.</param>
    /// <returns>The updated employee leave balance.</returns>
    /// <response code="200">Leave balance updated successfully.</response>
    /// <response code="400">Invalid leave balance data.</response>
    /// <response code="404">Leave balance not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateEmployeeLeaveBalanceDto dto)
    {
        var balance = await _service.UpdateAsync(id, dto);

        return Ok(balance);
    }

    /// <summary>
    /// Deletes an employee leave balance.
    /// Accessible only by administrators.
    /// </summary>
    /// <param name="id">The unique identifier of the leave balance.</param>
    /// <returns>No content if the leave balance is deleted successfully.</returns>
    /// <response code="204">Leave balance deleted successfully.</response>
    /// <response code="404">Leave balance not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}