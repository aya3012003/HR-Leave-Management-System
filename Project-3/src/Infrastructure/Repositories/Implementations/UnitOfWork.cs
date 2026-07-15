using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3.src.Infrastructure.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
