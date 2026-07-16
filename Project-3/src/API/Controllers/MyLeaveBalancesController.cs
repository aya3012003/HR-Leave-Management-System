using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Interfaces;
using System.Security.Claims;

namespace Project_3.src.API.Controllers
{
    [ApiController]
    [Route("api/my-leave-balances")]
    [Authorize(Roles = "Employee")]
    public class MyLeaveBalancesController : ControllerBase
    {
        private readonly IEmployeeLeaveBalanceService _service;

        public MyLeaveBalancesController(IEmployeeLeaveBalanceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyBalances()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var balances = await _service.GetMyBalancesAsync(userId);

            return Ok(balances);
        }
    }
}
