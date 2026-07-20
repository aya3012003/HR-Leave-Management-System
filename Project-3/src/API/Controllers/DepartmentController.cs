using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.DTOs;
using Project_3.src.Application.DTOs.DepartmentDTOs;
using Project_3.src.Application.Services.Interfaces;

/// <summary>
/// Provides endpoints for managing departments.
/// Create, update, and delete operations are restricted to administrators,
/// while retrieval operations are publicly accessible.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DepartmentController : ControllerBase
{
    private readonly ILogger<DepartmentController> _logger;
    private readonly IDepartmentService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="DepartmentController"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="service">Department service.</param>
    public DepartmentController(ILogger<DepartmentController> logger, IDepartmentService service)
    {
        _logger = logger;
        _service = service;
    }

    /// <summary>
    /// Retrieves a paginated list of departments.
    /// </summary>
    /// <param name="query">Pagination parameters.</param>
    /// <returns>A paginated list of departments.</returns>
    /// <response code="200">Departments retrieved successfully.</response>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] QueryParams query)
    {
        var result = await _service.GetPagedAsync(query);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a department by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the department.</param>
    /// <returns>The requested department.</returns>
    /// <response code="200">Department retrieved successfully.</response>
    /// <response code="404">Department not found.</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<DepartmentDto>> GetById(int id)
    {
        var department = await _service.GetByIdAsync(id);
        return Ok(department);
    }

    /// <summary>
    /// Creates a new department.
    /// </summary>
    /// <param name="dto">The department creation data.</param>
    /// <returns>The newly created department.</returns>
    /// <response code="201">Department created successfully.</response>
    /// <response code="400">Invalid department data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentDto dto)
    {
        var department = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    /// <summary>
    /// Updates an existing department.
    /// </summary>
    /// <param name="id">The unique identifier of the department.</param>
    /// <param name="dto">The updated department data.</param>
    /// <returns>The updated department.</returns>
    /// <response code="200">Department updated successfully.</response>
    /// <response code="400">Invalid department data.</response>
    /// <response code="404">Department not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<DepartmentDto>> Update(int id, [FromBody] UpdateDepartmentDto dto)
    {
        var department = await _service.UpdateAsync(id, dto);
        return Ok(department);
    }

    /// <summary>
    /// Deletes a department by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the department.</param>
    /// <returns>No content if the department is deleted successfully.</returns>
    /// <response code="204">Department deleted successfully.</response>
    /// <response code="404">Department not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}