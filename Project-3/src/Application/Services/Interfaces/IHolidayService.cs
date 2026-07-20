using Project_3.src.Application.DTOs;

namespace Project_3.src.Application.Services.Interfaces
{
    public interface IHolidayService
    {
        Task<IEnumerable<HolidayApiDto>> GetByYearAsync(int year, string countryCode = "EG");

    }
}
