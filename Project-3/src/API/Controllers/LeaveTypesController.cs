using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.DTOs;
using Project_3.src.Application.DTOs.LeaveTypeDto;
using Project_3.src.Application.Services.Interfaces;

namespace Project_3.src.API.Controllers
{
    /// <summary>
    /// Provides operations for managing leave types.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaveTypesController : ControllerBase
    {
        private readonly ILeaveTypeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaveTypesController"/> class.
        /// </summary>
        /// <param name="service">Leave type service.</param>
        public LeaveTypesController(ILeaveTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a paginated list of leave types.
        /// </summary>
        /// <param name="query">Pagination and filtering parameters.</param>
        /// <returns>A paginated list of leave types.</returns>
        /// <response code="200">Leave types retrieved successfully.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryParams query)
        {
            var result = await _service.GetPagedAsync(query);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a leave type by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the leave type.</param>
        /// <returns>The requested leave type.</returns>
        /// <response code="200">Leave type retrieved successfully.</response>
        /// <response code="404">Leave type not found.</response>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var leaveType = await _service.GetByIdAsync(id);

            return Ok(leaveType);
        }

        /// <summary>
        /// Creates a new leave type.
        /// </summary>
        /// <param name="dto">Leave type information.</param>
        /// <returns>The newly created leave type.</returns>
        /// <response code="201">Leave type created successfully.</response>
        /// <response code="400">Invalid request data.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLeaveTypeDto dto)
        {
            var leaveType = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = leaveType.Id },
                leaveType);
        }

        /// <summary>
        /// Updates an existing leave type.
        /// </summary>
        /// <param name="id">The unique identifier of the leave type.</param>
        /// <param name="dto">Updated leave type information.</param>
        /// <returns>The updated leave type.</returns>
        /// <response code="200">Leave type updated successfully.</response>
        /// <response code="404">Leave type not found.</response>
        /// <response code="400">Invalid request data.</response>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLeaveTypeDto dto)
        {
            var leaveType = await _service.UpdateAsync(id, dto);

            return Ok(leaveType);
        }

        /// <summary>
        /// Deletes a leave type.
        /// </summary>
        /// <param name="id">The unique identifier of the leave type.</param>
        /// <returns>No content if the deletion is successful.</returns>
        /// <response code="204">Leave type deleted successfully.</response>
        /// <response code="404">Leave type not found.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}