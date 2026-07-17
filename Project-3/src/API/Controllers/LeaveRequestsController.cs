using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.DTOs.LeaveRequestDTOs;
using Project_3.src.Application.Services.Interfaces;
using System.Security.Claims;

namespace Project_3.src.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveRequestService _service;

        public LeaveRequestsController(ILeaveRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll([FromQuery] LeaveRequestQueryParams query)
            => Ok(await _service.GetAllAsync(query));

        [HttpGet("my")]
        public async Task<IActionResult> GetMyRequests([FromQuery] LeaveRequestQueryParams query)
            => Ok(await _service.GetMyRequestsAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!, query));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
            => Ok(await _service.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
        {
            var result = await _service.CreateAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!, dto);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:int}/approve")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Approve(int id, [FromBody] LeaveRequestActionDto dto)
            => Ok(await _service.ApproveAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!, dto));

        [HttpPut("{id:int}/reject")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Reject(int id, [FromBody] LeaveRequestActionDto dto)
            => Ok(await _service.RejectAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!, dto));

        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
            => Ok(await _service.CancelAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!));
    }
}
