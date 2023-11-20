using FourCreate.Domain.Entities.Employment;

namespace FourCreate.Application.Contracts.Persistence
{
    public interface IEmploymentRepository : IGenericRepository<IEmployment, int>
    {
        Task<int> AddRangeAsync(IList<IEmployment> entities);
    }
}
