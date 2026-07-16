using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.DTOs;
using Project_3.src.Application.DTOs.LeaveTypeDto;
using Project_3.src.Application.Services.Interfaces;

namespace Project_3.src.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveTypesController : ControllerBase
    {
        private readonly ILeaveTypeService _service;

        public LeaveTypesController(ILeaveTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryParams query)
        {
            var result = await _service.GetPagedAsync(query);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var leaveType = await _service.GetByIdAsync(id);

            return Ok(leaveType);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateLeaveTypeDto dto)
        {
            var leaveType = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = leaveType.Id },
                leaveType);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateLeaveTypeDto dto)
        {
            var leaveType = await _service.UpdateAsync(id, dto);

            return Ok(leaveType);
        }

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

