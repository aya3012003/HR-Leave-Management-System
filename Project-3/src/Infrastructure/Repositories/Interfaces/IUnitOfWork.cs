namespace Project_3.src.Infrastructure.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();

    }
}
