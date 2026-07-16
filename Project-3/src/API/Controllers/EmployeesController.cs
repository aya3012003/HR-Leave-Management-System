using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.DTOs.EmployeeDTOs;
using Project_3.src.Application.Interfaces.IServices;
using Project_3.src.Application.Models;

using System.Security.Claims;

namespace Project_3.src.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize(Roles = $"{"Admin"},{"Manager"}")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? deptId = null,
            [FromQuery] string? search = null)
        {
            var result = await _employeeService.GetEmployeesAsync(page, pageSize, deptId, search);
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var employee = await _employeeService.GetEmployeeProfileAsync(userId);
            if (employee == null) return NotFound();

            return Ok(employee);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{"Admin"},{"Manager"}")]
        public async Task<IActionResult> GetById(string id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound(new { message = "Employee not found." });

            return Ok(employee);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdEmployee = await _employeeService.CreateEmployeeAsync(dto);
            if (createdEmployee == null) return BadRequest(new { message = "Failed to create employee or invalid role." });

            return StatusCode(StatusCodes.Status201Created, createdEmployee);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateEmployeeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, dto);
            if (updatedEmployee == null) return NotFound(new { message = "Employee not found or update failed." });

            return Ok(updatedEmployee);
        }

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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _employeeService.DeleteEmployeeAsync(id);
            if (!success) return NotFound(new { message = "Employee not found." });

            return NoContent();
        }
    }
}
