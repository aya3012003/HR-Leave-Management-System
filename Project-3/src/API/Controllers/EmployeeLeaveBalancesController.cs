using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.DTOs.EmployeeLeaveBalanceDtos;
using Project_3.src.Application.Services.Interfaces;
using System.Security.Claims;

namespace Project_3.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, Manager")]
    public class EmployeeLeaveBalancesController : ControllerBase
    {
        private readonly IEmployeeLeaveBalanceService _service;
        public EmployeeLeaveBalancesController(IEmployeeLeaveBalanceService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] EmployeeLeaveBalanceQueryParams query)
        {
            var result = await _service.GetPagedAsync(query);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var balance = await _service.GetByIdAsync(id);

            return Ok(balance);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create([FromBody] CreateEmployeeLeaveBalanceDto dto)
        {
            var balance = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = balance.Id },
                balance);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateEmployeeLeaveBalanceDto dto)
        {
            var balance = await _service.UpdateAsync(id, dto);

            return Ok(balance);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}
