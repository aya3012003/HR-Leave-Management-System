using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.DTOs.DepartmentDTOs;
using Project_3.src.Application.Services.Interfaces;

namespace Project_3.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DepartmentController : ControllerBase
    {
        private readonly ILogger<DepartmentController> _logger;
        private readonly IDepartmentService _service;
        public DepartmentController(ILogger<DepartmentController> logger, IDepartmentService service)
        {
            _logger = logger;
            _service = service;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (departments, totalCount) = await _service.GetPagedAsync(pageNumber, pageSize);

            return Ok(new
            {
                Items = departments,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<DepartmentDto>> GetById(int id)
        {
            var department = await _service.GetByIdAsync(id);
            return Ok(department);
        }
        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentDto dto)
        {
            var department = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<DepartmentDto>> Update(int id, [FromBody] UpdateDepartmentDto dto)
        {

                var department = await _service.UpdateAsync(id, dto);
                return Ok(department);
            
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
                await _service.DeleteAsync(id);
                return NoContent();

        }
    }
}
