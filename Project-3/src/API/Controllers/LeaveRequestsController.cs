using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.DTOs.LeaveRequestDTOs;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Interfaces;
using System.Security.Claims;

/// <summary>
/// Provides endpoints for managing leave requests.
/// Employees can create and manage their own requests,
/// while administrators and managers can review and process requests.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaveRequestsController"/> class.
    /// </summary>
    /// <param name="service">Leave request service.</param>
    public LeaveRequestsController(ILeaveRequestService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves a paginated list of leave requests.
    /// Accessible by administrators and managers.
    /// </summary>
    /// <param name="query">Filtering and pagination parameters.</param>
    /// <returns>A paginated list of leave requests.</returns>
    /// <response code="200">Leave requests retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAll([FromQuery] LeaveRequestQueryParams query)
        => Ok(await _service.GetAllAsync(query));

    /// <summary>
    /// Retrieves the leave requests of the currently authenticated user.
    /// </summary>
    /// <param name="query">Filtering and pagination parameters.</param>
    /// <returns>The authenticated user's leave requests.</returns>
    /// <response code="200">Leave requests retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyRequests([FromQuery] LeaveRequestQueryParams query)
        => Ok(await _service.GetMyRequestsAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!, query));

    /// <summary>
    /// Retrieves a leave request by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the leave request.</param>
    /// <returns>The requested leave request.</returns>
    /// <response code="200">Leave request retrieved successfully.</response>
    /// <response code="404">Leave request not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _service.GetByIdAsync(id));

    /// <summary>
    /// Creates a new leave request for the authenticated user.
    /// </summary>
    /// <param name="dto">The leave request details.</param>
    /// <returns>The newly created leave request.</returns>
    /// <response code="201">Leave request created successfully.</response>
    /// <response code="400">Invalid leave request data.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
    {
        var result = await _service.CreateAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!, dto);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Approves a pending leave request.
    /// Accessible by administrators and managers.
    /// </summary>
    /// <param name="id">The unique identifier of the leave request.</param>
    /// <param name="dto">The approval details, including the manager's comment.</param>
    /// <returns>The approved leave request.</returns>
    /// <response code="200">Leave request approved successfully.</response>
    /// <response code="400">The request cannot be approved.</response>
    /// <response code="404">Leave request not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpPut("{id:int}/approve")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Approve(int id, [FromBody] LeaveRequestActionDto dto)
        => Ok(await _service.ApproveAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!, dto));

    /// <summary>
    /// Rejects a pending leave request.
    /// Accessible by administrators and managers.
    /// </summary>
    /// <param name="id">The unique identifier of the leave request.</param>
    /// <param name="dto">The rejection details, including the manager's comment.</param>
    /// <returns>The rejected leave request.</returns>
    /// <response code="200">Leave request rejected successfully.</response>
    /// <response code="400">The request cannot be rejected.</response>
    /// <response code="404">Leave request not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpPut("{id:int}/reject")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Reject(int id, [FromBody] LeaveRequestActionDto dto)
        => Ok(await _service.RejectAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!, dto));

    /// <summary>
    /// Cancels a pending leave request submitted by the authenticated user.
    /// </summary>
    /// <param name="id">The unique identifier of the leave request.</param>
    /// <returns>The cancelled leave request.</returns>
    /// <response code="200">Leave request cancelled successfully.</response>
    /// <response code="400">The request cannot be cancelled.</response>
    /// <response code="404">Leave request not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
        => Ok(await _service.CancelAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!));
}