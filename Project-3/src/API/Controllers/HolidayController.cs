using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.DTOs;
using Project_3.src.Application.Services.Interfaces;

namespace Project_3.src.API.Controllers
{
    /// <summary>
    /// Provides endpoints for retrieving public holidays.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HolidayController : ControllerBase
    {
        private readonly IHolidayService _service;

        public HolidayController(IHolidayService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a list of public holidays for a specific year and country.
        /// </summary>
        /// <param name="year">The year to fetch holidays for.</param>
        /// <param name="countryCode">The ISO country code (defaults to EG).</param>
        /// <returns>A list of holidays.</returns>
        /// <response code="200">Returns the requested holidays.</response>
        [HttpGet("{year:int}")]
        [ProducesResponseType(typeof(IEnumerable<HolidayApiDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByYear(int year, [FromQuery] string countryCode = "EG")
        {
            var result = await _service.GetByYearAsync(year, countryCode);
            return Ok(result);
        }
    }
}
