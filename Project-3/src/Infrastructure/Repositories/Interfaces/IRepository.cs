using Project_3.src.Application.Models;
using System.Linq.Expressions;

namespace Project_3.src.Infrastructure.Repositories.Interfaces
{
    public interface IRepository<T> where T :  BaseEntity
    {
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

    }
}
