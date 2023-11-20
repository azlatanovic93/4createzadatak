namespace FourCreate.Application.Contracts.Persistence
{
    public interface IGenericRepository<TEntity, TID>
        where TEntity : class
    {
        Task<TID> AddAsync(TEntity entity);
    }

}
