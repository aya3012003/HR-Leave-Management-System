using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.Services.Interfaces;

namespace Project_3.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HolidayController : ControllerBase
    {
        private readonly IHolidayService _service;

        public HolidayController(IHolidayService service)
        {
            _service = service;
        }

        [HttpGet("{year:int}")]
        public async Task<IActionResult> GetByYear(int year, [FromQuery] string countryCode = "EG")
        {
            var result = await _service.GetByYearAsync(year, countryCode);
            return Ok(result);
        }

    }
}
