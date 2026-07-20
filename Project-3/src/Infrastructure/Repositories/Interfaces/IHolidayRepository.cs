using Project_3.src.Application.Models;

namespace Project_3.src.Infrastructure.Repositories.Interfaces
{
    public interface IHolidayRepository : IRepository<Holiday>
    {
        Task<List<Holiday>> GetByYearAsync( string countryCode, int year );

        Task<bool> ExistsForYearAsync( string countryCode, int year);
    }
}
