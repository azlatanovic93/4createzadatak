using FourCreate.Domain.Entities.Company;

namespace FourCreate.Application.Contracts.Persistence
{
    public interface ICompanyRepository : IGenericRepository<ICompany, long>
    {
        Task<IList<ICompany>> GetByIdsAsync(IList<long> ids);
        Task<ICompany> GetByNameAsync(string name);
    }
}
