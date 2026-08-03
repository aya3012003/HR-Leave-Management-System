using Project_3.src.Application.DTOs;

namespace Project_3.src.Application.Services.Interfaces
{
    public interface IHolidayApiService
    {
        Task<List<HolidayApiDto>> GetHolidaysAsync(string countryCode, int year);

    }
}
