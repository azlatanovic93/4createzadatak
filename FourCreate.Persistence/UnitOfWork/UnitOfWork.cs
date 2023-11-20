using FourCreate.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace FourCreate.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbContextTransaction _dbContextTransaction;
        private NpgsqlDbContext _dbContext;

        public UnitOfWork(NpgsqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CommitAsync()
        {
            await _dbContextTransaction.CommitAsync();
        }

        public async Task CreateTransactionAsync()
        {
            _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            await _dbContextTransaction.RollbackAsync();
            await _dbContextTransaction.DisposeAsync();
        }
    }
}
