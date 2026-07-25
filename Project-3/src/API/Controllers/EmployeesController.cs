using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.DTOs.EmployeeDTOs;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Interfaces;
using System.Security.Claims;

/// <summary>
/// Provides endpoints for managing employees and employee profiles.
/// Administrators can manage employees, managers can view employees,
/// and authenticated users can view and update their own profiles.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeesController"/> class.
    /// </summary>
    /// <param name="employeeService">Employee service.</param>
    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// Retrieves a paginated list of employees.
    /// </summary>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Number of employees per page.</param>
    /// <param name="deptId">Optional department identifier for filtering.</param>
    /// <param name="search">Optional search keyword.</param>
    /// <returns>A paginated list of employees.</returns>
    /// <response code="200">Employees retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpGet]
    [Authorize(Roles = $"{"Admin"},{"Manager"}")]
    public async Task<IActionResult> GetAll(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] int? deptId = null,
    [FromQuery] string? search = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var myProfile = await _employeeService.GetEmployeeProfileAsync(userId);

        // Enforce department-level isolation for Managers
        if (myProfile != null && myProfile.Roles.Contains("Manager") && !myProfile.Roles.Contains("Admin"))
        {
            // Force the department filter to match the manager's department
            deptId = myProfile.DepartmentId;
        }

        var result = await _employeeService.GetEmployeesAsync(page, pageSize, deptId, search);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the profile of the currently authenticated user.
    /// </summary>
    /// <returns>The authenticated user's profile.</returns>
    /// <response code="200">Profile retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Employee profile not found.</response>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var employee = await _employeeService.GetEmployeeProfileAsync(userId);
        if (employee == null) return NotFound();

        return Ok(employee);
    }

    /// <summary>
    /// Retrieves an employee by their identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employee.</param>
    /// <returns>The requested employee.</returns>
    /// <response code="200">Employee retrieved successfully.</response>
    /// <response code="404">Employee not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpGet("{id}")]
    [Authorize(Roles = $"{"Admin"},{"Manager"}")]
    public async Task<IActionResult> GetById(string id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null) return NotFound(new { message = "Employee not found." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var myProfile = await _employeeService.GetEmployeeProfileAsync(userId!);

        // Validate that the manager is looking at someone in their own department
        if (myProfile != null && myProfile.Roles.Contains("Manager") && !myProfile.Roles.Contains("Admin"))
        {
            if (employee.DepartmentId != myProfile.DepartmentId)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "You can only view employees within your own department." });
            }
        }

        return Ok(employee);
    }

    /// <summary>
    /// Creates a new employee.
    /// </summary>
    /// <param name="dto">The employee creation data.</param>
    /// <returns>The newly created employee.</returns>
    /// <response code="201">Employee created successfully.</response>
    /// <response code="400">Invalid employee data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var createdEmployee = await _employeeService.CreateEmployeeAsync(dto);
        if (createdEmployee == null) return BadRequest(new { message = "Failed to create employee or invalid role." });

        return StatusCode(StatusCodes.Status201Created, createdEmployee);
    }

    /// <summary>
    /// Updates an existing employee.
    /// </summary>
    /// <param name="id">The unique identifier of the employee.</param>
    /// <param name="dto">The updated employee information.</param>
    /// <returns>The updated employee.</returns>
    /// <response code="200">Employee updated successfully.</response>
    /// <response code="400">Invalid employee data.</response>
    /// <response code="404">Employee not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateEmployeeDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, dto);
        if (updatedEmployee == null) return NotFound(new { message = "Employee not found or update failed." });

        return Ok(updatedEmployee);
    }

    /// <summary>
    /// Updates the profile of the currently authenticated user.
    /// Department changes are ignored.
    /// </summary>
    /// <param name="dto">The updated profile information.</param>
    /// <returns>The updated employee profile.</returns>
    /// <response code="200">Profile updated successfully.</response>
    /// <response code="400">Invalid profile data.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateEmployeeDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        // Ignore Department updates if the user is trying to update their own profile
        dto.DepartmentId = null;

        var updatedEmployee = await _employeeService.UpdateEmployeeAsync(userId, dto);
        if (updatedEmployee == null) return BadRequest(new { message = "Failed to update profile." });

        return Ok(updatedEmployee);
    }

    /// <summary>
    /// Deletes an employee.
    /// </summary>
    /// <param name="id">The unique identifier of the employee.</param>
    /// <returns>No content if the employee is deleted successfully.</returns>
    /// <response code="204">Employee deleted successfully.</response>
    /// <response code="404">Employee not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _employeeService.DeleteEmployeeAsync(id);
        if (!success) return NotFound(new { message = "Employee not found." });

        return NoContent();
    }
}