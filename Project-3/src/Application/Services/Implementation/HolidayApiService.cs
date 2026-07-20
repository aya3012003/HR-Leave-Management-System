using Project_3.src.Application.DTOs;
using Project_3.src.Application.Services.Interfaces;

namespace Project_3.src.Application.Services.Implementation
{
    //We need to catch HttpRequestException and use the newly configured logger to record the failure without crashing the leave calculation
    public class HolidayApiService : IHolidayApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HolidayApiService> _logger;

        public HolidayApiService(HttpClient httpClient, ILogger<HolidayApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<HolidayApiDto>> GetHolidaysAsync(string countryCode, int year)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://date.nager.at/api/v4/Holidays/{countryCode}/{year}");
                response.EnsureSuccessStatusCode();
                var holidays = await response.Content.ReadFromJsonAsync<List<HolidayApiDto>>();
                return holidays ?? new List<HolidayApiDto>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Nager.Date API unavailable for {CountryCode}/{Year}. Falling back to weekend-only calculation.", countryCode, year);
                return new List<HolidayApiDto>(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching public holidays.");
                return new List<HolidayApiDto>(); 
            }
        }
    }
}
