using Microsoft.EntityFrameworkCore;
using Project_3.src.Application.Models;
using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3.src.Infrastructure.Repositories.Implementations
{
    public class HolidayRepository : Repository<Holiday>, IHolidayRepository
    {
        public HolidayRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Holiday>> GetByYearAsync(string countryCode , int year )
        {
            return await _dbSet
                    .AsNoTracking()
                    .Where(x => x.Year == year && x.CountryCode == countryCode)
                    .OrderBy(x => x.Date)
                    .ToListAsync();
        }

        public async Task<bool> ExistsForYearAsync(string countryCode, int year)
        {
            return await _dbSet.AnyAsync(x =>
                x.Year == year &&
                x.CountryCode == countryCode);
        }
    }

}
