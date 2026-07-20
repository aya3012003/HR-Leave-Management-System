using Project_3.src.Application.DTOs;
using Project_3.src.Application.Services.Interfaces;

namespace Project_3.src.Application.Services.Implementation
{
    public class HolidayApiService : IHolidayApiService
    {
        private readonly HttpClient _httpClient;
        public HolidayApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<HolidayApiDto>> GetHolidaysAsync( string countryCode, int year)
        {
            var response = await _httpClient.GetAsync($"https://date.nager.at/api/v4/Holidays/{countryCode}/{year}");
            response.EnsureSuccessStatusCode();
            var holidays = await response.Content.ReadFromJsonAsync<List<HolidayApiDto>>();
            return holidays ?? new List<HolidayApiDto>();
        }

    }
}
