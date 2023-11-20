namespace FourCreate.Application.Contracts.Persistence
{
    public interface IUnitOfWork
    {
        //Start the database Transaction
        Task CreateTransactionAsync();
        //Commit the database Transaction
        Task CommitAsync();
        //Rollback the database Transaction
        Task RollbackAsync();
    }
}
